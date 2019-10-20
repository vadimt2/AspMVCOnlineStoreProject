using ClassifiedProj.Models;
using DAL;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassifiedProj.Controllers
{
    public class HomeController : Controller
    {
        private DbManager dbManager;

        public HomeController()
        {
            dbManager = new DbManager();
        }
        // GET: Home
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Product");
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        [Route("About/")]
        public ActionResult About()
        {
            return View();
        }

        [Route("Error/")]
        public ActionResult Error()
        {
            return View();
        }
    }
}
