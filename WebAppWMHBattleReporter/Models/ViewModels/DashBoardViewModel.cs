using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class DashBoardViewModel
    {
        public List<Faction> Top10Factions { get; set; }
        public List<Theme> Top10Themes { get; set; }
        public List<Caster> Top10Casters { get; set; }
        public List<ApplicationUser> Top10Users { get; set; }
    }
}
