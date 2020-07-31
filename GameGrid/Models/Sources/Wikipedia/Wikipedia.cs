using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class Wikipedia : DBObject
    {
        public string URL { get; set; }
        public string Title { get; set; }
        public string Platforms { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public string Artists { get; set; }
        public string Composers { get; set; }
        public string Directors { get; set; }
        public string Producers { get; set; }
        public string Programmers { get; set; }
        public string Writers { get; set; }
        public string Designers { get; set; }
        public string Modes { get; set; }
        public string Series { get; set; }
        public string Notes { get; set; }
        public string DateString { get; set; }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            Wikipedia w = new Wikipedia()
            {
                URL = reader.GetString(0),
                Title = reader.GetString(1),
                Platforms = reader.GetString(2),
                Developer = reader.GetString(3),
                Publisher = reader.GetString(4),
                Genre = reader.GetString(5),
                Artists = reader.GetString(6),
                Composers = reader.GetString(7),
                Directors = reader.GetString(8),
                Producers = reader.GetString(9),
                Programmers = reader.GetString(10),
                Writers = reader.GetString(11),
                Designers = reader.GetString(12),
                Modes = reader.GetString(13),
                Series = reader.GetString(14),
                Notes = reader.GetString(15),
                DateString = reader.GetString(16)
            };
            return w;
        }
    } 
}
