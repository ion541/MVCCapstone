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
            model.forumTitle = ForumHelper.GetForumTitle(validForumId);
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
            model.title = ForumHelper.GetForumTitle(forumId.Value);
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


        public ActionResult ViewThread(int? threadid, int? post, bool fview = false, int page = 1)
        {
            // check to see if the thread id exist
            if (!ForumHelper.ValidateThreadId(threadid))
                return RedirectToAction("pagenotfound", "error");

            // if this is the first view, increment the threads view counter
            if (fview)
                ForumHelper.IncrementThreadViewCount(threadid.Value);

            // populate the model with data
            ThreadViewModel model = new ThreadViewModel();
            model.threadId = threadid.Value;
            model.forumId = ForumHelper.GetForumId(threadid.Value);
            model.lockAction = ForumHelper.GetLockActionString(threadid.Value);
            model.threadTitle = ForumHelper.GetThreadTitle(threadid.Value);
            model.postList = ForumHelper.GetPostList(threadid.Value, page); // list of posts in the thread

            // scroll to the post upon loading
            if (post.HasValue)
                ViewBag.ScrollTo = post.Value;

            return View(model);
        }

        
        [Authorize]
        [AjaxAction]
        public PartialViewResult ShowPost(int? threadId, int? replyPostId)
        {

            CreatePostModel model = new CreatePostModel();
            model.showPostSection = true;

            // check to see if user is logged and the thread id is valid
            if (!User.Identity.IsAuthenticated || !ForumHelper.ValidateThreadId(threadId))
                model.showPostSection = false;

            // do this after the thread is is validated
            if (ForumHelper.ThreadIsLocked(threadId.Value) && !User.IsInRole("admin"))
                model.showPostSection = false;

            if (replyPostId.HasValue)
            {
                // prevent user from replying to their own post
                if (ForumHelper.UserIsOwner(replyPostId.Value, AccHelper.GetUserId(User.Identity.Name)))
                    model.showPostSection = false;
            }

            // populate the model with the data
            if (model.showPostSection) 
            {
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
            }

            return PartialView("_CreatePost", model);
        }

        
        [Authorize]
        [AjaxAction]
        public PartialViewResult ShowEdit(int? postId, int page = 1, bool removeReply = false)
        {
            EditPostModel model = new EditPostModel();
            model.showEditSection = true;

 
            if (!User.Identity.IsAuthenticated // Prevent unauthenticated user from editting a post
                || !ForumHelper.ValidatePostId(postId) // prevent user from editting a post that does not exist
                || (!ForumHelper.UserIsOwner(postId.Value,AccHelper.GetUserId(User.Identity.Name)) && 
                     !User.IsInRole("admin"))) // prevent user from editting a post they did not posted unless they are an admin
                model.showEditSection = false;

            // do this after the post id is validated
            // prevent user from posting in a locked thread unless they are an admin
            if (ForumHelper.PostThreadIsLocked(postId.Value) && !User.IsInRole("admin"))
                model.showEditSection = false;

            if (model.showEditSection)
            {
                // find the post by the id
                Post post = db.Post.Find(postId.Value);
                model.postId = post.PostId;
                model.threadId = post.ThreadId;
                model.content = post.PostContent;
                model.page = page;

                // determine if there is a reply associated with the post
                if (post.ReplyPostContent != null && removeReply == false)
                {
                    model.hasReply = true;
                    model.replyContent = post.ReplyPostContent;
                    model.replyTo = post.ReplyTo;

                }
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

            // number of posts displayed per thread
            int displayPerPage = 10;
            // calculate the last page the newest post created would be in
            int pageToGo = (db.Post.Where(m => m.ThreadId == model.threadId).Count() + displayPerPage - 1) / displayPerPage;

            return RedirectToAction("viewthread", "forum", new { threadid = model.threadId, page = pageToGo, post = latestPost });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditPost(EditPostModel model)
        {
            // edit the post using the data posted
            ForumHelper.EditPost(model.postId, model.content, model.replyTo, model.replyContent);

            // reload the thread page
            return RedirectToAction("viewthread", "forum", new { threadid = model.threadId, page = model.page, post = model.postId, });
        }

        /// <summary>
        /// Flips the state of the thread to either locked or active
        /// </summary>
        /// <param name="threadId">the thread if to be searched</param>
        /// <returns>a string indicating the result</returns>
        public string LockThread(int? threadId)
        {
            // lock the thread if the user is an admin
            if (User.Identity.IsAuthenticated && User.IsInRole("admin") && threadId.HasValue)
                return ForumHelper.LockThread(threadId.Value);

            return "The thread status was not changed due to invalid authentication or thread id.";
            
        }
    }
}
