using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class VGFacts : DBObject
    {
        public string Url { get; set; }
        public int PageID { get; set; }
        public string Name { get; set; }
        public string Developer { get; set; }
        public string Series { get; set; }
        public string Systems { get; set; }
        public int TriviaCount { get; set; }
        public List<VGFactsTrivia> Trivia;

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            return base.GetFromReader(reader);
        }
    }
}