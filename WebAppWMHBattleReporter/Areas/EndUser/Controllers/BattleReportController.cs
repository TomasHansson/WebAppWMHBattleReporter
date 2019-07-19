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
            List<BattleReport> battleReports = await _db.BattleReports.Where(br => br.ConfirmedByOpponent).ToListAsync();
            return View(battleReports);
        }

        public async Task<IActionResult> Create()
        {
            BattleReportViewModel viewModel = new BattleReportViewModel()
            {
                Factions = await _db.Factions.ToListAsync(),
                BattleReport = new BattleReport(),
                StatusMessage = string.Empty
            };
            int firstFactionId = viewModel.Factions.FirstOrDefault().Id;
            viewModel.FirstFactionThemes = await _db.Themes.Where(t => t.FactionId == firstFactionId).ToListAsync();
            viewModel.FirstFactionCasters = await _db.Casters.Where(c => c.FactionId == firstFactionId).ToListAsync();
            viewModel.BattleReport.PostersUsername = User.Identity.Name;
            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetFactionThemeNames(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Factions.AnyAsync(f => f.Id == id))
                return NotFound();

            List<string> factionThemes = await _db.Themes.Where(t => t.FactionId == id).Select(t => t.Name).ToListAsync();
            return Json(factionThemes);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetFactionCasterNames(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Factions.AnyAsync(f => f.Id == id))
                return NotFound();

            List<string> factionCasters = await _db.Casters.Where(c => c.FactionId == id).Select(f => f.Name).ToListAsync();
            return Json(factionCasters);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BattleReportViewModel viewModel)
        {
            viewModel.StatusMessage = string.Empty;
            if (!ModelState.IsValid)
            {
                viewModel.Factions = await _db.Factions.ToListAsync();
                int firstFactionId = viewModel.Factions.FirstOrDefault().Id;
                viewModel.FirstFactionThemes = await _db.Themes.Where(t => t.FactionId == firstFactionId).ToListAsync();
                viewModel.FirstFactionCasters = await _db.Casters.Where(c => c.FactionId == firstFactionId).ToListAsync();
                return View(viewModel);
            }

            if (!await _db.ApplicationUsers.AnyAsync(au => au.UserName == viewModel.BattleReport.OpponentsUsername))
            {
                viewModel.Factions = await _db.Factions.ToListAsync();
                int firstFactionId = viewModel.Factions.FirstOrDefault().Id;
                viewModel.FirstFactionThemes = await _db.Themes.Where(t => t.FactionId == firstFactionId).ToListAsync();
                viewModel.FirstFactionCasters = await _db.Casters.Where(c => c.FactionId == firstFactionId).ToListAsync();
                viewModel.StatusMessage = "Error: There are no users in the database with the username specified for your opponent.";
                return View(viewModel);
            }

            viewModel.BattleReport.ConfirmationKey = (new Random().Next(10000));
            viewModel.BattleReport.ConfirmedByOpponent = false;

            Faction postersFaction = await _db.Factions.FindAsync(int.Parse(viewModel.BattleReport.PostersFaction));
            viewModel.BattleReport.PostersFaction = postersFaction.Name;

            Faction opponentsFaction = await _db.Factions.FindAsync(int.Parse(viewModel.BattleReport.OpponentsFaction));
            viewModel.BattleReport.OpponentsFaction = opponentsFaction.Name;

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

            List<BattleReport> usersBattleReports = await _db.BattleReports.Where(br => br.PostersUsername == id && br.ConfirmedByOpponent == false).ToListAsync();
            List<BattleReport> opponentsBattleReports = await _db.BattleReports.Where(br => br.OpponentsUsername == id && br.ConfirmedByOpponent == false).ToListAsync();
            if (usersBattleReports == null || opponentsBattleReports == null)
                return NotFound();

            UnconfirmedBattleReportsViewModel viewModel = new UnconfirmedBattleReportsViewModel()
            {
                UsersBattleReports = usersBattleReports,
                OpponentsBattleReports = opponentsBattleReports
            };
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
                Factions = await _db.Factions.ToListAsync(),
                BattleReport = battleReport,
                StatusMessage = string.Empty
            };

            viewModel.PosterWon = viewModel.BattleReport.WinnersUsername == User.Identity.Name;

            int postersFactionId = viewModel.Factions.First(f => f.Name == battleReport.PostersFaction).Id;
            viewModel.PostersFactionThemes = await _db.Themes.Where(t => t.FactionId == postersFactionId).ToListAsync();
            viewModel.PostersFactionCasters = await _db.Casters.Where(c => c.FactionId == postersFactionId).ToListAsync();

            int opponentsFactionId = viewModel.Factions.First(f => f.Name == battleReport.OpponentsFaction).Id;
            viewModel.OpponentsFactionThemes = await _db.Themes.Where(t => t.FactionId == opponentsFactionId).ToListAsync();
            viewModel.OpponentsFactionCasters = await _db.Casters.Where(c => c.FactionId == opponentsFactionId).ToListAsync();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBattleReportViewModel viewModel)
        {
            viewModel.StatusMessage = string.Empty;
            if (!ModelState.IsValid)
            {
                viewModel.Factions = await _db.Factions.ToListAsync();
                viewModel.PosterWon = viewModel.BattleReport.WinnersUsername == User.Identity.Name;
                int postersFactionId = viewModel.Factions.First(f => f.Name == viewModel.BattleReport.PostersFaction).Id;
                viewModel.PostersFactionThemes = await _db.Themes.Where(t => t.FactionId == postersFactionId).ToListAsync();
                viewModel.PostersFactionCasters = await _db.Casters.Where(c => c.FactionId == postersFactionId).ToListAsync();
                int opponentsFactionId = viewModel.Factions.First(f => f.Name == viewModel.BattleReport.OpponentsFaction).Id;
                viewModel.OpponentsFactionThemes = await _db.Themes.Where(t => t.FactionId == opponentsFactionId).ToListAsync();
                viewModel.OpponentsFactionCasters = await _db.Casters.Where(c => c.FactionId == opponentsFactionId).ToListAsync();
                return View(viewModel);
            }

            BattleReport battleReport = await _db.BattleReports.FindAsync(viewModel.BattleReport.Id);

            Faction postersFaction = await _db.Factions.FindAsync(int.Parse(viewModel.BattleReport.PostersFaction));
            battleReport.PostersFaction = postersFaction.Name;
            battleReport.PostersTheme = viewModel.BattleReport.PostersTheme;
            battleReport.PostersCaster = viewModel.BattleReport.PostersCaster;
            battleReport.PostersArmyList = viewModel.BattleReport.PostersArmyList;
            battleReport.PostersArmyPoints = viewModel.BattleReport.PostersArmyPoints;
            battleReport.PostersControlPoints = viewModel.BattleReport.PostersControlPoints;

            Faction opponentsFaction = await _db.Factions.FindAsync(int.Parse(viewModel.BattleReport.OpponentsFaction));
            battleReport.OpponentsFaction = opponentsFaction.Name;
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