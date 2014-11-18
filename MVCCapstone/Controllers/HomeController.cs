using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using MVCCapstone.Models;
using MVCCapstone.Helpers;
using PagedList;

namespace MVCCapstone.Controllers
{
    [ValidateInput(false)]
    public class HomeController : Controller
    {
        public UsersContext db = new UsersContext();

        /// <summary>
        /// GET: Main home page
        /// </summary>
        public ActionResult Index(int? page)
        {
            if (!page.HasValue || page.Value < 0)
                page = 1;

            HomePageModel model = new HomePageModel();

            // user is locked, log them out and redirect them to the error page
            if (User.Identity.IsAuthenticated && User.IsInRole("locked"))
            {
                WebSecurity.Logout();
                return RedirectToAction("lockedaccount", "error");
            }

            // list of books to be displayed

            List<BookDisplayModel> dispModel = new   List<BookDisplayModel>();
            List<Book> bookList = db.Book.ToList();
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

                    dispModel.Add(bookModel);
                }
            }

            dispModel.OrderBy(m => m.Published).ToList();            

            model.BookList = dispModel.ToPagedList(page.Value, 20) as IPagedList<BookDisplayModel>;


            return View(model);
        }


    }
}
