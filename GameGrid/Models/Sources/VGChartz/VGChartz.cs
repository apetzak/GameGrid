using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class VGChartz : DBObject
    {
        public int ID { get; set; }
        public string URL { get; set; }
        public string Name { get; set; }
        public string System { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public string AlternativeNames { get; set; }
        public decimal SoldNA { get; set; }
        public decimal SoldPAL { get; set; }
        public decimal SoldJAP { get; set; }
        public decimal SoldRest { get; set; }
        public decimal SoldTotal { get; set; }
        public int ShippingTotal { get; set; }
        public string ShippingTotalAsOf { get; set; }
        public string Ratings { get; set; }
        public DateTime DateNA { get; set; }
        public DateTime DatePAL { get; set; }
        public DateTime DateJAP { get; set; }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            VGChartz vc = new VGChartz()
            {
                Name = reader.GetString(0),
                System = reader.GetString(1),
                URL = reader.GetString(9)
            };
            return vc;
        }
    }
}
