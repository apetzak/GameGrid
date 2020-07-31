using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using System.Data.SqlClient;

namespace GameGrid.Models
{
    public class GameRankingsScraper : WebScraper
    {
        public static string UrlHome = "http://www.gamerankings.com/";
        private static string GFUrl = "https://gamefaqs.gamespot.com";
        private static string MUrl = "https://www.metacritic.com/game";
        private static int failCount = 0;

        public static Dictionary<string, string> SystemIDs()

        {
            var dict = new Dictionary<string, string>();
            Load(UrlHome + "/browse.html");
            var node = GetNode("//*[@name='site']");
            string[] arr = Remove(node.InnerHtml.Remove(0, 66), "<option value=", "</option>", ">").Split('\"');
            for (int i = 0; i < arr.Length; i += 2)
                dict.Add(arr[i], arr[i + 1]);
            return dict;
        }

        public static void Scrape()
        {
            ScrapeSearchScreens();
            ScrapePages();
            Save("GameRankings");
        }

        public static bool NoResultsFound()
        {
            foreach (HtmlNode n in GetNodes("//div"))
            {
                if (n.InnerText.Contains("No results were found"))
                    return true;
            }
            return false;
        }

        public static void ScrapeSearchScreens()
        {
            foreach (KeyValuePair<string, string> pair in SystemIDs())
            {
                int page = -1;
                while (true)
                {
                    Load(UrlHome + "/browse.html?site=" + pair.Key + "&page=" + page++ + "&sort=2&numrev=3");

                    if (NoResultsFound())
                        break;

                    var nodes = GetNodes("//tr");
                    foreach (HtmlNode n in nodes)
                    {
                        string cleanHtml = Remove(n.InnerHtml.Remove(0, n.InnerHtml.IndexOf("href") + 6),
                            "</a", "<br>", "</td", "<td>", "<span style=\"font-size: 35px\"><b>", "</b></span><br clear=\"left\">", "\t", "\r", "\n", "\"");

                        string[] values = cleanHtml.Split('>');

                        if (cleanHtml.Contains(">>") || values.Length > 5)  // not so clean
                        {
                            failCount++;
                            continue;
                        }

                        GameRankings g = new GameRankings();
                        g.Url = values[0];
                        if (!g.Url.Contains(pair.Key))
                            continue;
                        g.Name = values[1];

                        int l = values[2].Length;
                        string year = l > 6 ? values[2].Remove(0, l - 4).ToString() : "";
                        if (l > 6 && values[2][l - 6] == ',' && (year.StartsWith("1") || year.StartsWith("2")))
                        {
                            g.Developer = values[2].Remove(l - 6);
                            g.Year = Convert.ToInt32(year);
                        }
                        else
                        {
                            g.Developer = values[2];
                        }

                        if (!values[3].Contains("No Reviews") && !values[3].Contains("n/a"))
                        {
                            g.Rank = Convert.ToDecimal(values[3].Split('%')[0]);
                            g.Reviews = Convert.ToInt32(Remove(values[3], " Review", "s").Split('%')[1]);
                        }

                        games.Add(g);
                    }
                }
            }
        }

        public static void ScrapePages()
        {
            foreach (GameRankings o in games)
                Update(o);
        }

        public static void Update(GameRankings g)
        {
            Load(UrlHome + g.Url);
            if (UrlHome + "/" == web.ResponseUri.ToString())
                return;

            var node = GetNodes("//div//div").Where(n => n.OuterHtml.Contains("id=\"content")).First();
            string html = node.InnerHtml;
            string text = node.InnerText;

            g.System = GetSystem(text);
            g.Genre = GetGenre(text);

            GetDetails(g, text);

            if (html.Contains(MUrl))
                GetMetaCriticInfo(g, html, text);

            if (html.Contains(GFUrl))
                g.UrlGameFaqs = GetURL(html, GFUrl);
        }

        private static string GetSystem(string text)
        {
            text = text.Remove(0, 3);
            return text.Remove(text.IndexOf("\r\n"));
        }

        private static string GetGenre(string text)
        {
            text = text.Remove(text.IndexOf("\r\n\r\n")).Replace("\r\n &raquo; ", ">");
            return text.Remove(0, text.IndexOf(">") + 1).Replace("&#039;", "'");
        }

        private static void GetDetails(GameRankings g, string text)
        {
            string details = text.Remove(0, text.IndexOf("Description")).Replace("Description\r\n\t\r\n\t\t\r\n", "");
            if (details.Contains("Release Date:") && !details.Contains("Release Date: Canceled"))
                GetDate(g, details);
            g.Description = details.Remove(details.IndexOf("\t")).Replace("&quot;", "\"");
        }

        private static void GetDate(GameRankings g, string details)
        {
            string date = details.Remove(0, details.IndexOf("Release Date"));
            date = date.Remove(date.IndexOf("\t"));
            date = Remove(date, "Release Date: ", "\t", "\r", "\n");

            if (date.Contains(" (Japan)"))
            {
                date = date.Replace(" (Japan)", "");
                if (date.Length > 4 && !date.Contains("Q") && !date.Contains("T"))
                    g.DateJAP = Convert.ToDateTime(date);
            }
            else if (date.Contains(" (Europe)"))
            {
                date = date.Replace(" (Europe)", "");
                if (date.Length > 4 && !date.Contains("Q") && !date.Contains("T") && !date.Contains("Unknown"))
                    g.DatePAL = Convert.ToDateTime(date.Replace(" (Europe)", ""));
            }
            else if (date.Length > 4 && !date.Contains("(") && !date.Contains("Q") && !date.Contains("T") && !date.Contains("Unknown"))
            {
                g.DateNA = Convert.ToDateTime(date);
            }
        }

        private static void GetMetaCriticInfo(GameRankings g, string html, string text)
        {
            g.UrlMetaCritic = GetURL(html, MUrl);
            string s = text.Remove(0, text.IndexOf("MetaScore"));
            s = s.Remove(s.IndexOf(")"));
            s = Remove(s, "MetaScore: ", "Review", "s", "(");
            g.MRank = Convert.ToInt32(s.Split(' ')[0]);
            g.MReviews = Convert.ToInt32(s.Split(' ')[1]);
        }

        private static string GetURL(string html, string baseUrl)
        {
            html = html.Remove(0, html.IndexOf(baseUrl));
            html = html.Remove(html.IndexOf("class=") - 2).Replace(baseUrl, "");
            return html;
        }
    }
}