using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCCapstone.Controllers
{
    [ValidateInput(false)]
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            
            ViewBag.Message = "Uh... you are here because you have probably have been tampering with the url";

            return View();
        }

        /// <summary>
        /// Each error has its own controller allowing the developer to handle each error differently if required
        /// </summary>
        #region ErrorHandlers
        public ActionResult PageNotFound()
        {
            ViewBag.Message = "The page you are looking for does not exist";
            return View("Index");
        }

        public ActionResult UnauthorizedAccess()
        {
            ViewBag.Message = "You do not have the authorization to access this page";
            return View("Index");
        }

        public ActionResult LockedAccount()
        {
            ViewBag.Message = "Your account has been locked";
            return View("Index");
        }

        public ActionResult IndirectAccess()
        {
            ViewBag.Message = "You are forbidden to access this page through the url";
            return View("Index");
        }

        public ActionResult NotValidBookId()
        {
            ViewBag.Message = "The book you are attempting to find does not exist";
            return View("Index");
        }

        public ActionResult BookDeleted()
        {
            ViewBag.Message = "The page you are looking for does not exist";
            return View("Index");
        }

        public ActionResult General()
        {
            ViewBag.Message = "An error has occurred. Please contact the administrator with steps on how to reproduce the problem at anhvu.ho@mohawkcollege.ca";
            return View("Index");
        }

        #endregion


    }
}
