using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class ConfirmedBattleReportsViewModel
    {
        public List<BattleReport> BattleReports { get; set; }
        public List<string> TimePeriodOptions { get; set; }
        public string TimePeriod { get; set; }
        public List<string> FactionOptions { get; set; }
        public string Faction { get; set; }
        public List<string> ThemeOptions { get; set; }
        public string Theme { get; set; }
        public List<string> CasterOptions { get; set; }
        public string Caster { get; set; }
    }
}
