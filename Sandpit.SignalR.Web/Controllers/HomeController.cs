﻿using System.Web.Mvc;

namespace Sandpit.SignalR.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Chat()
        {
            return View();
        }

        public ActionResult Interactive()
        {
            return View();
        }
    }
}