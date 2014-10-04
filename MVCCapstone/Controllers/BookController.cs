using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Helpers;
using MVCCapstone.Models;

namespace MVCCapstone.Controllers
{
    public class BookController : Controller
    {

        public UsersContext db = new UsersContext();
        //
        // GET: /Book/

        public ActionResult Index()
        {

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(int bookid)
        {

            Book book = db.Book.Find(bookid);
           
  
            BookDisplayModel model = new BookDisplayModel();
            model.BookId = book.BookId.ToString();
            model.Title = book.Title;
            model.Author = book.Author;
            model.ISBN = book.ISBN;
            model.Published = book.Published;
            model.Publisher = book.Publisher;
            model.Language = BookHelper.GetLanguage(book.LanguageId);
            model.Genre = BookHelper.GetBookGenreList(book.BookId.ToString());  // find every genre the book is associated with

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

    }
}
