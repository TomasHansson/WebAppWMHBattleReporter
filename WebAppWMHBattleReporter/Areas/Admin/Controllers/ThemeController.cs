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
    public class ThemeController : Controller
    {
        private ApplicationDbContext _db;

        public ThemeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            ThemeViewModel viewModel = new ThemeViewModel()
            {
                Themes = await _db.Themes.ToListAsync(),
                Factions = await _db.Factions.ToListAsync()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            ThemeViewModel viewModel = new ThemeViewModel()
            {
                Factions = await _db.Factions.ToListAsync(),
                Themes = await _db.Themes.ToListAsync(),
                Theme = new Theme(),
                StatusMessage = string.Empty
            };
            return View(viewModel);
        }

        public async Task<IActionResult> GetThemes(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Factions.AnyAsync(f => f.Id == id))
                return NotFound();

            List<Theme> factionThemes = await _db.Themes.Where(t => t.FactionId == id).ToListAsync();
            return Json(new SelectList(factionThemes, "Id", "Name"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThemeViewModel viewModel)
        {
            viewModel.StatusMessage = string.Empty;
            if (!ModelState.IsValid)
                return View(viewModel);

            if (await _db.Themes.Where(t => t.FactionId == viewModel.Theme.FactionId).AnyAsync(t => t.Name == viewModel.Theme.Name))
            {
                viewModel.Factions = await _db.Factions.ToListAsync();
                viewModel.Themes = await _db.Themes.ToListAsync();
                viewModel.StatusMessage = "Error: A theme with that name for the given faction already exists in the database.";
                return View(viewModel);
            }

            _db.Themes.Add(viewModel.Theme);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Themes.AnyAsync(t => t.Id == id))
                return NotFound();

            Theme themeFromDB = await _db.Themes.Include(t => t.Faction).FirstAsync(t => t.Id == id);
            return View(themeFromDB);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Themes.AnyAsync(t => t.Id == id))
                return NotFound();

            ThemeViewModel viewModel = new ThemeViewModel()
            {
                Factions = await _db.Factions.ToListAsync(),
                Themes = await _db.Themes.ToListAsync(),
                Theme = await _db.Themes.Include(t => t.Faction).FirstAsync(t => t.Id == id),
                StatusMessage = string.Empty
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ThemeViewModel viewModel)
        {
            viewModel.StatusMessage = string.Empty;
            if (!ModelState.IsValid)
                return View(viewModel);

            if (await _db.Themes.Where(t => t.FactionId == viewModel.Theme.FactionId).AnyAsync(t => t.Name == viewModel.Theme.Name))
            {
                viewModel.Factions = await _db.Factions.ToListAsync();
                viewModel.Themes = await _db.Themes.ToListAsync();
                viewModel.StatusMessage = "Error: A theme with that name for the given faction already exists in the database.";
                return View(viewModel);
            }

            Theme themeFromDB = await _db.Themes.FindAsync(viewModel.Theme.Id);
            if (themeFromDB == null)
                return NotFound();

            themeFromDB.Name = viewModel.Theme.Name;
            themeFromDB.FactionId = viewModel.Theme.FactionId;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _db.Themes.AnyAsync(t => t.Id == id))
                return NotFound();

            Theme themeFromDB = await _db.Themes.Include(t => t.Faction).FirstAsync(t => t.Id == id);
            if (themeFromDB == null)
                return NotFound();

            return View(themeFromDB);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            Theme themeFromDB = await _db.Themes.FindAsync(id);
            if (themeFromDB == null)
                return NotFound();

            _db.Themes.Remove(themeFromDB);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}