using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Helpers;
using MVCCapstone.Models;
using System.Web.Security;

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
            int forumid;
            if (!ForumHelper.ValidateBookId(bookid, out forumid, User.IsInRole("admin")))
                return RedirectToAction("pagenotfound", "error");
          
            ThreadModel model = new ThreadModel();
            model.bookid = bookid.Value;
            model.threadList = ForumHelper.GetThreadList(forumid);
            model.bookTitle = BookHelper.GetTitle(bookid.Value);
            model.series = ForumHelper.IsSeries(forumid);
            if (model.series)
                model.sharedWith = ForumHelper.SharedWith(forumid, bookid.Value, User.IsInRole("admin"));
            

            return View(model);
        }

        [Authorize]
        public ActionResult CreateThread(int? bookid)
        {
            int forumid;
            if (!ForumHelper.ValidateBookId(bookid, out forumid, User.IsInRole("admin")))
                return RedirectToAction("pagenotfound", "error");

            CreateThreadModel model = new CreateThreadModel();
            model.bookid = bookid.Value;
            model.title = BookHelper.GetTitle(bookid.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateThread(CreateThreadModel model)
        {
            int threadId = -1;
            if (ModelState.IsValid)
            {
                ForumHelper.CreateThread(AccHelper.GetUserId(User.Identity.Name), model.bookid, model.threadTitle, model.post.content);
                threadId = db.Thread.OrderByDescending(m => m.ThreadCreated).Select(m => m.ThreadId).FirstOrDefault();
            }

            // the thread creation was not successful, redirect back to the thread list page
            if (threadId == -1)
                return RedirectToAction("thread", "forum", new { bookid = model.bookid });

            // new thread was created, redirect to it
            return RedirectToAction("viewthread", "forum", new { threadid = threadId });
        }

        public ActionResult ViewThread(int? threadid)
        {
            if (!ForumHelper.ValidateThreadId(threadid))
                return RedirectToAction("pagenotfound", "error");

            ForumHelper.IncrementThreadViewCount(threadid.Value);

            PostModel model = new PostModel();
            model.threadId = threadid.Value;
            model.bookId = ForumHelper.GetBookId(threadid.Value);
            model.threadTitle = ForumHelper.GetThreadTitle(threadid.Value);
            model.postList = ForumHelper.GetPostList(threadid.Value);
            
            return View(model);
        }

    }
}
