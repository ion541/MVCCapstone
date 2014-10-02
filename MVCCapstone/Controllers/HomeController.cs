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
    public class HomeController : Controller
    {
        public UsersContext db = new UsersContext();

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("locked"))
            {
                WebSecurity.Logout();
                return RedirectToAction("LockedAccount", "Error");
            }


            List<BookDisplayModel> model = new List<BookDisplayModel>();
           
            List<Book> bookList = db.Book.ToList();
            
            foreach (Book book in bookList)
            {
                BookDisplayModel bookModel = new BookDisplayModel();
                bookModel.Title = book.Title;
                bookModel.Author = book.Author;
                bookModel.ISBN = book.ISBN;
                bookModel.Published = book.Published;
                bookModel.Publisher = book.Publisher;
 
                if (book.ImageId == null)
                {
                    bookModel.ImagePath = BookHelper.GetDefaultImage();
                   
                }
                else
                {
                     bookModel.ImagePath = BookHelper.GetImagePath(book.ImageId.ToString());
                }
                bookModel.Language = BookHelper.GetLanguage(book.LanguageId);

                bookModel.Genre = BookHelper.GetBookGenreList(book.BookId.ToString());

                model.Add(bookModel);

            }


            ViewBag.BookList = model;
           
            return View();
        }


    }
}
