using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Helpers;
using MVCCapstone.Models;

namespace MVCCapstone.Controllers
{
    [ValidateInput(false)]
    public class BookController : Controller
    {

        public UsersContext db = new UsersContext();
        //
        // GET: /Book/

        public ActionResult Index()
        {

            return RedirectToAction("Index", "Home");
        }

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


        public PartialViewResult ChangeState(string bookid)
        {
            if (User.Identity.IsAuthenticated)
                ViewBag.State = BookHelper.ChangeState(bookid);
            return PartialView("_BookState");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookid"></param>
        /// <returns></returns>
        public ActionResult Bookmark(string bookid)
        {
            if (User.Identity.IsAuthenticated)
                ViewBag.Bookmark = BookHelper.Bookmark(User.Identity.Name, bookid);
            return PartialView("_BookmarkAdded");
        }


        /// <summary>
        /// Autocomplete for the search
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult Autocomplete(string term)
        {

            string[] bookList;
            // Get Tags from database
            if (User.Identity.IsAuthenticated && User.IsInRole("admin"))
            {
                bookList = db.Book.Where(m => m.Title.Contains(term)).Select(m => m.Title).ToArray();
            }
            else
            {
                bookList = db.Book.Where(m => m.Title.Contains(term) && m.State != "Hidden").Select(m => m.Title).ToArray();
            }

            return this.Json(bookList, JsonRequestBehavior.AllowGet);
        }


    }
}
