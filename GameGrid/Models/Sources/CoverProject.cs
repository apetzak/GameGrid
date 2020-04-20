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
    public class CoverProject : WebSource
    {
        public static string UrlHome = "http://www.thecoverproject.net/";
        public static string Url = "http://www.thecoverproject.net/view.php?cover_id=";
        public static string SystemUrl = "http://www.thecoverproject.net/view.php?cat_id=";

        public static Dictionary<string, string> SystemIDs()
        {
            var dict = new Dictionary<string, string>();
            Load(UrlHome);
            var nodes = Nodes("//li//a");
            foreach (HtmlNode n in nodes.Where(n => n.OuterHtml.Contains("/view.php?cat_id=")))
                dict.Add(n.InnerText, Remove(n.OuterHtml, "<a href=\"/view.php?cat_id=", n.InnerText, "\"></a>"));
            return dict;
        }

        public static void Scrape()
        {
            ScrapeSearchPages();
            ScrapeGamePages();
            Save("CoverProject");
        }

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
                        string url = String.Format("{0}{1}&page={2}&view={3}", SystemUrl, pair.Value, page++, letter);
                        Load(url);
                        List<HtmlNode> nodes;
                        try
                        {
                            nodes = Nodes("//*[@class='articleText']");
                        }
                        catch
                        {
                            // some pages don't have articleText
                            break;
                        }

                        if (nodes.Count == 1 && nodes[0].InnerHtml == "There are no covers that match this criteria.")
                            break;

                        foreach (HtmlNode n in nodes)
                        {
                            Game g = new Game(n.InnerText.TrimEnd(), pair.Key);
                            if (g.Name.Contains("(") && g.Name.EndsWith(")"))
                                g.Name = g.Name.Remove(g.Name.LastIndexOf("(") - 1);
                            string id = n.InnerHtml.Remove(0, 26);
                            id = id.Remove(id.IndexOf("\""));
                            g.Url = id;
                            Games.Add(g);
                        }
                    }
                }
            }
        }

        public static void ScrapeGamePages()
        {
            // todo
        }

        public override Game GetGameFromReader(System.Data.SqlClient.SqlDataReader reader)
        {
            Game g = new Game(reader.GetString(0), reader.GetString(1));
            g.UrlCoverProject = reader.GetString(2);
            return g;
        }
    }
}
