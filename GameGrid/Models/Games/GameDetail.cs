using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class GameDetail : DBObject
    {
        public int GameDetailID { get; set; }
        public int GameID { get; set; }
        public string System { get; set; }
        public int LocalPlayers { get; set; }
        public int OnlinePlayers { get; set; }

        public int CoverProjectID { get; set; }
        public string GameRankingsURL { get; set; }
        public string VGChartzURL { get; set; }
        public string WikipediaDetailURL { get; set; }

        public CoverProject CoverProject { get; set; }
        public GameRankings GameRankings { get; set; }
        public VGChartz VGChartz { get; set; }
        public WikipediaDetail WikipediaDetail { get; set; }

        public List<GameRelease> Releases { get; set; }

        public GameDetail()
        {
            Releases = new List<GameRelease>();
        }

        public GameDetail(int gameID, string system)
        {
            GameID = gameID;
            System = system;
            Releases = new List<GameRelease>();
        }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            GameDetail g = new GameDetail()
            {
                GameDetailID = reader.GetInt32(0),
                GameID = reader.GetInt32(1),
                System = reader.GetString(2),
                CoverProjectID = reader.GetInt32(5),
                GameRankingsURL = reader.GetString(6),
                VGChartzURL = reader.GetString(7),
                WikipediaDetailURL = reader.GetString(8)
            };
            return g;
        }
    }
}