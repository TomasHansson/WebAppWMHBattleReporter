using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class EditBattleReportViewModel
    {
        public List<Faction> Factions { get; set; }
        public List<Theme> PostersFactionThemes { get; set; }
        public List<Caster> PostersFactionCasters { get; set; }
        public List<Theme> OpponentsFactionThemes { get; set; }
        public List<Caster> OpponentsFactionCasters { get; set; }
        public BattleReport BattleReport { get; set; }
        public string StatusMessage { get; set; }
        public bool PosterWon { get; set; }
    }
}
