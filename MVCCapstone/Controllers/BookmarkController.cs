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

        //
        // GET: /Bookmark/
        public ActionResult Index(string sort, int page = 1, bool asc = false)
        {
            IPagedList<BookmarkDisplayModel> model = BookHelper.GetBookMarkList(User.Identity.Name, page, sort, asc);
            return View(model);
        }

        
        public string Remove(string bookid)
        {
            return BookHelper.RemoveBookmark(User.Identity.Name, bookid);
        }

    }
}
