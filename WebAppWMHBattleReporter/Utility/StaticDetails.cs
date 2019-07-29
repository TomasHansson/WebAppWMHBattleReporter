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
        public const string AllGameSizes = "All Game Sizes";
        public const string GameSize35Points = "35";
        public const string GameSize50Points = "50";
        public const string GameSize75Points = "75";
        public const string GameSize100Points = "100";
        public static readonly List<string> GameSizeFilterOptions = new List<string>() { AllGameSizes, GameSize35Points, GameSize50Points, GameSize75Points, GameSize100Points };
        public const string AllEndConditions = "All End Conditions";
        public static readonly List<string> EndconditionFilterOptions = new List<string>() { AllEndConditions, Scenario, Assassination, Clock };
        public const string AllScenarios = "All Scenarios";
        public static readonly List<string> ScenarioFilterOptions = new List<string>() { AllScenarios, ThePitII, Standoff, SpreadTheNet, Invasion, Mirage, ReconII };
        public const string AllOutcomes = "All Outcomes";
        public const string YouWon = "You Won";
        public const string YouLost = "You Lost";
        public static readonly List<string> OutComeOptions = new List<string>() { AllOutcomes, YouWon, YouLost };
        public const string Cygnar = "Cygnar";
        public const string Protectorate = "Protectorate of Menoth";
        public const string Khador = "Khador";
        public const string Cryx = "Cryx";
        public const string Retribution = "Retribution of Scyrah";
        public const string Convergence = "Convergence";
        public const string Crucible = "Crucible Guard";
        public const string Infernals = "Infernals";
        public const string Mercenaries = "Mercenaries";
        public const string Trollblood = "Trollblood";
        public const string Circle = "Circle Orboros";
        public const string Legion = "Legion of Everblight";
        public const string Skorne = "Skorne";
        public const string Grymkin = "Grymkin";
        public const string Minions = "Minions";
        public static readonly List<string> CurrentFactions = new List<string> { Cygnar, Protectorate, Khador, Cryx, Retribution, Convergence,
                                                    Crucible, Infernals, Mercenaries, Trollblood, Circle, Legion, Skorne, Grymkin, Minions };
        public static readonly Dictionary<string, string> CygnarThemes = new Dictionary<string, string>
        {
            { "Flame in the Darkness", Cygnar},
            { "Gravediggers", Cygnar},
            { "Heavy Metal", Cygnar},
            { "Sons of the Tempest", Cygnar},
            { "Storm Division" , Cygnar}
        };
        public static readonly Dictionary<string, string> CygnarCasters = new Dictionary<string, string>
        {
            { "Blaize 1", Cygnar},
            { "Brisbane 1", Cygnar},
            { "Brisbane 2", Cygnar},
            { "Caine 1", Cygnar},
            { "Caine 2", Cygnar},
            { "Caine 3", Cygnar},
            { "Darius 1", Cygnar},
            { "Haley 1", Cygnar},
            { "Haley 2", Cygnar},
            { "Haley 3", Cygnar},
            { "Jakes 2", Cygnar},
            { "Kraye 1", Cygnar},
            { "Maddox 1", Cygnar},
            { "Nemo 1", Cygnar},
            { "Nemo 2", Cygnar},
            { "Nemo 3", Cygnar},
            { "Sloan 1", Cygnar},
            { "Stryker 1", Cygnar},
            { "Stryker 2", Cygnar},
            { "Stryker 3", Cygnar},
            { "Sturgis 1", Cygnar}
        };
        public static readonly Dictionary<string, string> ProtectorateThemes = new Dictionary<string, string>
        {
            { "The Faithful Masses", Protectorate},
            { "The Creator's Might", Protectorate},
            { "Exemplar Interdiction", Protectorate},
            { "Warriors of the Old Faith", Protectorate},
            { "Guardians of the Temple", Protectorate}
        };
        public static readonly Dictionary<string, string> ProtectorateCasters = new Dictionary<string, string>
        {
            { "Amon 1", Protectorate},
            { "Cyrenia 1", Protectorate},
            { "Durant 2", Protectorate},
            { "Durst 1", Protectorate},
            { "Feora 1", Protectorate},
            { "Feora 2", Protectorate},
            { "Feora 3", Protectorate},
            { "Harbinger 1", Protectorate},
            { "High Reclaimer 1", Protectorate},
            { "High Reclaimer 2", Protectorate},
            { "Kreoss 1", Protectorate},
            { "Kreoss 2", Protectorate},
            { "Kreoss 3", Protectorate},
            { "Malekus 1", Protectorate},
            { "Reznik 1", Protectorate},
            { "Reznik 2", Protectorate},
            { "Severius 1", Protectorate},
            { "Severius 2", Protectorate},
            { "Thyra 1", Protectorate},
            { "Vindictus 1", Protectorate}
        };
        public static readonly Dictionary<string, string> KhadorThemes = new Dictionary<string, string>
        {
            { "Jaws of the Wolf", Khador},
            { "Warriors of the Old Faith", Khador},
            { "Flame in the Darkness", Khador},
            { "Armored Korps", Khador},
            { "Legions of Steel", Khador},
            { "Winter Guard Kommand", Khador},
            { "Wolves of Winter", Khador},
        };
        public static readonly Dictionary<string, string> KhadorCasters = new Dictionary<string, string>
        {
            { "Butcher 1", Khador},
            { "Butcher 2", Khador},
            { "Butcher 3", Khador},
            { "Harkevich 1", Khador},
            { "Irusk 1", Khador},
            { "Irusk 2", Khador},
            { "Karchev 1", Khador},
            { "Kozlov 1", Khador},
            { "Malakov 2", Khador},
            { "Old Witch 1", Khador},
            { "Old Witch 2", Khador},
            { "Sorscha 1", Khador},
            { "Sorscha 2", Khador},
            { "Sorscha 3", Khador},
            { "Strakhov 1", Khador},
            { "Strakhov 2", Khador},
            { "Vladimir 1", Khador},
            { "Vladimir 2", Khador},
            { "Vladimir 3", Khador},
            { "Zerkova 1", Khador},
            { "Zerkova 2", Khador},
        };
        public static readonly Dictionary<string, string> CryxThemes = new Dictionary<string, string>
        {
            { "Black Industries", Cryx},
            { "Dark Host", Cryx},
            { "The Ghost Fleet", Cryx},
            { "Scourge of the Broken Coast", Cryx}
        };
        public static readonly Dictionary<string, string> CryxCasters = new Dictionary<string, string>
        {
            { "Agathia 1", Cryx},
            { "Aiakos 2", Cryx},
            { "Asphyxious 1", Cryx},
            { "Asphyxious 2", Cryx},
            { "Asphyxious 3", Cryx},
            { "Coven 1", Cryx},
            { "Deneghra 1", Cryx},
            { "Deneghra 2", Cryx},
            { "Deneghra 3", Cryx},
            { "Goreshade 1", Cryx},
            { "Goreshade 2", Cryx},
            { "Goreshade 3", Cryx},
            { "Mortenebra 1", Cryx},
            { "Mortenebra 2", Cryx},
            { "Rahera 1", Cryx},
            { "Scaverous 1", Cryx},
            { "Skarre 1", Cryx},
            { "Skarre 2", Cryx},
            { "Skarre 3", Cryx},
            { "Sturgis 2", Cryx},
            { "Terminus 1", Cryx},
            { "Venethrax 1", Cryx}
        };
        public static readonly Dictionary<string, string> RetributionThemes = new Dictionary<string, string>
        {
            { "Defenders of Ios", Retribution},
            { "Legions of Dawn", Retribution},
            { "Forges of War", Retribution},
            { "Shadows of the Retribution", Retribution}
        };
        public static readonly Dictionary<string, string> RetributionCasters = new Dictionary<string, string>
        {
            { "Elara 2", Retribution},
            { "Garryth 1", Retribution},
            { "Garryth 2", Retribution},
            { "Goreshade 4", Retribution},
            { "Helynna 1", Retribution},
            { "Issyria 1", Retribution},
            { "Kaelyssa 1", Retribution},
            { "Ossyan 1", Retribution},
            { "Rahn 1", Retribution},
            { "Ravyn 1", Retribution},
            { "Thyron 1", Retribution},
            { "Vyros 1", Retribution},
            { "Vyros 2", Retribution},
        };
        public static readonly Dictionary<string, string> ConvergenceThemes = new Dictionary<string, string>
        {
            { "Clockwork Legions", Convergence},
            { "Destruction Initiative", Convergence}
        };
        public static readonly Dictionary<string, string> ConvergenceCasters = new Dictionary<string, string>
        {
            { "Aurora 1", Convergence},
            { "Axis 1", Convergence},
            { "Directix 1", Convergence},
            { "Locke 1" , Convergence},
            { "Lucant 1", Convergence},
            { "Orion 1", Convergence},
            { "Syntherion 1", Convergence}
        };
        public static readonly Dictionary<string, string> CrucibleThemes = new Dictionary<string, string>
        {
            { "Magnum Opus", Crucible},
            { "Prima Materia", Crucible}
        };
        public static readonly Dictionary<string, string> CrucibleCasters = new Dictionary<string, string>
        {
            { "Gearhart 1", Crucible},
            { "Locke 1", Crucible},
            { "Lukas 1", Crucible},
            { "Mackay 1", Crucible},
            { "Syvestro 1", Crucible},
        };
        public static readonly Dictionary<string, string> InfernalsThemes = new Dictionary<string, string>
        {
            { "Dark Legacy", Infernals},
            { "Hearts of Darkness", Infernals}
        };
        public static readonly Dictionary<string, string> InfernalsCasters = new Dictionary<string, string>
        {
            { "Agathon 1", Infernals},
            { "Omodamos 1", Infernals},
            { "Zaateroth 1", Infernals}
        };
        public static readonly Dictionary<string, string> MercenariesThemes = new Dictionary<string, string>
        {
            { "Flame in the Darkness", Mercenaries},
            { "Hammer Strike", Mercenaries},
            { "The Irregulars", Mercenaries},
            { "The Kingmaker's Army", Mercenaries},
            { "Llaelese Resistance", Mercenaries},
            { "Operating Theater", Mercenaries},
            { "Soldiers of Fortune", Mercenaries},
            { "Talion Charter", Mercenaries}
        };
        public static readonly Dictionary<string, string> MercenariesCasters = new Dictionary<string, string>
        {
            { "Ashlynn 1", Mercenaries},
            { "Blaize 1", Mercenaries},
            { "Caine 3", Mercenaries},
            { "Crosse 2", Mercenaries},
            { "Cyphon 1", Mercenaries},
            { "Damiano 1", Mercenaries},
            { "Durgen 1", Mercenaries},
            { "Fiona 1", Mercenaries},
            { "Gorten 1", Mercenaries},
            { "MacBain 1", Mercenaries},
            { "Magnus 1", Mercenaries},
            { "Magnus 2", Mercenaries},
            { "Montador 1", Mercenaries},
            { "Ossrum 1", Mercenaries},
            { "Rahera 1", Mercenaries},
            { "Shae 1", Mercenaries},
            { "Thexus 1", Mercenaries}
        };
        public static readonly Dictionary<string, string> TrollbloodThemes = new Dictionary<string, string>
        {
            { "Kriel Company", Trollblood},
            { "Band of Heroes", Trollblood},
            { "Power of Dhunia", Trollblood},
            { "Storm of the North", Trollblood}
        };
        public static readonly Dictionary<string, string> TrollbloodCasters = new Dictionary<string, string>
        {
            { "Borka 1", Trollblood},
            { "Borka 2", Trollblood},
            { "Calandra 1", Trollblood},
            { "Doomshaper 1", Trollblood},
            { "Doomshaper 2", Trollblood},
            { "Doomshaper 3", Trollblood},
            { "Grim 1", Trollblood},
            { "Grim 2", Trollblood},
            { "Grissel 1", Trollblood},
            { "Grissel 2", Trollblood},
            { "Gunnbjorn 1", Trollblood},
            { "Horgle 2", Trollblood},
            { "Kolgrima 1", Trollblood},
            { "Madrak 1", Trollblood},
            { "Madrak 2", Trollblood},
            { "Madrak 3", Trollblood},
            { "Ragnor 1", Trollblood},
            { "Skuld 1", Trollblood}
        };
        public static readonly Dictionary<string, string> CircleThemes = new Dictionary<string, string>
        {
            { "The Wild Hunt", Circle},
            { "Bones of Orboros", Circle},
            { "The Devourer's Host", Circle},
            { "Secret Masters", Circle}
        };
        public static readonly Dictionary<string, string> CircleCasters = new Dictionary<string, string>
        {
            { "Baldur 1", Circle},
            { "Baldur 2", Circle},
            { "Grayle 1", Circle},
            { "Iona 1", Circle},
            { "Kaya 1", Circle},
            { "Kaya 2", Circle},
            { "Kaya 3", Circle},
            { "Kromac 1", Circle},
            { "Kromac 2", Circle},
            { "Krueger 1", Circle},
            { "Krueger 2", Circle},
            { "Mohsar 1", Circle},
            { "Morvahna 1", Circle},
            { "Morvahna 2", Circle},
            { "Tanith 1", Circle},
            { "Thorle 1", Circle},
            { "Una 2", Circle},
            { "Wurmwood 1", Circle},
        };
        public static readonly Dictionary<string, string> LegionThemes = new Dictionary<string, string>
        {
            { "Children of the Dragon", Legion},
            { "Oracles of Annihilation", Legion},
            { "Primal Terrors", Legion},
            { "Ravens of War", Legion}
        };
        public static readonly Dictionary<string, string> LegionCasters = new Dictionary<string, string>
        {
            { "Absylonia 1", Legion},
            { "Absylonia 2", Legion},
            { "Anamag 1", Legion},
            { "Bethayne 1", Legion},
            { "Fyanna 2", Legion},
            { "Kallus 1", Legion},
            { "Kallus 2", Legion},
            { "Kryssa 1", Legion},
            { "Lylyth 1", Legion},
            { "Lylyth 2", Legion},
            { "Lylyth 3", Legion},
            { "Rhyvas 1", Legion},
            { "Saeryn 1", Legion},
            { "Thagrosh 1", Legion},
            { "Thagrosh 2", Legion},
            { "Twins 2", Legion},
            { "Vayl 1", Legion},
            { "Vayl 2", Legion}
        };
        public static readonly Dictionary<string, string> SkorneThemes = new Dictionary<string, string>
        {
            { "Disciples of Agony", Skorne},
            { "The Exalted", Skorne},
            { "Masters of War", Skorne},
            { "Winds of Death", Skorne}
        };
        public static readonly Dictionary<string, string> SkorneCasters = new Dictionary<string, string>
        {
            { "Hexeris 1", Skorne},
            { "Hexeris 2", Skorne},
            { "Jalaam 1", Skorne},
            { "Makeda 1", Skorne},
            { "Makeda 2", Skorne},
            { "Makeda 3", Skorne},
            { "Mordikaar 1", Skorne},
            { "Morghoul 1", Skorne},
            { "Morghoul 2", Skorne},
            { "Morghoul 3", Skorne},
            { "Naaresh 1", Skorne},
            { "Rasheth 1", Skorne},
            { "Xekaar 1", Skorne},
            { "Xerxis 1", Skorne},
            { "Xerxis 2", Skorne},
            { "Zaadesh 2", Skorne},
            { "Zaal 1", Skorne},
            { "Zaal 2", Skorne}
        };
        public static readonly Dictionary<string, string> GrymkinThemes = new Dictionary<string, string>
        {
            { "Bump in the Night", Grymkin},
            { "Dark Menagerie", Grymkin}
        };
        public static readonly Dictionary<string, string> GrymkinCasters = new Dictionary<string, string>
        {
            { "Child 1", Grymkin},
            { "Dreamer 1", Grymkin},
            { "Heretic 1", Grymkin},
            { "King of Nothing 1", Grymkin},
            { "Old Witch 3", Grymkin},
            { "Wanderer 1", Grymkin}
        };
        public static readonly Dictionary<string, string> MinionsThemes = new Dictionary<string, string>
        {
            { "The Blindwater Congregation", Minions},
            { "The Thornfall Alliance", Minions},
            { "Will Work For Food", Minions}
        };
        public static readonly Dictionary<string, string> MinionsCasters = new Dictionary<string, string>
        {
            { "Arkadius 1", Minions},
            { "Barnabas 1", Minions},
            { "Barnabas 2", Minions},
            { "Calaban 1", Minions},
            { "Carver 1", Minions},
            { "Helga 1", Minions},
            { "Jaga-Jaga 1", Minions},
            { "Maelok 1", Minions},
            { "Midas 1", Minions},
            { "Rask 1", Minions},
            { "Sturm 1 & Drang 1", Minions}
        };
        public static readonly List<Dictionary<string, string>> CurrentThemes = new List<Dictionary<string, string>>
        {
            CygnarThemes,
            ProtectorateThemes,
            KhadorThemes,
            CryxThemes,
            RetributionThemes,
            ConvergenceThemes,
            CrucibleThemes,
            InfernalsThemes,
            MercenariesThemes,
            TrollbloodThemes,
            CircleThemes,
            LegionThemes,
            SkorneThemes,
            GrymkinThemes,
            MinionsThemes
        };
        public static readonly List<Dictionary<string, string>> CurrentCasters = new List<Dictionary<string, string>>
        {
            CygnarCasters,
            ProtectorateCasters,
            KhadorCasters,
            CryxCasters,
            RetributionCasters,
            ConvergenceCasters,
            CrucibleCasters,
            InfernalsCasters,
            MercenariesCasters,
            TrollbloodCasters,
            CircleCasters,
            LegionCasters,
            SkorneCasters,
            GrymkinCasters,
            MinionsCasters
        };
    }
}
