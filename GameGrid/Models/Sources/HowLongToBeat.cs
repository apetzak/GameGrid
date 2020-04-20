using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using GameGrid.Models;

namespace GameGrid
{
    /// <summary>
    /// 
    /// </summary>
    public class HowLongToBeat : WebSource
    {
        public static string Url = "https://howlongtobeat.com/game.php?id=";
        public static List<int> failed = new List<int>();
        public static List<int> noSystem = new List<int>();

        public static void Scrape()
        {
            for (int i = 0; i < 58500; i++)
            {
                string url = Url + i;
                Load(url);
                if (url != web.ResponseUri.ToString())
                {
                    failed.Add(i); 
                    continue;
                }
                Game g = new Game(GetName());
                g.Url = i.ToString();
                Update(g);
                Games.Add(g);
            }
            Save("HowLongToBeat");
        }

        public static void Update(Game g)
        {
            GetTimes(g);
            GetAttributes(g);
            g.Description = GetDescription();
        }

        private static string GetName()
        {
            return Node("//*[@class='profile_header shadow_text']").InnerHtml.Replace("\n", "").TrimEnd();
        }

        public static void GetTimes(Game g)
        {
            var node = Node("//*[@class='game_times']");

            if (node == null)
                node = Node("//*[@class='game_times time_100']");

            if (node == null)
                return;

            var text = node.InnerText.Replace("&#189;", ".5").TrimStart().TrimStart('\n');
            var a = Remove(text, "\n\n", " Hours", " ").Split('\n');

            if (a[0] != "MainStory" || a[2] != "Main+Extras" || a[4] != "Completionist" || a[6] != "AllStyles")
                return;

            g.LengthMain = GetTime(a[1]);
            g.LengthMainExtras = GetTime(a[3]);
            g.LengthComplete = GetTime(a[5]);
            g.LengthAllPlayStyles = GetTime(a[7]);
        }

        public static double GetTime(string s)
        {
            if (s.Contains("--") || s == "")
                return 0;
            if (s.Contains("Mins"))
                return Math.Round(Convert.ToDouble(s.Replace("Mins", "")) / 60, 2);
            return Convert.ToDouble(s);
        } 

        public static string GetDescription()
        {
            var node = Node("//*[@style='margin-bottom: 10px;']");
            if (node == null)
                return null;
            return node.InnerText.TrimStart().TrimEnd('\n');
        }

        public static void GetAttributes(Game g)
        {
            var nodes = Nodes("//*[@class='profile_info']"); 
            foreach (HtmlNode n in nodes)
            {
                var a = n.InnerText.Replace("\n", "").TrimEnd().Split(':');
                string field = a[0];
                string value = a[1];

                if (field == "Developer")
                    g.Developer = value;
                else if (field == "Publisher")
                    g.Publisher = value;
                else if (field == "Playable On")
                    g.System = value;
                else if (field.StartsWith("Genre"))
                    g.Genre = value;
                else if (field == "NA")
                    g.DateNA = GetDate(value);
                else if (field == "EU")
                    g.DatePAL = GetDate(value);
                else if (field == "JP")
                    g.DateJAP = GetDate(value);
            }
        }

        public static DateTime GetDate(string s)
        {
            if (s.Length == 5 || s.Length == 6)           
                return new DateTime(Convert.ToInt32(s.TrimStart()), 1, 1);
            if (s.StartsWith(" "))
                return DateTime.MinValue;
            return Convert.ToDateTime(s);
        }

        public override Game GetGameFromReader(System.Data.SqlClient.SqlDataReader reader)
        {
            Game g = new Game(reader.GetString(0), reader.GetString(1))
            {
                LengthMain = Convert.ToDouble(reader.GetDecimal(2)),
                LengthMainExtras = Convert.ToDouble(reader.GetDecimal(3)),
                LengthComplete = Convert.ToDouble(reader.GetDecimal(4)),
                LengthAllPlayStyles = Convert.ToDouble(reader.GetDecimal(5)),
                Developer = reader.GetString(6),
                Publisher = reader.GetString(7),
                Description = reader.GetString(8),
                Genre = reader.GetString(9),
                DateNA = reader.GetDateTime(10),
                DatePAL = reader.GetDateTime(11),
                DateJAP = reader.GetDateTime(12),
                UrlHowLongToBeat = reader.GetString(13)
            };
            return g;
        }
    }
}
