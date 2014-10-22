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

        
        public ActionResult Thread(int? forumid, int page = 1)
        {
            if (!ForumHelper.ValidateForumId(forumid.Value))
                return RedirectToAction("pagenotfound", "error");

            int validForumId = forumid.Value;

            ForumModel model = new ForumModel();
            model.ForumId = validForumId;
            model.threadList = ForumHelper.GetThreadList(validForumId, page);
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
            int threadId = -1; // default value for not valid
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


        public ActionResult ViewThread(int? threadid, int? post, int page = 1)
        {
            if (!ForumHelper.ValidateThreadId(threadid))
                return RedirectToAction("pagenotfound", "error");

            ForumHelper.IncrementThreadViewCount(threadid.Value);

            PostModel model = new PostModel();
            model.threadId = threadid.Value;
            model.forumId = ForumHelper.GetForumId(threadid.Value);
            model.threadTitle = ForumHelper.GetThreadTitle(threadid.Value);
            model.postList = ForumHelper.GetPostList(threadid.Value, page);

            if (post.HasValue)
                ViewBag.ScrollTo = post.Value;

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
                    model.replyPostId = replyPostId.Value;
                }

            }
            model.threadId = threadId.Value;

            return PartialView("_CreatePost", model);
        }

        [Authorize]
        public PartialViewResult ShowEdit(int? postId, int page = 1, bool removeReply = false)
        {
            EditPostModel model = new EditPostModel();
            model.showEditSection = true;

            if (!User.Identity.IsAuthenticated || !ForumHelper.ValidatePostId(postId))
                model.showEditSection = false;

            Post post = db.Post.Find(postId.Value);
            model.postId = post.PostId;
            model.threadId = post.ThreadId;
            model.content = post.PostContent;
            model.page = page;
            
            if (post.ReplyPostContent != null && removeReply == false) 
            {
                model.hasReply = true;
                model.replyContent = post.ReplyPostContent;
                model.replyTo = post.ReplyTo;
               
            }

            return PartialView("_EditPost", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreatePost(CreatePostModel model)
        {
            ForumHelper.CreatePost(AccHelper.GetUserId(User.Identity.Name), model.threadId, model.content, model.replyContent, model.replyTo, model.replyPostId);

            // select the latest post id which will be used to scroll the html page to
            int latestPost = db.Post.Where(m => m.ThreadId == model.threadId).OrderByDescending(m => m.PostDate).Select(m => m.PostId).First();

            // page set to -1 which will bring the thread to the last page
            return RedirectToAction("viewthread", "forum", new { threadid = model.threadId, page = -1, post = latestPost });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditPost(EditPostModel model)
        {
            ForumHelper.EditPost(model.postId, model.content, model.replyTo, model.replyContent);
          
            return RedirectToAction("viewthread", "forum", new { threadid = model.threadId, page = model.page, post = model.postId });
        }
    }
}
