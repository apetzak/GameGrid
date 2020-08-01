using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace GameGrid.Models
{
    public class VGFactsScraper : WebScraper
    {
        public static string Url = "https://www.vgfacts.com/";

        public static void Scrape()
        {
            ScrapeSearchPages();

            foreach (VGFacts g in games)
                ScrapeTrivia(g);
        }

        public static void ScrapeTrivia(VGFacts g)
        {
            string gameUrl = $"{Url}game/{g.Url}/";
            Load(gameUrl);

            var nodes = TryGetNodes("//*[@class='triviabox']");
            if (nodes == null)
                return;

            g.Trivia = new List<VGFactsTrivia>();

            foreach (HtmlNode n in nodes)
            {
                VGFactsTrivia t = GetTrivia(n);
                t.GameUrl = g.Url;
                g.Trivia.Add(t);
            }
        }

        private static VGFactsTrivia GetTrivia(HtmlNode n)
        {
            string text = String.Empty;
            string tags = String.Empty;
            string contributor = String.Empty;
            string source = String.Empty;
            string triviaID = String.Empty;

            foreach (HtmlNode node in n.ChildNodes)
            {
                if (node.OuterHtml.StartsWith("<div class=\"trivia-content\""))
                {
                    text = node.InnerText;
                }
                else if (node.OuterHtml.StartsWith("<div class=\"trivia-tags\""))
                {
                    tags = node.InnerText;
                }
                else if (node.OuterHtml.StartsWith("<div class=\"trivia-meta\""))
                {
                    var list = node.InnerHtml.Split('<').ToList();
                    list.RemoveAll(s => !s.StartsWith("a href="));
                    for (int j = 0; j < list.Count; j++)
                        list[j] = list[j].Replace("a href=\"", "");

                    contributor = list[0].Split('>')[1];

                    foreach (string s in list)
                    {
                        if (s.EndsWith("Permalink"))
                            triviaID = Remove(s, "/\">Permalink", "/trivia/");

                        else if (s.EndsWith("Source"))
                            source = s.Replace("\" rel=\"external\">Source", "");
                    }
                }
            }

            VGFactsTrivia t = new VGFactsTrivia()
            {
                Text = text,
                Tags = tags,
                Contributor = contributor,
                SourceUrl = source,
                TriviaID = triviaID
            };

            return t;
        }

        public static void ScrapeSearchPages()
        {
            for (int i = 1; i < 140; i++) // Load each page
            {
                string pageUrl = $"{Url}game/page-{i}/";
                Load(pageUrl);

                var nodes = TryGetNodes("//*[@class='gameentry']");
                if (nodes == null)
                    continue;

                // Scrape all 20 rows on search page
                foreach (HtmlNode n in nodes)
                {
                    VGFacts g = GetSearchPageRow(n, i);
                    games.Add(g);
                }
            }
        }

        private static VGFacts GetSearchPageRow(HtmlNode n, int pageID)
        {
            var list = n.InnerHtml.Split('<').ToList();
            list.RemoveAll(s => !s.StartsWith("a href=\"/"));
            for (int j = 0; j < list.Count; j++)
                list[j] = list[j].Replace("a href=\"/", "");

            string gameUrl = String.Empty;
            string name = String.Empty;
            string developers = String.Empty;
            string series = String.Empty;
            string consoles = String.Empty;
            int triviaCount = 0;

            foreach (string s in list)
            {
                string text = s.Remove(0, s.LastIndexOf(">") + 1);

                if (s.StartsWith("game/") && String.IsNullOrEmpty(gameUrl))
                {
                    gameUrl = s.Remove(0, 5);
                    gameUrl = gameUrl.Remove(gameUrl.IndexOf('/'));
                }
                else if (s.Contains("title="))
                {
                    name = text;
                }
                else if (s.StartsWith("developer/"))
                {
                    developers += ", " + text;
                }
                else if (s.StartsWith("series/"))
                {
                    series = text;
                }
                else if (s.StartsWith("console/"))
                {
                    consoles += ", " + text;
                }
                else if (s.Contains("#trivia"))
                {
                    triviaCount = Convert.ToInt32(Remove(text, " ", "Trivia"));
                }
            }

            consoles = consoles.TrimStart(',').TrimStart();
            developers = developers.TrimStart(',').TrimStart();

            VGFacts g = new VGFacts()
            {
                PageID = pageID,
                Url = gameUrl,
                Name = name,
                Developer = developers,
                Series = series,
                Systems = consoles,
                TriviaCount = triviaCount
            };

            return g;
        }
    }
}