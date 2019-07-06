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
        public EntityResult Factions { get; set; }
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
