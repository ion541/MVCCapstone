using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MVCCapstone.Models;
using PagedList;
using MVCCapstone.Helpers;
using System.Globalization;
using System.IO;

namespace MVCCapstone.Controllers
{
    [ValidateInput(false)]
    public class SearchController : Controller
    {
        private UsersContext db = new UsersContext();

        public ActionResult Index()
        {
            return RedirectToAction("Advanced");
        }


        //
        // GET: /Search/Advanced
        public ActionResult Advanced(string title)
        {
            SearchModel model = new SearchModel();

           model.query = new SearchQuery();
           model.availableLanguage = LanguageHelper.DisplayList();

            model.availableGenres = BookHelper.GetGenreList();
            if (title != null)
            {
                model.query.title = title;
                model.books = db.Book.Where(m => m.Title.Contains(title)).OrderBy(m => m.Title).ToList();

                // only show hidden books to admins
                if (!User.Identity.IsAuthenticated || !User.IsInRole("admin"))
                    model.books = model.books.Where(m => m.State == "Visible").ToList();

                // if the result contains a book, see if the title inputted matches a book title exactly (expl
                if (model.books.Count() == 1)
                {
                    if (db.Book.Where(m => m.Title == title).Count() == 1)
                        return RedirectToAction("details", "book", new { bookid = db.Book.Where(m => m.Title == title).Select(m => m.BookId).FirstOrDefault() });
                }
                
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Advanced(SearchModel model)
        {
            
            List<Book> bookList = new List<Book>();
            List<Book> queryList;
            
            // used to determine if the booklist is adding books for the first time
            bool currentListBooksNotEmpty = false;  

            if (model.query.title != null)
            {
                queryList = db.Book.Where(m => m.Title.Contains(model.query.title)).OrderBy(m => m.Title).ToList();
                bookList = BookHelper.AddAllNewBooks(bookList, queryList, out currentListBooksNotEmpty);
            }
            
            if (model.query.author != null)
            {
                queryList = db.Book.Where(m => m.Author.Contains(model.query.author)).OrderBy(m => m.Title).ToList();
                bookList = BookHelper.ReturnSameBooks(bookList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
            }

            if (model.query.publisher != null)
            {
                queryList = db.Book.Where(m => m.Publisher.Contains(model.query.publisher)).OrderBy(m => m.Title).ToList();
                bookList = BookHelper.ReturnSameBooks(bookList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
            }

            // only want to filter by language if there are already books in the list otherwise it will list every book with the language
            if (currentListBooksNotEmpty)
            {
                queryList = db.Book.Where(m => m.LanguageId == model.query.language).ToList();
                bookList = BookHelper.ReturnSameBooks(bookList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
            }
            
            // filter by genres if at least a single genre was selected
            if (model.postedGenres != null)
            {
                if (model.postedGenres.GenreId.Count() > 0)
                {
                    List<int> postedGenreId = model.postedGenres.GenreId.Select(int.Parse).ToList();
                    List<int> bookWithPostedGenreId = db.BookGenre.Where(m => postedGenreId.Contains(m.GenreId)).Select(m => m.BookId).ToList();
                    queryList = db.Book.Where(m => bookWithPostedGenreId.Contains(m.BookId)).ToList();
                    bookList = BookHelper.ReturnSameBooks(bookList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
                }
            }

           

            model.books = bookList;

            // only show hidden books to admins
            if (!User.Identity.IsAuthenticated || !User.IsInRole("admin"))
                model.books = model.books.Where(m => m.State == "Visible").ToList();

            // if the result contains a book, see if the title inputted matches a book title exactly (expl
            if (model.books.Count() == 1)
            {
                if (db.Book.Where(m => m.Title == model.query.title).Count() == 1)
                    return RedirectToAction("details", "book", new { bookid = db.Book.Where(m => m.Title == model.query.title).Select(m => m.BookId).FirstOrDefault() });
            }

            return PartialView("_SearchResult",model);
        }

    }
}
