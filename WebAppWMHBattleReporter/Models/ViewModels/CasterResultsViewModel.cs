using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class CasterResultsViewModel
    {
        public string Faction { get; set; }
        public List<string> FactionOptions { get; set; }
        public List<CasterResult> CasterResults { get; set; }
    }
}
