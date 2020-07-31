using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace GameGrid.Models
{
    public class VGCollectScraper : WebScraper
    {
        public static string Url = "https://vgcollect.com/item/";

        private static List<string> list = new List<string>();

        public static void Scrape()
        {
            int notFound = 0;
            for (int i = 49425; i < 160000; i++)
            {
                Load(Url + i);

                var node = GetNode("//h1");
                if (node != null && node.InnerHtml == "404 Page Not Found")
                {
                    notFound++;
                    continue;
                }

                VGCollect g = new VGCollect();
                g.Name = GetNodes("//h2")[0].InnerHtml;
                g.ID = i;
                Update(g);
                games.Add(g);
            }
            Save("VGCollect");
        }

        public static void Update(VGCollect g)
        {
            foreach (HtmlNode n in GetNodes("//tbody//tr"))
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

        public static void SetRegion(VGCollect g)
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
        public static DateTime GetDate(string s, VGCollect g)
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
    }
}