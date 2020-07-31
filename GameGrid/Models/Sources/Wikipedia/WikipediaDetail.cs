using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class WikipediaDetail : DBObject
    {
        public string LinkURL { get; set; }
        public string WikipediaGameURL { get; set; }
        public string PageURL { get; set; }
        public string Title { get; set; }
        public string Platform { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public string Other { get; set; }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            WikipediaDetail wd = new WikipediaDetail()
            {
                LinkURL = reader.GetString(0),
                WikipediaGameURL = reader.GetString(1),
                PageURL = reader.GetString(2),
                Title = reader.GetString(3),
                Platform = reader.GetString(4),
                Developer = reader.GetString(5),
                Publisher = reader.GetString(6),
                Genre = reader.GetString(7),
                Other = reader.GetString(8)
            };
            return wd;
        }
    }
}