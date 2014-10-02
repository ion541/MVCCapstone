using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Models;


namespace MVCCapstone.Helpers
{
    public class BookHelper
    {
        /// <summary>
        /// returns a string that contains the path to where the uploaded book images are stored
        /// </summary>
        /// <returns>string of path to where book images are kept</returns>
        public static string GetServerPath()
        {
            return "Images/Book/";
        }

        /// <summary>
        /// Checks to see if the inputted isbn matches an isbn in the database
        /// </summary>
        /// <param name="isbn">the isbn to be searched for</param>
        /// <returns>true if a record exist with the inputted isbn otherwise false</returns>
        public static bool CheckISBN(string isbn)
        {
            UsersContext db = new UsersContext();

            int count = (from b in db.Book
                         where b.ISBN == isbn
                         select b).Count();

            if (count >= 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Get the value for the specified language id
        /// </summary>
        /// <param name="languageID">the id of the language</param>
        /// <returns>the value associated with the id</returns>
        public static string GetLanguage(string languageID)
        {
            UsersContext db = new UsersContext();
            int intLanguageId = Int32.Parse(languageID);

            string language = (from l in db.Languages
                               where l.Language_ID == intLanguageId
                               select l.Value).First();
            return language;


        }


        /// <summary>
        /// Get a list of the class GenreList that contains the genre id and table and return it
        /// </summary>
        /// <returns>A list of the GenreList class</returns>
        public static List<GenreList> GetGenreList() 
        {
            UsersContext db = new UsersContext();

            var genres = (from u in db.Genres
                            where u.Value != "Unset"
                            select new GenreList { GenreId = u.Genre_ID, Genre = u.Value }).OrderBy(u => u.GenreId);

            List<GenreList> roleList = new List<GenreList>();

            foreach (GenreList role in genres)
            {
                roleList.Add(role);
            }

            return roleList;
        }

        public static List<string> GetBookGenreList(string bookId)
        {
            UsersContext db = new UsersContext();

            List<string> genreIds = (from bg in db.BookGenre
                                        where bg.BookId == bookId
                                        select bg.GenreId).ToList();

            List<int> genreConvert = genreIds.Select(int.Parse).ToList();

            List<string> genreList = (from g in db.Genres
                                      where genreConvert.Contains(g.Genre_ID)
                                      select g.Value).ToList();

            return genreList;
        }

        /// <summary>
        /// Create a new Forum ID and return it
        /// </summary>
        /// <returns>the most recent id added to the forum table</returns>
        public static int CreateNewForumId()
        {
            UsersContext db = new UsersContext();

            // create a new record in the forum table which will then create a new forum id
            Forum forum = new Forum();
            db.Forum.Add(forum);
            db.SaveChanges();

            // select the mots recent forum id created
            int newForumID = db.Forum.OrderByDescending(m => m.ForumId).Select(m => m.ForumId).First();

            return newForumID;
        }

        /// <summary>
        /// Checks to see if the inputted Forum ID is in the database
        /// </summary>
        /// <param name="forumId">the forum id to be searched for</param>
        /// <returns>true if it does exist otherwise false</returns>
        public static bool CheckForForumIdExistence(int forumId)
        {

            UsersContext db = new UsersContext();

            int count = (from f in db.Forum
                         where f.ForumId == forumId
                         select f.ForumId).Count();
            return count == 1 ? true : false;
        }


        public static string InsertImageRecord(string filePath)
        {
            UsersContext db = new UsersContext();

            Image image = new Image();
            image.Path = filePath;
            image.DateAdded = DateTime.Now;

            db.Image.Add(image);
            db.SaveChanges();

            string imageId = db.Image.OrderByDescending(m => m.ImageId).Select(m => m.ImageId).First().ToString();
            return imageId;

        }

        public static string GetDefaultImage()
        {
            return "Images/default_image.jpg";
        }


        public static string GetImagePath(string bookImageId)
        {
            UsersContext db = new UsersContext();

            string defaultImage = "Images/default_image.jpg";

            if (bookImageId == null)
            {
                return defaultImage;
            }
            
            int intImageId = Int32.Parse(bookImageId);

            var ImagePath = (from i in db.Image
                             where i.ImageId == intImageId
                             select i.Path);

            if (ImagePath.Count() == 0)
            {
                return defaultImage;
            }
            else
            {
                return ImagePath.First();
            }

       
        }


        public static bool InsertBookRecord(BookManagementModel model, string language, string imageId)
        {
            var Book = new Book();


            Book.Title = model.BookTitle;
            Book.Author = model.Author;
            Book.LanguageId = language;
            Book.ImageId = imageId;
            Book.Published = model.Published;
            Book.Publisher = model.Publisher;
            Book.ISBN = model.ISBN;
            Book.ForumId = model.ForumId;

            UsersContext db = new UsersContext();
            db.Book.Add(Book);
            int recordInserted = db.SaveChanges();

            if (recordInserted == 1)
            {
                return true;
            } else 
            { 
                return false; 
            }
        }

        /// <summary>
        /// Gets the most recent book id added to the table
        /// </summary>
        /// <returns>book id</returns>
        public static int GetLastBookId()
        {
            UsersContext db = new UsersContext();
            int latestBookId = db.Book.OrderByDescending(b => b.BookId).Select(b => b.BookId).First();

            return latestBookId;

        }

        public static void InsertBookGenre(string bookId, string genreId)
        {
            UsersContext db = new UsersContext();

            int count = (from bg in db.BookGenre
                         where bg.BookId == bookId 
                            && bg.GenreId == genreId
                         select bg).Count();

            if (count == 0)
            {
                BookGenre bookGenre = new BookGenre();
                bookGenre.BookId = bookId;
                bookGenre.GenreId = genreId;

                db.BookGenre.Add(bookGenre);
                db.SaveChanges();
            }
        }

        public static void InsertBookGenreDefault(string bookId)
        {
            UsersContext db = new UsersContext();
            string defaultId = (from g in db.Genres
                                where g.Value == "Unset"
                                select g.Genre_ID).First().ToString();

            BookGenre bookGenre = new BookGenre();
            bookGenre.BookId = bookId;
            bookGenre.GenreId = defaultId;

            db.BookGenre.Add(bookGenre);
            db.SaveChanges();
        }
                  
    }
}