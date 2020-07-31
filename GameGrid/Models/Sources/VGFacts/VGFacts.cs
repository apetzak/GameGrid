using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class VGFacts : DBObject
    {
        public override DBObject GetFromReader(SqlDataReader reader)
        {
            return base.GetFromReader(reader);
        }
    }
}