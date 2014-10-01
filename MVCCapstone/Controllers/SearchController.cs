using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Models;
using MVCCapstone.Helpers;

namespace MVCCapstone.Controllers
{
    public class SearchController : Controller
    {
        private UsersContext db = new UsersContext();

        //
        // GET: /Search/

        public ActionResult Index()
        {
            ViewBag.Language = db.Languages.ToList();
            ViewBag.Genre = db.Genres.ToList();

            return View();
        }

    }
}
