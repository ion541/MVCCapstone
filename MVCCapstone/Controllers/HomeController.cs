using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using MVCCapstone.Models;
using MVCCapstone.Helpers;

namespace MVCCapstone.Controllers
{
    [ValidateInput(false)]
    public class HomeController : Controller
    {
        public UsersContext db = new UsersContext();

        /// <summary>
        /// GET: Main home page
        /// </summary>
        public ActionResult Index()
        {
            // user is locked, log them out and redirect them to the error page
            if (User.Identity.IsAuthenticated && User.IsInRole("locked"))
            {
                WebSecurity.Logout();
                return RedirectToAction("lockedaccount", "error");
            }

            // list of books to be displayed
            List<BookDisplayModel> model = new List<BookDisplayModel>();
           
            List<Book> bookList = db.Book.ToList();
            string ImageBasePath = Server.MapPath("~/");
            foreach (Book book in bookList)
            {
                BookDisplayModel bookModel = new BookDisplayModel();

               // to be viewable, the state of the book must be visible and the user is not an admin
                if (book.State != "Visible" && !(User.IsInRole("admin")))
                {
                   continue; // do not display this book in the view
                }
                else
                {
                    // book is set to be viewable or the the user is an admin
                    bookModel.BookId = book.BookId.ToString();
                    bookModel.Title = book.Title;
                    bookModel.Author = book.Author;
                    bookModel.Language = BookHelper.GetLanguage(book.LanguageId);
                    bookModel.Genre = BookHelper.GetBookGenreList(book.BookId);  // find every genre the book is associated with



                    // image can be null, if it is, use a default image otherwise attempt to find the image
                    if (book.ImageId == null)
                    {
                        bookModel.ImagePath = BookHelper.GetDefaultImage();
                    }
                    else
                    {
                        bookModel.ImagePath = BookHelper.GetImagePath(book.ImageId.ToString(), ImageBasePath);
                    }
                    model.Add(bookModel);
                }
            }
            ViewBag.BookList = model;

            return View();
        }


    }
}
