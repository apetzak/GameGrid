using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameGrid.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Wikipedia.Scrape();
            //var list = new Models.Master().GetList();
            //Models.SQLConnector.Insert(list, "Master");

            return View(Models.SQLConnector.LoadMaster());
        }

        public ActionResult About()
        {  
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Details(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return View(new Models.Game());

            Models.Game game = Models.Game.LoadAll().Where(g => g.Name == name).First();
            return View(game);
        }

        public ActionResult Edit(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return View(new Models.Game());

            Models.Game game = Models.Game.LoadAll().Where(g => g.Name == name).First();
            return View(game);
        }
    }
}