using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class ThemeViewModel
    {
        public List<Faction> Factions { get; set; }
        public List<Theme> Themes { get; set; }
        public Theme Theme { get; set; }
        public string StatusMessage { get; set; }
    }
}
