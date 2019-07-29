using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppWMHBattleReporter.Controllers;
using WebAppWMHBattleReporter.Data;
using WebAppWMHBattleReporter.Models;
using WebAppWMHBattleReporter.Models.ViewModels;
using WebAppWMHBattleReporter.Utility;

namespace WebAppWMHBattleReporter.Areas.EndUser.Controllers
{
    [Authorize]
    [Area("EndUser")]
    public class BattleReportController : Controller
    {
        private ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        public BattleReportController(ApplicationDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            List<string> factionOptions = new List<string>() { StaticDetails.AllFactions };
            List<string> themeOptions = new List<string>() { StaticDetails.AllThemes };
            List<string> casterOptions = new List<string>() { StaticDetails.AllCasters };
            List<Faction> factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
            foreach (Faction faction in factions)
                factionOptions.Add(faction.Name);

            BattleReportsListViewModel viewModel = new BattleReportsListViewModel()
            {
                BattleReports = await _db.BattleReports.OrderByDescending(br => br.DatePlayed).ToListAsync(),
                TimePeriod = StaticDetails.AllTime,
                TimePeriodOptions = StaticDetails.TimePeriodOptions,
                P1Faction = StaticDetails.AllFactions,
                FactionOptions = factionOptions,
                P1Theme = StaticDetails.AllThemes,
                P1ThemeOptions = themeOptions,
                P1Caster = StaticDetails.AllCasters,
                P1CasterOptions = casterOptions,
                P2Faction = StaticDetails.AllFactions,
                P2Theme = StaticDetails.AllThemes,
                P2ThemeOptions = themeOptions,
                P2Caster = StaticDetails.AllCasters,
                P2CasterOptions = casterOptions,
                GameSizeOptions = StaticDetails.GameSizeFilterOptions,
                GameSize = StaticDetails.AllGameSizes,
                EndConditionOptions = StaticDetails.EndconditionFilterOptions,
                EndCondition = StaticDetails.AllEndConditions,
                ScenarioOptions = StaticDetails.ScenarioFilterOptions,
                Scenario = StaticDetails.AllScenarios,
                HideFilters = true
            };
            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BattleReportsListViewModel viewModel)
        {
            var filteredBattleReports = _db.BattleReports.AsQueryable();
            viewModel.TimePeriodOptions = StaticDetails.TimePeriodOptions;
            List<Faction> factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
            List<Theme> themes = await _db.Themes.ToListAsync();
            List<Caster> casters = await _db.Casters.ToListAsync();
            viewModel.FactionOptions = new List<string>() { StaticDetails.AllFactions };
            foreach (Faction faction in factions)
                viewModel.FactionOptions.Add(faction.Name);
            viewModel.P1ThemeOptions = new List<string>() { StaticDetails.AllThemes };
            viewModel.P1CasterOptions = new List<string>() { StaticDetails.AllCasters };
            viewModel.P2ThemeOptions = new List<string>() { StaticDetails.AllThemes };
            viewModel.P2CasterOptions = new List<string>() { StaticDetails.AllCasters };
            viewModel.GameSizeOptions = StaticDetails.GameSizeFilterOptions;
            viewModel.EndConditionOptions = StaticDetails.EndconditionFilterOptions;
            viewModel.ScenarioOptions = StaticDetails.ScenarioFilterOptions;

            if (viewModel.TimePeriod == StaticDetails.LastYear)
                filteredBattleReports = filteredBattleReports.Where(br => br.DatePlayed > DateTime.Today.AddYears(-1));
            else if (viewModel.TimePeriod == StaticDetails.Last6Months)
                filteredBattleReports = filteredBattleReports.Where(br => br.DatePlayed > DateTime.Today.AddMonths(-6));
            else if (viewModel.TimePeriod == StaticDetails.LastMonth)
                filteredBattleReports = filteredBattleReports.Where(br => br.DatePlayed > DateTime.Today.AddMonths(-1));

            if (viewModel.P1Faction != StaticDetails.AllFactions)
            {
                Faction faction = factions.First(f => f.Name == viewModel.P1Faction);

                List<Theme> factionThemes = themes.Where(t => t.FactionId == faction.Id).OrderBy(t => t.Name).ToList();
                foreach (Theme theme in factionThemes)
                    viewModel.P1ThemeOptions.Add(theme.Name);

                List<Caster> factionCasters = casters.Where(c => c.FactionId == faction.Id).OrderBy(c => c.Name).ToList();
                foreach (Caster caster in factionCasters)
                    viewModel.P2CasterOptions.Add(caster.Name);
            }

            if (viewModel.P2Faction != StaticDetails.AllFactions)
            {
                Faction faction = factions.First(f => f.Name == viewModel.P1Faction);

                List<Theme> factionThemes = themes.Where(t => t.FactionId == faction.Id).OrderBy(t => t.Name).ToList();
                foreach (Theme theme in factionThemes)
                    viewModel.P2ThemeOptions.Add(theme.Name);

                List<Caster> enemyCasters = casters.Where(c => c.FactionId == faction.Id).OrderBy(c => c.Name).ToList();
                foreach (Caster caster in enemyCasters)
                    viewModel.P2CasterOptions.Add(caster.Name);
            }

            if (viewModel.P1Faction != StaticDetails.AllFactions && viewModel.P2Faction != StaticDetails.AllFactions)
                filteredBattleReports = filteredBattleReports.Where(br => (br.PostersFaction == viewModel.P1Faction && br.OpponentsFaction == viewModel.P2Faction) || (br.PostersFaction == viewModel.P2Faction && br.OpponentsFaction == viewModel.P1Faction));
            else if (viewModel.P1Faction != StaticDetails.AllFactions && viewModel.P2Faction == StaticDetails.AllFactions)
                filteredBattleReports = filteredBattleReports.Where(br => br.PostersFaction == viewModel.P1Faction || br.OpponentsFaction == viewModel.P1Faction);
            else if (viewModel.P2Faction != StaticDetails.AllFactions && viewModel.P1Faction == StaticDetails.AllFactions)
                filteredBattleReports = filteredBattleReports.Where(br => br.PostersFaction == viewModel.P2Faction || br.OpponentsFaction == viewModel.P2Faction);

            if (viewModel.P1Theme != StaticDetails.AllThemes && viewModel.P2Theme != StaticDetails.AllThemes)
                filteredBattleReports = filteredBattleReports.Where(br => (br.PostersTheme == viewModel.P1Theme && br.OpponentsTheme == viewModel.P2Theme) || (br.PostersTheme == viewModel.P2Theme && br.OpponentsTheme == viewModel.P1Theme));
            else if (viewModel.P1Theme != StaticDetails.AllThemes && viewModel.P2Theme == StaticDetails.AllThemes)
                filteredBattleReports = filteredBattleReports.Where(br => br.PostersTheme == viewModel.P1Theme || br.OpponentsTheme == viewModel.P1Theme);
            else if (viewModel.P2Theme != StaticDetails.AllThemes && viewModel.P1Theme == StaticDetails.AllThemes)
                filteredBattleReports = filteredBattleReports.Where(br => br.PostersTheme == viewModel.P2Theme || br.OpponentsTheme == viewModel.P2Theme);

            if (viewModel.P1Caster != StaticDetails.AllCasters && viewModel.P2Caster != StaticDetails.AllCasters)
                filteredBattleReports = filteredBattleReports.Where(br => (br.PostersCaster == viewModel.P1Caster && br.OpponentsCaster == viewModel.P2Caster) || (br.PostersCaster == viewModel.P2Caster && br.OpponentsCaster == viewModel.P1Caster));
            else if (viewModel.P1Caster != StaticDetails.AllCasters && viewModel.P2Caster == StaticDetails.AllCasters)
                filteredBattleReports = filteredBattleReports.Where(br => br.PostersCaster == viewModel.P1Caster || br.OpponentsCaster == viewModel.P1Caster);
            else if (viewModel.P2Caster != StaticDetails.AllCasters && viewModel.P1Caster == StaticDetails.AllCasters)
                filteredBattleReports = filteredBattleReports.Where(br => br.PostersCaster == viewModel.P2Caster || br.OpponentsCaster == viewModel.P2Caster);

            if (viewModel.GameSize != StaticDetails.AllGameSizes)
                filteredBattleReports = filteredBattleReports.Where(br => br.GameSize.ToString() == viewModel.GameSize);

            if (viewModel.EndCondition != StaticDetails.AllEndConditions)
                filteredBattleReports = filteredBattleReports.Where(br => br.EndCondition == viewModel.EndCondition);

            if (viewModel.Scenario != StaticDetails.AllScenarios)
                filteredBattleReports = filteredBattleReports.Where(br => br.Scenario == viewModel.Scenario);

            viewModel.BattleReports = await filteredBattleReports.ToListAsync();

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            BattleReportViewModel viewModel = new BattleReportViewModel()
            {
                Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync(),
                BattleReport = new BattleReport(),
                StatusMessage = string.Empty
            };
            int firstFactionId = viewModel.Factions.FirstOrDefault().Id;
            viewModel.FirstFactionThemes = await _db.Themes.Where(t => t.FactionId == firstFactionId).OrderBy(t => t.Name).ToListAsync();
            viewModel.FirstFactionCasters = await _db.Casters.Where(c => c.FactionId == firstFactionId).OrderBy(c => c.Name).ToListAsync();
            viewModel.BattleReport.PostersUsername = User.Identity.Name;
            return View(viewModel);
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BattleReportViewModel viewModel)
        {
            viewModel.StatusMessage = string.Empty;
            if (!ModelState.IsValid)
            {
                viewModel.Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
                int firstFactionId = viewModel.Factions.FirstOrDefault().Id;
                viewModel.FirstFactionThemes = await _db.Themes.Where(t => t.FactionId == firstFactionId).OrderBy(t => t.Name).ToListAsync();
                viewModel.FirstFactionCasters = await _db.Casters.Where(c => c.FactionId == firstFactionId).OrderBy(c => c.Name).ToListAsync();
                return View(viewModel);
            }

            if (!await _db.ApplicationUsers.AnyAsync(au => au.UserName == viewModel.BattleReport.OpponentsUsername))
            {
                viewModel.Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
                int firstFactionId = viewModel.Factions.FirstOrDefault().Id;
                viewModel.FirstFactionThemes = await _db.Themes.Where(t => t.FactionId == firstFactionId).OrderBy(t => t.Name).ToListAsync();
                viewModel.FirstFactionCasters = await _db.Casters.Where(c => c.FactionId == firstFactionId).OrderBy(c => c.Name).ToListAsync();
                viewModel.StatusMessage = "Error: There are no users in the database with the username specified for your opponent.";
                return View(viewModel);
            }

            viewModel.BattleReport.ConfirmationKey = (new Random().Next(1000, 10000));
            viewModel.BattleReport.ConfirmedByOpponent = false;

            viewModel.BattleReport.LosersUsername = viewModel.PosterWon ? viewModel.BattleReport.OpponentsUsername : viewModel.BattleReport.PostersUsername;
            viewModel.BattleReport.LosingFaction = viewModel.PosterWon ? viewModel.BattleReport.OpponentsFaction : viewModel.BattleReport.PostersFaction;
            viewModel.BattleReport.LosingTheme = viewModel.PosterWon ? viewModel.BattleReport.OpponentsTheme : viewModel.BattleReport.PostersTheme;
            viewModel.BattleReport.LosingCaster = viewModel.PosterWon ? viewModel.BattleReport.OpponentsCaster : viewModel.BattleReport.PostersCaster;

            viewModel.BattleReport.WinnersUsername = viewModel.PosterWon ? viewModel.BattleReport.PostersUsername : viewModel.BattleReport.OpponentsUsername;
            viewModel.BattleReport.WinningFaction = viewModel.PosterWon ? viewModel.BattleReport.PostersFaction : viewModel.BattleReport.OpponentsFaction;
            viewModel.BattleReport.WinningTheme = viewModel.PosterWon ? viewModel.BattleReport.PostersTheme : viewModel.BattleReport.OpponentsTheme;
            viewModel.BattleReport.WinningCaster = viewModel.PosterWon ? viewModel.BattleReport.PostersCaster : viewModel.BattleReport.OpponentsCaster;

            _db.BattleReports.Add(viewModel.BattleReport);

            ApplicationUser opponent = await _db.ApplicationUsers.FirstAsync(au => au.UserName == viewModel.BattleReport.OpponentsUsername);
            string opponentsEmail = opponent.Email;
            await _emailSender.SendEmailAsync(opponentsEmail, "Battle Report Posted", "The user " + viewModel.BattleReport.PostersUsername + " has added a battle report with you as their opponent. Please log in and confirm the battle report when able.");
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home", new { Area = "Home" });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport reportFromDB = await _db.BattleReports.FindAsync(id);
            if (reportFromDB == null)
                return NotFound();

            return View(reportFromDB);
        }

        public async Task<IActionResult> UnconfirmedBattleReports(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            if (id != User.Identity.Name)
                return Unauthorized();

            List<BattleReport> usersBattleReports = await _db.BattleReports.Where(br => br.PostersUsername == id && br.ConfirmedByOpponent == false).OrderByDescending(br => br.DatePlayed).ToListAsync();
            List<BattleReport> opponentsBattleReports = await _db.BattleReports.Where(br => br.OpponentsUsername == id && br.ConfirmedByOpponent == false).OrderByDescending(br => br.DatePlayed).ToListAsync();
            if (usersBattleReports == null || opponentsBattleReports == null)
                return NotFound();

            UnconfirmedBattleReportsViewModel viewModel = new UnconfirmedBattleReportsViewModel()
            {
                UsersBattleReports = usersBattleReports,
                OpponentsBattleReports = opponentsBattleReports
            };
            return View(viewModel);
        }

        public async Task<IActionResult> ConfirmedBattleReports(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            if (id != User.Identity.Name)
                return Unauthorized();

            List<BattleReport> battleReports = await _db.BattleReports.Where(br => (br.PostersUsername == id || br.OpponentsUsername == id) && br.ConfirmedByOpponent).OrderByDescending(br => br.DatePlayed).ToListAsync();
            if (battleReports == null)
                return NotFound();

            List<string> factionOptions = new List<string>() { StaticDetails.AllFactions };
            List<string> themeOptions = new List<string>() { StaticDetails.AllThemes };
            List<string> casterOptions = new List<string>() { StaticDetails.AllCasters };
            List<Faction> factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
            foreach (Faction faction in factions)
                factionOptions.Add(faction.Name);

            ConfirmedBattleReportsViewModel viewModel = new ConfirmedBattleReportsViewModel()
            {
                BattleReports = battleReports,
                UserName = User.Identity.Name,
                TimePeriod = StaticDetails.AllTime,
                TimePeriodOptions = StaticDetails.TimePeriodOptions,
                Faction = StaticDetails.AllFactions,
                FactionOptions = factionOptions,
                Theme = StaticDetails.AllThemes,
                ThemeOptions = themeOptions,
                Caster = StaticDetails.AllCasters,
                CasterOptions = casterOptions,
                EnemyFaction = StaticDetails.AllFactions,
                EnemyTheme = StaticDetails.AllThemes,
                EnemyThemeOptions = themeOptions,
                EnemyCaster = StaticDetails.AllCasters,
                EnemyCasterOptions = casterOptions,
                GameSizeOptions = StaticDetails.GameSizeFilterOptions,
                GameSize = StaticDetails.AllGameSizes,
                EndConditionOptions = StaticDetails.EndconditionFilterOptions,
                EndCondition = StaticDetails.AllEndConditions,
                ScenarioOptions = StaticDetails.ScenarioFilterOptions,
                Scenario = StaticDetails.AllScenarios,
                OutComeOptions = StaticDetails.OutComeOptions,
                OutCome = StaticDetails.AllOutcomes,
                HideFilters = true
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmedBattleReports(ConfirmedBattleReportsViewModel viewModel)
        {
            if (viewModel.UserName != User.Identity.Name)
                return Unauthorized();

            var filteredBattleReports = _db.BattleReports.Where(br => (br.PostersUsername == viewModel.UserName || br.OpponentsUsername == viewModel.UserName) && br.ConfirmedByOpponent);
            viewModel.TimePeriodOptions = StaticDetails.TimePeriodOptions;
            List<Faction> factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
            List<Theme> themes = await _db.Themes.ToListAsync();
            List<Caster> casters = await _db.Casters.ToListAsync();
            viewModel.FactionOptions = new List<string>() { StaticDetails.AllFactions };
            foreach (Faction faction in factions)
                viewModel.FactionOptions.Add(faction.Name);
            viewModel.ThemeOptions = new List<string>() { StaticDetails.AllThemes };
            viewModel.CasterOptions = new List<string>() { StaticDetails.AllCasters };
            viewModel.EnemyThemeOptions = new List<string>() { StaticDetails.AllThemes };
            viewModel.EnemyCasterOptions = new List<string>() { StaticDetails.AllCasters };
            viewModel.GameSizeOptions = StaticDetails.GameSizeFilterOptions;
            viewModel.EndConditionOptions = StaticDetails.EndconditionFilterOptions;
            viewModel.ScenarioOptions = StaticDetails.ScenarioFilterOptions;
            viewModel.OutComeOptions = StaticDetails.OutComeOptions;

            if (viewModel.TimePeriod == StaticDetails.LastYear)
                filteredBattleReports = filteredBattleReports.Where(br => br.DatePlayed > DateTime.Today.AddYears(-1));
            else if (viewModel.TimePeriod == StaticDetails.Last6Months)
                filteredBattleReports = filteredBattleReports.Where(br => br.DatePlayed > DateTime.Today.AddMonths(-6));
            else if (viewModel.TimePeriod == StaticDetails.LastMonth)
                filteredBattleReports = filteredBattleReports.Where(br => br.DatePlayed > DateTime.Today.AddMonths(-1));

            if (viewModel.Faction != StaticDetails.AllFactions)
            {
                Faction faction = factions.First(f => f.Name == viewModel.Faction);

                List<Theme> factionThemes = themes.Where(t => t.FactionId == faction.Id).OrderBy(t => t.Name).ToList();
                foreach (Theme theme in factionThemes)
                    viewModel.ThemeOptions.Add(theme.Name);

                List<Caster> factionCasters = casters.Where(c => c.FactionId == faction.Id).OrderBy(c => c.Name).ToList();
                foreach (Caster caster in factionCasters)
                    viewModel.CasterOptions.Add(caster.Name);

                filteredBattleReports = filteredBattleReports.Where(br => (br.PostersUsername == viewModel.UserName && br.PostersFaction == viewModel.Faction) || (br.OpponentsUsername == viewModel.UserName && br.OpponentsFaction == viewModel.Faction));
            }

            if (viewModel.Theme != StaticDetails.AllThemes)
                filteredBattleReports = filteredBattleReports.Where(br => (br.PostersUsername == viewModel.UserName && br.PostersTheme == viewModel.Theme) || (br.OpponentsUsername == viewModel.UserName && br.OpponentsTheme == viewModel.Theme));

            if (viewModel.Caster != StaticDetails.AllCasters)
                filteredBattleReports = filteredBattleReports.Where(br => (br.PostersUsername == viewModel.UserName && br.PostersCaster == viewModel.Caster) || (br.OpponentsUsername == viewModel.UserName && br.OpponentsCaster == viewModel.Caster));

            if (viewModel.EnemyFaction != StaticDetails.AllFactions)
            {
                Faction enemyFaction = factions.First(f => f.Name == viewModel.EnemyFaction);

                List<Theme> enemyThemes = themes.Where(t => t.FactionId == enemyFaction.Id).OrderBy(t => t.Name).ToList();
                foreach (Theme theme in enemyThemes)
                    viewModel.EnemyThemeOptions.Add(theme.Name);

                List<Caster> enemyCasters = casters.Where(c => c.FactionId == enemyFaction.Id).OrderBy(c => c.Name).ToList();
                foreach (Caster caster in enemyCasters)
                    viewModel.EnemyCasterOptions.Add(caster.Name);

                filteredBattleReports = filteredBattleReports.Where(br => (br.PostersUsername != viewModel.UserName && br.PostersFaction == viewModel.EnemyFaction) || (br.OpponentsUsername != viewModel.UserName && br.OpponentsFaction == viewModel.EnemyFaction));
            }

            if (viewModel.EnemyTheme != StaticDetails.AllThemes)
                filteredBattleReports = filteredBattleReports.Where(br => (br.PostersUsername != viewModel.UserName && br.PostersTheme == viewModel.EnemyTheme) || (br.OpponentsUsername != viewModel.UserName && br.OpponentsTheme == viewModel.EnemyTheme));

            if (viewModel.EnemyCaster != StaticDetails.AllCasters)
                filteredBattleReports = filteredBattleReports.Where(br => (br.PostersUsername != viewModel.UserName && br.PostersCaster == viewModel.EnemyCaster) || (br.OpponentsUsername != viewModel.UserName && br.OpponentsCaster == viewModel.EnemyCaster));

            if (viewModel.GameSize != StaticDetails.AllGameSizes)
                filteredBattleReports = filteredBattleReports.Where(br => br.GameSize.ToString() == viewModel.GameSize);

            if (viewModel.EndCondition != StaticDetails.AllEndConditions)
                filteredBattleReports = filteredBattleReports.Where(br => br.EndCondition == viewModel.EndCondition);

            if (viewModel.Scenario != StaticDetails.AllScenarios)
                filteredBattleReports = filteredBattleReports.Where(br => br.Scenario == viewModel.Scenario);

            if (viewModel.OutCome != StaticDetails.AllOutcomes)
            {
                if (viewModel.OutCome == StaticDetails.YouWon)
                    filteredBattleReports = filteredBattleReports.Where(br => br.WinnersUsername == viewModel.UserName);
                else if (viewModel.OutCome == StaticDetails.YouLost)
                    filteredBattleReports = filteredBattleReports.Where(br => br.LosersUsername == viewModel.UserName);
            }
                
            viewModel.BattleReports = await filteredBattleReports.OrderByDescending(br => br.DatePlayed).ToListAsync();

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

            if (battleReport.PostersUsername != User.Identity.Name)
                return Unauthorized();

            EditBattleReportViewModel viewModel = new EditBattleReportViewModel()
            {
                Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync(),
                BattleReport = battleReport,
                StatusMessage = string.Empty
            };

            viewModel.PosterWon = viewModel.BattleReport.WinnersUsername == User.Identity.Name;

            int postersFactionId = viewModel.Factions.First(f => f.Name == battleReport.PostersFaction).Id;
            viewModel.PostersFactionThemes = await _db.Themes.Where(t => t.FactionId == postersFactionId).OrderBy(t => t.Name).ToListAsync();
            viewModel.PostersFactionCasters = await _db.Casters.Where(c => c.FactionId == postersFactionId).OrderBy(c => c.Name).ToListAsync();

            int opponentsFactionId = viewModel.Factions.First(f => f.Name == battleReport.OpponentsFaction).Id;
            viewModel.OpponentsFactionThemes = await _db.Themes.Where(t => t.FactionId == opponentsFactionId).OrderBy(t => t.Name).ToListAsync();
            viewModel.OpponentsFactionCasters = await _db.Casters.Where(c => c.FactionId == opponentsFactionId).OrderBy(c => c.Name).ToListAsync();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBattleReportViewModel viewModel)
        {
            viewModel.StatusMessage = string.Empty;
            if (!ModelState.IsValid)
            {
                viewModel.Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
                viewModel.PosterWon = viewModel.BattleReport.WinnersUsername == User.Identity.Name;
                int postersFactionId = viewModel.Factions.First(f => f.Name == viewModel.BattleReport.PostersFaction).Id;
                viewModel.PostersFactionThemes = await _db.Themes.Where(t => t.FactionId == postersFactionId).OrderBy(t => t.Name).ToListAsync();
                viewModel.PostersFactionCasters = await _db.Casters.Where(c => c.FactionId == postersFactionId).OrderBy(c => c.Name).ToListAsync();
                int opponentsFactionId = viewModel.Factions.First(f => f.Name == viewModel.BattleReport.OpponentsFaction).Id;
                viewModel.OpponentsFactionThemes = await _db.Themes.Where(t => t.FactionId == opponentsFactionId).OrderBy(t => t.Name).ToListAsync();
                viewModel.OpponentsFactionCasters = await _db.Casters.Where(c => c.FactionId == opponentsFactionId).OrderBy(c => c.Name).ToListAsync();
                return View(viewModel);
            }

            BattleReport battleReport = await _db.BattleReports.FindAsync(viewModel.BattleReport.Id);

            battleReport.PostersFaction = viewModel.BattleReport.PostersFaction;
            battleReport.PostersTheme = viewModel.BattleReport.PostersTheme;
            battleReport.PostersCaster = viewModel.BattleReport.PostersCaster;
            battleReport.PostersArmyList = viewModel.BattleReport.PostersArmyList;
            battleReport.PostersArmyPoints = viewModel.BattleReport.PostersArmyPoints;
            battleReport.PostersControlPoints = viewModel.BattleReport.PostersControlPoints;

            battleReport.OpponentsFaction = viewModel.BattleReport.OpponentsFaction;
            battleReport.OpponentsTheme = viewModel.BattleReport.OpponentsTheme;
            battleReport.OpponentsCaster = viewModel.BattleReport.OpponentsCaster;
            battleReport.OpponentsArmyList = viewModel.BattleReport.OpponentsArmyList;
            battleReport.OpponentsArmyPoints = viewModel.BattleReport.OpponentsArmyPoints;
            battleReport.OpponentsControlPoints = viewModel.BattleReport.OpponentsControlPoints;

            battleReport.DatePlayed = viewModel.BattleReport.DatePlayed;
            battleReport.EndCondition = viewModel.BattleReport.EndCondition;
            battleReport.GameSize = viewModel.BattleReport.GameSize;
            battleReport.Scenario = viewModel.BattleReport.Scenario;

            battleReport.LosersUsername = viewModel.PosterWon ? viewModel.BattleReport.OpponentsUsername : viewModel.BattleReport.PostersUsername;
            battleReport.LosingFaction = viewModel.PosterWon ? viewModel.BattleReport.OpponentsFaction : viewModel.BattleReport.PostersFaction;
            battleReport.LosingTheme = viewModel.PosterWon ? viewModel.BattleReport.OpponentsTheme : viewModel.BattleReport.PostersTheme;
            battleReport.LosingCaster = viewModel.PosterWon ? viewModel.BattleReport.OpponentsCaster : viewModel.BattleReport.PostersCaster;

            battleReport.WinnersUsername = viewModel.PosterWon ? viewModel.BattleReport.PostersUsername : viewModel.BattleReport.OpponentsUsername;
            battleReport.WinningFaction = viewModel.PosterWon ? viewModel.BattleReport.PostersFaction : viewModel.BattleReport.OpponentsFaction;
            battleReport.WinningTheme = viewModel.PosterWon ? viewModel.BattleReport.PostersTheme : viewModel.BattleReport.OpponentsTheme;
            battleReport.WinningCaster = viewModel.PosterWon ? viewModel.BattleReport.PostersCaster : viewModel.BattleReport.OpponentsCaster;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(UnconfirmedBattleReports), new { id = User.Identity.Name });
        }

        public async Task<IActionResult> UserReportDetails(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

            if (battleReport.PostersUsername != User.Identity.Name)
                return Unauthorized();

            return View(battleReport);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

            if (battleReport.PostersUsername != User.Identity.Name)
                return Unauthorized();

            return View(battleReport);
        }

        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

            if (battleReport.PostersUsername != User.Identity.Name)
                return Unauthorized();

            _db.BattleReports.Remove(battleReport);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(UnconfirmedBattleReports), new { id = User.Identity.Name });
        }

        public async Task<IActionResult> AcceptUnconfirmedView(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

            if (battleReport.OpponentsUsername != User.Identity.Name)
                return Unauthorized();

            return View(battleReport);
        }

        public async Task<IActionResult> AcceptUnconfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

            if (battleReport.OpponentsUsername != User.Identity.Name)
                return Unauthorized();

            battleReport.ConfirmedByOpponent = true;

            ApplicationUser poster = await _db.ApplicationUsers.Where(au => au.UserName == battleReport.PostersUsername).FirstAsync();
            poster.NumberOfGamesPlayed++;
            if (battleReport.WinnersUsername == poster.UserName)
                poster.NumberOfGamesWon++;
            else
                poster.NumberOfGamesLost++;
            poster.Winrate = (float)poster.NumberOfGamesWon / (float)poster.NumberOfGamesPlayed;

            ApplicationUser opponent = await _db.ApplicationUsers.Where(au => au.UserName == battleReport.OpponentsUsername).FirstAsync();
            opponent.NumberOfGamesPlayed++;
            if (battleReport.WinnersUsername == opponent.UserName)
                opponent.NumberOfGamesWon++;
            else
                opponent.NumberOfGamesLost++;
            opponent.Winrate = (float)opponent.NumberOfGamesWon / (float)opponent.NumberOfGamesPlayed;

            Faction winningFaction = await _db.Factions.Where(f => f.Name == battleReport.WinningFaction).FirstAsync();
            winningFaction.NumberOfGamesPlayed++;
            winningFaction.NumberOfGamesWon++;
            winningFaction.Winrate = (float)winningFaction.NumberOfGamesWon / (float)winningFaction.NumberOfGamesPlayed;

            Faction losingFaction = await _db.Factions.Where(f => f.Name == battleReport.LosingFaction).FirstAsync();
            losingFaction.NumberOfGamesPlayed++;
            losingFaction.NumberOfGamesLost++;
            losingFaction.Winrate = (float)losingFaction.NumberOfGamesWon / (float)losingFaction.NumberOfGamesPlayed;

            Theme winningTheme = await _db.Themes.Where(t => t.Name == battleReport.WinningTheme).FirstAsync();
            winningTheme.NumberOfGamesPlayed++;
            winningTheme.NumberOfGamesWon++;
            winningTheme.Winrate = (float)winningTheme.NumberOfGamesWon / (float)winningTheme.NumberOfGamesPlayed;

            Theme losingTheme = await _db.Themes.Where(t => t.Name == battleReport.LosingTheme).FirstAsync();
            losingTheme.NumberOfGamesPlayed++;
            losingTheme.NumberOfGamesLost++;
            losingTheme.Winrate = (float)losingTheme.NumberOfGamesWon / (float)losingTheme.NumberOfGamesPlayed;

            Caster winningCaster = await _db.Casters.Where(c => c.Name == battleReport.WinningCaster).FirstAsync();
            winningCaster.NumberOfGamesPlayed++;
            winningCaster.NumberOfGamesWon++;
            winningCaster.Winrate = (float)winningCaster.NumberOfGamesWon / (float)winningCaster.NumberOfGamesPlayed;

            Caster losingCaster = await _db.Casters.Where(c => c.Name == battleReport.LosingCaster).FirstAsync();
            losingCaster.NumberOfGamesPlayed++;
            losingCaster.NumberOfGamesLost++;
            losingCaster.Winrate = (float)losingCaster.NumberOfGamesWon / (float)losingCaster.NumberOfGamesPlayed;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(UnconfirmedBattleReports), new { id = battleReport.OpponentsUsername });
        }

        public async Task<IActionResult> DeleteUnconfirmedView(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

            if (battleReport.OpponentsUsername != User.Identity.Name)
                return Unauthorized();

            return View(battleReport);
        }

        public async Task<IActionResult> DeleteUnconfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

            if (battleReport.OpponentsUsername != User.Identity.Name)
                return Unauthorized();

            string username = battleReport.OpponentsUsername;
            _db.BattleReports.Remove(battleReport);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(UnconfirmedBattleReports), new { id = username });
        }

        public async Task<IActionResult> DetailsUnconfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

            if (battleReport.OpponentsUsername != User.Identity.Name)
                return Unauthorized();

            return View(battleReport);
        }
    }
}