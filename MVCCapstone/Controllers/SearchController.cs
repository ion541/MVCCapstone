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

            // retrieve from a previous ajax post
            if (TempData["query"] != null)
            {
                model = TempData["query"] as SearchModel;
            }
            else
            {
                model.query = new SearchQuery();
            }

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
            
            List<Book> currentList = new List<Book>(); // list of books to be displayed
            List<Book> queryList;   // list of books to be added / compared to the current list
            List<string> errorMessages = new List<string>();
            
            // used to determine if the booklist is adding books for the first time
            bool currentListBooksNotEmpty = false;

            // filter the current list down by books that contains a title similar to the inputted title
            if (model.query.title != null)
            {
                queryList = db.Book.Where(m => m.Title.Contains(model.query.title)).OrderBy(m => m.Title).ToList();
                currentList = BookHelper.AddAllNewBooks(currentList, queryList, out currentListBooksNotEmpty);
            }


            // filter the current list down by books that contains a author similar to the inputted author
            if (model.query.author != null)
            {
                queryList = db.Book.Where(m => m.Author.Contains(model.query.author)).OrderBy(m => m.Title).ToList();
                currentList = BookHelper.ReturnSameBooks(currentList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
            }


            // filter the current list down by books that contains a publisher similar to the inputted publisher
            if (model.query.publisher != null)
            {
                queryList = db.Book.Where(m => m.Publisher.Contains(model.query.publisher)).OrderBy(m => m.Title).ToList();
                currentList = BookHelper.ReturnSameBooks(currentList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
            }


            // filter the current list down by the books that are published 
            if (model.query.yearFrom != null || model.query.yearTo != null)
            {
                int intYearFrom, intYearTo;
                DateTime yearTo = new DateTime(), yearFrom = new DateTime();

                // check to see if the year from is an integer
                bool validFrom = Int32.TryParse(model.query.yearFrom, out intYearFrom);
                if (validFrom)
                {
                    if (intYearFrom > 0)
                    {
                        yearFrom = new DateTime(intYearFrom, 1, 1);   // valid year input, set year from to the beginning of the year
                    }
                    else
                    {
                        validFrom = false;  // year from is an integer but is negative
                    }
                }
                // check to see if the year to is an integer
                bool validTo = Int32.TryParse(model.query.yearTo, out intYearTo);
                if (validTo)
                {
                    if (intYearTo > 0)
                    {
                        yearTo = new DateTime(intYearTo, 12, 31); // valid to input, set year to to the end of the year
                    }
                    else
                    {
                        validTo = false; // year to is an integer but is negative
                    }
                } 

                // make sure the user input range is appropriate
                if (validTo && validFrom && (intYearFrom > intYearTo))
                {
                    errorMessages.Add("Your Year From is greater then your Year To");
                }
                if (intYearFrom < 0 || intYearTo < 0)
                {
                    errorMessages.Add("Your years inputted must be greater then 0");
                }
                // get all dates published after the year from 
                else if (model.query.yearFrom != null && model.query.yearTo == null && validFrom)
                {
                    queryList = db.Book.Where(m => m.Published >= yearFrom).OrderBy(m => m.Title).ToList();
                    currentList = BookHelper.ReturnSameBooks(currentList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
                }
                // get all books published before the year to
                else if (model.query.yearFrom == null & model.query.yearTo != null && validTo)
                {
                    queryList = db.Book.Where(m => m.Published <= yearTo).OrderBy(m => m.Title).ToList();
                    currentList = BookHelper.ReturnSameBooks(currentList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
                }
                // get all books published between the year from and year to
                else if (validTo && validFrom)
                {
                    queryList = db.Book.Where(m => m.Published >= yearFrom && m.Published <= yearTo).OrderBy(m => m.Title).ToList();
                    currentList = BookHelper.ReturnSameBooks(currentList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
                }
                else
                {
                    errorMessages.Add("Your published range input must be numbers");
                }
            }


            // filter down by language if the id is not 8 (which represents multiple languages)
            if (model.query.language != "8" || !currentListBooksNotEmpty)
            {
                queryList = db.Book.Where(m => m.LanguageId == model.query.language).ToList();
                currentList = BookHelper.ReturnSameBooks(currentList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
            }


            // filter the current list down by books that contains a isbn similar to the inputted isbn
            if (model.query.isbn != null)
            {
                int isbn;
                if (Int32.TryParse(model.query.isbn, out isbn))
                {
                    queryList = db.Book.Where(m => m.ISBN.Contains(model.query.isbn)).ToList();
                    currentList = BookHelper.ReturnSameBooks(currentList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
                } 
                else
                {
                    errorMessages.Add("Your ISBN input is invalid.");
                }
            }
            

            // filter if at least a single genre was selected
            if (model.postedGenres != null)
            {
                if (model.postedGenres.GenreId.Count() > 0)
                {
                    // convert the posted genre list id to a list of int
                    List<int> postedGenreId = model.postedGenres.GenreId.Select(int.Parse).ToList();
                    // get every book id that has any of the genre id
                    List<int> bookWithPostedGenreId = db.BookGenre.Where(m => postedGenreId.Contains(m.GenreId)).Select(m => m.BookId).ToList();

                    // get every book that has any of the genre id
                    queryList = db.Book.Where(m => bookWithPostedGenreId.Contains(m.BookId)).ToList();
                    currentList = BookHelper.ReturnSameBooks(currentList, queryList, currentListBooksNotEmpty, out currentListBooksNotEmpty);
                }
            }


            ViewBag.ErrorMessages = errorMessages; // error messages to be displayed
            model.books = currentList.OrderBy(m => m.Title).ToList(); // sort the book one last time by the title

            // only show hidden books to admins
            if (!User.Identity.IsAuthenticated || !User.IsInRole("admin"))
                model.books = model.books.Where(m => m.State == "Visible").ToList();

            // if the result contains a book, see if there is a book with a title that matches the title
            if (model.books.Count() == 1)
            {
                if (db.Book.Where(m => m.Title == model.query.title).Count() == 1)
                    return RedirectToAction("details", "book", new { bookid = db.Book.Where(m => m.Title == model.query.title).Select(m => m.BookId).FirstOrDefault() });
            }

            TempData["query"] = model;
            return PartialView("_SearchResult",model);
        }

    }
}
