using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class Game
    {
        public string Name { get; set; }
        public string System { get; set; }
        public string Genre { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public DateTime DateNA { get; set; }
        public DateTime DatePAL { get; set; }
        public DateTime DateJAP { get; set; }
        public DateTime Date { get; set; }
        public string ESRB { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }

        // Wikipedia
        public string Artists { get; set; }
        public string Composers { get; set; }
        public string Directors { get; set; }
        public string Producers { get; set; }
        public string Programmers { get; set; }

        // GameRankings
        public int MRank { get; set; }
        public int MReviews { get; set; }
        public double Rank { get; set; }
        public int Reviews { get; set; }
        public int Year { get; set; }

        // PriceCharts
        public double BoxPrice { get; set; }
        public double CompletePrice { get; set; }
        public double GradedPrice { get; set; }
        public double LoosePrice { get; set; }
        public double ManualPrice { get; set; }
        public double NewPrice { get; set; }
        public string ASIN { get; set; }
        public int EPID { get; set; }
        public string Variants { get; set; }
        public int DiscCount { get; set; }
        public string AlsoOn { get; set; }
        public int PCID { get; set; }
        public int NAID { get; set; }
        public string UPC { get; set; }
        public string Notes { get; set; }

        // VGChartz
        public double SoldNA { get; set; }
        public double SoldPAL { get; set; }
        public double SoldJAP { get; set; }
        public double SoldRest { get; set; }
        public double SoldTotal { get; set; }

        // HowLongToBeat
        public double LengthMain { get; set; }
        public double LengthMainExtras { get; set; }
        public double LengthComplete { get; set; }
        public double LengthAllPlayStyles { get; set; }

        // VGCollect
        public string BoxText { get; set; }
        public string AltName { get; set; }
        public string BuyPrice { get; set; }
        public string EstimatedValue { get; set; }
        public string Barcode { get; set; }
        public string ItemNumber { get; set; }
        public string Cheats { get; set; }
        public string Source { get; set; }

        // GameFaqs
        public string Trivia { get; set; }
        public string Expansions { get; set; }
        public string Franchise { get; set; }
        public double Rating { get; set; }
        public int RatingVotes { get; set; }
        public double Difficulty { get; set; }
        public int DifficultyVotes { get; set; }
        public double Length { get; set; }
        public int LengthVotes { get; set; }

        // User
        public bool OwnedPhysical { get; set; }
        public bool OwnedDigital { get; set; }
        public bool OwnedManual { get; set; }
        public bool Completed { get; set; }
        public bool FullyCompleted { get; set; }
        public bool Wanted { get; set; }
        public string Note { get; set; }

        // Modes
        public bool Story { get; set; }
        public int LocalPlayers { get; set; }
        public int OnlinePlayers { get; set; }
        public bool Coop { get; set; }
        public string Modes { get; set; }

        // Other
        public string DateString { get; set; }
        public bool Deleted { get; set; }
        public string Url { get; set; }

        // Url
        public string UrlCoverProject { get; set; }
        public string UrlGameRankings { get; set; }
        public string UrlGameFaqs { get; set; }
        public string UrlHowLongToBeat { get; set; }
        public string UrlMetaCritic { get; set; }
        public string UrlPriceCharts { get; set; }
        public string UrlVGChartz { get; set; }
        public string UrlVGCollect { get; set; }
        public string UrlWikipedia { get; set; }

        public Game() { }

        public Game(string name)
        {
            Name = name;
        }

        public Game(string name, string system)
        {
            Name = name;
            System = system;
        }

        public static List<Game> LoadAll()
        {
            var list = new List<Game>();
            list.AddRange(SQLConnector.LoadTable(new CoverProject()));
            list.AddRange(SQLConnector.LoadTable(new GameRankings()));
            list.AddRange(SQLConnector.LoadTable(new HowLongToBeat()));
            list.AddRange(SQLConnector.LoadTable(new PriceCharts()));
            list.AddRange(SQLConnector.LoadTable(new VGChartz()));
            list.AddRange(SQLConnector.LoadTable(new VGCollect()));
            list.AddRange(SQLConnector.LoadTable(new Wikipedia()));
            return list.GetRange(0, 1000);
        }
    }
}