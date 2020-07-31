using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace GameGrid.Models
{
    class Master
    {
        public static List<DBObject> Games = new List<DBObject>();
        public static List<DBObject> GameDetails = new List<DBObject>();
        public static List<DBObject> GameReleases = new List<DBObject>();

        public List<DBObject> GetList()
        {
            LoadAll();
            //CleanAll();
            CombineGames();
            return Games;
        }

        public static void LoadAll()
        {
            //CreateDetails();
            //CreateGames();
            CreateReleases();
        }

        private static List<DBObject> LoadGamesWithDetails()
        {
            var games = SQLConnector.LoadTable(new Game());
            var details = SQLConnector.LoadTable(new GameDetail());
            foreach (Game g in games)
            {
                foreach (GameDetail d in details)
                {
                    if (g.GameID == d.GameID)
                        g.Details.Add(d);
                }
            }
            return games;
        }

        private static void CreateDetails()
        {
            var coverProjects = LoadSortedTable(new CoverProject());
            var gameRankings = LoadSortedTable(new GameRankings());
            var vgCharts = LoadSortedTable(new VGChartz());
            var wikiDetail = LoadSortedTable(new WikipediaDetail());

            foreach (CoverProject c in coverProjects)
            {
                bool gameExists = false;
                foreach (Game g in Games)
                {
                    if (g.Name.ToLower() == c.Name.ToLower())
                    {
                        if (g.GetDetail(c.System) != null)
                        {
                            g.GetDetail(c.System).CoverProjectID = c.GameID;
                        }
                        else
                        {
                            GameDetail d = new GameDetail(g.GameID, c.System);
                            d.CoverProjectID = c.GameID;
                            g.Details.Add(d);
                        }
                        gameExists = true;
                        break;
                    }
                }
                if (gameExists)
                    continue;
                Game game = new Game(Games.Count, c.Name);
                GameDetail detail = new GameDetail(game.GameID, c.System);
                detail.CoverProjectID = c.GameID;
                game.Details.Add(detail);
                Games.Add(game);
            }

            foreach (GameRankings gr in gameRankings)
            {
                bool gameExists = false;
                foreach (Game g in Games)
                {
                    if (g.Name.ToLower() == gr.Name.ToLower())
                    {
                        if (g.GetDetail(gr.System) != null)
                        {
                            g.GetDetail(gr.System).GameRankingsURL = gr.Url;
                        }
                        else
                        {
                            GameDetail d = new GameDetail(g.GameID, gr.System);
                            d.GameRankingsURL = gr.Url;
                            g.Details.Add(d);
                        }
                        gameExists = true;
                        break;
                    }
                }
                if (gameExists)
                    continue;
                Game game = new Game(Games.Count, gr.Name);
                GameDetail detail = new GameDetail(game.GameID, gr.System);
                detail.GameRankingsURL = gr.Url;
                game.Details.Add(detail);
                Games.Add(game);
            }

            foreach (VGChartz vgc in vgCharts)
            {
                bool gameExists = false;
                foreach (Game g in Games)
                {
                    if (g.Name.ToLower() == vgc.Name.ToLower())
                    {
                        if (g.GetDetail(vgc.System) != null)
                        {
                            g.GetDetail(vgc.System).VGChartzURL = vgc.URL;
                        }
                        else
                        {
                            GameDetail d = new GameDetail(g.GameID, vgc.System);
                            d.VGChartzURL = vgc.URL;
                            g.Details.Add(d);
                        }
                        gameExists = true;
                        break;
                    }
                }
                if (gameExists)
                    continue;
                Game game = new Game(Games.Count, vgc.Name);
                GameDetail detail = new GameDetail(game.GameID, vgc.System);
                detail.VGChartzURL = vgc.URL;
                game.Details.Add(detail);
                Games.Add(game);
            }

            foreach (WikipediaDetail wd in wikiDetail)
            {
                bool gameExists = false;
                foreach (Game g in Games)
                {
                    if (g.Name.ToLower() == wd.Title.ToLower())
                    {
                        if (g.GetDetail(wd.Platform) != null)
                        {
                            g.GetDetail(wd.Platform).WikipediaDetailURL = wd.PageURL;
                        }
                        else
                        {
                            GameDetail d = new GameDetail(g.GameID, wd.Platform);
                            d.WikipediaDetailURL = wd.PageURL;
                            g.Details.Add(d);
                        }
                        gameExists = true;
                        break;
                    }
                }
                if (gameExists)
                    continue;
                Game game = new Game(Games.Count, wd.Title);
                GameDetail detail = new GameDetail(game.GameID, wd.Platform);
                detail.WikipediaDetailURL = wd.PageURL;
                game.Details.Add(detail);
                Games.Add(game);
            }

            foreach (Game g in Games)
            {
                foreach (GameDetail d in g.Details)
                {
                    d.GameDetailID = GameDetails.Count;
                    GameDetails.Add(d);
                }
            }

            SQLConnector.Insert(Games);
            SQLConnector.Insert(GameDetails);
        }

        private static void CreateGames()
        {
            var existingGames = LoadGamesWithDetails();
            var existingDetails = SQLConnector.LoadTable(new GameDetail());

            var newGames = new List<DBObject>();
            var newDetails = new List<DBObject>();

            var wikipedias = LoadSortedTable(new Wikipedia());
            var howLongToBeats = LoadSortedTable(new HowLongToBeat());

            wikipedias.RemoveAll(w => (w as Wikipedia).Title == "");

            foreach (Wikipedia w in wikipedias)
            {
                Game g = new Game(w.Title);
                g.WikipediaURL = w.URL;

                CreateDetailsFromGame(g, w.Platforms);

                int hltbIindex = -1;

                foreach (HowLongToBeat hltb in howLongToBeats)
                {
                    if (hltb.Name.ToLower() == w.Title.ToLower())
                    {
                        hltbIindex = howLongToBeats.IndexOf(hltb);
                        g.HowLongToBeatID = Convert.ToInt32(hltb.Url);

                        CreateDetailsFromGame(g, hltb.System);
                        break;
                    }
                }

                if (g.HowLongToBeatID != 0)
                    howLongToBeats.RemoveAt(hltbIindex);

                if (g.Details.Count > 0)
                    newGames.Add(g);
            }

            foreach (HowLongToBeat hltb in howLongToBeats)
            {
                Game g = new Game(hltb.Name);
                g.HowLongToBeatID = Convert.ToInt32(hltb.Url);

                CreateDetailsFromGame(g, hltb.System);

                if (g.Details.Count > 0)
                    newGames.Add(g);
            }

            foreach (Game g in existingGames)
            {
                int newGameIndex = -1;
                foreach (Game g2 in newGames)
                {
                    if (g.Name.ToLower() == g2.Name.ToLower())
                    {
                        g.WikipediaURL = g2.WikipediaURL;
                        g.HowLongToBeatID = g2.HowLongToBeatID;
                        g2.GameID = g.GameID;
                        newGameIndex = newGames.IndexOf(g2);
                        foreach (GameDetail d in g2.Details)
                        {
                            if (!g.HasDetail(d.System))
                            {
                                d.GameID = g.GameID;
                                newDetails.Add(d);
                            }
                        }
                        break;
                    }
                }

                if (newGameIndex != -1)
                    newGames.RemoveAt(newGameIndex);
            }

            var newGameDetails = new List<DBObject>();
            foreach (Game g in newGames)
            {
                foreach (GameDetail d in g.Details)
                    newGameDetails.Add(d);
            }

            #region Set IDs
            int id = existingGames.Count;
            foreach (Game g in newGames)
                g.GameID = id++;
            id = existingDetails.Count;
            foreach (GameDetail d in newGameDetails)
                d.GameDetailID = id++;
            foreach (GameDetail d in newDetails)
                d.GameDetailID = id++;

            foreach (Game g in newGames)
            {
                foreach (GameDetail d in g.Details)
                    d.GameID = g.GameID;
            }
            #endregion

            SQLConnector.Insert(newGames);
            SQLConnector.Insert(newGameDetails);
            SQLConnector.Insert(newDetails);

            foreach (Game g in existingGames)
                SQLConnector.Update(g, "GameID");
        }

        private static void CreateDetailsFromGame(Game g, string systems)
        {
            foreach (string s in systems.Split(','))
            {
                string sys = MatchSystem(s);
                if (ValidSystems.Contains(sys) && !g.HasDetail(sys))
                {
                    GameDetail d = new GameDetail();
                    d.System = sys;
                    g.Details.Add(d);
                }
            }
        }

        private static void CreateReleases()
        {
            var existingGames = SQLConnector.LoadTable(new Game());
            var existingDetails = SQLConnector.LoadTable(new GameDetail());
            var coverProjectCovers = LoadSortedTable(new CoverProjectCover());
            var coverProjects = LoadSortedTable(new CoverProject());

            var priceChartings = LoadSortedTable(new PriceCharting());
            var vgCollectings = LoadSortedTable(new VGCollect());
        }

        /// <summary>
        /// Load a table, remove all entries with invalid systems, sort by name
        /// </summary>
        /// <param name="dbo"></param>
        private static List<DBObject> LoadSortedTable(DBObject dbo)
        {
            var list = SQLConnector.LoadTable(dbo);

            Type t = dbo.GetType();

            PropertyInfo pi = t.GetProperty("System");
            if (pi == null)
                pi = t.GetProperty("Platform");
            if (pi == null)
                return list;

            foreach (DBObject o in list)
            {
                pi.SetValue(o, MatchSystem(pi.GetValue(o).ToString()));
            }

            list.RemoveAll(g => !ValidSystems.Contains(pi.GetValue(g)));

            pi = t.GetProperty("Name");
            if (pi == null)
                pi = t.GetProperty("Title");
            if (pi == null)
                return list;

            list = list.OrderBy(o => pi.GetValue(o)).ToList();
            return list;
        }

        private List<DBObject> LoadVGCollect()
        {
            List<DBObject> list = SQLConnector.LoadTable(new VGCollect());
            //list.RemoveAll(g => g.Name.Contains("[") && g.Name.Contains("]"));
            //foreach (DBObject dbo in list)
            //{
            //    if (dbo.Developer == "NA")
            //        dbo.Developer = null;
            //    if (dbo.Publisher == "NA")
            //        dbo.Publisher = null;
            //    if (dbo.Genre == "NA")
            //        dbo.Genre = null;
            //    if (dbo.DateString == "NA")
            //        dbo.DateString = null;
            //    if (dbo.Barcode == "N/A")
            //        dbo.Barcode = null;
            //    if (dbo.ItemNumber == "N/A")
            //        dbo.ItemNumber = null;
            //    if (dbo.ESRB == "None")
            //        dbo.ESRB = null;
            //    if (dbo.System.Contains("[") && dbo.System.Contains("]"))
            //    {
            //        if (dbo.DateString == null || dbo.DateString == "")
            //            dbo.DateString = dbo.System.Remove(0, dbo.System.Length - 5);
            //        else
            //            dbo.DateString += "/" + dbo.System.Remove(0, dbo.System.Length - 5);
            //        dbo.System = dbo.System.Remove(dbo.System.Length - 5);
            //    }
            //}
            return list;
        }

        private void CleanAll()
        {
            SetBlankToNull();
            RemoveGamesWithInvalidSystems();
        }

        private void SplitGamesIntoDetails()
        {
            //var splitGames = new List<DBObject>();
            //foreach (Game g in Games.Where(_g => (_g as Game).System.Contains(",")))
            //{
            //    foreach (string s in g.System.Split(','))
            //    {
            //        GameDetail _g = new GameDetail();
            //        //CopyGame(_g, g);
            //        //_g.System = s;
            //        splitGames.Add(_g);
            //    }
            //}
            //GameDetails.RemoveAll(g => (g as GameDetail).System.Contains(","));
            //GameDetails.AddRange(splitGames);
        }

        private static string MatchSystem(string s)
        {
            s = s.TrimEnd(' ').TrimStart(' ').Replace("\t", "");
            if (s == "NES" || s == "Nintendo Entertainment System")
                s = "Nintendo";
            else if (s == "SNES" || s == "Super Nintendo Entertainment System")
                s = "Super Nintendo";
            else if (s == "N64")
                s = "Nintendo 64";
            else if (s == "GC" || s == "Nintendo Gamecube" || s == "Nintendo GameCube" || s == "GameCube")
                s = "Nintendo Gamecube";
            else if (s == "WII" || s == "Wii")
                s = "Nintendo Wii";
            else if (s == "WIIU" || s == "Wii U")
                s = "Nintendo Wii U";
            else if (s == "NS" || s == "Switch")
                s = "Nintendo Switch";
            else if (s == "GB" || s == "Game Boy")
                s = "Game Boy";
            else if (s == "GBC")
                s = "Game Boy Color";
            else if (s == "GBA" || s == "Game Boy Advance")
                s = "Game Boy Advance";
            else if (s == "DS")
                s = "Nintendo DS";
            else if (s == "3DS" || s == "Nintendo 2DS")
                s = "Nintendo 3DS";
            else if (s == "VB" || s == "Virtual Boy")
                s = "Virtual Boy";
            else if (s == "Master System" || s == "MASTER")
                s = "Sega Master System";
            else if (s == "Sega Genesis" || s == "GEN")
                s = "Sega Genesis";
            else if (s == "SAT" || s == "Saturn")
                s = "Sega Saturn";
            else if (s == "DC" || s == "DREAMCAST")
                s = "Sega Dreamcast";
            else if (s == "PS1" || s == "PS" || s == "PlayStation")
                s = "Playstation";
            else if (s == "PS2" || s == "PlayStation 2")
                s = "Playstation 2";
            else if (s == "PS3" || s == "PlayStation 3")
                s = "Playstation 3";
            else if (s == "PS4" || s == "PlayStation 4")
                s = "Playstation 4";
            else if (s == "PlayStation VR")
                s = "Playstation VR";
            else if (s == "PSP" || s == "PlayStation Portable")
                s = "Playstation Portable";
            else if (s == "VITA" || s == "PlayStation Vita")
                s = "Playstation Vita";
            else if (s == "XBOX")
                s = "Xbox";
            else if (s == "X360" || s == "XBOX 360")
                s = "Xbox 360";
            else if (s == "XBOX ONE")
                s = "Xbox One";
            return s;
        }

        public void SetBlankToNull()
        {
            Type t = Type.GetType("Master_List.Game");
            var info = typeof(Game).GetTypeInfo();
            foreach (Game g in Games)
            {
                foreach (FieldInfo f in info.DeclaredFields)
                {
                    string type = f.ToString().Split('<')[0].TrimEnd();
                    var value = f.GetValue(g);
                    if (type.Contains("System.String") && value != null && value.ToString() == String.Empty)
                        f.SetValue(g, null);
                }
            }
        }

        private static void RemoveGamesWithInvalidSystems()
        {
            var systemsToRemove = new List<string>();
            foreach (string s in GetSystemsFromGames())
            {
                if (!ValidSystems.Contains(s))
                    systemsToRemove.Add(s);
            }
            foreach (string s in systemsToRemove)
                GameDetails.RemoveAll(g => (g as GameDetail).System == s);
        }

        private static List<string> GetSystemsFromGames()
        {
            var list = new List<string>();
            foreach (GameDetail g in GameDetails)
            {
                if (!list.Contains(g.System))
                    list.Add(g.System);
            }
            list.Sort();
            return list;
        }

        private void CombineGames()
        {
            int i = 0;
            //foreach (string s in ValidSystems)
            //{
            //    i++;
            //    var gamesAdded = new List<string>();
            //    foreach (GameDetail g in GameDetails.Where(g => g.System == s))
            //    {
            //        string name = StrippedName(g.Name);
            //        if (gamesAdded.Contains(name))
            //            continue;
            //        if (!gamesAdded.Contains(name))
            //            gamesAdded.Add(name);

            //        foreach (GameDetail _g in GameDetails.Where(_g => _g.System == s && StrippedName(_g.Name) == name && g != _g))
            //        {
            //            CopyGame(g, _g);
            //            _g.Deleted = true;
            //        }
            //    }
            //    GameDetails.RemoveAll(g => g.Deleted);
            //}
        }

        private static string StrippedName(string s)
        {
            return s.ToLower().Replace("'", "").Replace(",", "").Replace("°", "").Replace(":", "").Replace("!", "");
        }

        public static void CopyGame(Game g, Game _g)
        {
            if (_g.Name.Length > g.Name.Length)
                g.Name = _g.Name;

            if (_g.Developer != null && (g.Developer == null || _g.Developer.Contains(g.Developer)))
                g.Developer = _g.Developer;
            else if (g.Developer != null && _g.Developer != null && !g.Developer.ToLower().Contains(_g.Developer.ToLower()))
                g.Developer += "/" + _g.Developer;

            //if (_g.Publisher != null && (g.Publisher == null || _g.Publisher.Contains(g.Publisher)))
            //    g.Publisher = _g.Publisher;
            //else if (g.Publisher != null && _g.Publisher != null && !g.Publisher.ToLower().Contains(_g.Publisher.ToLower()))
            //    g.Publisher += "/" + _g.Publisher;

            if (_g.Genre != null && (g.Genre == null || _g.Genre.Contains(g.Genre)))
                g.Genre = _g.Genre;
            else if (g.Genre != null && _g.Genre != null && !g.Genre.ToLower().Contains(_g.Genre.ToLower()))
                g.Genre += "/" + _g.Genre;

            var properties = typeof(Game).GetProperties();

            foreach (PropertyInfo pi in properties)
            {
                string p = pi.ToString();
                object val = pi.GetValue(_g);

                if (p.Contains("String") && pi.GetValue(g) == null && val != null ||
                    p.Contains("DateTime") && Convert.ToDateTime(val) == DateTime.MinValue && Convert.ToDateTime(val) != null ||
                    p.Contains("Double") && Convert.ToDouble(val) == 0 && Convert.ToDouble(val) != 0 ||
                    p.Contains("Int32") && Convert.ToInt32(val) == 0 && Convert.ToInt32(val) != 0 ||
                    p.Contains("Boolean"))
                    pi.SetValue(g, val, null);
            }
        }

        public static List<string> ValidSystems = new List<string>
        {
            "Nintendo", "Super Nintendo", "Nintendo 64", "Nintendo Gamecube", "Nintendo Wii", "Nintendo Wii U", "Nintendo Switch",
            "Game Boy", "Game Boy Color", "Game Boy Advance", "Nintendo DS", "Nintendo 3DS", "Virtual Boy",
            "Sega Master System", "Sega Genesis", "Sega Saturn", "Sega Dreamcast",
            "Playstation", "Playstation 2", "Playstation 3", "Playstation 4", "Playstation VR", "Playstation Portable", "Playstation Vita",
            "Xbox", "Xbox 360", "Xbox One"
        };
    }
}