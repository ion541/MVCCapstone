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
        public ActionResult Index(ErrorMessages? Message)
        {
            
            ViewBag.Message =
                Message == ErrorMessages.ErrorPageNotFound ? "The page you are looking for does not exist"
                : Message == ErrorMessages.ErrorUnauthorized ? "You do not have the authorization to access this page"
                : Message == ErrorMessages.ErrorLockedAccount ? "Your account has been locked."
                : Message == ErrorMessages.ErrorIndirectAccess ? "You are forbidden to access this page through the url."
                : Message == ErrorMessages.BookIdInvalid ? "The book you are attempting to find does not exist."
                : Message == ErrorMessages.BookDeleted ? "The book is deleted / You do not have permission to view the book."
                : Message == ErrorMessages.General ? "An error has occurred. Please contact the administrator with steps on how to reproduce the problem at anhvu.ho@mohawkcollege.ca"
                : "Uh... you are here because you have probably have been tampering with the url";

            return View();
        }

        /// <summary>
        /// Each error has its own controller allowing the developer to handle each error differently if required
        /// </summary>
        #region ErrorHandlers
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

        public ActionResult NotValidBookId()
        {
            return RedirectToAction("Index", "Error", new { Message = ErrorMessages.BookIdInvalid });
        }

        public ActionResult BookDeleted()
        {
            return RedirectToAction("Index", "Error", new { Message = ErrorMessages.BookDeleted });
        }

        public ActionResult General()
        {
            return RedirectToAction("Index", "Error", new { Message = ErrorMessages.General });
        }

        #endregion

        /// <summary>
        /// Enumeration for error messages
        /// </summary>
        public enum ErrorMessages
        {
            ErrorPageNotFound,
            ErrorUnauthorized,
            ErrorLockedAccount,
            ErrorIndirectAccess,
            BookIdInvalid,
            BookDeleted,
            General,
            UploadImage
        }

    }
}
