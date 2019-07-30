using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class ThemeResultsViewModel
    {
        public string Faction { get; set; }
        public List<string> FactionOptions { get; set; }
        public List<ThemeResult> ThemeResults { get; set; }
    }
}
