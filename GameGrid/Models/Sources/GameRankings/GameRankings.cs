using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class GameRankings : DBObject
    {
        public string Name { get; set; }
        public string System { get; set; }
        public string Developer { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public decimal Rank { get; set; }
        public int Reviews { get; set; }
        public string Description { get; set; }
        public DateTime DateNA { get; set; }
        public DateTime DatePAL { get; set; }
        public DateTime DateJAP { get; set; }
        public int MRank { get; set; }
        public int MReviews { get; set; }
        public string UrlMetaCritic { get; set; }
        public string UrlGameFaqs { get; set; }
        public string Url { get; set; }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            GameRankings gr = new GameRankings()
            {
                Name = reader.GetString(0),
                System = reader.GetString(1),
                Developer = reader.GetString(2),
                Genre = reader.GetString(3),
                Year = reader.GetInt32(4),
                Rank = reader.GetDecimal(5),
                Reviews = reader.GetInt32(6),
                Description = reader.GetString(7),
                DateNA = reader.GetDateTime(8),
                DatePAL = reader.GetDateTime(9),
                DateJAP = reader.GetDateTime(10),
                MRank = reader.GetInt32(11),
                MReviews = reader.GetInt32(12),
                UrlMetaCritic = reader.GetString(13),
                UrlGameFaqs = reader.GetString(14),
                Url = reader.GetString(15)
            };
            return gr;
        }
    }  
}
