using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace GameGrid.Models
{
    public class CoverProjectScraper : WebScraper
    {
        public static string UrlHome = "http://www.thecoverproject.net/";
        public static string Url = "http://www.thecoverproject.net/view.php?game_id=";
        public static string UrlCover = "http://www.thecoverproject.net/view.php?cover_id=";
        public static string UrlSystem = "http://www.thecoverproject.net/view.php?cat_id=";
        public static List<DBObject> covers = new List<DBObject>();

        public static Dictionary<string, string> SystemIDs()
        {
            var dict = new Dictionary<string, string>();
            Load(UrlHome);
            var nodes = GetNodes("//li//a");
            foreach (HtmlNode n in nodes.Where(n => n.OuterHtml.Contains("/view.php?cat_id=")))
                dict.Add(n.InnerText, Remove(n.OuterHtml, "<a href=\"/view.php?cat_id=", n.InnerText, "\"></a>"));
            return dict;
        }

        public static void Scrape()
        {
            ScrapeGames();
            ScrapeCovers();
            SQLConnector.Insert(games);
            SQLConnector.Insert(covers);

            Save("CoverProject");
        }

        public static void ScrapeGames()
        {
            for (int i = 0; i < 11000; i++)
            {
                Load(Url + i);

                if (web.ResponseUri.ToString() == "http://www.thecoverproject.net/index.php")
                    continue;

                CoverProject cp = new CoverProject();
                cp.GameID = i;
                cp.Name = GetNode("//*[@class='pageBody']/h2").InnerHtml;
                cp.System = GetNode("//*[@class='newsHeader']/a").InnerHtml;

                List<HtmlNode> nodes = TryGetNodes("//*[@class='pageBody']/div/ul/li");
                if (nodes == null)
                    continue;

                foreach (HtmlNode node in nodes)
                {
                    if (node.InnerText == "Available Covers" ||
                        node.InnerText == "Available Manuals")
                        continue;

                    if (node.InnerHtml.Contains("cover_id="))
                    {
                        string coverID = node.InnerHtml.Remove(0, node.InnerHtml.IndexOf("id=") + 3);
                        coverID = coverID.Remove(coverID.IndexOf("\""));

                        CoverProjectCover cpc = new CoverProjectCover();
                        cpc.CoverID = Convert.ToInt32(coverID);
                        cpc.GameID = i;
                        covers.Add(cpc);
                    }
                    else if (node.InnerHtml.Contains("/images/manuals"))
                    {
                        string imageURL = node.InnerHtml.Remove(0, node.InnerHtml.IndexOf("href=") + 6);
                        imageURL = imageURL.Remove(imageURL.IndexOf("\""));
                        cp.Manuals += imageURL + " ";
                    }
                    else
                    {

                    }
                }

                games.Add(cp);
            }
        }

        public static void UpdateCover(CoverProjectCover cpc)
        {
            Load(UrlCover + cpc.CoverID);

            HtmlNode node = TryGetNode("//td/h3");
            if (node == null)
                return;

            foreach (HtmlNode n in node.ParentNode.ChildNodes)
            {
                if (n.OuterHtml.Contains("img") && cpc.ImageURL == null)
                {
                    string url = node.InnerHtml.Replace("<img src=\"", "");
                    url = url.Remove(url.IndexOf("\""));
                    cpc.ImageURL = url.Replace("https://coverproject.sfo2.cdn.digitaloceanspaces.com", "");
                }
                else if (n.OuterHtml.Contains("action=profile"))
                {
                    cpc.CreatedBy = n.InnerText;
                }
                else if (n.InnerText.Contains(":"))
                {
                    if (n.InnerText == "Cover Details:")
                        continue;

                    string field = n.InnerText.Split(':')[0];
                    string val = n.InnerText.Split(':')[1].TrimStart().TrimEnd();

                    if (field == "Description")
                        cpc.Description = val;
                    else if (field == "Format")
                        cpc.Format = val;
                    else if (field == "Created by")
                        cpc.CreatedBy = val;
                    else if (field == "Region")
                        cpc.Region = val;
                    else if (field == "Case Type")
                        cpc.CaseType = val;
                }
                else if (n.InnerText.Contains("has been d"))
                {
                    string s = n.InnerText.Replace("This cover has been downloaded ", "").Replace(" times", "");
                    cpc.Downloads = Convert.ToInt32(s);
                }
            }
        }

        public static void ScrapeCovers()
        {
            foreach (CoverProjectCover cpc in covers)
                UpdateCover(cpc);
        }

        // NOT USED
        public static void ScrapeSearchPages()
        {
            var systems = SystemIDs();
            char[] alphabet = "9abcdefghijklmnopqrstuvwxyz".ToCharArray();
            foreach (KeyValuePair<string, string> pair in systems)
            {
                foreach (char letter in alphabet)
                {
                    int page = 1;
                    while (true)
                    {
                        string url = String.Format("{0}{1}&page={2}&view={3}", UrlSystem, pair.Value, page++, letter);
                        Load(url);

                        List<HtmlNode> nodes = TryGetNodes("//*[@class='articleText']");
                        if (nodes == null)
                            break; // some pages don't have articleText

                        if (nodes.Count == 1 && nodes[0].InnerHtml == "There are no covers that match this criteria.")
                            break;

                        foreach (HtmlNode n in nodes)
                        {
                            CoverProject g = new CoverProject();
                            g.Name = n.InnerText.TrimEnd();
                            g.System = pair.Key;
                            if (g.Name.Contains("(") && g.Name.EndsWith(")"))
                                g.Name = g.Name.Remove(g.Name.LastIndexOf("(") - 1);
                            string id = n.InnerHtml.Remove(0, 26);
                            id = id.Remove(id.IndexOf("\""));
                            g.GameID = Convert.ToInt32(id);
                            games.Add(g);
                        }
                    }
                }
            }
        }
    }
}