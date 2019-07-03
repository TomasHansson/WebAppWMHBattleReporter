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
            if (string.IsNullOrWhiteSpace(id))
                id = StaticDetails.AllRegions;
            UserResultsViewModel viewModel = new UserResultsViewModel()
            {
                Region = id,
                UserResults = await UserResults(id)
            };
            return View(viewModel);
        }

        private async Task<List<UserResult>> UserResults(string region)
        {
            List<UserResult> userResults = new List<UserResult>();
            List<ApplicationUser> users = region == StaticDetails.AllRegions ?
                await _db.ApplicationUsers.OrderByDescending(au => au.Winrate).ToListAsync() :
                await _db.ApplicationUsers.Where(au => au.Region == region).OrderByDescending(au => au.Winrate).ToListAsync();

            foreach (ApplicationUser user in users)
            {
                Dictionary<string, int> gamesPlayedWithEachFaction = new Dictionary<string, int>();
                Dictionary<string, int> gamesPlayedWithEachTheme = new Dictionary<string, int>();
                Dictionary<string, int> gamesPlayedWithEachCaster = new Dictionary<string, int>();

                Dictionary<string, int> gamesWonWithEachFaction = new Dictionary<string, int>();
                Dictionary<string, int> gamesWonWithEachTheme = new Dictionary<string, int>();
                Dictionary<string, int> gamesWonWithEachCaster = new Dictionary<string, int>();

                List<BattleReport> usersBattleReports = await _db.BattleReports.Where(br => br.PostersUsername == user.UserName || br.OpponentsUsername == user.UserName).ToListAsync();

                CollectBattleReportsData(user, usersBattleReports, gamesPlayedWithEachFaction, gamesPlayedWithEachTheme, gamesPlayedWithEachCaster,
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

                UserResult usersResult = new UserResult()
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

                userResults.Add(usersResult);
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