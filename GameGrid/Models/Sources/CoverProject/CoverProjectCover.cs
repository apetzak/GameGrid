using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class CoverProjectCover : DBObject
    {
        public int CoverID { get; set; }
        public int GameID { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public string CreatedBy { get; set; }
        public string Region { get; set; }
        public string CaseType { get; set; }
        public int Downloads { get; set; }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            CoverProjectCover cp = new CoverProjectCover()
            {
                CoverID = reader.GetInt32(0),
                GameID = reader.GetInt32(1),
                ImageURL = reader.GetString(2),
                Description = reader.GetString(3),
                Format = reader.GetString(4),
                CreatedBy = reader.GetString(5),
                Region = reader.GetString(6),
                CaseType = reader.GetString(7),
                Downloads = reader.GetInt32(8)
            };
            return cp;
        }
    }
}