using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models
{
    public class ThemeResult
    {
        public int ListPosition { get; set; }
        public string Name { get; set; }
        public int NumberOfGamesPlayed { get; set; }
        public int NumberOfGamesWon { get; set; }
        public int NumberOfGamesLost { get; set; }
        public int NumberOfMirrorMatches { get; set; }
        public float Winrate { get; set; }
        public string MostPlayedCaster { get; set; }
        public int GamesWithMostPlayedCaster { get; set; }
        public string BestPerformingCaster { get; set; }
        public float WinrateBestPerformingCaster { get; set; }
    }
}
