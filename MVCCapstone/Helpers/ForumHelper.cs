using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Models;
using PagedList;

namespace MVCCapstone.Helpers
{
    // helper class that contains methods / functions related to the forum
    public class ForumHelper
    {

        public static int DeleteForum(int forumId)
        {
            UsersContext db = new UsersContext();

            Forum forum = db.Forum.Find(forumId);

            if (forum == null) return 0;

            db.Forum.Remove(forum);

            List<Thread> threadList = db.Thread.Where(m => m.ForumId == forumId).ToList();
            foreach (Thread thread in threadList)
                db.Thread.Remove(thread);

            List<int> threadIds = threadList.Select(m => m.ThreadId).ToList();

            List<Post> postList = db.Post.Where(m => threadIds.Contains(m.ThreadId)).ToList();
            foreach (Post post in postList)
                db.Post.Remove(post);

            return db.SaveChanges();
                
            
        }


        /// <summary>
        /// Get the forum id associated with the thread.
        /// Returns -1 if the forum id does not exist
        /// </summary>
        /// <param name="threadId">the id of the threadid to be searched</param>
        /// <returns>the forum id of the book</returns>
        public static int GetForumId(int threadId)
        {
            UsersContext db = new UsersContext();

            if (db.Thread.Where(m => m.ThreadId == threadId).Count() == 1)
                return db.Thread.Where(m => m.ThreadId == threadId).Select(m => m.ForumId).First();

            return -1;
        }


        /// <summary>
        /// Checks to see if the forum id exists and is a valid value
        /// Returns a boolean to indicate if it passes the criteria
        /// </summary>
        /// <param name="forumid">the forum id to be checked</param>
        /// <returns>true if it passes the criterias</returns>
        public static bool ValidateForumId(int? forumId)
        {
            UsersContext db = new UsersContext();
            // check to see if it has a value or is of a different data type
            if (!forumId.HasValue)
                return false;

            // see if the forum id exist
            Forum forumid = db.Forum.Find(forumId);
            if (forumId == null)
                return false;

            return true;
        }

        /// <summary>
        /// Returns a boolean to indicate if the thread is is valid
        /// </summary>
        /// <param name="threadId">the id of the thread to be validated</param>
        /// <returns>a boolean</returns>
        public static bool ValidateThreadId(int? threadId)
        {
            if (!threadId.HasValue)
                return false;

            UsersContext db = new UsersContext();

            if (db.Thread.Find(threadId) == null)
                return false;

            return true;
        }

        /// <summary>
        /// Returns a boolean to indicate if the post id is valid
        /// </summary>
        /// <param name="postId">the id of the post to be validated</param>
        /// <returns>a boolean</returns>
        public static bool ValidatePostId(int? postId)
        {
            if (!postId.HasValue)
                return false;

            UsersContext db = new UsersContext();

            if (db.Post.Find(postId) == null)
                return false;

            return true;
        }


        /// <summary>
        /// Identify whether the forum id is a series or belongs to a standalone book and return
        /// the appropriate title for the string
        /// </summary>
        /// <param name="forumId">the forum id to be searched</param>
        /// <returns>a string containing the book's title / series title</returns>
        public static string GetForumTitle(int forumId)
        {
            UsersContext db = new UsersContext();
            Forum forum = db.Forum.Find(forumId);
            if (forum == null)
                return "ERROR - FORUM ID NOT FOUND";

            if (forum.SeriesTitle != null)
                return forum.SeriesTitle;

            if (db.Book.Where(m => m.ForumId == forumId).Count() == 1)
                return db.Book.Where(m => m.ForumId == forumId).Select(m => m.Title).First();

            return "ERROR - MULTIPLE STANDALONE BOOK TITLES FOUND";
             
        }

        public static string GetLockActionString(int threadId)
        {
            UsersContext db = new UsersContext();
            Thread thread = db.Thread.Find(threadId);

            return (thread.State == "Locked") ? "Unlock Thread" : "Lock Thread";

        }

        /// <summary>
        /// Get the title of the thread
        /// </summary>
        /// <param name="threadId">the id of the thread to be searched</param>
        /// <returns>string containing title of thread</returns>
        public static string GetThreadTitle(int threadId)
        {
            UsersContext db = new UsersContext();
            return db.Thread.Where(m => m.ThreadId == threadId).Select(m => m.Title).FirstOrDefault();
          
        }

        /// <summary>
        /// Determines if the the book's forum id is being shared
        /// </summary>
        /// <param name="forumid">the forum id of the book to be searched</param>
        /// <returns>boolean indicating whether multiple books shares the forum id</returns>
        public static bool IsSeries(int forumId)
        {
            UsersContext db = new UsersContext();
            if (db.Book.Where(m => m.ForumId == forumId).Count() > 1)
                return true;
            return false;

        }

        /// <summary>
        /// Get every title and bookid as an object that shares the forum id
        /// </summary>
        /// <param name="forumid">the forum id to be searched</param>
        /// <param name="bookId">the book id to be excluded</param>
        /// <returns>a list of objects containing the id of a book and its title</returns>
        public static List<SharedWith> SharedWith(int forumId, bool isAdmin)
        {
            UsersContext db = new UsersContext();
            List<SharedWith> bookList;

            // only admins can see hidden books
            if (isAdmin)
            {
                bookList = db.Book.Where(m => m.ForumId == forumId)
                    .OrderBy(m => m.Title)
                    .Select(m => new SharedWith { bookid = m.BookId, title = m.Title }).ToList();
            }
            else
            {
                bookList = db.Book.Where(m => m.ForumId == forumId && m.State == "Visible")
                    .OrderBy(m => m.Title)
                    .Select(m => new SharedWith { bookid = m.BookId, title = m.Title }).ToList();
            }

            return bookList;
        }

        /// <summary>
        /// Gets the list of posts that is associated with the thread id
        /// </summary>
        /// <param name="threadId">the id used to find post associated with it</param>
        /// <param name="page">the page number to display</param>
        /// <returns>a pagination object of posts</returns>
        public static IPagedList<PostViewModel> GetPostList(int threadId, int page)
        {
            UsersContext db = new UsersContext();

            int displayPerPage = 10;

            //  page cannot be negative
            if (page <= 0) page = 1;


            IPagedList<PostViewModel> postList = (from p in db.Post
                            join t in db.Thread on p.ThreadId equals t.ThreadId
                            join u in db.UserProfiles on p.UserId equals u.UserId
                            join m in db.Membership on u.UserId equals m.UserId
                            where p.ThreadId == threadId
                            select new PostViewModel
                            {
                                postId = p.PostId,
                                userName = u.UserName,
                                memberSince = m.CreateDate,
                                totalPost = db.Post.Where(x => x.UserId == u.UserId).Count(),
                                postContent = p.PostContent,
                                replyTo = p.ReplyTo,
                                replyPostContent = p.ReplyPostContent,
                                datePosted = p.PostDate,
                                dateModified = p.ModifiedDate
                            }).OrderBy(m => m.datePosted).ToPagedList(page, displayPerPage) as IPagedList<PostViewModel>;
            
            return postList;
        }


        /// <summary>
        /// Get every thread that is being shared with the forum id
        /// </summary>
        /// <param name="forumid">the forum id of the book to be searched</param>
        /// <param name="page">The page of thread to be displayed</param>
        /// <returns>list of threads sharing the forum id</returns>
        public static IPagedList<ThreadModel> GetThreadList(int forumId, int page)
        {
            int threadToDisplay = 20; // display 20 threads per page

            // page viewed cannot be negative
            if (page <= 0) page = 1;

            UsersContext db = new UsersContext();
            var threads = (from t in db.Thread
                            join f in db.Forum on t.ForumId equals f.ForumId
                            where t.ForumId == forumId
                            select new ThreadModel
                            {
                                ThreadId = t.ThreadId,
                                ForumId = f.ForumId,
                                Title = t.Title,
                                ThreadCreated = t.ThreadCreated,
                                ThreadCreator = t.ThreadCreator,
                                LatestPost = t.LatestPost,
                                LatestPoster = t.LatestPoster,
                                State = t.State,
                                TotalPost = (db.Post.Where(m => m.ThreadId == t.ThreadId).Count()),
                                TotalView = t.TotalView
                                }).OrderByDescending(t => t.LatestPost).ToList();

            // format the date of the string to make it more easily understandable to viewers
            foreach (ThreadModel thread in threads)
                thread.DateString = FormatForumDate(thread.LatestPost);
            
 
            return threads.ToPagedList(page, threadToDisplay) as IPagedList<ThreadModel>;
        }


        /// <summary>
        /// Returns a string indicating if the date was today, yesterday
        /// </summary>
        /// <param name="date">the date to be checked</param>
        /// <returns>a string representing the day</returns>
        public static string FormatForumDate(DateTime date)
        {
            if (date.DayOfYear == DateTime.Today.DayOfYear)
                return "Today";
            if (date.DayOfYear == DateTime.Today.AddDays(-1).DayOfYear)
                return "Yesterday";
            return date.ToShortDateString();
        }

        /// <summary>
        /// Create a thread based off of the user input.
        /// </summary>
        /// <param name="userId">the user id who created the thread</param>
        /// <param name="forumId">the forum id the thread will be associated with</param>
        /// <param name="threadTitle">the title of the thread the user inputted</param>
        /// <param name="post">the first post of the thread by the user</param>
        public static void CreateThread(int userId, int forumId, string threadTitle, string post)
        {
            UsersContext db = new UsersContext();

            Thread thread = new Thread();
            thread.Title = threadTitle;
            thread.ForumId = forumId;
            thread.ThreadCreator = AccHelper.GetUserName(userId);
            thread.ThreadCreated = DateTime.Now;
            thread.State = "Active";    // default state, allows other user to post to it
            thread.TotalView = 0;
            db.Thread.Add(thread);
            db.SaveChanges();

            int threadId = db.Thread.OrderByDescending(m => m.ThreadId).Select(m => m.ThreadId).FirstOrDefault();
            CreatePost(userId, threadId, post);
        }

        public static void EditPost(int postId, string content, string replyTo = null, string replyContent = null)
        {
            UsersContext db = new UsersContext();
            Post post = db.Post.Find(postId);
            post.PostContent = content;
            post.ReplyTo = replyTo;
            post.ReplyPostContent = replyContent;
            post.ModifiedDate = DateTime.Today.ToShortDateString() + " - " + DateTime.Now.ToLongTimeString();
            db.SaveChanges();
        }

       /// <summary>
       /// Creates a post
       /// </summary>
       /// <param name="userId">the user id who created the post</param>
       /// <param name="threadId">the thread id the user posted to</param>
       /// <param name="post">the content of the post</param>
        public static void CreatePost(int userId, int threadId, string post, string replyContent = null, string replyTo = null, int replyPostId = 0)
        {
            UsersContext db = new UsersContext();

            if (replyTo != null)
                replyTo = replyTo + " post #" + replyPostId;

            Post userPost = new Post();
            userPost.UserId = userId;
            userPost.ThreadId = threadId;
            userPost.ReplyTo = replyTo;
            userPost.ReplyPostContent = replyContent;
            userPost.PostContent = post;
            userPost.PostDate = DateTime.Now;
            userPost.ModifiedDate = null;
            db.Post.Add(userPost);
            db.SaveChanges();

            UpdateThreadInfo(threadId, userId); // updates the thread ifnromation
        }

        /// <summary>
        /// Updates the thread to show the most recent post and date posted
        /// </summary>
        /// <param name="threadId">the id of the thread to be updated</param>
        /// <param name="userId">the id of the user who posted to the thread</param>
        public static void UpdateThreadInfo(int threadId, int userId)
        {
            UsersContext db = new UsersContext();

            Thread thread = db.Thread.Find(threadId);
            thread.LatestPost = DateTime.Now;
            thread.LatestPoster = AccHelper.GetUserName(userId);
            db.SaveChanges();
        }

        /// <summary>
        /// Increments the total view of the thread
        /// </summary>
        /// <param name="threadId">the id of the thread</param>
        public static void IncrementThreadViewCount(int threadId)
        {
            UsersContext db = new UsersContext();
            Thread thread = db.Thread.Find(threadId);
            thread.TotalView += 1;
            db.SaveChanges();

        }

        /// <summary>
        /// Checks to see if there exist a post with the post id
        /// </summary>
        /// <param name="postId">the id of the post to be searced</param>
        /// <returns>boolean indicating whether the post exists or not</returns>
        public static bool PostIdExist(int postId)
        {
            UsersContext db = new UsersContext();
            if (db.Post.Find(postId) == null)
                return false;

            return true;
        }
        /// <summary>
        /// Finds the post by its id and return the content of that post
        /// </summary>
        /// <param name="postId">the id of the post to be searched</param>
        /// <returns>string containing the post content</returns>
        public static string GetPostContent(int postId)
        {
            UsersContext db = new UsersContext();
            Post post = db.Post.Find(postId);

            if (post == null) return "Error, unable to find post";

            return post.PostContent;

        }

        /// <summary>
        /// Get the username of the associated with the post id
        /// </summary>
        /// <param name="postId">the post id to be searched</param>
        /// <returns>a string containing the username associated with the post id</returns>
        public static string GetPostUserName(int postId)
        {
            UsersContext db = new UsersContext();
            Post post = db.Post.Find(postId);
            if (post == null) return "Error, unable to find username";

            return  AccHelper.GetUserName(post.UserId);

        }

        /// <summary>
        /// Determine if there exists a post that is owned by the user id
        /// </summary>
        /// <param name="postId">the post id to be searched</param>
        /// <param name="userId">the user id to be matched to the post</param>
        /// <returns>a boolean</returns>
        public static bool UserIsOwner(int postId, int userId)
        {
            UsersContext db = new UsersContext();
            if (db.Post.Where(m => m.PostId == postId && m.UserId == userId).Count() == 1)
                return true;
            return false;
        }


        /// <summary>
        /// Flips the current state of the thread to active or locked depending on what it is at the time
        /// </summary>
        /// <param name="threadId">the thread id to be searched</param>
        /// <returns>a string caontaining the result of this method</returns>
        public static string LockThread(int threadId)
        {
            UsersContext db = new UsersContext();
            Thread thread = db.Thread.Find(threadId);

            if (thread == null)
                return "Invalid thread id";

            thread.State = (thread.State == "Active") ? "Locked" : "Active";
            db.SaveChanges();
          
            string message = "The status of the thread has been changed to " + thread.State;
            return (thread.State == "Active") ? message += ". Users will be able to post in this thread." :
                message += ". Only admins will be able to post in this thread.";

        }

        /// <summary>
        /// Return a boolean indicating whether the thread is locked
        /// </summary>
        /// <param name="threadId">the thread id to be searched</param>
        /// <returns>a boolean</returns>
        public static bool ThreadIsLocked(int threadId)
        {
            UsersContext db = new UsersContext();
            Thread thread = db.Thread.Find(threadId);

            return (thread.State == "Locked") ? true : false;
        }

        /// <summary>
        /// Finds the thread associated with the post id and return the a boolean
        /// indicating if the thread is locked
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public static bool PostThreadIsLocked(int postId)
        {
            UsersContext db = new UsersContext();
            Post post = db.Post.Find(postId);
            Thread thread = db.Thread.Find(post.ThreadId);

            return (thread.State == "Locked") ? true : false;
        }
    }
}