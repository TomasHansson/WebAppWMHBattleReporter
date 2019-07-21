using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class ConfirmedBattleReportsViewModel
    {
        public List<BattleReport> BattleReports { get; set; }
        public string UserName { get; set; }
        public List<string> TimePeriodOptions { get; set; }
        public string TimePeriod { get; set; }
        public List<string> FactionOptions { get; set; }
        public string Faction { get; set; }
        public List<string> ThemeOptions { get; set; }
        public string Theme { get; set; }
        public List<string> CasterOptions { get; set; }
        public string Caster { get; set; }
        public string EnemyFaction { get; set; }
        public List<string> EnemyThemeOptions { get; set; }
        public string EnemyTheme { get; set; }
        public List<string> EnemyCasterOptions { get; set; }
        public string EnemyCaster { get; set; }
        public List<string> GameSizeOptions { get; set; }
        public string GameSize { get; set; }
        public List<string> EndConditionOptions { get; set; }
        public string EndCondition { get; set; }
        public List<string> ScenarioOptions { get; set; }
        public string Scenario { get; set; }
        public List<string> OutComeOptions { get; set; }
        public string OutCome { get; set; }
        public bool HideFilters { get; set; }
    }
}
