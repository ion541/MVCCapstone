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
    public class ForumController : Controller
    {

        public UsersContext db = new UsersContext();

        //
        // GET: /Forum/
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult Thread(int? bookid)
        {
            if (!bookid.HasValue)
                return RedirectToAction("pagenotfound", "error");


            int forumid = ForumHelper.GetForumId(bookid.Value);
            if (forumid == -1)
                return RedirectToAction("notvalidbookid", "error");

            ThreadModel model = new ThreadModel();
            model.bookid = bookid.Value;
            model.threadList = ForumHelper.GetThreadList(forumid);
            model.bookTitle = BookHelper.GetTitle(bookid.Value);
            model.series = ForumHelper.IsSeries(forumid);
            if (model.series)
                model.sharedWith = ForumHelper.SharedWith(forumid);

            return View(model);
        }

        public ActionResult CreateThread(int? bookid)
        {
            if (!bookid.HasValue) 
                return RedirectToAction("notvalidbookid", "error");
           
            if (!BookHelper.BookExists(bookid.Value)) 
                return RedirectToAction("notvalidbookid", "error");

            CreateThreadModel model = new CreateThreadModel();
            model.bookid = bookid.Value;
            model.title = BookHelper.GetTitle(bookid.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateThread(int? bookid, CreateThreadModel model)
        {

            return View();
        }

    }
}
