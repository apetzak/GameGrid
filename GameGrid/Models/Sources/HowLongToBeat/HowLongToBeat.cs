using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class HowLongToBeat : DBObject
    {
        public string Name { get; set; }
        public string System { get; set; }
        public decimal LengthMain { get; set; }
        public decimal LengthMainExtras { get; set; }
        public decimal LengthComplete { get; set; }
        public decimal LengthAllPlayStyles { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public DateTime DateNA { get; set; }
        public DateTime DatePAL { get; set; }
        public DateTime DateJAP { get; set; }
        public string Url { get; set; }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            HowLongToBeat hltb = new HowLongToBeat()
            {
                Name = reader.GetString(0),
                System = reader.GetString(1),
                LengthMain = reader.GetDecimal(2),
                LengthMainExtras = reader.GetDecimal(3),
                LengthComplete = reader.GetDecimal(4),
                LengthAllPlayStyles = reader.GetDecimal(5),
                Developer = reader.GetString(6),
                Publisher = reader.GetString(7),
                Description = reader.GetString(8),
                Genre = reader.GetString(9),
                DateNA = reader.GetDateTime(10),
                DatePAL = reader.GetDateTime(11),
                DateJAP = reader.GetDateTime(12),
                Url = reader.GetString(13)
            };
            return hltb;
        }
    }
}
