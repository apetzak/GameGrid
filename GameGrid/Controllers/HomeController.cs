using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameGrid.Models;

namespace GameGrid.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var list3 = Models.SQLConnector.LoadTable(new CoverProjectCover());
            //foreach (CoverProjectCover cpc in list3)
            //{
            //    if (String.IsNullOrEmpty(cpc.CreatedBy))
            //    {
            //        CoverProjectScraper.UpdateCover(cpc);
            //        Models.SQLConnector.Update(cpc, "CoverID");
            //    }
            //}

            //WikipediaScraper.ScrapeLists();
            //WikipediaScraper.Scrape();

            //Models.Master.LoadAll();

            return View(SQLConnector.LoadTable(new Game()));
        }

        public ActionResult About()
        {  
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Details(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return View(new Game());

            DBObject game = SQLConnector.LoadTable(new Game()).Where(g => (g as Game).Name == name).First();
            return View(game);
        }

        public ActionResult Edit(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return View(new Game());

            DBObject game = SQLConnector.LoadTable(new Game()).Where(g => (g as Game).Name == name).First();
            return View(game);
        }
    }
}