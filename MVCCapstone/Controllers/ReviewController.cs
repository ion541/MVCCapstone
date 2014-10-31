using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Models;
using MVCCapstone.Helpers;

namespace MVCCapstone.Controllers
{
    [ValidateInput(false)]
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

            ReviewListModel model = new ReviewListModel();
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
            if (id.HasValue)
            {
                if (BookHelper.BookExists(id.Value))
                {
                    ReviewModel model = new ReviewModel();
                    model.bookId = id.Value;
                    model.bookTitle = BookHelper.GetTitle(id.Value);

                    return View(model);
                }
                else
                {
                    return RedirectToAction("pagenotfound", "error");
                }
            }
            else
            {
                return RedirectToAction("notvalidbookid", "error");
            }
        }

        [AjaxAction]
        [HttpPost]
        [Authorize]
        public ActionResult CreatePreview(ReviewModel model)
        {
           
            if (model.recommend == "yes")
            {
                model.isRecommended = true;
            }
            else
            {
                model.isRecommended = false;
            }

            model.author = User.Identity.Name;
            model.reviewCreated = DateTime.Today;

            if (model.reviewTitle == null) model.reviewTitle = "Empty Title";
            if (model.reviewContent == null) model.reviewContent = "Empty review";
            model.reviewContent = ReviewHelper.ReviewFilter(model.reviewContent);



            return PartialView("_Review", model);
        }

    }
}
