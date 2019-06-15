using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Utility
{
    public static class StaticDetails
    {
        public const string Administrator = "Administrator";
        public const string EndUser = "EndUser";
        public static readonly List<string> Regions = new List<string>{ "Africa", "Asia", "Europe", "North America", "Oceania", "South America" };
    }
}
