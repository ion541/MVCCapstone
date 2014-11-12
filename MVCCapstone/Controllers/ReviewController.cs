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



        public ActionResult Book(int? id, int page = 1, string sortby = "popular")
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
            model.bookId = id.Value;
            model.bookTitle = BookHelper.GetTitle(id.Value);
            model.reviewList = ReviewHelper.GetReviews(id.Value, sortby, page, 20);
            

            return View(model);
        }

  
        public ActionResult Id(int? id)
        {

            if (!ReviewHelper.ReviewIdValid(id))
                return RedirectToAction("pagenotfound", "error");

            Review review = db.Review.Find(id.Value);
            ReviewModel model = ReviewHelper.SetReviewModel(review, false, true);

            // check to see if the user is logged in whether or not they have rated the review
            if (User.Identity.IsAuthenticated){
                int userId = AccHelper.GetUserId(User.Identity.Name);

                ViewBag.Rated = false; //default
                
                // check to see if the user has rated this specific review
                if (db.ReviewRate.Where(m => m.ReviewId == id.Value && m.UserId == userId).FirstOrDefault() != null)
                {
                    ViewBag.UserRated = db.ReviewRate.Where(m => m.ReviewId == id.Value && m.UserId == userId).Select(m => m.Rate).First();
                    ViewBag.Rated = true; // default

                    ViewBag.Message = "helpful";
                    if (ViewBag.UserRated == "down")
                        ViewBag.Message = "NOT helpful";

                }
            }
            
            return View("Review",model);
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
                    return RedirectToAction("notvalidbookid", "error");
                }
            }
            else
            {
                return RedirectToAction("pagenotfound", "error");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateReview(ReviewModel model)
        {
            Review review = new Review();

            review.BookId = model.bookId;
            review.UserId = AccHelper.GetUserId(User.Identity.Name);
            review.Title = model.reviewTitle;
            review.Recommended = model.recommend;
            review.Content = model.reviewContent;
            review.DateCreated = DateTime.Now;
            review.DateModified = DateTime.Now;

            db.Review.Add(review);
            db.SaveChanges();

            int latestReviewId = db.Review.OrderByDescending(m => m.ReviewId).Select(m => m.ReviewId).First();
            return RedirectToAction("id", new { id = latestReviewId });
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

            if (model.reviewTitle == null) model.reviewTitle = "Empty Title";
            if (model.reviewContent == null) model.reviewContent = "Empty review";

            // is a preview, display a stack of tags if the content's tags are not well formed
            model.reviewContent = ReviewHelper.ReviewFilter(model.reviewContent, true);

            return PartialView("_Review", model);
        }


        [Authorize]
        public ActionResult Edit(int? id)
        {

            if (!ReviewHelper.ReviewIdValid(id))
                return RedirectToAction("pagenotfound", "error");

            Review review = db.Review.Find(id.Value);
            ReviewModel model = ReviewHelper.SetReviewModel(review, false, false);

            // only allow the user to edit the review if they are the owner of the review or is an admin
            if (AccHelper.GetUserId(model.author.ToLower()) != AccHelper.GetUserId(User.Identity.Name.ToLower()) && !User.IsInRole("admin"))
                return RedirectToAction("unauthorizedaccess", "error");


            return View("EditReview",model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditReview(ReviewModel model)
        {
            Review review = db.Review.Find(model.reviewId);

            review.Title = model.reviewTitle;
            review.Recommended = model.recommend;
            review.Content = model.reviewContent;
            review.DateModified = DateTime.Now;

            db.SaveChanges();

            return RedirectToAction("id", new { id = model.reviewId });
        }

   
        [Authorize]
        [AjaxAction]
        public ActionResult DeletePrompt(int? id)
        {
            if (!ReviewHelper.ReviewIdValid(id))
                return RedirectToAction("pagenotfound", "error");

            Review review = db.Review.Find(id.Value);

            if (review.UserId != AccHelper.GetUserId(User.Identity.Name) && !User.IsInRole("admin"))
                return RedirectToAction("unauthorizedaccess", "error");

            return PartialView("_Delete", review);
        }

        [Authorize]
        [AjaxAction]
        [HttpPost]
        public ActionResult DeleteReview(int? id)
        {
            if (!ReviewHelper.ReviewIdValid(id))
                return RedirectToAction("pagenotfound", "error");

            Review review = db.Review.Find(id.Value);

            if (review.UserId != AccHelper.GetUserId(User.Identity.Name) && !User.IsInRole("admin"))
                return RedirectToAction("unauthorizedaccess", "error");

            db.Review.Remove(review);
            int rowDeleted = db.SaveChanges();

            ViewBag.Deleted = false;

            if (rowDeleted == 1)
            {
                ViewBag.Deleted = true;

                var ratings = db.ReviewRate.Where(m => m.ReviewId == id.Value).ToList();
                foreach (ReviewRate rating in ratings)
                    db.ReviewRate.Remove(rating);

                ViewBag.RatingsDeleted = db.SaveChanges();
            }
  
            return PartialView("_DeleteResult");
        }



        [HttpPost]
        [Authorize]
        [AjaxAction]
        public string Rate(string rate, int reviewId)
        {
            bool alreadyRated = false;

            int userId = AccHelper.GetUserId(User.Identity.Name);

            if (db.ReviewRate.Where(m => m.ReviewId == reviewId && m.UserId == userId).Count() > 0)
                alreadyRated = true;

             ReviewRate rating;

            if (alreadyRated)
            {
                rating = db.ReviewRate.Where(m => m.ReviewId == reviewId && m.UserId == userId).First();
                rating.Rate = rate;
            }
            else
            {
                 rating = new ReviewRate();
                 rating.UserId = userId;
                 rating.ReviewId = reviewId;
                 rating.Rate = rate;
                 db.ReviewRate.Add(rating);
            }

            db.SaveChanges();

            return "Thank you for rating this review";
        }

    }
}
