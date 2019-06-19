using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models
{
    public class BattleReport
    {
        public int Id { get; set; }

        [Required]
        public DateTime DatePlayed { get; set; }

        [Required]
        public string PostersUsername { get; set; }

        [Required]
        public string OpponentsUsername { get; set; }
        public bool ConfirmedByOpponent { get; set; }
        public int ConfirmationKey { get; set; }
        public int GameSize { get; set; }

        [Required]
        public string Scenario { get; set; }

        [Required]
        public string PostersFaction { get; set; }

        [Required]
        public string PostersCaster { get; set; }

        [Required]
        public string PostersTheme { get; set; }
        public int PostersControlPoints { get; set; }
        public int PostersArmyPoints { get; set; }

        [Required]
        public string PostersArmyList { get; set; }

        [Required]
        public string OpponentsFaction { get; set; }

        [Required]
        public string OpponentsCaster { get; set; }

        [Required]
        public string OpponentsTheme { get; set; }
        public int OpponentsControlPoints { get; set; }
        public int OpponentsArmyPoints { get; set; }

        [Required]
        public string OpponentsArmyList { get; set; }

        [Required]
        public string EndCondition { get; set; }

        public string WinnersUsername { get; set; }
        public string WinningFaction { get; set; }
        public string WinningCaster { get; set; }
        public string WinningTheme { get; set; }
        public string LosersUsername { get; set; }
        public string LosingFaction { get; set; }
        public string LosingCaster { get; set; }
        public string LosingTheme { get; set; }
    }
}
