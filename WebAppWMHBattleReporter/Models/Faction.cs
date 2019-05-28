using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models
{
    public class Faction
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int NumberOfGamesPlayed { get; set; }
        public int NumberOfGamesWon { get; set; }
        public int NumberOfGamesLost { get; set; }
        public float Winrate { get; set; }
    }
}
