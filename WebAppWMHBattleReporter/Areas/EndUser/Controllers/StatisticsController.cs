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
                    UserResult = new UserResult(),
                    Factions = new List<Faction>(),
                    Themes = new List<Theme>(),
                    Casters = new List<Caster>()
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
                Factions = FactionResults(user.UserName, battleReports),
                Themes = ThemeResults(user.UserName, battleReports),
                Casters = CasterResults(user.UserName, battleReports),
                GameSizes = GameSizeResults(user.UserName, battleReports),
                Scenarios = ScenarioResults(user.UserName, battleReports),
                EndConditions = EndConditionResults(user.UserName, battleReports),
                VersusFactions = VersusFactionResults(user.UserName, battleReports),
                VersusThemes = VersusThemesResults(user.UserName, battleReports),
                VersusCasters = VersusCasterResults(user.UserName, battleReports)
            };
            return View(viewModel);
        }

        private List<EntityResult> GameSizeResults(string username, List<BattleReport> battleReports)
        {
            List<EntityResult> playedGameSizes = new List<EntityResult>();
            foreach (BattleReport battleReport in battleReports)
            {
                string gameSize = battleReport.GameSize.ToString();
                if (playedGameSizes.Any(gs => gs.Name == gameSize))
                {
                    EntityResult currentGameSize = playedGameSizes.Find(gs => gs.Name == gameSize);
                    currentGameSize.NumberOfGamesPlayed++;
                    if (battleReport.WinnersUsername == username)
                        currentGameSize.NumberOfGamesWon++;
                    else
                        currentGameSize.NumberOfGamesLost++;
                    currentGameSize.Winrate = (float)currentGameSize.NumberOfGamesWon / (float)currentGameSize.NumberOfGamesPlayed;
                }
                else
                {
                    EntityResult currentGameSize = new EntityResult()
                    {
                        Name = gameSize,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinnersUsername == username ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinnersUsername == username ? 1 : 0
                    };
                    currentGameSize.Winrate = (float)currentGameSize.NumberOfGamesWon / (float)currentGameSize.NumberOfGamesPlayed;
                    playedGameSizes.Add(currentGameSize);
                }
            }
            return playedGameSizes.OrderByDescending(gs => gs.Winrate).ToList();
        }

        private List<EntityResult> ScenarioResults(string username, List<BattleReport> battleReports)
        {
            List<EntityResult> playedScenarios = new List<EntityResult>();
            foreach (BattleReport battleReport in battleReports)
            {
                string scenarioName = battleReport.Scenario;
                if (playedScenarios.Any(s => s.Name == scenarioName))
                {
                    EntityResult scenario = playedScenarios.Find(s => s.Name == scenarioName);
                    scenario.NumberOfGamesPlayed++;
                    if (battleReport.WinnersUsername == username)
                        scenario.NumberOfGamesWon++;
                    else
                        scenario.NumberOfGamesLost++;
                    scenario.Winrate = (float)scenario.NumberOfGamesWon / (float)scenario.NumberOfGamesPlayed;
                }
                else
                {
                    EntityResult scenario = new EntityResult()
                    {
                        Name = scenarioName,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinnersUsername == username ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinnersUsername == username ? 1 : 0
                    };
                    scenario.Winrate = (float)scenario.NumberOfGamesWon / (float)scenario.NumberOfGamesPlayed;
                    playedScenarios.Add(scenario);
                }
            }
            return playedScenarios.OrderByDescending(s => s.Winrate).ToList();
        }

        private List<EntityResult> EndConditionResults(string username, List<BattleReport> battleReports)
        {
            List<EntityResult> playedEndConditions = new List<EntityResult>();
            foreach (BattleReport battleReport in battleReports)
            {
                string endConditionName = battleReport.EndCondition;
                if (playedEndConditions.Any(e => e.Name == endConditionName))
                {
                    EntityResult endCondition = playedEndConditions.Find(e => e.Name == endConditionName);
                    endCondition.NumberOfGamesPlayed++;
                    if (battleReport.WinnersUsername == username)
                        endCondition.NumberOfGamesWon++;
                    else
                        endCondition.NumberOfGamesLost++;
                    endCondition.Winrate = (float)endCondition.NumberOfGamesWon / (float)endCondition.NumberOfGamesPlayed;
                }
                else
                {
                    EntityResult endCondition = new EntityResult()
                    {
                        Name = endConditionName,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinnersUsername == username ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinnersUsername == username ? 1 : 0
                    };
                    endCondition.Winrate = (float)endCondition.NumberOfGamesWon / (float)endCondition.NumberOfGamesPlayed;
                    playedEndConditions.Add(endCondition);
                }
            }
            return playedEndConditions.OrderByDescending(e => e.Winrate).ToList();
        }

        private List<EntityResult> VersusFactionResults(string username, List<BattleReport> battleReports)
        {
            List<EntityResult> versusFactions = new List<EntityResult>();
            foreach (BattleReport battleReport in battleReports)
            {
                string opposingFaction = battleReport.PostersUsername == username ? battleReport.OpponentsFaction : battleReport.PostersFaction;
                if (versusFactions.Any(f => f.Name == opposingFaction))
                {
                    EntityResult enemyFaction = versusFactions.Find(f => f.Name == opposingFaction);
                    enemyFaction.NumberOfGamesPlayed++;
                    if (battleReport.WinnersUsername == username)
                        enemyFaction.NumberOfGamesWon++;
                    else
                        enemyFaction.NumberOfGamesLost++;
                    enemyFaction.Winrate = (float)enemyFaction.NumberOfGamesWon / (float)enemyFaction.NumberOfGamesPlayed;
                }
                else
                {
                    EntityResult enemeyFaction = new EntityResult()
                    {
                        Name = opposingFaction,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinnersUsername == username ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinnersUsername == username ? 1 : 0
                    };
                    enemeyFaction.Winrate = (float)enemeyFaction.NumberOfGamesWon / (float)enemeyFaction.NumberOfGamesPlayed;
                    versusFactions.Add(enemeyFaction);
                }
            }
            return versusFactions.OrderByDescending(f => f.Winrate).ToList();
        }

        private List<EntityResult> VersusThemesResults(string username, List<BattleReport> battleReports)
        {
            List<EntityResult> versusThemes = new List<EntityResult>();
            foreach (BattleReport battleReport in battleReports)
            {
                string opposingTheme = battleReport.PostersUsername == username ? battleReport.OpponentsTheme : battleReport.PostersTheme;
                if (versusThemes.Any(f => f.Name == opposingTheme))
                {
                    EntityResult enemyTheme = versusThemes.Find(f => f.Name == opposingTheme);
                    enemyTheme.NumberOfGamesPlayed++;
                    if (battleReport.WinnersUsername == username)
                        enemyTheme.NumberOfGamesWon++;
                    else
                        enemyTheme.NumberOfGamesLost++;
                    enemyTheme.Winrate = (float)enemyTheme.NumberOfGamesWon / (float)enemyTheme.NumberOfGamesPlayed;
                }
                else
                {
                    EntityResult enemeyTheme = new EntityResult()
                    {
                        Name = opposingTheme,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinnersUsername == username ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinnersUsername == username ? 1 : 0
                    };
                    enemeyTheme.Winrate = (float)enemeyTheme.NumberOfGamesWon / (float)enemeyTheme.NumberOfGamesPlayed;
                    versusThemes.Add(enemeyTheme);
                }
            }
            return versusThemes.OrderByDescending(t => t.Winrate).ToList();
        }

        private List<EntityResult> VersusCasterResults(string username, List<BattleReport> battleReports)
        {
            List<EntityResult> versusCasters = new List<EntityResult>();
            foreach (BattleReport battleReport in battleReports)
            {
                string opposingCaster = battleReport.PostersUsername == username ? battleReport.OpponentsCaster : battleReport.PostersCaster;
                if (versusCasters.Any(f => f.Name == opposingCaster))
                {
                    EntityResult enemyCaster = versusCasters.Find(f => f.Name == opposingCaster);
                    enemyCaster.NumberOfGamesPlayed++;
                    if (battleReport.WinnersUsername == username)
                        enemyCaster.NumberOfGamesWon++;
                    else
                        enemyCaster.NumberOfGamesLost++;
                    enemyCaster.Winrate = (float)enemyCaster.NumberOfGamesWon / (float)enemyCaster.NumberOfGamesPlayed;
                }
                else
                {
                    EntityResult enemeyCaster = new EntityResult()
                    {
                        Name = opposingCaster,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinnersUsername == username ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinnersUsername == username ? 1 : 0
                    };
                    enemeyCaster.Winrate = (float)enemeyCaster.NumberOfGamesWon / (float)enemeyCaster.NumberOfGamesPlayed;
                    versusCasters.Add(enemeyCaster);
                }
            }
            return versusCasters.OrderByDescending(c => c.Winrate).ToList();
        }

        private List<Faction> FactionResults(string username, List<BattleReport> battleReports)
        {
            List<Faction> playedFactions = new List<Faction>();
            foreach (BattleReport battleReport in battleReports)
            {
                string factionName = battleReport.PostersUsername == username ? battleReport.PostersFaction : battleReport.OpponentsFaction;
                if (playedFactions.Any(f => f.Name == factionName))
                {
                    Faction faction = playedFactions.Find(f => f.Name == factionName);
                    faction.NumberOfGamesPlayed++;
                    if (battleReport.WinnersUsername == username)
                        faction.NumberOfGamesWon++;
                    else
                        faction.NumberOfGamesLost++;
                    faction.Winrate = (float)faction.NumberOfGamesWon / (float)faction.NumberOfGamesPlayed;
                }
                else
                {
                    Faction faction = new Faction()
                    {
                        Name = factionName,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinnersUsername == username ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinnersUsername == username ? 1 : 0
                    };
                    faction.Winrate = (float)faction.NumberOfGamesWon / (float)faction.NumberOfGamesPlayed;
                    playedFactions.Add(faction);
                }
            }
            return playedFactions.OrderByDescending(f => f.Winrate).ToList();
        }

        private List<Theme> ThemeResults(string username, List<BattleReport> battleReports)
        {
            List<Theme> playedThemes = new List<Theme>();
            foreach (BattleReport battleReport in battleReports)
            {
                string themeName = battleReport.PostersUsername == username ? battleReport.PostersTheme : battleReport.OpponentsTheme;
                if (playedThemes.Any(f => f.Name == themeName))
                {
                    Theme theme = playedThemes.Find(t => t.Name == themeName);
                    theme.NumberOfGamesPlayed++;
                    if (battleReport.WinnersUsername == username)
                        theme.NumberOfGamesWon++;
                    else
                        theme.NumberOfGamesLost++;
                    theme.Winrate = (float)theme.NumberOfGamesWon / (float)theme.NumberOfGamesPlayed;
                }
                else
                {
                    Theme theme = new Theme()
                    {
                        Name = themeName,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinnersUsername == username ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinnersUsername == username ? 1 : 0
                    };
                    theme.Winrate = (float)theme.NumberOfGamesWon / (float)theme.NumberOfGamesPlayed;
                    playedThemes.Add(theme);
                }
            }
            return playedThemes.OrderByDescending(t => t.Winrate).ToList();
        }

        private List<Caster> CasterResults(string username, List<BattleReport> battleReports)
        {
            List<Caster> playedCasters = new List<Caster>();
            foreach (BattleReport battleReport in battleReports)
            {
                string casterName = battleReport.PostersUsername == username ? battleReport.PostersCaster : battleReport.OpponentsCaster;
                if (playedCasters.Any(c => c.Name == casterName))
                {
                    Caster caster = playedCasters.Find(c => c.Name == casterName);
                    caster.NumberOfGamesPlayed++;
                    if (battleReport.WinnersUsername == username)
                        caster.NumberOfGamesWon++;
                    else
                        caster.NumberOfGamesLost++;
                    caster.Winrate = (float)caster.NumberOfGamesWon / (float)caster.NumberOfGamesPlayed;
                }
                else
                {
                    Caster caster = new Caster()
                    {
                        Name = casterName,
                        NumberOfGamesPlayed = 1,
                        NumberOfGamesLost = battleReport.WinnersUsername == username ? 0 : 1,
                        NumberOfGamesWon = battleReport.WinnersUsername == username ? 1 : 0
                    };
                    caster.Winrate = (float)caster.NumberOfGamesWon / (float)caster.NumberOfGamesPlayed;
                    playedCasters.Add(caster);
                }
            }
            return playedCasters.OrderByDescending(c => c.Winrate).ToList();
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