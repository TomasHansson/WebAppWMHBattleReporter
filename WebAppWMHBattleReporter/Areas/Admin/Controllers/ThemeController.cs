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

            List<BattleReport> battleReports = await _db.BattleReports.Where(br => br.PostersTheme == themeFromDB.Name || br.OpponentsTheme == themeFromDB.Name).ToListAsync();
            if (battleReports.Count > 0 && themeFromDB.FactionId != viewModel.Theme.FactionId)
            {
                viewModel.Factions = await _db.Factions.ToListAsync();
                viewModel.Themes = await _db.Themes.ToListAsync();
                viewModel.StatusMessage = "Error: You cannot change the Faction for a Theme for which there are already posted Battle Reports.";
                return View(viewModel);
            }

            foreach (BattleReport battleReport in battleReports)
            {
                if (battleReport.PostersTheme == themeFromDB.Name)
                    battleReport.PostersTheme = viewModel.Theme.Name;
                if (battleReport.OpponentsTheme == themeFromDB.Name)
                    battleReport.OpponentsTheme = viewModel.Theme.Name;
                if (battleReport.WinningTheme == themeFromDB.Name)
                    battleReport.WinningTheme = viewModel.Theme.Name;
                if (battleReport.LosingTheme == themeFromDB.Name)
                    battleReport.LosingTheme = viewModel.Theme.Name;
            }

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

            ThemeViewModel viewModel = new ThemeViewModel()
            {
                Theme = themeFromDB
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return NotFound();

            Theme themeFromDB = await _db.Themes.Include(t => t.Faction).FirstAsync(t => t.Id == id);
            if (themeFromDB == null)
                return NotFound();

            if (await _db.BattleReports.AnyAsync(br => br.WinningTheme == themeFromDB.Name || br.LosingTheme == themeFromDB.Name))
            {
                ThemeViewModel viewModel = new ThemeViewModel()
                {
                    Theme = themeFromDB,
                    StatusMessage = "Error: You cannot delete a Theme for which there are already posted Battle Reports."
                };
                return View(nameof(Delete), viewModel);
            }

            _db.Themes.Remove(themeFromDB);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}