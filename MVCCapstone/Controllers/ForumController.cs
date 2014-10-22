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

        
        public ActionResult Thread(int? forumid)
        {
            if (!ForumHelper.ValidateForumId(forumid.Value))
                return RedirectToAction("pagenotfound", "error");

            int validForumId = forumid.Value;

            ThreadModel model = new ThreadModel();
            model.ForumId = validForumId;
            model.threadList = ForumHelper.GetThreadList(validForumId);
            model.bookTitle = BookHelper.GetTitle(validForumId);
            model.series = ForumHelper.IsSeries(validForumId);
            if (model.series)
                model.sharedWith = ForumHelper.SharedWith(validForumId, User.IsInRole("admin"));
            

            return View(model);
        }

        [Authorize]
        public ActionResult CreateThread(int? forumId)
        {
            if (!ForumHelper.ValidateForumId(forumId))
                return RedirectToAction("pagenotfound", "error");

            CreateThreadModel model = new CreateThreadModel();
            model.forumid = forumId.Value;
            model.title = BookHelper.GetTitle(forumId.Value);
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
                ForumHelper.CreateThread(AccHelper.GetUserId(User.Identity.Name), model.forumid, model.threadTitle, model.post.content);
                threadId = db.Thread.OrderByDescending(m => m.ThreadCreated).Select(m => m.ThreadId).FirstOrDefault();
            }

            // the thread creation was not successful, redirect back to the thread list page
            if (threadId == -1)
                return RedirectToAction("thread", "forum", new { bookid = model.forumid });

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
            model.forumId = ForumHelper.GetForumId(threadid.Value);
            model.threadTitle = ForumHelper.GetThreadTitle(threadid.Value);
            model.postList = ForumHelper.GetPostList(threadid.Value);
            
            return View(model);
        }


        [Authorize]
        public PartialViewResult ShowPost(int? threadId, int? replyPostId)
        {

            CreatePostModel model = new CreatePostModel();
            model.showPostSection = true;

            if (!User.Identity.IsAuthenticated || !ForumHelper.ValidateThreadId(threadId))
                model.showPostSection = false;


            if (replyPostId.HasValue)
            {
                if (ForumHelper.PostIdExist(replyPostId.Value))
                {
                    model.showReply = true;
                    model.replyTo = ForumHelper.GetPostUserName(replyPostId.Value);
                    model.replyContent = ForumHelper.GetPostContent(replyPostId.Value);
                }

            }
            model.threadId = threadId.Value;

            return PartialView("_CreatePost", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreatePost(CreatePostModel model)
        {
            ViewBag.reply = model.replyContent;
            ForumHelper.CreatePost(AccHelper.GetUserId(User.Identity.Name), model.threadId, model.content, model.replyContent, model.replyTo);

            return RedirectToAction("viewthread", "forum", new { threadid = model.threadId });
        }

    }
}
