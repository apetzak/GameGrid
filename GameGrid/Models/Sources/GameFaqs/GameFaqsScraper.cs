using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace GameGrid.Models
{
    public class GameFaqsScraper : WebScraper
    {
        //    public static string UrlHome = "https://gamefaqs.gamespot.com";

        //    public static void Scrape()
        //    {
        //        //var list = DataController.GameFaqsUrls();

        //        //foreach (string s in list)
        //        //{
        //        //    Load(UrlHome + s);
        //        //    Game g = new Game();
        //        //    g.Url = s;
        //        //    g.Name = GetName();
        //        //    g.System = GetSystem();
        //        //    g.Description = GetDescription();
        //        //    SetDetails(g);
        //        //    SetUserRatings(g);
        //        //    SetCheats(g);
        //        //    SetTrivia(g);
        //        //    Games.Add(g);
        //        //}
        //        Save("GameFaqs");
        //    }

        //    public static string GetName()
        //    {
        //        return GetNode("//*[@class='page-title']").InnerText;
        //    }

        //    public static string GetSystem()
        //    {
        //        return GetNode("//*[@class='platform-title']").InnerText;
        //    }

        //    public static string GetDescription()
        //    {
        //        return GetNode("//*[@class='desc']").InnerText;
        //    }

        //    public static void SetDetails(Game g)
        //    {
        //        var node = GetNode("//*[@class='pod pod_gameinfo']");
        //        var text = Remove(node.InnerText, "\r\n", "\t\t", "Game Detail");
        //        var list = text.Split('\t');

        //        foreach (string s in list.Where(s => s.Contains(":")))
        //        {
        //            string field = s.Remove(s.IndexOf(":")).TrimStart();

        //            if (field == "Release")
        //                g.Date = Convert.ToDateTime(s.Replace("Release: ", ""));
        //            else if (field == "Platform")
        //                g.System = s.Replace("Platform: ", "");
        //            else if (field == "Genre")
        //                g.Genre = s.Replace("Genre: ", "");
        //            else if (field == "Expansions")
        //                g.Expansions = s.Replace("Expansions: ", "");
        //            else if (field == "Also Known As")
        //                g.AltName = s.Replace("Also Known As: ", "");
        //            else if (field == "Franchise")
        //                g.Franchise = s.Replace("Franchise: ", "");
        //            else if (field == "Publisher")
        //                g.Publisher = s.Replace("Publisher: ", "");
        //            else if (field == "Developer/Publisher")
        //            {
        //                g.Developer = s.Replace("Developer/Publisher: ", "");
        //                g.Publisher = s.Replace("Developer/Publisher: ", "");
        //            }
        //            else
        //            {

        //            }
        //        }
        //    }

        //    public static void SetUserRatings(Game g)
        //    {
        //        var nodes = GetNodes("//*[@class='subsection-title']");

        //        foreach (HtmlNode n in nodes.Where(n => !n.InnerText.Contains("Not Yet Rated")))
        //        {
        //            string text = n.InnerText.Replace("total votes", "").TrimEnd();
        //            if (text.Contains("Rating:"))
        //            {
        //                text = text.Remove(0, 7);
        //                g.Rating = Convert.ToDouble(text.Remove(4)) * 2;
        //                g.RatingVotes = Convert.ToInt32(text.Remove(0, 8));
        //            }
        //            else if (text.Contains("Difficulty:"))
        //            {
        //                text = Remove(text.Remove(0, 11), "of ", "Simple", "Easy", "Just Right", "Tough", "Unforgiving", "-");
        //                var arr = text.Split(' ');
        //                g.Difficulty = Convert.ToDouble(arr[0].Replace("%", ""));
        //                g.DifficultyVotes = Convert.ToInt32(arr[1]);
        //            }
        //            else if (text.Contains("Length:"))
        //            {
        //                text = Remove(text.Remove(0, 7), "Length:", "hours");
        //                var arr = text.Split(' ');
        //                g.Length = Convert.ToDouble(arr[0]);
        //                g.LengthVotes = Convert.ToInt32(arr[1]);
        //            }
        //        }
        //    }

        //    public static void SetCheats(Game g)
        //    {
        //        var node = GetNode("//*[@class='content_nav content_nav_split']");
        //        if (!node.InnerText.Contains("Cheats"))
        //            return;

        //        Load(UrlHome + g.Url + "/cheats");
        //        var nodes = GetNodes("//pod");
        //    }

        //    public static void SetTrivia(Game g)
        //    {
        //        var node = GetNode("//*[@class='content_nav content_nav_split']");
        //        if (!node.InnerText.Contains("Trivia"))
        //            return;

        //        Load(UrlHome + g.Url + "/trivia");
        //        var nodes = GetNodes("//");
        //    }
    }
}