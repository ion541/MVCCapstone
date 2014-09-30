using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCCapstone.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index(ErrorMessages? Message)
        {
            
            ViewBag.Message =
                Message == ErrorMessages.ErrorPageNotFound ? "The page you are looking for does not exist"
                : Message == ErrorMessages.ErrorUnauthorized ? "You do not have the authorization to access this page"
                : Message == ErrorMessages.ErrorLockedAccount ? "Your account has been locked."
                : Message == ErrorMessages.ErrorIndirectAccess ? "You attempted to indirectly access a page."
                : "Error message does not exist.";

            return View();
        }


        public ActionResult PageNotFound()
        {

            return RedirectToAction("Index", "Error", new { Message = ErrorMessages.ErrorPageNotFound });
        }

        public ActionResult UnauthorizedAccess()
        {
            return RedirectToAction("Index", "Error", new { Message = ErrorMessages.ErrorUnauthorized });
        }

        public ActionResult LockedAccount()
        {
            return RedirectToAction("Index", "Error", new { Message = ErrorMessages.ErrorLockedAccount });
        }

        public ActionResult IndirectAccess()
        {
            return RedirectToAction("Index", "Error", new { Message = ErrorMessages.ErrorIndirectAccess });
        }


        public enum ErrorMessages
        {
            ErrorPageNotFound,
            ErrorUnauthorized,
            ErrorLockedAccount,
            ErrorIndirectAccess
        }

    }
}
