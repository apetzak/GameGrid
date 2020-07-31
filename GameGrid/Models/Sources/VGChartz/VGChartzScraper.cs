using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace GameGrid.Models
{
    public class VGChartzScraper : WebScraper
    {
        public static string Url = "http://www.vgchartz.com/platform/";

        public static void Scrape()
        {
            for (int i = 1; i < 87; i++)
            {
                string url = Url + i;
                Load(url);

                if (url != web.ResponseUri.ToString())
                    continue;

                string system = GetNode("//*[@class='chart_title']//h1").InnerText;

                var nodes = GetNodes("//tr");

                foreach (HtmlNode node in nodes)
                {
                    var split = node.InnerText.Split('\t');
                    var list = new List<string>();
                    foreach (string s in split)
                    {
                        if (s != "" && s != "\n")
                            list.Add(s.Replace("\n", ""));
                    }
                    list.RemoveRange(0, 1);

                    if (list[0] == "Game" || list.Count < 8 || list[1] == " ")
                        continue;

                    VGChartz g = new VGChartz();
                    g.Name = list[0];
                    g.System = system;
                    g.URL = GetUrl(node.InnerHtml);
                    g.Genre = list[2];

                    int j = 3;
                    if (list.Count == 9)
                        g.Publisher = list[j++];
                    g.SoldNA = Convert.ToDecimal(list[j++]);
                    g.SoldPAL = Convert.ToDecimal(list[j++]);
                    g.SoldJAP = Convert.ToDecimal(list[j++]);
                    g.SoldRest = Convert.ToDecimal(list[j++]);
                    g.SoldTotal = Convert.ToDecimal(list[j++]);
                    games.Add(g);
                }
            }
            Save("VGChartz");
        }

        public static string GetUrl(string html)
        {
            html = html.Remove(0, html.IndexOf("http"));
            html = html.Replace("http://www.vgchartz.com/game", "");
            return html.Remove(html.IndexOf("\""));
        }

        //public override Game GetGameFromReader(System.Data.SqlClient.SqlDataReader reader)
        //{
        //    Game g = new Game(reader.GetString(0), reader.GetString(1));
        //    g.Genre = reader.GetString(2);
        //    g.Publisher = reader.GetString(3);
        //    g.SoldNA = Convert.ToDouble(reader.GetDecimal(4));
        //    g.SoldPAL = Convert.ToDouble(reader.GetDecimal(5));
        //    g.SoldJAP = Convert.ToDouble(reader.GetDecimal(6));
        //    g.SoldTotal = Convert.ToDouble(reader.GetDecimal(7));
        //    g.SoldRest = Convert.ToDouble(reader.GetDecimal(8));
        //    g.UrlVGChartz = reader.GetString(9);
        //    return g;
        //}
    }
}