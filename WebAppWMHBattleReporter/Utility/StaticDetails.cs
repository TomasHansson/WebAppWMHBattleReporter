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
        public const string AllRegions = "All Regions";
        public const string Africa = "Africa";
        public const string Asia = "Asia";
        public const string Europe = "Europe";
        public const string NorthAmerica = "North America";
        public const string Oceania = "Oceania";
        public const string SouthAmerica = "South America";
        public static readonly List<string> Regions = new List<string>{ Africa, Asia, Europe, NorthAmerica, Oceania, SouthAmerica };
        public static readonly List<string> AllAndRegions = new List<string>{ AllRegions, Africa, Asia, Europe, NorthAmerica, Oceania, SouthAmerica };
        public const int GameSize35 = 35; 
        public const int GameSize50 = 50; 
        public const int GameSize75 = 75; 
        public const int GameSize100 = 100; 
        public static readonly List<int> GameSizeOptions = new List<int> { GameSize35, GameSize50, GameSize75, GameSize100 };
        public const string ThePitII = "The Pit II";
        public const string Standoff = "Standoff";
        public const string SpreadTheNet = "Spread The Net";
        public const string Invasion = "Invasion";
        public const string Mirage = "Mirage";
        public const string ReconII = "Recon II";
        public static readonly List<string> ScenarioOptions = new List<string> { ThePitII, Standoff, SpreadTheNet, Invasion, Mirage, ReconII };
        public const string Scenario = "Scenario";
        public const string Assassination = "Assassination";
        public const string Clock = "Clock";
        public static readonly List<string> EndConditionOptions = new List<string> { Scenario, Assassination, Clock };
        public const string FactionType = "Faction";
        public const string ThemeType = "Theme";
        public const string CasterType = "Caster";
        public const string GameSizeType = "Game Size";
        public const string ScenarioType = "Scenario";
        public const string EndConditionType = "End Condition";
        public const string VersusFactionType = "Enemy Faction";
        public const string VersusThemeType = "Enemy Theme";
        public const string VersusCasterType = "Enemy Caster";
        public const string NoRecords = "No records";
        public const string AllTime = "All Time";
        public const string LastYear = "Last Year";
        public const string Last6Months = "Last 6 Months";
        public const string LastMonth = "Last Month";
        public static readonly List<string> TimePeriodOptions = new List<string>() { AllTime, LastYear, Last6Months, LastMonth };
        public const string AllFactions = "All Factions";
        public const string AllThemes = "All Themes";
        public const string AllCasters = "All Casters";
    }
}
