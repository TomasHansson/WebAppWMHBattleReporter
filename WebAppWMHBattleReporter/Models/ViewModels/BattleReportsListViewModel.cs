using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class BattleReportsListViewModel
    {
        public List<BattleReport> BattleReports { get; set; }
        public List<string> TimePeriodOptions { get; set; }
        public string TimePeriod { get; set; }
        public List<string> FactionOptions { get; set; }
        public string P1Faction { get; set; }
        public List<string> P1ThemeOptions { get; set; }
        public string P1Theme { get; set; }
        public List<string> P1CasterOptions { get; set; }
        public string P1Caster { get; set; }
        public string P2Faction { get; set; }
        public List<string> P2ThemeOptions { get; set; }
        public string P2Theme { get; set; }
        public List<string> P2CasterOptions { get; set; }
        public string P2Caster { get; set; }
        public List<string> GameSizeOptions { get; set; }
        public string GameSize { get; set; }
        public List<string> EndConditionOptions { get; set; }
        public string EndCondition { get; set; }
        public List<string> ScenarioOptions { get; set; }
        public string Scenario { get; set; }
        public bool HideFilters { get; set; }
    }
}
