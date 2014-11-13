using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MVCCapstone.Filters;
using WebMatrix.WebData;
using MVCCapstone.Models;
using MVCCapstone.Helpers;
using PagedList;

namespace MVCCapstone.Controllers
{
    [ValidateInput(false)]
    [Authorize]
    public class BookmarkController : Controller
    {
        public UsersContext db = new UsersContext();

        /// <summary>
        /// GET: get the users bookmark
        /// </summary>
        /// <param name="sort">the column to be sorted</param>
        /// <param name="page">the page to be displayed</param>
        /// <param name="asc">the direction of the osrt</param>
        public ActionResult Index(string sort, int page = 1, bool asc = false)
        {
            IPagedList<BookmarkDisplayModel> model = BookHelper.GetBookMarkList(User.Identity.Name, page, sort, asc);
            return View(model);
        }

        
        /// <summary>
        /// Removes the selected bookmark
        /// </summary>
        /// <param name="bookid">The id of the book to be removed from the bookmark</param>
        public string Remove(string bookid)
        {
            return BookHelper.RemoveBookmark(User.Identity.Name, bookid);
        }

    }
}
