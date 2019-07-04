using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class UserResultViewModel
    {
        public string Username { get; set; }
        public string StatusMessage { get; set; }
        public UserResult UserResult { get; set; }
        public List<Faction> Factions { get; set; }
        public List<Theme> Themes { get; set; }
        public List<Caster> Casters { get; set; }
    }
}
