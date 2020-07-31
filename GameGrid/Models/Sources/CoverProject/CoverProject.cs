using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class CoverProject : DBObject
    {
        public int GameID { get; set; }
        public string Name { get; set; }
        public string System { get; set; }
        public string Manuals { get; set; }
        public List<CoverProjectCover> Covers;

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            CoverProject cp = new CoverProject()
            {
                GameID = reader.GetInt32(0),
                Name = reader.GetString(1),
                System = reader.GetString(2),
                Manuals = reader.GetString(3)
            };
            return cp;
        }
    }
}
