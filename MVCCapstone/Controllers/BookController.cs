using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Helpers;
using MVCCapstone.Models;
using PagedList;

namespace MVCCapstone.Controllers
{
    [ValidateInput(false)]
    public class BookController : Controller
    {

        public UsersContext db = new UsersContext();



        /// <summary>
        /// GET: Redirect the user to the home page since no books were selected
        /// </summary>
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// GET: Display the details of the book as well as the most recent posts associated with
        /// the book and the top 8 reviews
        /// </summary>
        /// <param name="bookid">the id of the book to be displayed</param>
        public ActionResult Details(string bookid)
        {

            int idValidBook;
            if (!Int32.TryParse(bookid, out idValidBook))
            {
                return RedirectToAction("NotValidBookId", "Error");
            }

            Book book = db.Book.Find(idValidBook);
            // if the book id is invalid, redirect to the error page
            if (book == null)
                return RedirectToAction("NotValidBookId", "Error");


            // books that are set to hidden can only be viewed by admins
            if (book.State == "Hidden" && !User.IsInRole("admin"))
                return RedirectToAction("BookDeleted", "Error");


            BookDisplayModel model = new BookDisplayModel();

            // book details
            model.BookId = book.BookId.ToString();
            model.Title = book.Title;
            model.Author = book.Author;
            model.ISBN = book.ISBN;
            model.Published = book.Published.ToShortDateString();
            model.Publisher = book.Publisher;
            model.State = book.State;
            model.Language = BookHelper.GetLanguage(book.LanguageId);
            model.Genre = BookHelper.GetBookGenreList(book.BookId);  // find every genre the book is associated with
            model.Synopsis = (book.Synopsis == null) ? "N/A" : book.Synopsis;
            model.ForumId = book.ForumId;

            // top 8 threads
            model.ThreadList = ForumHelper.GetThreadList(book.ForumId, 1, 8);
            // top 8 reviews sorted by number of ratings
            model.ReviewList = ReviewHelper.GetReviews(book.BookId, "popular", 1, 8);

            model.DiscussionTitle = ForumHelper.GetForumTitle(book.ForumId);
            model.IsSeries = ForumHelper.IsSeries(book.ForumId);

            if (model.IsSeries)
                model.SeriesTitle = ForumHelper.GetForumTitle(book.ForumId);
                

            //image can be null, if it is, use a default image otherwise attempt to find the image 
            if (book.ImageId == null)
            {
                model.ImagePath = BookHelper.GetDefaultImage();
            }
            else
            {
                string ImageBasePath = Server.MapPath("~/");
                model.ImagePath = BookHelper.GetImagePath(book.ImageId.ToString(), ImageBasePath);
            }

            return View(model);
        }


        /// <summary>
        /// POST: Change the state of the book to hidden or change it to be seen by the public
        /// </summary>
        /// <param name="bookid">The id of the book to be changed</param>
        [AjaxAction]
        public PartialViewResult ChangeState(string bookid)
        {
            // change the state of the book to hidden or show the book if its hidden
            // make sure the user is logged in and is an admin
            if (User.Identity.IsAuthenticated && User.IsInRole("admin"))
                ViewBag.State = BookHelper.ChangeState(bookid);
            return PartialView("_BookState");
        }


        /// <summary>
        /// POST: Gets the top 8 reviews based on the users sort
        /// </summary>
        /// <param name="bookid">The id of the book</param>
        /// <param name="sortby">The sort of the review</param>
        /// <returns></returns>
        [AjaxAction]
        public ActionResult Review(int bookid, string sortby)
        {
            if (BookHelper.BookExists(bookid))
            {
                // get the top 8 reviews depending on the user sort
                ViewBag.ReviewList = ReviewHelper.GetReviews(bookid, sortby, 1, 8);
                ViewBag.Sort = sortby;
                return PartialView("_DetailBookReview");
            }

            return RedirectToAction("NotValidBookId", "Error");
        }

        

        /// <summary>
        /// POST: Adds the book to the users bookmark list
        /// </summary>
        /// <param name="bookid">the book id to be added</param>
        [AjaxAction]
        public PartialViewResult Bookmark(string bookid)
        {
            if (User.Identity.IsAuthenticated)
                ViewBag.Bookmark = BookHelper.Bookmark(User.Identity.Name, bookid);
            return PartialView("_BookmarkAdded");
        }
    }
}
