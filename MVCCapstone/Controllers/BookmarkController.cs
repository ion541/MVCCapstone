using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MVCCapstone.Filters;
using WebMatrix.WebData;

namespace MVCCapstone.Controllers
{
    //
    // PUT IN CODE FOR AUTHORIZE
    //
    public class BookmarkController : Controller
    {
        //
        // GET: /Bookmark/

        public ActionResult Index()
        {
            return View();
        }

    }
}
