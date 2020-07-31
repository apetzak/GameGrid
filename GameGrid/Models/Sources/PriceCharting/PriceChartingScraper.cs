using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    /// <summary>
    /// 1 Game Per Page, Count ~53000
    /// ID Structure: game/system/name 
    ///               game/#
    /// </summary>
    public class PriceChartingScraper : WebScraper
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
                PriceCharting g = new PriceCharting();
                g.Url = web.ResponseUri.ToString().Replace(Url, "");
                g.PCID = i;
                GetNameAndSystem(g);
                Update(g, true);
                games.Add(g);
            }
            Save("PriceCharts");
        }

        public static void Update(PriceCharting g, bool isNew = false)
        {
            if (!isNew)
                Load(Url + g.Url);
            g.LoosePrice = GetPrice("used");
            g.CompletePrice = GetPrice("complete");
            g.NewPrice = GetPrice("new");
            g.GradedPrice = GetPrice("graded");
            g.BoxPrice = GetPrice("box_only");
            g.ManualPrice = GetPrice("manual_only");
            GetDetails(g);
            g.NAID = GetNAID();
        }

        private static void GetNameAndSystem(PriceCharting g)
        {
            HtmlNode node = GetNode("//*[@id='product_name']");
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
            string text = GetNode(String.Format("//*[@id='{0}_price']", s)).InnerText;
            if (text.Contains("N/A"))
                return 0;
            return Convert.ToDouble(Remove(text, " ", "\n", "$"));
        }

        private static void GetDetails(PriceCharting g)
        {
            var nodes = GetNodes("//*[@id='attribute']//tr");
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
            try { nodes = GetNodes("//*[@id='full_details']//a"); }
            catch { return 0; }
            foreach (HtmlNode n in nodes)
            {
                if (n.InnerHtml == "More Details" && !n.OuterHtml.Contains("atarigamer"))
                    return Convert.ToInt32(n.OuterHtml.Remove(n.OuterHtml.IndexOf("target=") - 2).Remove(0, 9).Replace(NAUrl, ""));
            }
            return 0;
        }
    }
}