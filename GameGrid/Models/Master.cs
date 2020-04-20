using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace GameGrid.Models
{
    class Master
    {
        public List<Game> Games = new List<Game>();

        public List<Game> GetList()
        {
            LoadAll();
            CleanAll();
            CombineGames();
            return Games;
        }

        private void LoadAll()
        {
            Games.AddRange(SQLConnector.LoadTable(new CoverProject()));
            Games.AddRange(SQLConnector.LoadTable(new GameRankings()));
            Games.AddRange(SQLConnector.LoadTable(new HowLongToBeat()));
            Games.AddRange(SQLConnector.LoadTable(new PriceCharts()));
            Games.AddRange(SQLConnector.LoadTable(new VGChartz()));
            Games.AddRange(LoadVGCollect());
            Games.AddRange(SQLConnector.LoadTable(new Wikipedia()));
        }

        private List<Game> LoadVGCollect()
        {
            List<Game> list = SQLConnector.LoadTable(new VGCollect());
            list.RemoveAll(g => g.Name.Contains("[") && g.Name.Contains("]"));
            foreach (Game g in list)
            {
                if (g.Developer == "NA")
                    g.Developer = null;
                if (g.Publisher == "NA")
                    g.Publisher = null;
                if (g.Genre == "NA")
                    g.Genre = null;
                if (g.DateString == "NA")
                    g.DateString = null;
                if (g.Barcode == "N/A")
                    g.Barcode = null;
                if (g.ItemNumber == "N/A")
                    g.ItemNumber = null;
                if (g.ESRB == "None")
                    g.ESRB = null;
                if (g.System.Contains("[") && g.System.Contains("]"))
                {
                    if (g.DateString == null || g.DateString == "")
                        g.DateString = g.System.Remove(0, g.System.Length - 5);
                    else
                        g.DateString += "/" + g.System.Remove(0, g.System.Length - 5);
                    g.System = g.System.Remove(g.System.Length - 5);
                }
            }
            return list;
        }

        private void CleanAll()
        {
            Games.RemoveAll(g => string.IsNullOrEmpty(g.System));
            SplitGamesWithMultipleSystems();
            MatchSystem();
            SetBlankToNull();
            RemoveGamesWithInvalidSystems();
        }

        private void SplitGamesWithMultipleSystems()
        {
            var splitGames = new List<Game>();
            foreach (Game g in Games.Where(_g => _g.System.Contains(",")))
            {
                foreach (string s in g.System.Split(','))
                {
                    Game _g = new Game(g.Name);
                    CopyGame(_g, g);
                    _g.System = s;
                    splitGames.Add(_g);
                }
            }
            Games.RemoveAll(g => g.System.Contains(","));
            Games.AddRange(splitGames);
        }

        private void MatchSystem()
        {
            foreach (Game g in Games)
            {
                string s = g.System.TrimEnd(' ').TrimStart(' ').Replace("\t", "");
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
                    s = "Nintendo Game Boy";
                else if (s == "GBC" || s == "Game Boy Color")
                    s = "Nintendo Game Boy Color";
                else if (s == "GBA" || s == "Game Boy Advance")
                    s = "Nintendo Game Boy Advance";
                else if (s == "DS")
                    s = "Nintendo DS";
                else if (s == "3DS" || s == "Nintendo 2DS")
                    s = "Nintendo 3DS";
                else if (s == "VB" || s == "Virtual Boy")
                    s = "Nintendo Virtual Boy";
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
                g.System = s;
            }
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

        private void RemoveGamesWithInvalidSystems()
        {
            var systemsToRemove = new List<string>();
            foreach (string s in GetSystemsFromGames())
            {
                if (!ValidSystems.Contains(s))
                    systemsToRemove.Add(s);
            }
            foreach (string s in systemsToRemove)
                Games.RemoveAll(g => g.System == s);
        }

        private List<string> GetSystemsFromGames()
        {
            var list = new List<string>();
            foreach (Game g in Games)
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
            foreach (string s in ValidSystems)
            {
                i++;
                var gamesAdded = new List<string>();
                foreach (Game g in Games.Where(g => g.System == s))
                {
                    string name = StrippedName(g.Name);
                    if (gamesAdded.Contains(name))
                        continue;
                    if (!gamesAdded.Contains(name))
                        gamesAdded.Add(name);

                    foreach (Game _g in Games.Where(_g => _g.System == s && StrippedName(_g.Name) == name && g != _g))
                    {
                        CopyGame(g, _g);
                        _g.Deleted = true;
                    }
                }
                Games.RemoveAll(g => g.Deleted);
            }
        }

        private string StrippedName(string s)
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

            if (_g.Publisher != null && (g.Publisher == null || _g.Publisher.Contains(g.Publisher)))
                g.Publisher = _g.Publisher;
            else if (g.Publisher != null && _g.Publisher != null && !g.Publisher.ToLower().Contains(_g.Publisher.ToLower()))
                g.Publisher += "/" + _g.Publisher;

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
            "Nintendo Game Boy", "Nintendo Game Boy Color", "Nintendo Game Boy Advance", "Nintendo DS", "Nintendo 3DS", "Nintendo Virtual Boy",
            "Sega Master System", "Sega Genesis", "Sega Saturn", "Sega Dreamcast",
            "Playstation", "Playstation 2", "Playstation 3", "Playstation 4", "Playstation VR", "Playstation Portable", "Playstation Vita",
            "Xbox", "Xbox 360", "Xbox One", "PC", "Mac"
        };
    }
}