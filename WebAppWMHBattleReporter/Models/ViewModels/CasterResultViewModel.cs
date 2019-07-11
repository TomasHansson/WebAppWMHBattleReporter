using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class CasterResultViewModel
    {
        public string Caster { get; set; }
        public string StatusMessage { get; set; }
        public List<Faction> Factions { get; set; }
        public List<Caster> Casters { get; set; }
        public CasterResult CasterResult { get; set; }
        public EntityResult Themes { get; set; }
        public EntityResult GameSizes { get; set; }
        public EntityResult Scenarios { get; set; }
        public EntityResult EndConditions { get; set; }
        public EntityResult VersusFactions { get; set; }
        public EntityResult VersusThemes { get; set; }
        public EntityResult VersusCasters { get; set; }
    }
}
