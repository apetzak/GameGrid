using System;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class VGCollect : DBObject
    {
        public string Name { get; set; }
        public string System { get; set; }
        public int ID { get; set; }
        public string Genre { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Region { get; set; }
        public string ESRB { get; set; }
        public string BoxText { get; set; }
        public string Description { get; set; }
        public string AltName { get; set; }
        public string BuyPrice { get; set; }
        public string EstimatedValue { get; set; }
        public string Barcode { get; set; }
        public string ItemNumber { get; set; }
        public string UrlVGCollect { get; set; }

        public override DBObject GetFromReader(SqlDataReader reader)
        {
            VGCollect vc = new VGCollect()
            {
                Name = reader.GetString(0),
                System = reader.GetString(1),
                ID = Convert.ToInt32(reader.GetString(15))
            };
            return vc;
        }
    }   
}
