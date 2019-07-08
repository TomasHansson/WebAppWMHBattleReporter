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
                Factions = EntityResult(StaticDetails.FactionType, user.UserName, battleReports),
                Themes = EntityResult(StaticDetails.ThemeType, user.UserName, battleReports),
                Casters = EntityResult(StaticDetails.CasterType, user.UserName, battleReports),
                GameSizes = EntityResult(StaticDetails.GameSizeType, user.UserName, battleReports),
                Scenarios = EntityResult(StaticDetails.ScenarioType, user.UserName, battleReports),
                EndConditions = EntityResult(StaticDetails.EndConditionType, user.UserName, battleReports),
                VersusFactions = EntityResult(StaticDetails.VersusFactionType, user.UserName, battleReports),
                VersusThemes = EntityResult(StaticDetails.VersusThemeType, user.UserName, battleReports),
                VersusCasters = EntityResult(StaticDetails.VersusCasterType, user.UserName, battleReports)
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

        //public async Task<IActionResult> Faction(string id)
        //{

        //}

        private List<FactionResult> CreateFactionResults(List<Faction> factions, List<Theme> themes, List<Caster> casters, List<BattleReport> battleReports)
        {
            List<FactionResult> factionResults = new List<FactionResult>();
            foreach (Faction faction in factions)
            {
                //List<BattleReport> factionsBattleReports = battleReports.Where(br => br.PostersFaction == faction.Name || br.OpponentsFaction == faction.Name).ToList();
                //List<Theme> factionsPlayedThemes = new List<Theme>();
                //List<Caster> factionsPlayedCasters = new List<Caster>();
                int mirrorMatches = battleReports.Where(br => br.WinningFaction == faction.Name && br.LosingFaction == faction.Name).Count();
                FactionResult factionResult = new FactionResult()
                {
                    Name = faction.Name,
                    NumberOfGamesPlayed = faction.NumberOfGamesPlayed,
                    NumberOfGamesLost = faction.NumberOfGamesLost,
                    NumberOfGamesWon = faction.NumberOfGamesWon,
                    Winrate = faction.Winrate,
                    NumberOfMirrorMatches = mirrorMatches
                };
                //foreach (BattleReport battleReport in factionsBattleReports)
                //{
                //    factionResult.NumberOfGamesPlayed++;
                //    if (battleReport.PostersFaction == battleReport.OpponentsFaction)
                //    {
                //        factionResult.NumberOfGamesLost++;
                //        factionResult.NumberOfGamesWon++;
                //        factionResult.NumberOfMirrorMatches++;
                //    }
                //    else
                //    {
                //        if (battleReport.WinningFaction == faction.Name)
                //            factionResult.NumberOfGamesWon++;
                //        else
                //            factionResult.NumberOfGamesLost++;
                //    }
                //    if (battleReport.WinningFaction == faction.Name)
                //    {
                //        if (factionsPlayedThemes.Any(t => t.Name == battleReport.WinningTheme))
                //        {
                //            Theme theme = factionsPlayedThemes.First(t => t.Name == battleReport.WinningTheme);
                //            theme.NumberOfGamesPlayed++;
                //            theme.NumberOfGamesWon++;
                //            theme.Winrate = (float)theme.NumberOfGamesWon / (float)theme.NumberOfGamesPlayed;
                //        }
                //        else
                //        {
                //            Theme theme = new Theme()
                //            {
                //                Name = battleReport.WinningTheme,
                //                NumberOfGamesPlayed = 1,
                //                NumberOfGamesWon = 1,
                //                Winrate = 1
                //            };
                //            factionsPlayedThemes.Add(theme);
                //        }
                //        if (factionsPlayedCasters.Any(c => c.Name == battleReport.WinningCaster))
                //        {
                //            Caster caster = factionsPlayedCasters.First(c => c.Name == battleReport.WinningCaster);
                //            caster.NumberOfGamesPlayed++;
                //            caster.NumberOfGamesWon++;
                //            caster.Winrate = (float)caster.NumberOfGamesWon / (float)caster.NumberOfGamesPlayed;
                //        }
                //        else
                //        {
                //            Caster caster = new Caster()
                //            {
                //                Name = battleReport.WinningCaster,
                //                NumberOfGamesPlayed = 1,
                //                NumberOfGamesWon = 1,
                //                Winrate = 1
                //            };
                //            factionsPlayedCasters.Add(caster);
                //        }
                //    }
                //    if (battleReport.LosingFaction == faction.Name)
                //    {
                //        if (factionsPlayedThemes.Any(t => t.Name == battleReport.LosingTheme))
                //        {
                //            Theme theme = factionsPlayedThemes.First(t => t.Name == battleReport.LosingTheme);
                //            theme.NumberOfGamesPlayed++;
                //            theme.NumberOfGamesLost++;
                //            theme.Winrate = (float)theme.NumberOfGamesWon / (float)theme.NumberOfGamesPlayed;
                //        }
                //        else
                //        {
                //            Theme theme = new Theme()
                //            {
                //                Name = battleReport.LosingTheme,
                //                NumberOfGamesPlayed = 1,
                //                NumberOfGamesLost = 1
                //            };
                //            factionsPlayedThemes.Add(theme);
                //        }
                //        if (factionsPlayedCasters.Any(c => c.Name == battleReport.LosingCaster))
                //        {
                //            Caster caster = factionsPlayedCasters.First(c => c.Name == battleReport.LosingCaster);
                //            caster.NumberOfGamesPlayed++;
                //            caster.NumberOfGamesLost++;
                //            caster.Winrate = (float)caster.NumberOfGamesWon / (float)caster.NumberOfGamesPlayed;
                //        }
                //        else
                //        {
                //            Caster caster = new Caster()
                //            {
                //                Name = battleReport.LosingCaster,
                //                NumberOfGamesPlayed = 1,
                //                NumberOfGamesLost = 1
                //            };
                //            factionsPlayedCasters.Add(caster);
                //        }
                //    }
                //}
                //factionResult.Winrate = (float)(factionResult.NumberOfGamesWon - factionResult.NumberOfMirrorMatches) / (float)(factionResult.NumberOfGamesPlayed - factionResult.NumberOfMirrorMatches);
                //Theme mostPlayedTheme = factionsPlayedThemes.OrderByDescending(t => t.NumberOfGamesPlayed).FirstOrDefault();
                //factionResult.MostPlayedTheme = mostPlayedTheme.Name;
                //factionResult.GamesWithMostPlayedTheme = mostPlayedTheme.NumberOfGamesPlayed;
                //Theme bestPerformingTheme = factionsPlayedThemes.OrderByDescending(t => t.Winrate).FirstOrDefault();
                //factionResult.BestPerformingTheme = bestPerformingTheme.Name;
                //factionResult.WinrateBestPerformingTheme = bestPerformingTheme.Winrate;
                //Caster mostPlayedCaster = factionsPlayedCasters.OrderByDescending(c => c.NumberOfGamesPlayed).FirstOrDefault();
                //factionResult.MostPlayedCaster = mostPlayedCaster.Name;
                //factionResult.GamesWithMostPlayedCaster = mostPlayedCaster.NumberOfGamesPlayed;
                //Caster bestPerformingCaster = factionsPlayedCasters.OrderByDescending(c => c.Winrate).FirstOrDefault();
                //factionResult.BestPerformingCaster = bestPerformingCaster.Name;
                //factionResult.WinrateBestPerformingCaster = bestPerformingCaster.Winrate;
                List<Theme> factionThemes = themes.Where(t => t.FactionId == faction.Id).ToList();
                Theme mostPlayedTheme = factionThemes.OrderByDescending(t => t.NumberOfGamesPlayed).FirstOrDefault();
                factionResult.MostPlayedTheme = mostPlayedTheme.Name;
                factionResult.GamesWithMostPlayedTheme = mostPlayedTheme.NumberOfGamesPlayed;
                Theme bestPerformingTheme = factionThemes.OrderByDescending(t => t.Winrate).FirstOrDefault();
                factionResult.BestPerformingTheme = bestPerformingTheme.Name;
                factionResult.WinrateBestPerformingTheme = bestPerformingTheme.Winrate;
                List<Caster> factionCasters = casters.Where(c => c.FactionId == faction.Id).ToList();
                Caster mostPlayedCaster = factionCasters.OrderByDescending(c => c.NumberOfGamesPlayed).FirstOrDefault();
                factionResult.MostPlayedCaster = mostPlayedCaster.Name;
                factionResult.GamesWithMostPlayedCaster = mostPlayedCaster.NumberOfGamesPlayed;
                Caster bestPerformingCaster = factionCasters.OrderByDescending(c => c.Winrate).FirstOrDefault();
                factionResult.BestPerformingCaster = bestPerformingCaster.Name;
                factionResult.WinrateBestPerformingCaster = bestPerformingCaster.Winrate;
                factionResults.Add(factionResult);
            }
            factionResults = factionResults.OrderByDescending(fr => fr.Winrate).ToList();
            return factionResults;
        }

        private EntityResult EntityResult(string type, string username, List<BattleReport> battleReports)
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
            result.Results = result.Results.OrderByDescending(gs => gs.Winrate).ToList();
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