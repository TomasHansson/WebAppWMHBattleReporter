using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class CasterViewModel
    {
        public List<Faction> Factions { get; set; }
        public List<Caster> Casters { get; set; }
        public Caster Caster { get; set; }
        public string StatusMessage { get; set; }
    }
}
