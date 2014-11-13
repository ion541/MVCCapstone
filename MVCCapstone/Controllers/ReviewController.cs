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




        /// <summary>
        /// GET: Default index page, redirect to home page since no review is to be displayed
        /// </summary>
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }



        /// <summary>
        /// GET: Display the reviews for the selected book
        /// </summary>
        /// <param name="id">The id of the book</param>
        /// <param name="page">The page to be displayed</param>
        /// <param name="sortby">The sort</param>
        /// <returns></returns>
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

  

        /// <summary>
        /// GET: Display the review
        /// </summary>
        /// <param name="id">The id of the review</param>
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


        /// <summary>
        /// GET: Display the page to create a review
        /// </summary>
        /// <param name="id">The id of the book the review will be associated with</param>
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

        /// <summary>
        /// POST: Attempt to create a new review
        /// </summary>
        /// <param name="model">the data of the new review</param>
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



        /// <summary>
        /// POST: Display a preview based on the users input
        /// </summary>
        /// <param name="model">The data of the review</param>
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

            // could be empty during a preview
            if (model.reviewTitle == null) model.reviewTitle = "Empty Title";
            if (model.reviewContent == null) model.reviewContent = "Empty review";

            // is a preview, display a stack of tags if the content's tags are not well formed
            model.reviewContent = ReviewHelper.ReviewFilter(model.reviewContent, true);

            return PartialView("_Review", model);
        }




        /// <summary>
        /// GET: Display the edit page for the review
        /// </summary>
        /// <param name="id">The id of the review to be editted</param>
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




        /// <summary>
        /// POST: Edit the review
        /// </summary>
        /// <param name="model">The data of the editted review</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult EditReview(ReviewModel model)
        {
            // find the review and change it to the model's data
            Review review = db.Review.Find(model.reviewId);

            review.Title = model.reviewTitle;
            review.Recommended = model.recommend;
            review.Content = model.reviewContent;
            review.DateModified = DateTime.Now;

            db.SaveChanges();

            // redirect user to the review page
            return RedirectToAction("id", new { id = model.reviewId });
        }




        /// <summary>
        /// POST: Prompt the user to confirm their decision before deleting the review
        /// </summary>
        /// <param name="id">The id of the review to be deleted</param>
        [Authorize]
        [AjaxAction]
        public ActionResult DeletePrompt(int? id)
        {
            if (!ReviewHelper.ReviewIdValid(id))
                return PartialView("_Delete");

            Review review = db.Review.Find(id.Value);

            // only allow the owner / admin to delete the review
            if (review.UserId != AccHelper.GetUserId(User.Identity.Name) && !User.IsInRole("admin"))
                return PartialView("_Delete");

            return PartialView("_Delete", review);
        }



        /// <summary>
        /// POST: Attempt to the delete the review
        /// </summary>
        /// <param name="id">the id of the review  to be deleted</param>
        [Authorize]
        [AjaxAction]
        [HttpPost]
        public ActionResult DeleteReview(int? id)
        {
            if (!ReviewHelper.ReviewIdValid(id))
                return RedirectToAction("pagenotfound", "error");

            Review review = db.Review.Find(id.Value);

            // only allow the owner / admin to delete the review
            if (review.UserId != AccHelper.GetUserId(User.Identity.Name) && !User.IsInRole("admin"))
                return RedirectToAction("unauthorizedaccess", "error");

            db.Review.Remove(review);
            int rowDeleted = db.SaveChanges();

            ViewBag.Deleted = false;

            // determine if the review was successfully deleted
            if (rowDeleted == 1)
            {
                ViewBag.Deleted = true;

                // delete every rating from the database associated with the review
                var ratings = db.ReviewRate.Where(m => m.ReviewId == id.Value).ToList();
                foreach (ReviewRate rating in ratings)
                    db.ReviewRate.Remove(rating);

                ViewBag.RatingsDeleted = db.SaveChanges();
            }
  
            return PartialView("_DeleteResult");
        }


        

        /// <summary>
        /// POST: Insert a record of a rating for the review
        /// </summary>
        /// <param name="rate">The rating the user provided</param>
        /// <param name="reviewId">The id of the review being rated</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [AjaxAction]
        public string Rate(string rate, int reviewId)
        {

            if (db.Review.Find(reviewId) == null)
                return "This review does not exist...";

            bool alreadyRated = false;

            int userId = AccHelper.GetUserId(User.Identity.Name);

            // determine if the user has already rated the review
            if (db.ReviewRate.Where(m => m.ReviewId == reviewId && m.UserId == userId).Count() > 0)
                alreadyRated = true;

             ReviewRate rating;

            if (alreadyRated)
            {
                // find the record and change it
                rating = db.ReviewRate.Where(m => m.ReviewId == reviewId && m.UserId == userId).First();
                rating.Rate = rate;
            }
            else
            {
                // insert a new record for the rating
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
