using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class VGFactsTrivia : DBObject
    {
        public string TriviaID { get; set; }
        public string GameUrl { get; set; }
        public string SourceUrl { get; set; }
        public string Text { get; set; }
        public string Tags { get; set; }
        public string Contributor { get; set; }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            return base.GetFromReader(reader);
        }
    }
}