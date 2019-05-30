using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAppWMHBattleReporter.Data;

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

        public IActionResult Index()
        {
            return View();
        }
    }
}