using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppWMHBattleReporter.Models
{
    public class CasterResult
    {
        public int ListPosition { get; set; }
        public string Name { get; set; }
        public int NumberOfGamesPlayed { get; set; }
        public int NumberOfGamesWon { get; set; }
        public int NumberOfGamesLost { get; set; }
        public int NumberOfMirrorMatches { get; set; }
        public float Winrate { get; set; }
        public string MostPlayedTheme { get; set; }
        public int GamesWithMostPlayedTheme { get; set; }
        public string BestPerformingTheme { get; set; }
        public float WinrateBestPerformingTheme { get; set; }
        public string MostPlayedScenario { get; set; }
        public int GamesMostPlayedScenario { get; set; }
        public string BestPerformingScenario { get; set; }
        public float WinrateBestPerformingScenario { get; set; }
        public string MostPlayedEndCondition { get; set; }
        public int GamesMostPlayedEndCondition { get; set; }
        public string BestPerformingEndCondition { get; set; }
        public float WinrateBestPerformingEndCondition { get; set; }
    }
}
