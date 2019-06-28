using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppWMHBattleReporter.Controllers;
using WebAppWMHBattleReporter.Data;
using WebAppWMHBattleReporter.Models;
using WebAppWMHBattleReporter.Models.ViewModels;

namespace WebAppWMHBattleReporter.Areas.EndUser.Controllers
{
    [Area("EndUser")]
    public class BattleReportController : Controller
    {
        private ApplicationDbContext _db;

        public BattleReportController(ApplicationDbContext db)
        {
            _db = db;
        }

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
            viewModel.BattleReport.PostersUsername = "Posters Username"; // This should be changed to fetch the username with the use of the UserClaim.
            return View(viewModel);
        }

        public async Task<IActionResult> GetFactionThemeNames(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Factions.AnyAsync(f => f.Id == id))
                return NotFound();

            List<string> factionThemes = await _db.Themes.Where(t => t.FactionId == id).Select(t => t.Name).ToListAsync();
            return Json(factionThemes);
        }

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
            viewModel.BattleReport.WinningTheme = viewModel.PosterWon ? viewModel.BattleReport.PostersTheme : viewModel.BattleReport.OpponentsFaction;
            viewModel.BattleReport.WinningCaster = viewModel.PosterWon ? viewModel.BattleReport.PostersCaster : viewModel.BattleReport.OpponentsCaster;

            _db.BattleReports.Add(viewModel.BattleReport);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home", new { Area = "Home" });
        }

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

            List<BattleReport> usersUnconfirmedBattleReports = await _db.BattleReports.Where(br => br.OpponentsUsername == id && br.ConfirmedByOpponent == false).ToListAsync();
            if (usersUnconfirmedBattleReports == null)
                return NotFound();

            return View(usersUnconfirmedBattleReports);
        }

        public async Task<IActionResult> AcceptUnconfirmedView(int? id)
        {
            // Add necessary data for a view where the user can accept an Unconfirmed Battle Report.
            return View();
        }

        public async Task<IActionResult> AcceptUnconfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

            string username = battleReport.OpponentsUsername;
            battleReport.ConfirmedByOpponent = true;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(UnconfirmedBattleReports), new { id = username });
        }

        public async Task<IActionResult> DeleteUnconfirmedView(int? id)
        {
            // Add necessary data for a view where the user can delete an Unconfirmed Battle Report.
            return View();
        }

        public async Task<IActionResult> DeleteUnconfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            BattleReport battleReport = await _db.BattleReports.FindAsync(id);
            if (battleReport == null)
                return NotFound();

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

            return View(battleReport);
        }
    }
}