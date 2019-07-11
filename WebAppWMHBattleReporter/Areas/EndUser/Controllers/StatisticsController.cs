using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppWMHBattleReporter.Data;
using WebAppWMHBattleReporter.Models;
using WebAppWMHBattleReporter.Models.ViewModels;
using WebAppWMHBattleReporter.Utility;

namespace WebAppWMHBattleReporter.Areas.EndUser.Controllers
{
    [Area("EndUser")]
    public class StatisticsController : Controller
    {
        private ApplicationDbContext _db;

        public StatisticsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            DashBoardViewModel viewModel = new DashBoardViewModel()
            {
                Top10Factions = await _db.Factions.OrderByDescending(f => f.Winrate).Take(10).ToListAsync(),
                Top10Themes = await _db.Themes.OrderByDescending(t => t.Winrate).Take(10).ToListAsync(),
                Top10Casters = await _db.Casters.OrderByDescending(c => c.Winrate).Take(10).ToListAsync()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Users(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !StaticDetails.AllAndRegions.Contains(id))
                id = StaticDetails.AllRegions;
            UserResultsViewModel viewModel = new UserResultsViewModel()
            {
                Region = id,
                UserResults = await UserResults(id)
            };
            return View(viewModel);
        }

        public new async Task<IActionResult> User(string id)
        {
            if (!await _db.ApplicationUsers.AnyAsync(au => au.UserName == id) || string.IsNullOrWhiteSpace(id))
            {
                UserResultViewModel errorMessageViewModel = new UserResultViewModel()
                {
                    Username = id,
                    StatusMessage = "You must specify the name of a registered user.",
                };
                return View(errorMessageViewModel);
            }

            ApplicationUser user = await _db.ApplicationUsers.FirstAsync(au => au.UserName == id);
            List<BattleReport> battleReports = await _db.BattleReports.Where(br => br.PostersUsername == user.UserName || br.OpponentsUsername == user.UserName).ToListAsync();
            UserResultViewModel viewModel = new UserResultViewModel()
            {
                Username = user.UserName,
                StatusMessage = string.Empty,
                UserResult = CreateUserResult(user, battleReports),
                Factions = UserEntityResult(StaticDetails.FactionType, user.UserName, battleReports),
                Themes = UserEntityResult(StaticDetails.ThemeType, user.UserName, battleReports),
                Casters = UserEntityResult(StaticDetails.CasterType, user.UserName, battleReports),
                GameSizes = UserEntityResult(StaticDetails.GameSizeType, user.UserName, battleReports),
                Scenarios = UserEntityResult(StaticDetails.ScenarioType, user.UserName, battleReports),
                EndConditions = UserEntityResult(StaticDetails.EndConditionType, user.UserName, battleReports),
                VersusFactions = UserEntityResult(StaticDetails.VersusFactionType, user.UserName, battleReports),
                VersusThemes = UserEntityResult(StaticDetails.VersusThemeType, user.UserName, battleReports),
                VersusCasters = UserEntityResult(StaticDetails.VersusCasterType, user.UserName, battleReports)
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Factions()
        {
            List<Faction> factions = await _db.Factions.ToListAsync();
            List<Theme> themes = await _db.Themes.ToListAsync();
            List<Caster> casters = await _db.Casters.ToListAsync();
            List<BattleReport> battleReports = await _db.BattleReports.ToListAsync();
            List<FactionResult> factionResults = CreateFactionResults(factions, themes, casters, battleReports);
            return View(factionResults);
        }

        public async Task<IActionResult> Faction(string id)
        {
            FactionResultViewModel viewModel = new FactionResultViewModel
            {
                Factions = await _db.Factions.ToListAsync()
            };
            if (!await _db.Factions.AnyAsync(f => f.Name == id))
            {
                viewModel.Faction = viewModel.Factions.FirstOrDefault().Name;
                viewModel.StatusMessage = "Select a faction to view its results.";
                return View(viewModel);
            }

            Faction faction = await _db.Factions.FirstAsync(f => f.Name == id);
            List<Theme> factionThemes = await _db.Themes.Where(t => t.FactionId == faction.Id).ToListAsync();
            List<Caster> factionCasters = await _db.Casters.Where(c => c.FactionId == faction.Id).ToListAsync();
            List<BattleReport> factionBattleReports = await _db.BattleReports.Where(br => br.WinningFaction == faction.Name || br.LosingFaction == faction.Name).ToListAsync();
            int numberOfMirrorMatches = factionBattleReports.Where(br => br.WinningFaction == br.LosingFaction).Count();
            FactionResult factionResult = CreateFactionResult(faction, factionThemes, factionCasters, numberOfMirrorMatches);

            viewModel.Faction = faction.Name;
            viewModel.StatusMessage = string.Empty;
            viewModel.FactionResult = factionResult;
            viewModel.Themes = FactionEntityResult(StaticDetails.ThemeType, faction.Name, factionBattleReports);
            viewModel.Casters = FactionEntityResult(StaticDetails.CasterType, faction.Name, factionBattleReports);
            viewModel.GameSizes = FactionEntityResult(StaticDetails.GameSizeType, faction.Name, factionBattleReports);
            viewModel.Scenarios = FactionEntityResult(StaticDetails.ScenarioType, faction.Name, factionBattleReports);
            viewModel.EndConditions = FactionEntityResult(StaticDetails.EndConditionType, faction.Name, factionBattleReports);
            viewModel.VersusFactions = FactionEntityResult(StaticDetails.VersusFactionType, faction.Name, factionBattleReports);
            viewModel.VersusThemes = FactionEntityResult(StaticDetails.VersusThemeType, faction.Name, factionBattleReports);
            viewModel.VersusCasters = FactionEntityResult(StaticDetails.VersusCasterType, faction.Name, factionBattleReports);
            return View(viewModel);
        }

        public async Task<IActionResult> Themes()
        {
            List<Theme> themes = await _db.Themes.ToListAsync();
            List<BattleReport> battleReports = await _db.BattleReports.ToListAsync();
            List<ThemeResult> themeResults = CreateThemeResults(themes, battleReports);
            return View(themeResults);
        }

        public async Task<IActionResult> Theme(string id)
        {
            ThemeResultViewModel viewModel = new ThemeResultViewModel
            {
                Factions = await _db.Factions.ToListAsync(),
                Themes = await _db.Themes.ToListAsync()
            };
            if (!await _db.Themes.AnyAsync(t => t.Name == id))
            {
                viewModel.Theme = viewModel.Themes.FirstOrDefault().Name;
                viewModel.StatusMessage = "Select a theme to view its results.";
                return View(viewModel);
            }

            Theme theme = await _db.Themes.FirstAsync(t => t.Name == id);
            List<BattleReport> themeBattleReports = await _db.BattleReports.Where(br => br.WinningTheme == theme.Name || br.LosingTheme == theme.Name).ToListAsync();
            int numberOfMirrorMatches = themeBattleReports.Where(br => br.WinningTheme == br.LosingTheme).Count();
            ThemeResult themeResult = CreateThemeResult(theme, themeBattleReports);

            viewModel.Theme = theme.Name;
            viewModel.StatusMessage = string.Empty;
            viewModel.ThemeResult = themeResult;
            viewModel.Casters = ThemeEntityResult(StaticDetails.CasterType, theme.Name, themeBattleReports);
            viewModel.GameSizes = ThemeEntityResult(StaticDetails.GameSizeType, theme.Name, themeBattleReports);
            viewModel.Scenarios = ThemeEntityResult(StaticDetails.ScenarioType, theme.Name, themeBattleReports);
            viewModel.EndConditions = ThemeEntityResult(StaticDetails.EndConditionType, theme.Name, themeBattleReports);
            viewModel.VersusFactions = ThemeEntityResult(StaticDetails.VersusFactionType, theme.Name, themeBattleReports);
            viewModel.VersusThemes = ThemeEntityResult(StaticDetails.VersusThemeType, theme.Name, themeBattleReports);
            viewModel.VersusCasters = ThemeEntityResult(StaticDetails.VersusCasterType, theme.Name, themeBattleReports);

            return View(viewModel);
        }

        public async Task<IActionResult> Casters()
        {
            List<Caster> casters = await _db.Casters.ToListAsync();
            List<BattleReport> battleReports = await _db.BattleReports.ToListAsync();
            List<CasterResult> casterResults = CreateCasterResults(casters, battleReports);
            return View(casterResults);
        }

        public async Task<IActionResult> Caster(string id)
        {
            CasterResultViewModel viewModel = new CasterResultViewModel
            {
                Factions = await _db.Factions.ToListAsync(),
                Casters = await _db.Casters.ToListAsync()
            };
            if (!await _db.Casters.AnyAsync(c => c.Name == id))
            {
                viewModel.Caster = viewModel.Casters.FirstOrDefault().Name;
                viewModel.StatusMessage = "Select a caster to view its results.";
                return View(viewModel);
            }

            Caster caster = await _db.Casters.FirstAsync(c => c.Name == id);
            List<BattleReport> casterBattleReports = await _db.BattleReports.Where(br => br.WinningCaster == caster.Name || br.LosingCaster == caster.Name).ToListAsync();
            CasterResult casterResult = CreateCasterResult(caster, casterBattleReports);

            viewModel.Caster = caster.Name;
            viewModel.StatusMessage = string.Empty;
            viewModel.CasterResult = casterResult;
            viewModel.Themes = CasterEntityResult(StaticDetails.ThemeType, caster.Name, casterBattleReports);
            viewModel.GameSizes = CasterEntityResult(StaticDetails.GameSizeType, caster.Name, casterBattleReports);
            viewModel.Scenarios = CasterEntityResult(StaticDetails.ScenarioType, caster.Name, casterBattleReports);
            viewModel.EndConditions = CasterEntityResult(StaticDetails.EndConditionType, caster.Name, casterBattleReports);
            viewModel.VersusFactions = CasterEntityResult(StaticDetails.VersusFactionType, caster.Name, casterBattleReports);
            viewModel.VersusThemes = CasterEntityResult(StaticDetails.VersusThemeType, caster.Name, casterBattleReports);
            viewModel.VersusCasters = CasterEntityResult(StaticDetails.VersusCasterType, caster.Name, casterBattleReports);

            viewModel.CasterResult.MostPlayedTheme = viewModel.Themes.Results.OrderByDescending(t => t.NumberOfGamesPlayed).FirstOrDefault().Name;
            viewModel.CasterResult.GamesWithMostPlayedTheme = viewModel.Themes.Results.OrderByDescending(t => t.NumberOfGamesPlayed).FirstOrDefault().NumberOfGamesPlayed;
            viewModel.CasterResult.BestPerformingTheme = viewModel.Themes.Results.OrderByDescending(t => t.Winrate).FirstOrDefault().Name;
            viewModel.CasterResult.WinrateBestPerformingTheme = viewModel.Themes.Results.OrderByDescending(t => t.Winrate).FirstOrDefault().Winrate;
            viewModel.CasterResult.MostPlayedScenario = viewModel.Scenarios.Results.OrderByDescending(s => s.NumberOfGamesPlayed).FirstOrDefault().Name;
            viewModel.CasterResult.GamesMostPlayedScenario = viewModel.Scenarios.Results.OrderByDescending(s => s.NumberOfGamesPlayed).FirstOrDefault().NumberOfGamesPlayed;
            viewModel.CasterResult.BestPerformingScenario = viewModel.Scenarios.Results.OrderByDescending(s => s.Winrate).FirstOrDefault().Name;
            viewModel.CasterResult.WinrateBestPerformingScenario = viewModel.Scenarios.Results.OrderByDescending(s => s.Winrate).FirstOrDefault().Winrate;
            viewModel.CasterResult.MostPlayedEndCondition = viewModel.EndConditions.Results.OrderByDescending(ec => ec.NumberOfGamesPlayed).FirstOrDefault().Name;
            viewModel.CasterResult.GamesMostPlayedEndCondition = viewModel.EndConditions.Results.OrderByDescending(ec => ec.NumberOfGamesPlayed).FirstOrDefault().NumberOfGamesPlayed;
            viewModel.CasterResult.BestPerformingEndCondition = viewModel.EndConditions.Results.OrderByDescending(ec => ec.Winrate).FirstOrDefault().Name;
            viewModel.CasterResult.WinrateBestPerformingEndCondition = viewModel.EndConditions.Results.OrderByDescending(ec => ec.Winrate).FirstOrDefault().Winrate;

            return View(viewModel);
        }

        private CasterResult CreateCasterResult(Caster caster, List<BattleReport> battleReports)
        {
            IEnumerable<BattleReport> casterBattleReports = battleReports.Where(br => br.WinningCaster == caster.Name || br.LosingCaster == caster.Name);
            CasterResult casterResult = new CasterResult()
            {
                Name = caster.Name,
                NumberOfGamesPlayed = caster.NumberOfGamesPlayed,
                NumberOfGamesLost = caster.NumberOfGamesLost,
                NumberOfGamesWon = caster.NumberOfGamesWon,
                Winrate = caster.Winrate,
                NumberOfMirrorMatches = casterBattleReports.Where(br => br.WinningCaster == br.LosingCaster).Count()
            };
            List<Theme> playedThemes = new List<Theme>();
            foreach (BattleReport battleReport in casterBattleReports)
            {
                if (battleReport.WinningCaster == caster.Name && battleReport.LosingCaster == caster.Name)
                {
                    if (playedThemes.Any(t => t.Name == battleReport.WinningTheme))
                    {
                        Theme theme = playedThemes.First(t => t.Name == battleReport.WinningTheme);
                        theme.NumberOfGamesPlayed += 2;
                        theme.NumberOfGamesWon++;
                        theme.NumberOfGamesLost++;
                        theme.Winrate = (float)theme.NumberOfGamesWon / (float)theme.NumberOfGamesPlayed;
                    }
                    else
                    {
                        Theme theme = new Theme()
                        {
                            Name = battleReport.WinningCaster,
                            NumberOfGamesPlayed = 2,
                            NumberOfGamesWon = 1,
                            NumberOfGamesLost = 1,
                            Winrate = 0.5f
                        };
                        playedThemes.Add(theme);
                    }
                }
                else if (battleReport.WinningCaster == caster.Name)
                {
                    if (playedThemes.Any(t => t.Name == battleReport.WinningTheme))
                    {
                        Theme theme = playedThemes.First(t => t.Name == battleReport.WinningTheme);
                        theme.NumberOfGamesPlayed++;
                        theme.NumberOfGamesWon++;
                        theme.Winrate = (float)theme.NumberOfGamesWon / (float)theme.NumberOfGamesPlayed;
                    }
                    else
                    {
                        Theme theme = new Theme()
                        {
                            Name = battleReport.WinningTheme,
                            NumberOfGamesPlayed = 1,
                            NumberOfGamesWon = 1,
                            NumberOfGamesLost = 0,
                            Winrate = 1
                        };
                        playedThemes.Add(theme);
                    }
                }
                else if (battleReport.LosingCaster == caster.Name)
                {
                    if (playedThemes.Any(t => t.Name == battleReport.LosingTheme))
                    {
                        Theme theme = playedThemes.First(t => t.Name == battleReport.LosingTheme);
                        theme.NumberOfGamesPlayed++;
                        theme.NumberOfGamesLost++;
                        theme.Winrate = (float)theme.NumberOfGamesWon / (float)theme.NumberOfGamesPlayed;
                    }
                    else
                    {
                        Theme theme = new Theme()
                        {
                            Name = battleReport.LosingTheme,
                            NumberOfGamesPlayed = 1,
                            NumberOfGamesWon = 0,
                            NumberOfGamesLost = 1,
                            Winrate = 0
                        };
                        playedThemes.Add(theme);
                    }
                }
            }

            Theme mostPlayedTheme = playedThemes.OrderByDescending(t => t.NumberOfGamesPlayed).FirstOrDefault();
            casterResult.MostPlayedTheme = mostPlayedTheme.Name;
            casterResult.GamesWithMostPlayedTheme = mostPlayedTheme.NumberOfGamesPlayed;
            Theme bestPerformingTheme = playedThemes.OrderByDescending(t => t.Winrate).FirstOrDefault();
            casterResult.BestPerformingTheme = bestPerformingTheme.Name;
            casterResult.WinrateBestPerformingTheme = bestPerformingTheme.Winrate;
            return casterResult;
        }

        private List<CasterResult> CreateCasterResults(List<Caster> casters, List<BattleReport> battleReports)
        {
            List<CasterResult> casterResults = new List<CasterResult>();
            foreach (Caster caster in casters)
                casterResults.Add(CreateCasterResult(caster, battleReports));
            casterResults = casterResults.OrderByDescending(cr => cr.Winrate).ToList();
            return casterResults;
        }

        private ThemeResult CreateThemeResult(Theme theme, List<BattleReport> battleReports)
        {
            IEnumerable<BattleReport> themeBattleReports = battleReports.Where(br => br.WinningTheme == theme.Name || br.LosingTheme == theme.Name);
            ThemeResult themeResult = new ThemeResult()
            {
                Name = theme.Name,
                NumberOfGamesPlayed = theme.NumberOfGamesPlayed,
                NumberOfGamesLost = theme.NumberOfGamesLost,
                NumberOfGamesWon = theme.NumberOfGamesWon,
                Winrate = theme.Winrate,
                NumberOfMirrorMatches = themeBattleReports.Where(br => br.WinningTheme == br.LosingTheme).Count()
            };
            List<Caster> playedCasters = new List<Caster>();
            foreach (BattleReport battleReport in themeBattleReports)
            {
                if (battleReport.WinningTheme == theme.Name && battleReport.LosingTheme == theme.Name)
                {
                    if (playedCasters.Any(c => c.Name == battleReport.WinningCaster))
                    {
                        Caster caster = playedCasters.First(c => c.Name == battleReport.WinningCaster);
                        caster.NumberOfGamesPlayed += 2;
                        caster.NumberOfGamesWon++;
                        caster.NumberOfGamesLost++;
                        caster.Winrate = (float)caster.NumberOfGamesWon / (float)caster.NumberOfGamesPlayed;
                    }
                    else
                    {
                        Caster caster = new Caster()
                        {
                            Name = battleReport.WinningCaster,
                            NumberOfGamesPlayed = 2,
                            NumberOfGamesWon = 1,
                            NumberOfGamesLost = 1,
                            Winrate = 0.5f
                        };
                        playedCasters.Add(caster);
                    }
                }
                else if (battleReport.WinningTheme == theme.Name)
                {
                    if (playedCasters.Any(c => c.Name == battleReport.WinningCaster))
                    {
                        Caster caster = playedCasters.First(c => c.Name == battleReport.WinningCaster);
                        caster.NumberOfGamesPlayed++;
                        caster.NumberOfGamesWon++;
                        caster.Winrate = (float)caster.NumberOfGamesWon / (float)caster.NumberOfGamesPlayed;
                    }
                    else
                    {
                        Caster caster = new Caster()
                        {
                            Name = battleReport.WinningCaster,
                            NumberOfGamesPlayed = 1,
                            NumberOfGamesWon = 1,
                            NumberOfGamesLost = 0,
                            Winrate = 1
                        };
                        playedCasters.Add(caster);
                    }
                }
                else if (battleReport.LosingTheme == theme.Name)
                {
                    if (playedCasters.Any(c => c.Name == battleReport.LosingCaster))
                    {
                        Caster caster = playedCasters.First(c => c.Name == battleReport.LosingCaster);
                        caster.NumberOfGamesPlayed++;
                        caster.NumberOfGamesLost++;
                        caster.Winrate = (float)caster.NumberOfGamesWon / (float)caster.NumberOfGamesPlayed;
                    }
                    else
                    {
                        Caster caster = new Caster()
                        {
                            Name = battleReport.LosingCaster,
                            NumberOfGamesPlayed = 1,
                            NumberOfGamesWon = 0,
                            NumberOfGamesLost = 1,
                            Winrate = 0
                        };
                        playedCasters.Add(caster);
                    }
                }
            }
            Caster mostPlayedCaster = playedCasters.OrderByDescending(c => c.NumberOfGamesPlayed).FirstOrDefault();
            themeResult.MostPlayedCaster = mostPlayedCaster.Name;
            themeResult.GamesWithMostPlayedCaster = mostPlayedCaster.NumberOfGamesPlayed;
            Caster bestPerformingCaster = playedCasters.OrderByDescending(c => c.Winrate).FirstOrDefault();
            themeResult.BestPerformingCaster = bestPerformingCaster.Name;
            themeResult.WinrateBestPerformingCaster = bestPerformingCaster.Winrate;
            return themeResult;
        }

        private List<ThemeResult> CreateThemeResults(List<Theme> themes, List<BattleReport> battleReports)
        {
            List<ThemeResult> themeResults = new List<ThemeResult>();
            foreach (Theme theme in themes)
                themeResults.Add(CreateThemeResult(theme, battleReports));
            themeResults = themeResults.OrderByDescending(tr => tr.Winrate).ToList();
            return themeResults;
        }

        private FactionResult CreateFactionResult(Faction faction, List<Theme> factionThemes, List<Caster> factionCasters, int numberOfMirrorMatches)
        {
            FactionResult factionResult = new FactionResult()
            {
                Name = faction.Name,
                NumberOfGamesPlayed = faction.NumberOfGamesPlayed,
                NumberOfGamesLost = faction.NumberOfGamesLost,
                NumberOfGamesWon = faction.NumberOfGamesWon,
                Winrate = faction.Winrate,
                NumberOfMirrorMatches = numberOfMirrorMatches
            };
            Theme mostPlayedTheme = factionThemes.OrderByDescending(t => t.NumberOfGamesPlayed).FirstOrDefault();
            factionResult.MostPlayedTheme = mostPlayedTheme.Name;
            factionResult.GamesWithMostPlayedTheme = mostPlayedTheme.NumberOfGamesPlayed;
            Theme bestPerformingTheme = factionThemes.OrderByDescending(t => t.Winrate).FirstOrDefault();
            factionResult.BestPerformingTheme = bestPerformingTheme.Name;
            factionResult.WinrateBestPerformingTheme = bestPerformingTheme.Winrate;
            Caster mostPlayedCaster = factionCasters.OrderByDescending(c => c.NumberOfGamesPlayed).FirstOrDefault();
            factionResult.MostPlayedCaster = mostPlayedCaster.Name;
            factionResult.GamesWithMostPlayedCaster = mostPlayedCaster.NumberOfGamesPlayed;
            Caster bestPerformingCaster = factionCasters.OrderByDescending(c => c.Winrate).FirstOrDefault();
            factionResult.BestPerformingCaster = bestPerformingCaster.Name;
            factionResult.WinrateBestPerformingCaster = bestPerformingCaster.Winrate;
            return factionResult;
        }

        private List<FactionResult> CreateFactionResults(List<Faction> factions, List<Theme> themes, List<Caster> casters, List<BattleReport> battleReports)
        {
            List<FactionResult> factionResults = new List<FactionResult>();
            foreach (Faction faction in factions)
            {
                int numberOfMirrorMatches = battleReports.Where(br => br.WinningFaction == faction.Name && br.LosingFaction == faction.Name).Count();
                List<Theme> factionThemes = themes.Where(t => t.FactionId == faction.Id).ToList();
                List<Caster> factionCasters = casters.Where(c => c.FactionId == faction.Id).ToList();
                factionResults.Add(CreateFactionResult(faction, factionThemes, factionCasters, numberOfMirrorMatches));
            }
            factionResults = factionResults.OrderByDescending(fr => fr.Winrate).ToList();
            return factionResults;
        }

        private EntityResult CasterEntityResult(string type, string caster, List<BattleReport> battleReports)
        {
            EntityResult result = new EntityResult
            {
                Type = type,
                Results = new List<Entity>()
            };
            foreach (BattleReport battleReport in battleReports)
            {
                string entityName = string.Empty;
                switch (result.Type)
                {
                    case StaticDetails.ThemeType: entityName = battleReport.WinningCaster == caster ? battleReport.WinningTheme : battleReport.LosingTheme; break;
                    case StaticDetails.GameSizeType: entityName = battleReport.GameSize.ToString(); break;
                    case StaticDetails.ScenarioType: entityName = battleReport.Scenario; break;
                    case StaticDetails.EndConditionType: entityName = battleReport.EndCondition; break;
                    case StaticDetails.VersusFactionType: entityName = battleReport.WinningCaster == caster ? battleReport.LosingFaction : battleReport.WinningFaction; break;
                    case StaticDetails.VersusThemeType: entityName = battleReport.WinningCaster == caster ? battleReport.LosingTheme : battleReport.WinningTheme; break;
                    case StaticDetails.VersusCasterType: entityName = battleReport.WinningCaster == caster ? battleReport.LosingCaster : battleReport.WinningCaster; break;
                }
                if (result.Results.Any(e => e.Name == entityName))
                {
                    Entity entity = result.Results.Find(e => e.Name == entityName);
                    entity.NumberOfGamesPlayed++;
                    if (battleReport.WinningCaster == caster)
                        entity.NumberOfGamesWon++;
                    else
                        entity.NumberOfGamesLost++;
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                }
                else
                {
                    Entity entity = new Entity()
                    {
                        Name = entityName,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinningCaster == caster ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinningCaster == caster ? 1 : 0
                    };
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                    result.Results.Add(entity);
                }
            }
            result.Results = result.Results.OrderByDescending(e => e.Winrate).ToList();
            return result;
        }

        private EntityResult ThemeEntityResult(string type, string theme, List<BattleReport> battleReports)
        {
            EntityResult result = new EntityResult
            {
                Type = type,
                Results = new List<Entity>()
            };
            foreach (BattleReport battleReport in battleReports)
            {
                string entityName = string.Empty;
                switch (result.Type)
                {
                    case StaticDetails.CasterType: entityName = battleReport.WinningTheme == theme ? battleReport.WinningCaster : battleReport.LosingCaster; break;
                    case StaticDetails.GameSizeType: entityName = battleReport.GameSize.ToString(); break;
                    case StaticDetails.ScenarioType: entityName = battleReport.Scenario; break;
                    case StaticDetails.EndConditionType: entityName = battleReport.EndCondition; break;
                    case StaticDetails.VersusFactionType: entityName = battleReport.WinningTheme == theme ? battleReport.LosingFaction : battleReport.WinningFaction; break;
                    case StaticDetails.VersusThemeType: entityName = battleReport.WinningTheme == theme ? battleReport.LosingTheme : battleReport.WinningTheme; break;
                    case StaticDetails.VersusCasterType: entityName = battleReport.WinningTheme == theme ? battleReport.LosingCaster : battleReport.WinningCaster; break;
                }
                if (result.Results.Any(e => e.Name == entityName))
                {
                    Entity entity = result.Results.Find(e => e.Name == entityName);
                    entity.NumberOfGamesPlayed++;
                    if (battleReport.WinningTheme == theme)
                        entity.NumberOfGamesWon++;
                    else
                        entity.NumberOfGamesLost++;
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                }
                else
                {
                    Entity entity = new Entity()
                    {
                        Name = entityName,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinningTheme == theme ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinningTheme == theme ? 1 : 0
                    };
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                    result.Results.Add(entity);
                }
            }
            result.Results = result.Results.OrderByDescending(e => e.Winrate).ToList();
            return result;
        }

        private EntityResult FactionEntityResult(string type, string faction, List<BattleReport> battleReports)
        {
            EntityResult result = new EntityResult
            {
                Type = type,
                Results = new List<Entity>()
            };
            foreach (BattleReport battleReport in battleReports)
            {
                string entityName = string.Empty;
                switch (result.Type)
                {
                    case StaticDetails.ThemeType: entityName = battleReport.WinningFaction == faction ? battleReport.WinningTheme : battleReport.LosingTheme; break;
                    case StaticDetails.CasterType: entityName = battleReport.WinningFaction == faction ? battleReport.WinningCaster : battleReport.LosingCaster; break;
                    case StaticDetails.GameSizeType: entityName = battleReport.GameSize.ToString(); break;
                    case StaticDetails.ScenarioType: entityName = battleReport.Scenario; break;
                    case StaticDetails.EndConditionType: entityName = battleReport.EndCondition; break;
                    case StaticDetails.VersusFactionType: entityName = battleReport.WinningFaction == faction ? battleReport.LosingFaction : battleReport.WinningFaction; break;
                    case StaticDetails.VersusThemeType: entityName = battleReport.WinningFaction == faction ? battleReport.LosingTheme : battleReport.WinningTheme; break;
                    case StaticDetails.VersusCasterType: entityName = battleReport.WinningFaction == faction ? battleReport.LosingCaster : battleReport.WinningCaster; break;
                }
                if (result.Results.Any(e => e.Name == entityName))
                {
                    Entity entity = result.Results.Find(e => e.Name == entityName);
                    entity.NumberOfGamesPlayed++;
                    if (battleReport.WinningFaction == faction)
                        entity.NumberOfGamesWon++;
                    else
                        entity.NumberOfGamesLost++;
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                }
                else
                {
                    Entity entity = new Entity()
                    {
                        Name = entityName,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinningFaction == faction ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinningFaction == faction ? 1 : 0
                    };
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                    result.Results.Add(entity);
                }
            }
            result.Results = result.Results.OrderByDescending(e => e.Winrate).ToList();
            return result;
        }

        private EntityResult UserEntityResult(string type, string username, List<BattleReport> battleReports)
        {
            EntityResult result = new EntityResult
            {
                Type = type,
                Results = new List<Entity>()
            };
            foreach (BattleReport battleReport in battleReports)
            {
                string entityName = string.Empty;
                switch (result.Type)
                {
                    case StaticDetails.FactionType: entityName = battleReport.PostersUsername == username ? battleReport.PostersFaction : battleReport.OpponentsFaction; break;
                    case StaticDetails.ThemeType: entityName = battleReport.PostersUsername == username ? battleReport.PostersTheme : battleReport.OpponentsTheme; break;
                    case StaticDetails.CasterType: entityName = battleReport.PostersUsername == username ? battleReport.PostersCaster : battleReport.OpponentsCaster; break;
                    case StaticDetails.GameSizeType: entityName = battleReport.GameSize.ToString(); break;
                    case StaticDetails.ScenarioType: entityName = battleReport.Scenario; break;
                    case StaticDetails.EndConditionType: entityName = battleReport.EndCondition; break;
                    case StaticDetails.VersusFactionType: entityName = battleReport.PostersUsername == username ? battleReport.OpponentsFaction : battleReport.PostersFaction; break;
                    case StaticDetails.VersusThemeType: entityName = battleReport.PostersUsername == username ? battleReport.OpponentsTheme : battleReport.PostersTheme; break;
                    case StaticDetails.VersusCasterType: entityName = battleReport.PostersUsername == username ? battleReport.OpponentsCaster : battleReport.PostersCaster; break;
                }
                if (result.Results.Any(e => e.Name == entityName))
                {
                    Entity entity = result.Results.Find(e => e.Name == entityName);
                    entity.NumberOfGamesPlayed++;
                    if (battleReport.WinnersUsername == username)
                        entity.NumberOfGamesWon++;
                    else
                        entity.NumberOfGamesLost++;
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                }
                else
                {
                    Entity entity = new Entity()
                    {
                        Name = entityName,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinnersUsername == username ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinnersUsername == username ? 1 : 0
                    };
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                    result.Results.Add(entity);
                }
            }
            result.Results = result.Results.OrderByDescending(e => e.Winrate).ToList();
            return result;
        }

        private UserResult CreateUserResult(ApplicationUser user, List<BattleReport> battleReports)
        {
            Dictionary<string, int> gamesPlayedWithEachFaction = new Dictionary<string, int>();
            Dictionary<string, int> gamesPlayedWithEachTheme = new Dictionary<string, int>();
            Dictionary<string, int> gamesPlayedWithEachCaster = new Dictionary<string, int>();

            Dictionary<string, int> gamesWonWithEachFaction = new Dictionary<string, int>();
            Dictionary<string, int> gamesWonWithEachTheme = new Dictionary<string, int>();
            Dictionary<string, int> gamesWonWithEachCaster = new Dictionary<string, int>();

            CollectBattleReportsData(user, battleReports, gamesPlayedWithEachFaction, gamesPlayedWithEachTheme, gamesPlayedWithEachCaster,
                                     gamesWonWithEachFaction, gamesWonWithEachTheme, gamesWonWithEachCaster);

            string mostPlayedFaction = FindTopResult(gamesPlayedWithEachFaction, out int gamesWithMostPlayedFaction);
            string mostPlayedTheme = FindTopResult(gamesPlayedWithEachTheme, out int gamesWithMostPlayedTheme);
            string mostPlayedCaster = FindTopResult(gamesPlayedWithEachCaster, out int gamesWithMostPlayedCaster);

            Dictionary<string, float> winrateWithEachFaction = new Dictionary<string, float>();
            Dictionary<string, float> winrateWithEachTheme = new Dictionary<string, float>();
            Dictionary<string, float> winrateWithEachCaster = new Dictionary<string, float>();

            FindWinRate(gamesPlayedWithEachFaction, gamesWonWithEachFaction, winrateWithEachFaction);
            FindWinRate(gamesPlayedWithEachTheme, gamesWonWithEachTheme, winrateWithEachTheme);
            FindWinRate(gamesPlayedWithEachCaster, gamesWonWithEachCaster, winrateWithEachCaster);

            string bestPerformingFaction = FindTopResult(winrateWithEachFaction, out float winrateBestPerformingFaction);
            string bestPerformingTheme = FindTopResult(winrateWithEachTheme, out float winrateBestPerformingTheme);
            string bestPerformingCaster = FindTopResult(winrateWithEachCaster, out float winrateBestPerformingCaster);

            UserResult userResult = new UserResult()
            {
                Username = user.UserName,
                NumberOfGamesPlayed = user.NumberOfGamesPlayed,
                NumberOfGamesWon = user.NumberOfGamesWon,
                NumberOfGamesLost = user.NumberOfGamesLost,
                Winrate = user.Winrate,
                MostPlayedFaction = mostPlayedFaction,
                GamesWithMostPlayedFaction = gamesWithMostPlayedFaction,
                MostPlayedTheme = mostPlayedTheme,
                GamesWithMostPlayedTheme = gamesWithMostPlayedTheme,
                MostPlayedCaster = mostPlayedCaster,
                GamesWithMostPlayedCaster = gamesWithMostPlayedCaster,
                BestPerformingFaction = bestPerformingFaction,
                WinrateBestPerformingFaction = winrateBestPerformingFaction,
                BestPerformingTheme = bestPerformingTheme,
                WinrateBestPerformingTheme = winrateBestPerformingTheme,
                BestPerformingCaster = bestPerformingCaster,
                WinrateBestPerformingCaster = winrateBestPerformingCaster
            };

            return userResult;
        }

        private async Task<List<UserResult>> UserResults(string region)
        {
            List<UserResult> userResults = new List<UserResult>();
            List<ApplicationUser> users = region == StaticDetails.AllRegions ?
                await _db.ApplicationUsers.OrderByDescending(au => au.Winrate).ToListAsync() :
                await _db.ApplicationUsers.Where(au => au.Region == region).OrderByDescending(au => au.Winrate).ToListAsync();

            foreach (ApplicationUser user in users)
            {
                List<BattleReport> usersBattleReports = await _db.BattleReports.Where(br => br.PostersUsername == user.UserName || br.OpponentsUsername == user.UserName).ToListAsync();
                userResults.Add(CreateUserResult(user, usersBattleReports));
            }

            return userResults;
        }

        private void CollectBattleReportsData(ApplicationUser user, List<BattleReport> usersBattleReports,
            Dictionary<string, int> gamesPlayedWithEachFaction, Dictionary<string, int> gamesPlayedWithEachTheme, Dictionary<string, int> gamesPlayedWithEachCaster,
            Dictionary<string, int> gamesWonWithEachFaction, Dictionary<string, int> gamesWonWithEachTheme, Dictionary<string, int> gamesWonWithEachCaster)
        {
            foreach (BattleReport battleReport in usersBattleReports)
            {
                CollectFactionResults(user, gamesPlayedWithEachFaction, gamesWonWithEachFaction, battleReport);
                CollectThemeResults(user, gamesPlayedWithEachTheme, gamesWonWithEachTheme, battleReport);
                CollectCasterResults(user, gamesPlayedWithEachCaster, gamesWonWithEachCaster, battleReport);
            }
        }

        private void CollectFactionResults(ApplicationUser user, Dictionary<string, int> gamesPlayedWithEachFaction, Dictionary<string, int> gamesWonWithEachFaction, BattleReport battleReport)
        {
            string faction = user.UserName == battleReport.PostersUsername ? battleReport.PostersFaction : battleReport.OpponentsFaction;
            if (gamesPlayedWithEachFaction.ContainsKey(faction))
                gamesPlayedWithEachFaction[faction]++;
            else
                gamesPlayedWithEachFaction.Add(faction, 1);

            if (faction == battleReport.WinningFaction)
            {
                if (gamesWonWithEachFaction.ContainsKey(faction))
                    gamesWonWithEachFaction[faction]++;
                else
                    gamesWonWithEachFaction.Add(faction, 1);
            }
        }

        private void CollectThemeResults(ApplicationUser user, Dictionary<string, int> gamesPlayedWithEachTheme, Dictionary<string, int> gamesWonWithEachTheme, BattleReport battleReport)
        {
            string theme = user.UserName == battleReport.PostersUsername ? battleReport.PostersTheme : battleReport.OpponentsTheme;
            if (gamesPlayedWithEachTheme.ContainsKey(theme))
                gamesPlayedWithEachTheme[theme]++;
            else
                gamesPlayedWithEachTheme.Add(theme, 1);

            if (theme == battleReport.WinningTheme)
            {
                if (gamesWonWithEachTheme.ContainsKey(theme))
                    gamesWonWithEachTheme[theme]++;
                else
                    gamesWonWithEachTheme.Add(theme, 1);
            }
        }

        private void CollectCasterResults(ApplicationUser user, Dictionary<string, int> gamesPlayedWithEachCaster, Dictionary<string, int> gamesWonWithEachCaster, BattleReport battleReport)
        {
            string caster = user.UserName == battleReport.PostersUsername ? battleReport.PostersCaster : battleReport.OpponentsCaster;
            if (gamesPlayedWithEachCaster.ContainsKey(caster))
                gamesPlayedWithEachCaster[caster]++;
            else
                gamesPlayedWithEachCaster.Add(caster, 1);

            if (caster == battleReport.WinningCaster)
            {
                if (gamesWonWithEachCaster.ContainsKey(caster))
                    gamesWonWithEachCaster[caster]++;
                else
                    gamesWonWithEachCaster.Add(caster, 1);
            }
        }

        private void FindWinRate(Dictionary<string, int> gamesPlayedWithEachItem, Dictionary<string, int> gamesWonWithEachItem, Dictionary<string, float> winrateWithEachItem)
        {
            foreach (var keyValuePair in gamesPlayedWithEachItem)
            {
                string item = keyValuePair.Key;
                float gamesPlayed = keyValuePair.Value;
                float gamesWon = gamesWonWithEachItem.ContainsKey(item) ? gamesWonWithEachItem[item] : 0;
                winrateWithEachItem.Add(item, gamesWon / gamesPlayed);
            }
        }

        private string FindTopResult<T>(Dictionary<string, T> results, out T value)
        {
            value = results.Values.Max();
            return results.First(kvp => kvp.Value.Equals(results.Values.Max())).Key;
        }
    }
}