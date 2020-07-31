using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class PriceCharting : DBObject
    {
        public string Name { get; set; }
        public string System { get; set; }
        public int PCID { get; set; }
        public string Url { get; set; }
        public string Region { get; set; }
        public string Variant { get; set; }
        public string Variants { get; set; }
        public double BoxPrice { get; set; }
        public double CompletePrice { get; set; }
        public double GradedPrice { get; set; }
        public double LoosePrice { get; set; }
        public double ManualPrice { get; set; }
        public double NewPrice { get; set; }
        public string Genre { get; set; }
        public DateTime Date { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string ESRB { get; set; }
        public string PEGI { get; set; }
        public string ASIN { get; set; }
        public int EPID { get; set; }
        public int DiscCount { get; set; }
        public int PlayerCount { get; set; }
        public string AlsoOn { get; set; }
        public int NAID { get; set; }
        public string UPC { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string EAN_GTIN { get; set; }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            PriceCharting pc = new PriceCharting()
            {
                Name = reader.GetString(0),
                System = reader.GetString(1),
                LoosePrice = Convert.ToDouble(reader.GetDecimal(2)),
                CompletePrice = Convert.ToDouble(reader.GetDecimal(3)),
                NewPrice = Convert.ToDouble(reader.GetDecimal(4)),
                GradedPrice = Convert.ToDouble(reader.GetDecimal(5)),
                ManualPrice = Convert.ToDouble(reader.GetDecimal(6)),
                BoxPrice = Convert.ToDouble(reader.GetDecimal(7)),
                Genre = reader.GetString(8),
                Date = reader.GetDateTime(9),
                ESRB = reader.GetString(10),
                Publisher = reader.GetString(11),
                Developer = reader.GetString(12),
                AlsoOn = reader.GetString(13),
                Notes = reader.GetString(14),
                UPC = reader.GetString(15),
                ASIN = reader.GetString(16),
                EPID = reader.GetInt32(17),
                Variants = reader.GetString(18),
                Description = reader.GetString(19),
                DiscCount = reader.GetInt32(20),
                PCID = reader.GetInt32(21),
                NAID = reader.GetInt32(22),
                Url = reader.GetString(23)
            };
            return pc;
        }
    }
}
