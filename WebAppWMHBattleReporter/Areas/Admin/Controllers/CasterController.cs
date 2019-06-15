using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppWMHBattleReporter.Data;
using WebAppWMHBattleReporter.Models;
using WebAppWMHBattleReporter.Models.ViewModels;

namespace WebAppWMHBattleReporter.Areas.Admin.Controllers
{
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
                Factions = await _db.Factions.ToListAsync()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            CasterViewModel viewModel = new CasterViewModel()
            {
                Factions = await _db.Factions.ToListAsync(),
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

            List<Caster> factionCasters = await _db.Casters.Where(c => c.FactionId == id).ToListAsync();
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
                viewModel.Factions = await _db.Factions.ToListAsync();
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
                Factions = await _db.Factions.ToListAsync(),
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
                viewModel.Factions = await _db.Factions.ToListAsync();
                viewModel.Casters = await _db.Casters.ToListAsync();
                viewModel.StatusMessage = "Error: A caster with that name for the given faction already exists in the database.";
                return View(viewModel);
            }

            Caster casterFromDB = await _db.Casters.FindAsync(viewModel.Caster.Id);
            if (casterFromDB == null)
                return NotFound();

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

            return View(casterFromDB);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            Caster casterFromDB = await _db.Casters.FindAsync(id);
            if (casterFromDB == null)
                return NotFound();

            _db.Casters.Remove(casterFromDB);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}