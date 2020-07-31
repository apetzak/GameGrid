using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class DBObject
    {
        public DateTime Updated;

        public string TableName
        {
            get
            {
                return GetType().Name;
            }
        }

        public virtual void Save()
        {

        }

        public virtual DBObject GetFromReader(SqlDataReader reader)
        {
            return new DBObject();
        }
    }
}