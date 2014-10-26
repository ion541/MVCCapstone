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
        /// Check the database to see if the book exists
        /// </summary>
        /// <param name="bookid">the id of the book to be searched</param>
        /// <returns>boolean indicating whether it exists or not</returns>
        public static bool BookExists(int bookid)
        {
            UsersContext db = new UsersContext();
            if (db.Book.Find(bookid) != null)
                return true;

            return false;

        }



        /// <summary>
        /// Gets the title of the book
        /// </summary>
        /// <param name="bookid">the id of the book to be searched</param>
        /// <returns>the title of the book</returns>
        public static string GetTitle(int bookid)
        {
            UsersContext db = new UsersContext();
            return db.Book.Where(m => m.BookId == bookid).Select(m => m.Title).FirstOrDefault();
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
                               select l.Value).First( );
            return language;


        }

        /// <summary>
        /// Gets the state of the book
        /// </summary>
        /// <param name="bookid">the id of the book to be searched</param>
        /// <returns>the state of the book</returns>
        public static string GetState(int bookid)
        {
            UsersContext db = new UsersContext();
            return db.Book.Where(m => m.BookId == bookid).Select(m => m.State).FirstOrDefault();
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
                roleList.Add(role);


            return roleList;
        }

        /// <summary>
        /// Gets a list of genres related to the book id and return it in a formatted string
        /// </summary>
        /// <param name="bookId">the id of the book to be searched for</param>
        /// <returns></returns>
        public static string GetBookGenreList(int bookId)
        {
            UsersContext db = new UsersContext();

            // gets the list of genre id associated with the book id
            List<int> genreIds = (from bg in db.BookGenre
                                        where bg.BookId == bookId
                                        select bg.GenreId).ToList();

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
        public static int CreateNewForumId(string title = null)
        {
            UsersContext db = new UsersContext();

            // create a new record in the forum table which will then create a new forum id
            Forum forum = new Forum();
            if (title != null)
                forum.SeriesTitle = title;
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
        /// <returns>true if the record was inserted otherwise false</returns>
        public static bool InsertBookRecord(BookManagementModel model, string language)
        {
            // create a model of the Book and pass the data it over from the View Model
            var Book = new Book();
            Book.Title = model.BookTitle;
            Book.Author = model.Author;
            Book.LanguageId = language;
            Book.ImageId = model.Image;
            Book.Publisher = model.Publisher;

            // split the date string and use it to create a datetime object
            string[] dateString = model.Published.Split('/');
            int[] date = Array.ConvertAll<string, int>(dateString, int.Parse);
            DateTime published = new DateTime(date[2], date[1], date[0]);
            Book.Published = published;

            Book.ISBN = model.ISBN;
            Book.Synopsis = model.Synopsis;
            Book.State = "Visible"; // default, should be visible to other users
            Book.ForumId = Int32.Parse(model.ForumId);

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
        public static void InsertBookGenre(int bookId, int genreId)
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
        public static void InsertBookGenreDefault(int bookId)
        {
            UsersContext db = new UsersContext();
            int defaultId = (from g in db.Genres
                             where g.Value == "Unset"
                             select g.Genre_ID).First();

            BookGenre bookGenre = new BookGenre();
            bookGenre.BookId = bookId;
            bookGenre.GenreId = defaultId;

            db.BookGenre.Add(bookGenre);
            db.SaveChanges();
        }


        /// <summary>
        /// Switches the current state of the book to the other (Visible / Hidden)
        /// </summary>
        /// <param name="bookId">the id of the book to be switched</param>
        /// <returns>a string indicating its current status after the state switch</returns>
        public static string ChangeState(string bookId)
        {
          
            int idBook;
            if (!Int32.TryParse(bookId, out idBook))
                return "The book id is not valid";

            UsersContext db = new UsersContext();
   
            Book book = db.Book.Find(idBook);
            if (book == null)
                return "The book was not found.";

            if (book.State == "Visible")
            {
                book.State = "Hidden";
            }
            else
            {
                book.State = "Visible";
            }
            db.SaveChanges();

            return book.State;
        }

        /// <summary>
        /// Adds a record into the bookmark table that contains the userid, bookid, and time added
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="bookId">the book id</param>
        /// <returns>a message that informs the user of the bookmarking </returns>
        public static string Bookmark(string username, string bookId)
        {

            int idBook;
            if (!Int32.TryParse(bookId, out idBook))
                return "Book id is is invalid.";


            int idUser = AccHelper.GetUserId(username);
            if (idUser == -1)
                return "Unable to find user in database.";

            UsersContext db = new UsersContext();

            if (db.Book.Find(idBook) == null)
                return "This book does not exist";

            if (db.Bookmark.Where(m => m.BookId == idBook && m.UserId == idUser).Count() > 0)
                return "Already bookmarked";

            Bookmark bookmark = new Bookmark();
            bookmark.UserId = idUser;
            bookmark.BookId = idBook;
            bookmark.DateAdded = DateTime.Today;
            db.Bookmark.Add(bookmark);
            db.SaveChanges();

            return "Successfully bookmarked";
        }

        public static string RemoveBookmark(string username, string bookid)
        {
            int idBook;
            if (!Int32.TryParse(bookid, out idBook))
                return "Book is is invalid.";


            int idUser = AccHelper.GetUserId(username);
            if (idUser == -1)
                return "Unable to find user in database.";

            UsersContext db = new UsersContext();
            Bookmark bookmark = db.Bookmark.Where(m => m.BookId == idBook && m.UserId == idUser).FirstOrDefault();
            if (bookmark == null)
                return "The bookmark you are attempting to remove does not exist.";

            db.Bookmark.Remove(bookmark);
            db.SaveChanges();

            return "The bookmark was successfully removed.";
        }

        public static IPagedList<BookmarkDisplayModel> GetBookMarkList(string username, int page, string sortby, bool ascend)
        {
            // page cannot be negative
            if (page <= 0) page = 1;

            UsersContext db = new UsersContext();

            var bookmarkList = (from d in db.UserProfiles
                                join bm in db.Bookmark on d.UserId equals bm.UserId
                                join b in db.Book on bm.BookId equals b.BookId
                                where d.UserName.Contains(username)
                                select new BookmarkDisplayModel { BookId = b.BookId, Author = b.Author, Title = b.Title, DateAdded = bm.DateAdded });

            switch (sortby)
            {
                case "title":
                    bookmarkList = ((ascend) ? bookmarkList.OrderBy(u => u.Title) : bookmarkList.OrderByDescending(u => u.Title));
                    break;
                case "date":
                default:
                    bookmarkList = ((ascend) ? bookmarkList.OrderBy(u => u.DateAdded) : bookmarkList.OrderByDescending(u => u.DateAdded));
                    break;
            }
    

            return bookmarkList.ToPagedList(page, 20) as IPagedList<BookmarkDisplayModel>;
        }

        /// <summary>
        /// Returns a list of books where any new books discovered in the query list will be aded to the current list
        /// </summary>
        /// <param name="currentList">the list of books to be added to</param>
        /// <param name="queryList">the list of new books to be added to the current list</param>
        /// <param name="firstBooksAdded">boolean used to determine if books have been added to the current list</param>
        /// <returns>a list of unique books containing both the query and current list</returns>
        public static List<Book> AddAllNewBooks(List<Book> currentList, List<Book> queryList, out bool currentListBooksNotEmpty)
        {
            currentListBooksNotEmpty = true;

            foreach (Book book in queryList)
            {
                if (!currentList.Contains(book))
                    currentList.Add(book);
            }

            return currentList;
        }

        /// <summary>
        /// Compares both the query list and current list and return a new list of books that contains books in both list
        /// 
        /// If this is the first time books are being added to the current list and therefore nothing to compare / match,
        /// this method will call and return the list from the AddAllBooks method
        /// </summary>
        /// <param name="currentList">the list of books to be checked</param>
        /// <param name="queryList">the list of books to be matched</param>
        /// <param name="currentListEmpty">boolean used to determine if books have been added to the current list</param>
        /// <param name="currentListBooksNotEmpty"></param>
        /// <returns>a list of books that only contains the books in both list or the query list of books if the current list is empty </returns>
        public static List<Book> ReturnSameBooks(List<Book> currentList, List<Book> queryList, bool currentListEmpty, out bool currentListBooksNotEmpty)
        {
            currentListBooksNotEmpty = true;

            if (currentListEmpty == false)
                return AddAllNewBooks(currentList, queryList, out currentListEmpty);

            List<Book> newList = new List<Book>();
            
            foreach (Book book in queryList)
            {
                if (currentList.Contains(book))
                    newList.Add(book);
            }

            return newList;
        }

    }
}