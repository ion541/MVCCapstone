using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Models;


namespace MVCCapstone.Helpers
{
    /// <summary>
    /// Helper class that contains functions / methods related to books
    /// </summary>
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
                            select new GenreList { GenreId = u.Genre_ID, Genre = u.Value }).OrderBy(u => u.Genre);

            List<GenreList> roleList = new List<GenreList>();

            foreach (GenreList role in genres)
            {
                roleList.Add(role);
            }

            return roleList;
        }

        /// <summary>
        /// Gets a list of genres related to the book id and return it in a formatted string
        /// </summary>
        /// <param name="bookId">the id of the book to be searched for</param>
        /// <returns></returns>
        public static string GetBookGenreList(string bookId)
        {
            UsersContext db = new UsersContext();

            // gets the list of genre id associated with the book id
            List<int> genreIds = (from bg in db.BookGenre
                                        where bg.BookId == bookId
                                        select bg.GenreId).Select(int.Parse).ToList();

            // get the list of strings associated with the genre id
            List<string> genreList = (from g in db.Genres
                                      where genreIds.Contains(g.Genre_ID)
                                      select g.Value).ToList();

            // convert the list of genres into a string and return it
            string genreString = "";
            if (genreList.Count() > 0)
            {
                genreString = genreList[0];
                for (int i = 1; i < genreList.Count(); i++)
                    genreString += ", " + genreList[i];
            }
            return genreString;
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


        /// <summary>
        /// Insert a record of the image into the database and return the id of that record
        /// </summary>
        /// <param name="filePath">the path of the file</param>
        /// <returns>the id of the image record inserted</returns>
        public static string InsertImageRecord(string filePath)
        {
            UsersContext db = new UsersContext();

            Image image = new Image();
            image.Path = "/" + filePath;
            image.DateAdded = DateTime.Now;

            db.Image.Add(image);
            db.SaveChanges();

            // select the most recent id of the image inserted
            return  db.Image.OrderByDescending(m => m.ImageId).Select(m => m.ImageId).First().ToString();

        }

        /// <summary>
        /// Returns the path of a default image
        /// </summary>
        /// <returns>string containing the path to an image</returns>
        public static string GetDefaultImage()
        {
            return "/Images/default_image.jpg";
        }


        /// <summary>
        /// Get the path an image based on the id
        /// </summary>
        /// <param name="bookImageId">the id of the image to be searched for</param>
        /// <param name="imageBasePath">The base path of the server</param>
        /// <returns>the path to the image or the default image if it doesnt exist</returns>
        public static string GetImagePath(string bookImageId, string imageBasePath)
        {
            UsersContext db = new UsersContext();

            string defaultImage = GetDefaultImage();

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
                if (File.Exists(imageBasePath + ImagePath.First()))
                {
                    return ImagePath.First();
                }
                else
                {
                    // since the image doesn't exist, remove the image Id's from the database
                    CorrectImageTables(bookImageId);    
                    return defaultImage;
                }
            }
        }

        /// <summary>
        /// Method is called when the imageId points to a file that does not exist
        /// Upon being called, this method will clear every fields / records associated with the imageId
        /// </summary>
        /// <param name="imageId">the image id</param>
        public static void CorrectImageTables(string imageId)
        {
            UsersContext db = new UsersContext();

            int intImageId = Int32.Parse(imageId);

            // find the image by id and remove it from the Image table
            Image image = db.Image.Find(intImageId);
            db.Image.Remove(image);

            List<int> bookIdList = (from b in db.Book
                                    where b.ImageId == imageId
                                    select b.BookId).ToList();

            if (bookIdList.Count > 0)
            {
                for (int i = 0; i < bookIdList.Count; i++)
                {
                    Book book = db.Book.Find(bookIdList[i]);
                    book.ImageId = null;
                }
            }

            db.SaveChanges();
            
        }

        /// <summary>
        /// Insert a record of the book into the database
        /// </summary>
        /// <param name="model">the model which contains the data for the book</param>
        /// <param name="language">the id of the language</param>
        /// <param name="imageId">the id of the image</param>
        /// <returns>true if the record was inserted otherwise false</returns>
        public static bool InsertBookRecord(BookManagementModel model, string language, string imageId)
        {
            // create a model of the Book and pass the data it over from the View Model
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

        /// <summary>
        /// Insert a record that contains the book id and genre id
        /// </summary>
        /// <param name="bookId">the id the book</param>
        /// <param name="genreId">the id of the gren</param>
        public static void InsertBookGenre(string bookId, string genreId)
        {
            UsersContext db = new UsersContext();

            // check to see if there is a duplicate
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

        /// <summary>
        /// Insert the default genre value for books that wernt assigned genres
        /// </summary>
        /// <param name="bookId">the id of the book associated with the genre</param>
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