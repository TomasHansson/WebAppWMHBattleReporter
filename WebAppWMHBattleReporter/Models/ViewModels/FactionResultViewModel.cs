using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class FactionResultViewModel
    {
        public string Faction { get; set; }
        public string StatusMessage { get; set; }
        public List<Faction> Factions { get; set; }
        public FactionResult FactionResult { get; set; }
        public EntityResult Themes { get; set; }
        public EntityResult Casters { get; set; }
        public EntityResult GameSizes { get; set; }
        public EntityResult Scenarios { get; set; }
        public EntityResult EndConditions { get; set; }
        public EntityResult VersusFactions { get; set; }
        public EntityResult VersusThemes { get; set; }
        public EntityResult VersusCasters { get; set; }
    }
}
