using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using GameGrid.Models;

namespace GameGrid
{
    /// <summary>
    /// 1 Game Per Page, Count ~53000
    /// ID Structure: game/system/name 
    ///               game/#
    /// </summary>
    public class PriceCharts : WebSource
    {
        public static string UrlHome = "http://www.pricecharting.com/";
        public static string Url = "https://www.pricecharting.com/game";
        private static string NAUrl = "http://www.nintendoage.com/index.cfm?FuseAction=Element.View&amp;egID=";
        private static List<int> failed = new List<int>();

        public static void Scrape()
        {
            for (int i = 1963; i < 55000; i++)
            {
                string url = Url + "/" + i;
                Load(url);
                if (url == web.ResponseUri.ToString() || UrlHome == web.ResponseUri.ToString())
                {
                    failed.Add(i);
                    continue;
                }
                Game g = new Game("");
                g.Url = web.ResponseUri.ToString().Replace(Url, "");
                g.PCID = i;
                GetNameAndSystem(g);
                Update(g, true);
                Games.Add(g);
            }
            Save("PriceCharts");
        }

        public static void Update(Game g, bool isNew = false)
        {
            if (!isNew)
                Load(Url + g.UrlPriceCharts);
            g.LoosePrice = GetPrice("used");
            g.CompletePrice = GetPrice("complete");
            g.NewPrice = GetPrice("new");
            g.GradedPrice = GetPrice("graded");
            g.BoxPrice = GetPrice("box_only");
            g.ManualPrice = GetPrice("manual_only");
            GetDetails(g);
            g.NAID = GetNAID();
        }

        private static void GetNameAndSystem(Game g)
        {
            HtmlNode node = Node("//*[@id='product_name']");
            string[] arr = node.InnerText.Split('\n');
            g.Name = arr[1].TrimStart(' ').TrimEnd(' ');
            g.System = arr[2].TrimStart(' ').TrimEnd(' ').Replace("amp;", "");
            if (g.System.Contains("PAL "))
                g.System = g.System.Replace("PAL ", "");
            else if (g.System.Contains("JP "))
                g.System = g.System.Replace("JP ", "");
        }

        private static double GetPrice(string s)
        {
            string text = Node(String.Format("//*[@id='{0}_price']", s)).InnerText;
            if (text.Contains("N/A"))
                return 0;
            return Convert.ToDouble(Remove(text, " ", "\n", "$"));
        }

        private static void GetDetails(Game g)
        {
            var nodes = Nodes("//*[@id='attribute']//tr");
            foreach (HtmlNode n in nodes)
            {
                string text = Remove(n.InnerText, "  ", "\n");

                if (text.EndsWith("none"))
                    continue;

                if (text.Contains("Genre:"))
                    g.Genre = text.Replace("Genre:", "").Replace("amp;", "");
                else if (text.Contains("ESRB Rating:"))
                    g.ESRB = text.Replace("ESRB Rating:", "");
                else if (text.Contains("Publisher:"))
                    g.Publisher = text.Replace("Publisher:", "");
                else if (text.Contains("Developer:"))
                    g.Developer = text.Replace("Developer:", "");
                else if (text.Contains("Disc Count:"))
                    g.DiscCount = Convert.ToInt32(Remove(text, "Disc Count:", "game disc", "s", " "));
                else if (text.Contains("Also Compatible On:"))
                    g.AlsoOn = text.Replace("Also Compatible On:", "");
                else if (text.Contains("Notes:"))
                    g.Notes = text.Replace("Notes:", "");
                else if (text.Contains("UPC:"))
                    g.UPC = text.Replace("UPC:", "");
                else if (text.Contains("ASIN (Amazon):"))
                    g.ASIN = text.Replace("ASIN (Amazon):", "");
                else if (text.Contains("ePID (eBay):"))
                    g.EPID = Convert.ToInt32(text.Replace("ePID (eBay):", ""));
                else if (text.Contains("Variants:"))
                    g.Variants = text.Replace("Variants:", "").TrimEnd(',');
                else if (text.Contains("Description:"))
                    g.Description = text.Replace("Description:", "");
                else if (text.Contains("Release Date:"))
                    g.Date = Convert.ToDateTime(text.Replace("Release Date:", ""));
            }
        }

        private static int GetNAID()
        {
            var nodes = new List<HtmlNode>();
            try { nodes = Nodes("//*[@id='full_details']//a"); }
            catch { return 0; }
            foreach (HtmlNode n in nodes)
            {
                if (n.InnerHtml == "More Details" && !n.OuterHtml.Contains("atarigamer"))
                    return Convert.ToInt32(n.OuterHtml.Remove(n.OuterHtml.IndexOf("target=") - 2).Remove(0, 9).Replace(NAUrl, ""));
            }
            return 0;
        }

        public override Game GetGameFromReader(System.Data.SqlClient.SqlDataReader reader)
        {
            Game g = new Game(reader.GetString(0), reader.GetString(1))
            {
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
                UrlPriceCharts = reader.GetString(23)
            };
            return g;
        }
    }
}
