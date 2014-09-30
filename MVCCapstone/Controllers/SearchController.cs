using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Models;

namespace MVCCapstone.Controllers
{
    public class SearchController : Controller
    {
        private SearchDBContext db = new SearchDBContext();

        //
        // GET: /Search/

        public ActionResult Index()
        {
            var genreList = from g in db.Genres
                            select g;
            genreList = genreList.OrderBy(g => g.Genre_ID);

            ViewBag.Language = db.Languages.ToList();
            ViewBag.Genre = genreList.ToList();

            return View();
        }

    }
}
