using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models.ViewModels
{
    public class UnconfirmedBattleReportsViewModel
    {
        public List<BattleReport> UsersBattleReports { get; set; }
        public List<BattleReport> OpponentsBattleReports { get; set; }
    }
}
