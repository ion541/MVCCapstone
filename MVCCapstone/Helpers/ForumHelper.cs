using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Models;
using PagedList;

namespace MVCCapstone.Helpers
{
    public class ForumHelper
    {
        public static int GetForumId(int bookid)
        {
            UsersContext db = new UsersContext();

            if (db.Book.Where(m => m.BookId == bookid).Count() == 1)
                return db.Book.Where(m => m.BookId == bookid).Select(m => m.ForumId).First();

            return -1;
        }

        public static bool IsSeries(int forumid)
        {
            UsersContext db = new UsersContext();
            if (db.Book.Where(m => m.ForumId == forumid).Count() > 1)
                return true;
            return false;

        }

        public static List<SharedWith> SharedWith(int forumid)
        {
            UsersContext db = new UsersContext();
            List<SharedWith> bookList = db.Book.Where(m => m.ForumId == forumid).OrderBy(m => m.Title).Select(m => new SharedWith {bookid = m.BookId, title = m.Title}).ToList();

            return bookList;
        }

        public static List<Thread> GetThreadList(int forumid)
        {
            UsersContext db = new UsersContext();
            var threads = (from f in db.Forum
                           join b in db.Book on f.ForumId equals b.ForumId
                           join t in db.Thread on b.BookId equals t.BookId
                           select t).OrderBy(t => t.LatestPost).ToList();
            return threads;
        }
    }
}