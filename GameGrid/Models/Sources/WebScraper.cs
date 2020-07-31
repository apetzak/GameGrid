using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;

namespace GameGrid.Models
{
    /*
    ID Structure: 
    Page Count: 
    Game Datatype(s): Game, Detail, or Release
    Last Updated: 4/20/2020
    Notes: 
    */

    /// <summary>
    /// Includes shared variables and helper methods for WebSources
    /// </summary>
    public class WebScraper
    {
        public static List<DBObject> games = new List<DBObject>();
        public static HtmlWeb web = new HtmlWeb();
        public static HtmlDocument document = new HtmlDocument();

        public static string Remove(string text, params string[] strings)
        {
            foreach (string s in strings)
                text = text.Replace(s, "");
            return text;
        }

        public static void Load(string url)
        {
            document = web.Load(url);
        }

        public static List<HtmlNode> GetNodes(string xpath)
        {
            return document.DocumentNode.SelectNodes(xpath).ToList();
        }

        public static List<HtmlNode> TryGetNodes(string xpath)
        {
            try
            {
                return GetNodes(xpath);
            }
            catch
            {
                return null;
            }
        }

        public static HtmlNode GetNode(string xpath)
        {
            return document.DocumentNode.SelectSingleNode(xpath);
        }

        public static HtmlNode TryGetNode(string xpath)
        {
            try
            {
                return GetNode(xpath);
            }
            catch
            {
                return null;
            }
        }

        public static void Save(string table = "")
        {
            SQLConnector.Insert(games);
        }
    }
}
