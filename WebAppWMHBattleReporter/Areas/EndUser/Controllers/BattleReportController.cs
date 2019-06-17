﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}