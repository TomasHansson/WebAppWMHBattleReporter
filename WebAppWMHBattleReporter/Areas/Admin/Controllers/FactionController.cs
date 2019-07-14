using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppWMHBattleReporter.Data;
using WebAppWMHBattleReporter.Models;
using WebAppWMHBattleReporter.Models.ViewModels;
using WebAppWMHBattleReporter.Utility;

namespace WebAppWMHBattleReporter.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Administrator)]
    [Area("Admin")]
    public class FactionController : Controller
    {
        private ApplicationDbContext _db;

        public FactionController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            List<Faction> factionsFromDB = await _db.Factions.ToListAsync();
            return View(factionsFromDB);
        }

        public IActionResult Create()
        {
            return View(new FactionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FactionViewModel viewModel)
        {
            viewModel.StatusMessage = string.Empty;
            if (!ModelState.IsValid)
                return View(viewModel);

            if(await _db.Factions.AnyAsync(f => f.Name == viewModel.Faction.Name))
            {
                viewModel.StatusMessage = "Error: A faction with that name already exists in the database.";
                return View(viewModel);
            }

            _db.Factions.Add(viewModel.Faction);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            Faction factionFromDB = await _db.Factions.FindAsync(id);
            if (factionFromDB == null)
                return NotFound();

            return View(factionFromDB);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            Faction factionFromDB = await _db.Factions.FindAsync(id);
            if (factionFromDB == null)
                return NotFound();

            FactionViewModel viewModel = new FactionViewModel()
            {
                Faction = factionFromDB
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FactionViewModel viewModel)
        {
            viewModel.StatusMessage = string.Empty;
            if (!ModelState.IsValid)
                return View(viewModel);

            if (await _db.Factions.AnyAsync(f => f.Name == viewModel.Faction.Name))
            {
                viewModel.StatusMessage = "Error: A faction with that name already exists in the database.";
                return View(viewModel);
            }

            Faction factionFromDB = await _db.Factions.FindAsync(viewModel.Faction.Id);
            if (factionFromDB == null)
                return NotFound();

            List<BattleReport> battleReports = await _db.BattleReports.Where(br => br.PostersFaction == factionFromDB.Name || br.OpponentsFaction == factionFromDB.Name).ToListAsync();
            foreach (BattleReport battleReport in battleReports)
            {
                if (battleReport.PostersFaction == factionFromDB.Name)
                    battleReport.PostersFaction = viewModel.Faction.Name;
                if (battleReport.OpponentsFaction == factionFromDB.Name)
                    battleReport.OpponentsFaction = viewModel.Faction.Name;
                if (battleReport.WinningFaction == factionFromDB.Name)
                    battleReport.WinningFaction = viewModel.Faction.Name;
                if (battleReport.LosingFaction == factionFromDB.Name)
                    battleReport.LosingFaction = viewModel.Faction.Name;
            }

            factionFromDB.Name = viewModel.Faction.Name;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            Faction factionFromDB = await _db.Factions.FindAsync(id);
            if (factionFromDB == null)
                return NotFound();

            FactionViewModel viewModel = new FactionViewModel()
            {
                Faction = factionFromDB
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            Faction factionFromDB = await _db.Factions.FindAsync(id);
            if (factionFromDB == null)
                return NotFound();

            if (await _db.BattleReports.AnyAsync(br => br.WinningFaction == factionFromDB.Name || br.LosingFaction == factionFromDB.Name))
            {
                FactionViewModel viewModel = new FactionViewModel()
                {
                    Faction = factionFromDB,
                    StatusMessage = "Error: You cannot delete a Faction for which there are already posted Battle Reports."
                };
                return View(nameof(Delete), viewModel);
            }

            _db.Factions.Remove(factionFromDB);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}