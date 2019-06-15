using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAppWMHBattleReporter.Data;

namespace WebAppWMHBattleReporter.Areas.Home.Controllers
{
    [Area("Home")]
    public class BattleReportController : Controller
    {
        private ApplicationDbContext _db;

        public BattleReportController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}