using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class UserResultsViewModel
    {
        public string Region { get; set; }
        public List<UserResult> UserResults { get; set; }
    }
}
