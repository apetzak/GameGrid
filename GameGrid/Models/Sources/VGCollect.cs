using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using GameGrid.Models;

namespace GameGrid
{
    /// <summary>
    /// 
    /// </summary>
    public class VGCollect : WebSource
    {
        public static string Url = "https://vgcollect.com/item/";

        private static List<string> list = new List<string>();

        public static void Scrape()
        {
            int notFound = 0;
            for (int i = 49425; i < 160000; i++)
            {
                Load(Url + i);

                var node = Node("//h1");
                if (node != null && node.InnerHtml == "404 Page Not Found")
                {
                    notFound++;
                    continue;
                }

                Game g = new Game(Nodes("//h2")[0].InnerHtml);
                g.Url = i.ToString();
                Update(g);
                Games.Add(g);
            }
            Save("VGCollect");
        }

        public static void Update(Game g)
        {
            foreach (HtmlNode n in Nodes("//tbody//tr"))
            {
                string text = n.InnerText.Replace("\r\n", "").Replace("  ", "");

                g.EstimatedValue = GetText(g.EstimatedValue, text, "Estimated Value:");
                g.BuyPrice = GetText(g.BuyPrice, text, "Buy:");
                g.Developer = GetText(g.Developer, text, "Developer(s):");
                g.Publisher = GetText(g.Publisher, text, "Publisher(s):");
                g.Genre = GetText(g.Genre, text, "Genre:");
                g.ESRB = GetText(g.ESRB, text, "Rating:");
                g.System = GetText(g.System, text, "Platform:");
                g.Description = GetText(g.Description, text, "Description:");
                g.AltName = GetText(g.AltName, text, "Alt-Name:");
                g.Barcode = GetText(g.Barcode, text, "Barcode:");
                g.ItemNumber = GetText(g.ItemNumber, text, "Item Number:");
                g.BoxText = GetText(g.BoxText, text, "Box Text:");

                if (text.Contains("Release Date:"))
                    g.Date = GetDate(text, g);
            }

            SetRegion(g);
        }

        public static void SetRegion(Game g)
        {
            foreach (string s in new List<string> { " [US]", " [EU]", " [NA]", " [JP]" })
            {
                if (g.System.Contains(s))
                {
                    g.System = g.System.Replace(s, "");
                    g.Region = s.TrimStart().TrimStart('[').TrimEnd((']'));
                    break;
                }
            }
        }
         
        public static string GetText(string val, string text, string field)
        {
            if (text.Contains(field) && !text.EndsWith("NA"))
                return text.Replace(field, "").TrimStart();
            if (val != null)
                return val;
            return null;
        }

        public static int failedDates = 0;
        public static DateTime GetDate(string s, Game g)
        {
            string text = s.Replace("Release Date:", "");
            try
            {
                DateTime date = new DateTime();
                int year = Convert.ToInt32(text.Remove(0, text.Length - 4));
                text = text.Remove(text.Length - 4);
                int day;
                try
                {
                    day = Convert.ToInt32(text.Remove(0, text.Length - 2));
                    text = text.Remove(text.Length - 2);
                }
                catch
                {
                    day = Convert.ToInt32(text.Remove(0, text.Length - 1));
                    text = text.Remove(text.Length - 1);
                }
                int month = Convert.ToDateTime(String.Format("{0}-{1}-{2}", day, text, year)).Month;
                date = new DateTime(year, month, day);
                return date;
            }           
            catch
            {
                failedDates++;
                g.DateString = s.Replace("Release Date:", "");
                return DateTime.MinValue;
            }
        }

        public override Game GetGameFromReader(System.Data.SqlClient.SqlDataReader reader)
        {
            Game g = new Game(reader.GetString(0), reader.GetString(1));
            g.Genre = reader.GetString(2);
            g.Developer = reader.GetString(3);
            g.Publisher = reader.GetString(4);
            g.Date = reader.GetDateTime(5);
            g.Region = reader.GetString(6);
            g.ESRB = reader.GetString(7);
            g.BoxText = reader.GetString(8);
            g.Description = reader.GetString(9);
            g.AltName = reader.GetString(10);
            g.BuyPrice = reader.GetString(11);
            g.EstimatedValue = reader.GetString(12);
            g.Barcode = reader.GetString(13);
            g.ItemNumber = reader.GetString(14);
            g.UrlVGCollect = reader.GetString(15);
            return g;
        }
    }
}
