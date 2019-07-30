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
                Top10Factions = await _db.Factions.Where(f => f.NumberOfGamesPlayed > 0).OrderByDescending(f => f.Winrate).ThenBy(f => f.Name).Take(10).ToListAsync(),
                Top10Themes = await _db.Themes.Where(t => t.NumberOfGamesPlayed > 0).OrderByDescending(t => t.Winrate).ThenBy(t => t.Name).Take(10).ToListAsync(),
                Top10Casters = await _db.Casters.Where(c => c.NumberOfGamesPlayed > 0).OrderByDescending(c => c.Winrate).ThenBy(c => c.Name).Take(10).ToListAsync(),
                Top10Users = await _db.ApplicationUsers.Where(au => au.NumberOfGamesPlayed > 0).OrderByDescending(au => au.Winrate).ThenBy(au => au.UserName).Take(10).ToListAsync()
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
            List<BattleReport> battleReports = await _db.BattleReports.Where(br => br.ConfirmedByOpponent && (br.PostersUsername == user.UserName || br.OpponentsUsername == user.UserName)).OrderByDescending(br => br.DatePlayed).ToListAsync();
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
            List<Faction> factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
            List<Theme> themes = await _db.Themes.ToListAsync();
            List<Caster> casters = await _db.Casters.ToListAsync();
            List<BattleReport> battleReports = await _db.BattleReports.Where(br => br.ConfirmedByOpponent).OrderByDescending(br => br.DatePlayed).ToListAsync();
            List<FactionResult> factionResults = CreateFactionResults(factions, themes, casters, battleReports);
            return View(factionResults);
        }

        public async Task<IActionResult> Faction(string id)
        {
            FactionResultViewModel viewModel = new FactionResultViewModel
            {
                Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync()
            };
            if (!await _db.Factions.AnyAsync(f => f.Name == id))
            {
                viewModel.Faction = viewModel.Factions.FirstOrDefault().Name;
                viewModel.StatusMessage = "Select a faction to view its results.";
                return View(viewModel);
            }

            Faction faction = await _db.Factions.FirstAsync(f => f.Name == id);
            List<Theme> factionThemes = await _db.Themes.Where(t => t.FactionId == faction.Id).OrderBy(t => t.Name).ToListAsync();
            List<Caster> factionCasters = await _db.Casters.Where(c => c.FactionId == faction.Id).OrderBy(c => c.Name).ToListAsync();
            List<BattleReport> factionBattleReports = await _db.BattleReports.Where(br => br.ConfirmedByOpponent && (br.WinningFaction == faction.Name || br.LosingFaction == faction.Name)).OrderByDescending(br => br.DatePlayed).ToListAsync();
            FactionResult factionResult = CreateFactionResult(faction, factionThemes, factionCasters, factionBattleReports);

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

        public async Task<IActionResult> Themes(string id)
        {
            ThemeResultsViewModel viewModel = new ThemeResultsViewModel();
            List<Faction> factions = await _db.Factions.ToListAsync();
            List<string> factionOptions = new List<string> { StaticDetails.AllFactions };
            List<string> factionNames = factions.Select(f => f.Name).OrderBy(f => f).ToList();
            foreach (string factionName in factionNames)
                factionOptions.Add(factionName);
            viewModel.FactionOptions = factionOptions;
            if (string.IsNullOrWhiteSpace(id) || !viewModel.FactionOptions.Contains(id))
                viewModel.Faction = StaticDetails.AllFactions;
            else
                viewModel.Faction = id;

            if (viewModel.Faction == StaticDetails.AllFactions)
            {
                List<Theme> themes = await _db.Themes.OrderBy(t => t.Name).ToListAsync();
                List<BattleReport> battleReports = await _db.BattleReports.Where(br => br.ConfirmedByOpponent).OrderByDescending(br => br.DatePlayed).ToListAsync();
                viewModel.ThemeResults = CreateThemeResults(themes, battleReports);
            }
            else
            {
                Faction faction = factions.First(f => f.Name == id);
                List<Theme> factionThemes = await _db.Themes.Where(t => t.FactionId == faction.Id).OrderBy(t => t.Name).ToListAsync();
                List<BattleReport> factionBattleReports = await _db.BattleReports.Where(br => br.ConfirmedByOpponent && (br.PostersFaction == id || br.OpponentsFaction == id)).OrderByDescending(br => br.DatePlayed).ToListAsync();
                viewModel.ThemeResults = CreateThemeResults(factionThemes, factionBattleReports);
            }
            
            return View(viewModel);
        }

        public async Task<IActionResult> Theme(string id)
        {
            ThemeResultViewModel viewModel = new ThemeResultViewModel
            {
                Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync(),
                Themes = await _db.Themes.OrderBy(t => t.Name).ToListAsync()
            };
            if (!viewModel.Themes.Any(t => t.Name == id))
            {
                viewModel.Faction = viewModel.Factions.FirstOrDefault().Name;
                Faction faction = viewModel.Factions.First(f => f.Name == viewModel.Faction);
                viewModel.Themes = viewModel.Themes.Where(t => t.FactionId == faction.Id).ToList();
                viewModel.Theme = viewModel.Themes.FirstOrDefault().Name;
                viewModel.StatusMessage = "Select a theme to view its results.";
                return View(viewModel);
            }

            Theme theme = viewModel.Themes.First(t => t.Name == id);
            Faction themesFaction = viewModel.Factions.First(f => f.Id == theme.FactionId);
            List<BattleReport> themeBattleReports = await _db.BattleReports.Where(br => br.ConfirmedByOpponent && (br.WinningTheme == theme.Name || br.LosingTheme == theme.Name)).OrderByDescending(br => br.DatePlayed).ToListAsync();
            ThemeResult themeResult = CreateThemeResult(theme, themeBattleReports);

            viewModel.Faction = themesFaction.Name;
            viewModel.Theme = theme.Name;
            viewModel.Themes = viewModel.Themes.Where(t => t.FactionId == themesFaction.Id).ToList();
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

        public async Task<IActionResult> GetFactionThemeNames(string id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Factions.AnyAsync(f => f.Name == id))
                return NotFound();

            Faction faction = await _db.Factions.FirstAsync(f => f.Name == id);
            List<string> factionThemes = await _db.Themes.Where(t => t.FactionId == faction.Id).Select(t => t.Name).OrderBy(t => t).ToListAsync();
            return Json(factionThemes);
        }

        public async Task<IActionResult> GetFactionCasterNames(string id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Factions.AnyAsync(f => f.Name == id))
                return NotFound();

            Faction faction = await _db.Factions.FirstAsync(f => f.Name == id);
            List<string> factionCasters = await _db.Casters.Where(c => c.FactionId == faction.Id).Select(c => c.Name).OrderBy(c => c).ToListAsync();
            return Json(factionCasters);
        }

        public async Task<IActionResult> Casters(string id)
        {
            CasterResultsViewModel viewModel = new CasterResultsViewModel();
            List<Faction> factions = await _db.Factions.ToListAsync();
            List<string> factionOptions = new List<string> { StaticDetails.AllFactions };
            List<string> factionNames = factions.Select(f => f.Name).OrderBy(f => f).ToList();
            foreach (string factionName in factionNames)
                factionOptions.Add(factionName);
            viewModel.FactionOptions = factionOptions;
            if (string.IsNullOrWhiteSpace(id) || !viewModel.FactionOptions.Contains(id))
                viewModel.Faction = StaticDetails.AllFactions;
            else
                viewModel.Faction = id;

            if (viewModel.Faction == StaticDetails.AllFactions)
            {
                List<Caster> casters = await _db.Casters.OrderBy(c => c.Name).ToListAsync();
                List<BattleReport> battleReports = await _db.BattleReports.Where(br => br.ConfirmedByOpponent).OrderByDescending(br => br.DatePlayed).ToListAsync();
                viewModel.CasterResults = CreateCasterResults(casters, battleReports);
            }
            else
            {
                Faction faction = factions.First(f => f.Name == id);
                List<Caster> factionCasters = await _db.Casters.Where(c => c.FactionId == faction.Id).OrderBy(c => c.Name).ToListAsync();
                List<BattleReport> factionBattleReports = await _db.BattleReports.Where(br => br.ConfirmedByOpponent && (br.PostersFaction == id || br.OpponentsFaction == id)).OrderByDescending(br => br.DatePlayed).ToListAsync();
                viewModel.CasterResults = CreateCasterResults(factionCasters, factionBattleReports);
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Caster(string id)
        {
            CasterResultViewModel viewModel = new CasterResultViewModel
            {
                Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync(),
                Casters = await _db.Casters.OrderBy(c => c.Name).ToListAsync()
            };
            if (!viewModel.Casters.Any(c => c.Name == id))
            {
                viewModel.Faction = viewModel.Factions.FirstOrDefault().Name;
                Faction faction = viewModel.Factions.First(f => f.Name == viewModel.Faction);
                viewModel.Casters = viewModel.Casters.Where(c => c.FactionId == faction.Id).ToList();
                viewModel.Caster = viewModel.Casters.FirstOrDefault().Name;
                viewModel.StatusMessage = "Select a caster to view its results.";
                return View(viewModel);
            }

            Caster caster = viewModel.Casters.First(c => c.Name == id);
            Faction castersFaction = viewModel.Factions.First(f => f.Id == caster.FactionId);
            List<BattleReport> casterBattleReports = await _db.BattleReports.Where(br => br.ConfirmedByOpponent && (br.WinningCaster == caster.Name || br.LosingCaster == caster.Name)).OrderByDescending(br => br.DatePlayed).ToListAsync();
            CasterResult casterResult = CreateCasterResult(caster, casterBattleReports);

            viewModel.Faction = castersFaction.Name;
            viewModel.Caster = caster.Name;
            viewModel.Casters = viewModel.Casters.Where(c => c.FactionId == castersFaction.Id).ToList();
            viewModel.StatusMessage = string.Empty;
            viewModel.CasterResult = casterResult;
            viewModel.Themes = CasterEntityResult(StaticDetails.ThemeType, caster.Name, casterBattleReports);
            viewModel.GameSizes = CasterEntityResult(StaticDetails.GameSizeType, caster.Name, casterBattleReports);
            viewModel.Scenarios = CasterEntityResult(StaticDetails.ScenarioType, caster.Name, casterBattleReports);
            viewModel.EndConditions = CasterEntityResult(StaticDetails.EndConditionType, caster.Name, casterBattleReports);
            viewModel.VersusFactions = CasterEntityResult(StaticDetails.VersusFactionType, caster.Name, casterBattleReports);
            viewModel.VersusThemes = CasterEntityResult(StaticDetails.VersusThemeType, caster.Name, casterBattleReports);
            viewModel.VersusCasters = CasterEntityResult(StaticDetails.VersusCasterType, caster.Name, casterBattleReports);

            return View(viewModel);
        }

        private CasterResult CreateCasterResult(Caster caster, List<BattleReport> battleReports)
        {
            List<BattleReport> casterBattleReports = battleReports.Where(br => br.WinningCaster == caster.Name || br.LosingCaster == caster.Name).OrderByDescending(br => br.DatePlayed).ToList();
            CasterResult casterResult = new CasterResult()
            {
                Name = caster.Name,
                NumberOfGamesPlayed = caster.NumberOfGamesPlayed,
                NumberOfGamesLost = caster.NumberOfGamesLost,
                NumberOfGamesWon = caster.NumberOfGamesWon,
                Winrate = caster.Winrate,
                NumberOfMirrorMatches = casterBattleReports.Where(br => br.WinningCaster == br.LosingCaster).Count()
            };
            if (casterBattleReports.Count == 0)
            {
                casterResult.MostPlayedTheme = StaticDetails.NoRecords;
                casterResult.BestPerformingTheme = StaticDetails.NoRecords;
                casterResult.MostPlayedScenario = StaticDetails.NoRecords;
                casterResult.BestPerformingScenario = StaticDetails.NoRecords;
                casterResult.MostPlayedEndCondition = StaticDetails.NoRecords;
                casterResult.BestPerformingEndCondition = StaticDetails.NoRecords;
            }
            else
            {
                EntityResult themes = CasterEntityResult(StaticDetails.ThemeType, caster.Name, battleReports);
                EntityResult scenarios = CasterEntityResult(StaticDetails.ScenarioType, caster.Name, casterBattleReports);
                EntityResult endConditions = CasterEntityResult(StaticDetails.EndConditionType, caster.Name, casterBattleReports);
                casterResult.MostPlayedTheme = themes.Results.OrderByDescending(t => t.NumberOfGamesPlayed).FirstOrDefault().Name;
                casterResult.GamesWithMostPlayedTheme = themes.Results.OrderByDescending(t => t.NumberOfGamesPlayed).FirstOrDefault().NumberOfGamesPlayed;
                casterResult.BestPerformingTheme = themes.Results.OrderByDescending(t => t.Winrate).FirstOrDefault().Name;
                casterResult.WinrateBestPerformingTheme = themes.Results.OrderByDescending(t => t.Winrate).FirstOrDefault().Winrate;
                casterResult.MostPlayedScenario = scenarios.Results.OrderByDescending(s => s.NumberOfGamesPlayed).FirstOrDefault().Name;
                casterResult.GamesMostPlayedScenario = scenarios.Results.OrderByDescending(s => s.NumberOfGamesPlayed).FirstOrDefault().NumberOfGamesPlayed;
                casterResult.BestPerformingScenario = scenarios.Results.OrderByDescending(s => s.Winrate).FirstOrDefault().Name;
                casterResult.WinrateBestPerformingScenario = scenarios.Results.OrderByDescending(s => s.Winrate).FirstOrDefault().Winrate;
                casterResult.MostPlayedEndCondition = endConditions.Results.OrderByDescending(ec => ec.NumberOfGamesPlayed).FirstOrDefault().Name;
                casterResult.GamesMostPlayedEndCondition = endConditions.Results.OrderByDescending(ec => ec.NumberOfGamesPlayed).FirstOrDefault().NumberOfGamesPlayed;
                casterResult.BestPerformingEndCondition = endConditions.Results.OrderByDescending(ec => ec.Winrate).FirstOrDefault().Name;
                casterResult.WinrateBestPerformingEndCondition = endConditions.Results.OrderByDescending(ec => ec.Winrate).FirstOrDefault().Winrate;
            }
            return casterResult;
        }

        private List<CasterResult> CreateCasterResults(List<Caster> casters, List<BattleReport> battleReports)
        {
            List<CasterResult> casterResults = new List<CasterResult>();
            foreach (Caster caster in casters)
                casterResults.Add(CreateCasterResult(caster, battleReports));
            return casterResults.OrderByDescending(cr => cr.Winrate).ToList();
        }

        private ThemeResult CreateThemeResult(Theme theme, List<BattleReport> battleReports)
        {
            List<BattleReport> themeBattleReports = battleReports.Where(br => br.WinningTheme == theme.Name || br.LosingTheme == theme.Name).OrderByDescending(br => br.DatePlayed).ToList();
            ThemeResult themeResult = new ThemeResult()
            {
                Name = theme.Name,
                NumberOfGamesPlayed = theme.NumberOfGamesPlayed,
                NumberOfGamesLost = theme.NumberOfGamesLost,
                NumberOfGamesWon = theme.NumberOfGamesWon,
                Winrate = theme.Winrate,
                NumberOfMirrorMatches = themeBattleReports.Where(br => br.WinningTheme == br.LosingTheme).Count()
            };
            if (themeBattleReports.Count == 0)
            {
                themeResult.MostPlayedCaster = StaticDetails.NoRecords;
                themeResult.BestPerformingCaster = StaticDetails.NoRecords;
            }
            else
            {
                EntityResult casters = ThemeEntityResult(StaticDetails.CasterType, theme.Name, battleReports);
                Entity mostPlayedCaster = casters.Results.OrderByDescending(c => c.NumberOfGamesPlayed).FirstOrDefault();
                themeResult.MostPlayedCaster = mostPlayedCaster.Name;
                themeResult.GamesWithMostPlayedCaster = mostPlayedCaster.NumberOfGamesPlayed;
                Entity bestPerformingCaster = casters.Results.OrderByDescending(c => c.Winrate).FirstOrDefault();
                themeResult.BestPerformingCaster = bestPerformingCaster.Name;
                themeResult.WinrateBestPerformingCaster = bestPerformingCaster.Winrate;
            }
            return themeResult;
        }

        private List<ThemeResult> CreateThemeResults(List<Theme> themes, List<BattleReport> battleReports)
        {
            List<ThemeResult> themeResults = new List<ThemeResult>();
            foreach (Theme theme in themes)
                themeResults.Add(CreateThemeResult(theme, battleReports));
            return themeResults.OrderByDescending(tr => tr.Winrate).ToList();
        }

        private FactionResult CreateFactionResult(Faction faction, List<Theme> factionThemes, List<Caster> factionCasters, List<BattleReport> factionBattleReports)
        {
            FactionResult factionResult = new FactionResult()
            {
                Name = faction.Name,
                NumberOfGamesPlayed = faction.NumberOfGamesPlayed,
                NumberOfGamesLost = faction.NumberOfGamesLost,
                NumberOfGamesWon = faction.NumberOfGamesWon,
                Winrate = faction.Winrate,
                NumberOfMirrorMatches = factionBattleReports.Where(br => br.WinningFaction == br.LosingFaction).Count()
            };
            if (factionBattleReports.Count == 0)
            {
                factionResult.MostPlayedTheme = StaticDetails.NoRecords;
                factionResult.BestPerformingTheme = StaticDetails.NoRecords;
                factionResult.MostPlayedCaster = StaticDetails.NoRecords;
                factionResult.BestPerformingCaster = StaticDetails.NoRecords;
            }
            else
            {
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
            }
            return factionResult;
        }

        private List<FactionResult> CreateFactionResults(List<Faction> factions, List<Theme> themes, List<Caster> casters, List<BattleReport> battleReports)
        {
            List<FactionResult> factionResults = new List<FactionResult>();
            foreach (Faction faction in factions)
            {
                List<Theme> factionThemes = themes.Where(t => t.FactionId == faction.Id).OrderBy(t => t.Name).ToList();
                List<Caster> factionCasters = casters.Where(c => c.FactionId == faction.Id).OrderBy(c => c.Name).ToList();
                List<BattleReport> factionBattleReports = battleReports.Where(br => br.ConfirmedByOpponent && (br.WinningFaction == faction.Name || br.LosingFaction == faction.Name)).OrderByDescending(br => br.DatePlayed).ToList();
                factionResults.Add(CreateFactionResult(faction, factionThemes, factionCasters, factionBattleReports));
            }
            return factionResults.OrderByDescending(fr => fr.Winrate).ToList();
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
                    entity.NumberOfGamesPlayed = battleReport.WinningCaster == battleReport.LosingCaster ? entity.NumberOfGamesPlayed + 2 : entity.NumberOfGamesPlayed + 1;
                    entity.NumberOfGamesLost = battleReport.LosingCaster == caster ? entity.NumberOfGamesLost + 1: entity.NumberOfGamesLost;
                    entity.NumberOfGamesWon = battleReport.WinningCaster == caster ? entity.NumberOfGamesWon + 1 : entity.NumberOfGamesWon;
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                }
                else
                {
                    Entity entity = new Entity()
                    {
                        Name = entityName,
                        NumberOfGamesPlayed = battleReport.WinningCaster == battleReport.LosingCaster ? 2 : 1,
                        NumberOfGamesLost = battleReport.LosingCaster == caster ? 1 : 0,
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
                    entity.NumberOfGamesPlayed = battleReport.WinningTheme == battleReport.LosingTheme ? entity.NumberOfGamesPlayed + 2 : entity.NumberOfGamesPlayed + 1;
                    entity.NumberOfGamesLost = battleReport.LosingTheme == theme ? entity.NumberOfGamesLost + 1 : entity.NumberOfGamesLost;
                    entity.NumberOfGamesWon = battleReport.WinningTheme == theme ? entity.NumberOfGamesWon + 1 : entity.NumberOfGamesWon;
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                }
                else
                {
                    Entity entity = new Entity()
                    {
                        Name = entityName,
                        NumberOfGamesPlayed = battleReport.WinningTheme == battleReport.LosingTheme ? 2 : 1,
                        NumberOfGamesLost = battleReport.LosingTheme == theme ? 1 : 0,
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
                    entity.NumberOfGamesPlayed = battleReport.WinningFaction == battleReport.LosingFaction ? entity.NumberOfGamesPlayed + 2 : entity.NumberOfGamesPlayed + 1;
                    entity.NumberOfGamesLost = battleReport.LosingFaction == faction ? entity.NumberOfGamesLost + 1 : entity.NumberOfGamesLost;
                    entity.NumberOfGamesWon = battleReport.WinningFaction == faction ? entity.NumberOfGamesWon + 1 : entity.NumberOfGamesWon;
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                }
                else
                {
                    Entity entity = new Entity()
                    {
                        Name = entityName,
                        NumberOfGamesPlayed = battleReport.WinningFaction == battleReport.LosingFaction ? 2 : 1,
                        NumberOfGamesLost = battleReport.LosingFaction == faction ? 1 : 0,
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
                    entity.NumberOfGamesLost = battleReport.LosersUsername == username ? entity.NumberOfGamesLost + 1 : entity.NumberOfGamesLost;
                    entity.NumberOfGamesWon = battleReport.WinnersUsername == username ? entity.NumberOfGamesWon + 1 : entity.NumberOfGamesWon;
                    entity.Winrate = (float)entity.NumberOfGamesWon / (float)entity.NumberOfGamesPlayed;
                }
                else
                {
                    Entity entity = new Entity()
                    {
                        Name = entityName,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.LosersUsername == username ? 1 : 0,
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
            UserResult userResult = new UserResult()
            {
                Username = user.UserName,
                NumberOfGamesPlayed = user.NumberOfGamesPlayed,
                NumberOfGamesWon = user.NumberOfGamesWon,
                NumberOfGamesLost = user.NumberOfGamesLost,
                Winrate = user.Winrate,
            };
            if (battleReports.Count == 0)
            {
                userResult.MostPlayedFaction = StaticDetails.NoRecords;
                userResult.MostPlayedTheme = StaticDetails.NoRecords;
                userResult.MostPlayedCaster = StaticDetails.NoRecords;
                userResult.BestPerformingFaction = StaticDetails.NoRecords;
                userResult.BestPerformingTheme = StaticDetails.NoRecords;
                userResult.BestPerformingCaster = StaticDetails.NoRecords;
            }
            else
            {
                EntityResult factionResult = UserEntityResult(StaticDetails.FactionType, user.UserName, battleReports);
                EntityResult themeResult = UserEntityResult(StaticDetails.ThemeType, user.UserName, battleReports);
                EntityResult casterResult = UserEntityResult(StaticDetails.CasterType, user.UserName, battleReports);
                userResult.MostPlayedFaction = factionResult.Results.OrderByDescending(e => e.NumberOfGamesPlayed).FirstOrDefault().Name;
                userResult.GamesWithMostPlayedFaction = factionResult.Results.OrderByDescending(e => e.NumberOfGamesPlayed).FirstOrDefault().NumberOfGamesPlayed;
                userResult.MostPlayedTheme = themeResult.Results.OrderByDescending(e => e.NumberOfGamesPlayed).FirstOrDefault().Name;
                userResult.GamesWithMostPlayedTheme = themeResult.Results.OrderByDescending(e => e.NumberOfGamesPlayed).FirstOrDefault().NumberOfGamesPlayed;
                userResult.MostPlayedCaster = casterResult.Results.OrderByDescending(e => e.NumberOfGamesPlayed).FirstOrDefault().Name;
                userResult.GamesWithMostPlayedCaster = casterResult.Results.OrderByDescending(e => e.NumberOfGamesPlayed).FirstOrDefault().NumberOfGamesPlayed;
                userResult.BestPerformingFaction = factionResult.Results.OrderByDescending(e => e.Winrate).FirstOrDefault().Name;
                userResult.WinrateBestPerformingFaction = factionResult.Results.OrderByDescending(e => e.Winrate).FirstOrDefault().Winrate;
                userResult.BestPerformingTheme = themeResult.Results.OrderByDescending(e => e.Winrate).FirstOrDefault().Name;
                userResult.WinrateBestPerformingTheme = themeResult.Results.OrderByDescending(e => e.Winrate).FirstOrDefault().Winrate;
                userResult.BestPerformingCaster = casterResult.Results.OrderByDescending(e => e.Winrate).FirstOrDefault().Name;
                userResult.WinrateBestPerformingCaster = casterResult.Results.OrderByDescending(e => e.Winrate).FirstOrDefault().Winrate;
            }
            return userResult;
        }

        private async Task<List<UserResult>> UserResults(string region)
        {
            List<UserResult> userResults = new List<UserResult>();
            List<ApplicationUser> users = region == StaticDetails.AllRegions ?
                await _db.ApplicationUsers.Where(au => au.NumberOfGamesPlayed > 0).OrderByDescending(au => au.Winrate).ThenBy(au => au.UserName).ToListAsync() :
                await _db.ApplicationUsers.Where(au => au.NumberOfGamesPlayed > 0 && au.Region == region).OrderByDescending(au => au.Winrate).ThenBy(au => au.UserName).ToListAsync();
            foreach (ApplicationUser user in users)
            {
                List<BattleReport> usersBattleReports = await _db.BattleReports.Where(br => br.PostersUsername == user.UserName || br.OpponentsUsername == user.UserName).OrderByDescending(br => br.DatePlayed).ToListAsync();
                userResults.Add(CreateUserResult(user, usersBattleReports));
            }
            return userResults;
        }
    }
}