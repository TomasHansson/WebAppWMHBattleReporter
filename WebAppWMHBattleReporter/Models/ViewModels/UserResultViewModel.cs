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
        public List<EntityResult> GameSizes { get; set; }
        public List<EntityResult> Scenarios { get; set; }
        public List<EntityResult> EndConditions { get; set; }
        public List<EntityResult> VersusFactions { get; set; }
        public List<EntityResult> VersusThemes { get; set; }
        public List<EntityResult> VersusCasters { get; set; }
    }
}
