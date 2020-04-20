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
    public class Wikipedia : WebSource
    {
        public static string Url = "https://en.wikipedia.org/wiki";

        public static void Scrape()
        {
            ScrapeLists();
            foreach (Game g in Games)
                Update(g);
            Save("Wikipedia");
        }

        public static void Update(Game g)
        {
            if (g.Url == null)
                return;
            string u = Url + "/" + g.Url;
            Load(Url + "/" + g.Url);

            List<HtmlNode> table;

            try
            {
                table = Nodes("//*[@class='infobox hproduct']//tbody//tr");
            }
            catch
            {
                return;
            }

            foreach (HtmlNode node in table)
            {
                string field = node.FirstChild.InnerText;
                string value = node.LastChild.InnerText;

                if (field == "Developer(s)")
                    g.Developer = value;

                else if (field == "Publisher(s)")
                    g.Publisher = value;

                else if (field == "Platform(s)")
                    g.Notes += field + ": " + value + "\n";

                else if (field == "Release")
                    g.Notes += field + ": " + value + "\n";

                else if (field == "Genre(s)")
                    g.Genre = value;

                else if (field == "Mode(s)")
                    g.Modes = value;

                else if (field == "Programmer(s)")
                    g.Programmers += value;

                else if (field == "Composer(s)")
                    g.Composers += value;

                else if (field == "Producer(s)")
                    g.Producers += value;

                else if (field == "Director(s)")
                    g.Directors += value;

                else if (field == "Artist(s)")
                    g.Artists += value;

                else if (field != "Cover art")
                {

                }
            }
            int i = Games.IndexOf(g);
        }

        public static void AddGameboyMultiList()
        {
            string path = @"C:\Users\Alec\Documents\Visual Studio 2015\Projects\Lists\Master List\GBMulti.txt"; // fix path
            string text = System.IO.File.ReadAllText(@path);
            List<string> list = text.Split('\r').ToList();
            string genre = "";
            int players = 2;
            int i = 0;
            foreach (string s in list)
            {
                i++;
                string line = s.Replace("\n", "");
                if (line.Contains("Four Player"))
                {
                    players = 4;
                    genre = null;
                }
                else if (i != 239 && list[list.IndexOf(s) + 1] == "\n")
                {
                    genre = list[list.IndexOf(s) + 2].Replace("\n", "");
                }
                else if (genre != "" && line != "")
                {
                    Game g = new Game(line);
                    g.Genre = genre;
                    g.LocalPlayers = players;
                    g.System = "GB";
                    Games.Add(g);
                }
            }
        }

        public static Dictionary<string, string> Systems()
        {
            var dict = new Dictionary<string, string>();
            Load(Url + "/Lists_of_video_games");
            var nodes = Nodes("//ul//li//a").Where(n => n.InnerText.Contains("List of"));
            foreach (HtmlNode n in nodes)
            {
                string url = n.OuterHtml.Remove(0, 14);
                url = url.Remove(url.IndexOf("\" t")).Replace("\" class=\"mw-redirect", "");
                dict.Add(Remove(n.InnerText, "List of ", " games"), url);
                if (n.InnerText == "List of VTech Handheld electronic games")
                    break;
            }                       

            dict["PlayStation"] = "/List_of_PlayStation_games_(A–L)";
            dict.Add("PlayStation_", "/List_of_PlayStation_games_(M–Z)");
            dict["PlayStation 3"] = "/List_of_PlayStation_3_games_released_on_disc";
            //dict.Remove("games for the original Game Boy");
            //dict.Add("Game Boy", "/List_of_Game_Boy_games");

            return dict;
        }    

        public static List<string> TableHeaders(string xPath)
        {
            var nodes = Nodes(xPath + "//th");
            var list = new List<string>();
            foreach (HtmlNode n in nodes)
                list.Add(Remove(n.InnerText, "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "&#", ";", "?", "(s)", "\n").ToLower().TrimEnd());
            if (list.Contains("release date") && (list.Contains("na") || list.Contains("north america") || list.Contains("ntsc")))
                list.Remove("release date");
            return list;
        }

        private static string GetUrl(string html)
        {
            if (!html.Contains("href=") || html.Contains("page does not exist"))
                return null;
            html = html.Remove(0, html.IndexOf("href") + 12);
            html = html.Remove(html.IndexOf("\""));
            return html;
        }

        private static bool IsDate(Game g, string v, string h)
        {
            string region = "";
            if (h.Contains("year") || h == "release date" || h == "date released" || h == "released" || h == "released dates" || h == "date")
            {
                if (v.Length == 4 && !v.Contains("?") && !v.Contains("e") && !v.Contains("y"))
                {
                    g.Year = Convert.ToInt32(v);
                    return true;
                }
                region = "all";
            }
            else if (h == "na" || h == "north america" || h.Contains("u.s.") || h == "na release" || h.Contains("ntsc"))
            {
                region = "na";
            }
            else if (h == "pal" || h == "eu" || h == "eu/pal" || h == "europe" || h.Contains("eu release") || h.Contains("e.u."))
            {
                region = "pal";
            }
            else if (h == "jap" || h == "jp" || h == "japan")
            {
                region = "jap";
            }
            else
            {
                return false;
            }

            var date = new DateTime();

            try
            {
                date = Convert.ToDateTime(v);
            }
            catch
            {
                return true;
            }

            if (region == "all")
                g.Date = date;
            else if (region == "na")
                g.DateNA = date;
            else if (region == "pal")
                g.DatePAL = date;
            else if (region == "jap")
                g.DateJAP = date;

            return true;         
        }

        public static List<string> failed = new List<string>();
        public static List<string> columns = new List<string>();

        public static void ScrapeLists()
        {
            var systems = Systems();

            foreach (KeyValuePair<string, string> pair in systems)
            {
                if (pair.Key == "PlayStation")
                {

                }

                Load(Url + pair.Value);
                var tables = new List<HtmlNode>();
                try
                {
                    tables = Nodes("//table").Where(n => (n.InnerText.Contains("Name") || n.InnerText.Contains("Title")) &&
                                                         (n.InnerText.Contains("Developer") || n.InnerText.Contains("Publisher") || n.InnerText.Contains("Genre"))).ToList();
                }
                catch
                {

                }

                if (tables.Count == 0)
                    continue;

                foreach (HtmlNode table in tables)
                {
                    var headers = TableHeaders(table.XPath);                               
                    var rows = Nodes(table.XPath + "//tr");
                    rows.RemoveAt(0);

                    foreach (HtmlNode row in rows)
                    {
                        var values = row.InnerText.Replace("\n\n", "\t").Replace("\n", "").Split('\t');

                        //if (headers.Count != values.Length)
                        //    continue;

                        Game g = new Game();
                        g.Url = GetUrl(row.OuterHtml);
                        g.System = pair.Key;

                        for (int i = 0; i < values.Length; i++)
                        {
                            string v = Remove(values[i], "amp;");
                            v = v.Contains("&#") ? v.Remove(v.IndexOf("&#")) : v;
                            if (v == "" || i >= headers.Count)
                                continue;
                            string h = headers[i];

                            if (h == "name" || h == "title")
                                g.Name = v;
                            else if (h.Contains("developer"))
                                g.Developer = v;
                            else if (h.Contains("publisher"))
                                g.Publisher = v;
                            else if (h == "genre")
                                g.Genre = v;
                            else if (!IsDate(g, v, h))
                            {
                                if (!columns.Contains(h))
                                    columns.Add(h);
                                g.Notes += "\n" + h + " : " + v;
                            }
                        }
                        if (g.Notes != null)
                            g.Notes = g.Notes.TrimStart('\n');
                        Games.Add(g);
                    }
                }
                System.Diagnostics.Debug.Write(Games.Count + " " + pair.Key);
            }
            columns.Sort();

            foreach (Game g in Games.Where(_g => _g.System == "PlayStation_"))
                g.System = "PlayStation";           
        }

        private static int failedDates = 0;

        private static void GetDate(Game g, string text) // needs work
        {
            if (!text.Contains("Release"))
                return;

            List<string> list = text.Split('\n').ToList();
            list.RemoveAll(s => s.ToString() == "");
            list.RemoveAll(s => s.ToString() == "Release");
            foreach (string s in list)
                ReplaceLinks(s);

            foreach (string s in list)
            {
                string date = s;
                g.DateString += date;

                #region Try to get dates
                //if (list.Count == 1)
                //{
                //    return;
                //}


                //if (s.Contains("JP") || s.Contains("NA") || s.Contains("PAL"))
                //{

                //}

                //try
                //{
                //    if (s.Contains("JP"))
                //        g.DateJAP = Convert.ToDateTime(date.Replace("JP: ", ""));
                //    else if (s.Contains("NA"))
                //        g.DateNA = Convert.ToDateTime(date.Replace("NA: ", ""));
                //    else if (s.Contains("PAL") || s.Contains("EU"))
                //        g.DatePAL = Convert.ToDateTime(date.Replace("EU: ", "").Replace("PAL: ", ""));
                //}
                //catch
                //{
                //    failedDates++;
                //    continue;
                //}
                #endregion
            }
        }

        public static void Clean(Game g)
        {
            if (g.Name != null)
                g.Name = ReplaceLinks(g.Name);
            if (g.Publisher != null)
                g.Publisher = ReplaceLinks(g.Publisher);
            if (g.Developer != null)
                g.Developer = ReplaceLinks(g.Developer);
            if (g.Genre != null)
                g.Genre = ReplaceLinks(g.Genre);
            if (g.Composers != null)
                g.Composers = ReplaceLinks(g.Composers);
            if (g.Programmers != null)
                g.Programmers = ReplaceLinks(g.Programmers);
            if (g.Artists != null)
                g.Artists = ReplaceLinks(g.Artists);
            if (g.Modes != null)
                g.Modes = ReplaceLinks(g.Modes);
        }

        private static string ReplaceLinks(string s)
        {
            for (int i = 0; i < 50; i++)
                if (s.Contains("[" + i + "]"))
                    s = s.Replace("[" + i + "]", "");
            return s;
        }

        public override Game GetGameFromReader(System.Data.SqlClient.SqlDataReader reader)
        {
            Game g = new Game(reader.GetString(0), reader.GetString(1));
            g.Developer = reader.GetString(2);
            g.Publisher = reader.GetString(3);
            g.Genre = reader.GetString(4);
            g.DateNA = reader.GetDateTime(5);
            g.DatePAL = reader.GetDateTime(6);
            g.DateJAP = reader.GetDateTime(7);
            g.Directors = reader.GetString(8);
            g.Producers = reader.GetString(9);
            g.Programmers = reader.GetString(10);
            g.Artists = reader.GetString(11);
            g.Modes = reader.GetString(12);
            g.Composers = reader.GetString(13);
            g.Notes = reader.GetString(14);
            g.UrlWikipedia = reader.GetString(15);
            return g;
        }
    }
}
