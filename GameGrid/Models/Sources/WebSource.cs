using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using GameGrid.Models;

namespace GameGrid
{
    /// <summary>
    /// Includes shared variables and helper methods for WebSources
    /// </summary>
    public class WebSource
    {
        public static List<Game> Games = new List<Game>();
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

        public static List<HtmlNode> Nodes(string xpath)
        {
            return document.DocumentNode.SelectNodes(xpath).ToList();
        }

        public static HtmlNode Node(string xpath)
        {
            return document.DocumentNode.SelectSingleNode(xpath);
        }

        public static void Save(string table)
        {
            SQLConnector.Insert(Games, table);
        }

        public virtual Game GetGameFromReader(System.Data.SqlClient.SqlDataReader reader)
        {
            return new Game();
        }
    }
}
