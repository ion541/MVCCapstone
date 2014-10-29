using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Models;
using MVCCapstone.Helpers;

namespace MVCCapstone.Controllers
{
    public class ReviewController : Controller
    {
        public UsersContext db = new UsersContext();


        //
        // GET: /Review/
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Book(int? id)
        {
            // bookid parameter must be valid and the book must also exist
            if (!id.HasValue)
            {
                return RedirectToAction("pagenotfound", "error");
            }
            else if (!BookHelper.BookExists(id.Value))
            {
                return RedirectToAction("pagenotfound", "error");
            }

            ReviewModel model = new ReviewModel();
            model.reviewList = db.Review.Where(m => m.BookId == id).ToList();


            return View(model);
        }



        
        public ActionResult Id(int? id)
        {
            return View("Review");
        }



        [Authorize]
        public ActionResult Create(int? id)
        {
            return View();
        }

    }
}
