using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppWMHBattleReporter.Data;
using WebAppWMHBattleReporter.Models;
using WebAppWMHBattleReporter.Models.ViewModels;

namespace WebAppWMHBattleReporter.Areas.Admin.Controllers
{
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
            if(!ModelState.IsValid)
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
    }
}