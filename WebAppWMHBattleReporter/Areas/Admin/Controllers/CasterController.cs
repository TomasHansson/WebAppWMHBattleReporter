using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppWMHBattleReporter.Data;
using WebAppWMHBattleReporter.Models;
using WebAppWMHBattleReporter.Models.ViewModels;
using WebAppWMHBattleReporter.Utility;

namespace WebAppWMHBattleReporter.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.Administrator)]
    [Area("Admin")]
    public class CasterController : Controller
    {
        private ApplicationDbContext _db;

        public CasterController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            CasterViewModel viewModel = new CasterViewModel()
            {
                Casters = await _db.Casters.ToListAsync(),
                Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            CasterViewModel viewModel = new CasterViewModel()
            {
                Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync(),
                Casters = await _db.Casters.ToListAsync(),
                Caster = new Caster(),
                StatusMessage = string.Empty
            };
            return View(viewModel);
        }

        public async Task<IActionResult> GetCasters(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Factions.AnyAsync(f => f.Id == id))
                return NotFound();

            List<Caster> factionCasters = await _db.Casters.Where(c => c.FactionId == id).OrderBy(c => c.Name).ToListAsync();
            return Json(new SelectList(factionCasters, "Id", "Name"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CasterViewModel viewModel)
        {
            viewModel.StatusMessage = string.Empty;
            if (!ModelState.IsValid)
                return View(viewModel);

            if (await _db.Casters.Where(c => c.FactionId == viewModel.Caster.FactionId).AnyAsync(c => c.Name == viewModel.Caster.Name))
            {
                viewModel.Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
                viewModel.Casters = await _db.Casters.ToListAsync();
                viewModel.StatusMessage = "Error: A caster with that name for the given faction already exists in the database.";
                return View(viewModel);
            }

            _db.Casters.Add(viewModel.Caster);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Casters.AnyAsync(c => c.Id == id))
                return NotFound();

            Caster casterFromDB = await _db.Casters.Include(c => c.Faction).FirstAsync(c => c.Id == id);
            return View(casterFromDB);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Casters.AnyAsync(c => c.Id == id))
                return NotFound();

            CasterViewModel viewModel = new CasterViewModel()
            {
                Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync(),
                Casters = await _db.Casters.ToListAsync(),
                Caster = await _db.Casters.Include(c => c.Faction).FirstAsync(c => c.Id == id),
                StatusMessage = string.Empty
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CasterViewModel viewModel)
        {
            viewModel.StatusMessage = string.Empty;
            if (!ModelState.IsValid)
                return View(viewModel);

            if (await _db.Casters.Where(c => c.FactionId == viewModel.Caster.FactionId).AnyAsync(c => c.Name == viewModel.Caster.Name))
            {
                viewModel.Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
                viewModel.Casters = await _db.Casters.ToListAsync();
                viewModel.StatusMessage = "Error: A caster with that name for the given faction already exists in the database.";
                return View(viewModel);
            }

            Caster casterFromDB = await _db.Casters.FindAsync(viewModel.Caster.Id);
            if (casterFromDB == null)
                return NotFound();

            List<BattleReport> battleReports = await _db.BattleReports.Where(br => br.PostersCaster == casterFromDB.Name || br.OpponentsCaster == casterFromDB.Name).ToListAsync();
            if (battleReports.Count > 0 && casterFromDB.FactionId != viewModel.Caster.FactionId)
            {
                viewModel.Factions = await _db.Factions.OrderBy(f => f.Name).ToListAsync();
                viewModel.Casters = await _db.Casters.ToListAsync();
                viewModel.StatusMessage = "Error: You cannot change the Faction for a Caster for which there are already posted Battle Reports.";
                return View(viewModel);
            }

            foreach (BattleReport battleReport in battleReports)
            {
                if (battleReport.PostersCaster == casterFromDB.Name)
                    battleReport.PostersCaster = viewModel.Caster.Name;
                if (battleReport.OpponentsCaster == casterFromDB.Name)
                    battleReport.OpponentsCaster = viewModel.Caster.Name;
                if (battleReport.WinningCaster == casterFromDB.Name)
                    battleReport.WinningCaster = viewModel.Caster.Name;
                if (battleReport.LosingCaster == casterFromDB.Name)
                    battleReport.LosingCaster = viewModel.Caster.Name;
            }

            casterFromDB.Name = viewModel.Caster.Name;
            casterFromDB.FactionId = viewModel.Caster.FactionId;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Casters.AnyAsync(t => t.Id == id))
                return NotFound();

            Caster casterFromDB = await _db.Casters.Include(c => c.Faction).FirstAsync(c => c.Id == id);
            if (casterFromDB == null)
                return NotFound();

            CasterViewModel viewModel = new CasterViewModel()
            {
                Caster = casterFromDB
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            Caster casterFromDB = await _db.Casters.Include(c => c.Faction).FirstAsync(c => c.Id == id);
            if (casterFromDB == null)
                return NotFound();

            if (await _db.BattleReports.AnyAsync(br => br.WinningCaster == casterFromDB.Name || br.LosingCaster == casterFromDB.Name))
            {
                CasterViewModel viewModel = new CasterViewModel()
                {
                    Caster = casterFromDB,
                    StatusMessage = "Error: You cannot delete a Caster for which there are already posted Battle Reports."
                };
                return View(nameof(Delete), viewModel);
            }

            _db.Casters.Remove(casterFromDB);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}