using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class BattleReportViewModel
    {
        public List<Faction> Factions { get; set; }
        public List<Theme> FirstFactionThemes { get; set; }
        public List<Caster> FirstFactionCasters { get; set; }
        public BattleReport BattleReport { get; set; }
        public string StatusMessage { get; set; }

        public List<int> GameSizeOptions { get; set; }
        public List<string> ScenarioOptions { get; set; }
        public List<string> EndConditionOptions { get; set; }
        public bool PosterWon { get; set; }
    }
}
