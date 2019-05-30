using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models
{
    public class Caster
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int FactionId { get; set; }

        [ForeignKey("FactionId")]
        public virtual Faction Faction { get; set; }

        public int NumberOfGamesPlayed { get; set; }
        public int NumberOfGamesWon { get; set; }
        public int NumberOfGamesLost { get; set; }
        public float Winrate { get; set; }
    }
}
