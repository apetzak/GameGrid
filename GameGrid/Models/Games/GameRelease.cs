using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class GameRelease : DBObject
    {
        public int GameReleaseID { get; set; }
        public int GameDetailID { get; set; }
        public string Region { get; set; }
        public string Variant { get; set; }
        public string Publisher { get; set; }
        public DateTime Date { get; set; }
        public bool Physical { get; set; }
        public bool Digital { get; set; }

        public string PriceChartingURL { get; set; }
        public int VGCollectID { get; set; }
        public int CoverProjectCoverID { get; set; }

        public PriceCharting PriceCharting { get; set; }
        public VGCollect VGCollect { get; set; }
        public CoverProjectCover CoverProjectCover { get; set; }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            GameRelease g = new GameRelease()
            {
                GameReleaseID = reader.GetInt32(0),
                GameDetailID = reader.GetInt32(1),
                Region = reader.GetString(2),
                Variant = reader.GetString(3),
                Publisher = reader.GetString(4),
                Date = reader.GetDateTime(5),
                Physical = reader.GetInt32(6) == 0,
                Digital = reader.GetInt32(7) == 0,
                PriceChartingURL = reader.GetString(8),
                VGCollectID = reader.GetInt32(9),
                CoverProjectCoverID = reader.GetInt32(10)
            };
            return g;
        }
    }
}