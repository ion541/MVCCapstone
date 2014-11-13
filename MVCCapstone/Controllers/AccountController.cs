using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using MVCCapstone.Filters;
using MVCCapstone.Models;
using MVCCapstone.Helpers;

namespace MVCCapstone.Controllers
{
    [ValidateInput(false)]
    [Authorize]
    public class AccountController : Controller
    {

        private UsersContext db = new UsersContext();

        /// <summary>
        ///  Get the list of questions from the Question table and store it in the ViewBag
        /// </summary>
        private void PopulateQuestionList()
        {
            var questionList = from d in db.Questions
                               select d;

            // qList to be used in the View
            ViewBag.qList = new SelectList(questionList, "Question_ID", "Value");
        }



        /// <summary>
        /// GET: Index page of account management, display the options available
        /// </summary>
        public ActionResult Index()
        { 
            return RedirectToAction("Manage", "Account");
        }




        /// <summary>
        /// GET: Login page 
        /// </summary>
        /// <param name="returnUrl">page to be redirected to</param>
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }



        /// <summary>
        /// POST: Attempt to log the user in
        /// Accounts that are locked will be logged off and redirected to an error page
        /// </summary>
        /// <param name="model">the login model</param>
        /// <param name="returnUrl">url to be redirected to upon succesffuly logging on</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
         
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                // if the user account is locked, logout and redirect the user to an error page
                if (Roles.IsUserInRole(model.UserName, "locked")) {
                    WebSecurity.Logout();
                    return RedirectToAction("LockedAccount", "Error");
                }

                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }


        /// <summary>
        /// POST: Logs the user off
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
          
            return RedirectToAction("Index", "Home");
        }



        /// <summary>
        /// GET: Show the page to reset the user's password
        /// </summary>
        /// <param name="message">the message to be displayed along the process</param>
        /// <param name="DisplayFields">display the next set of fields required to reset passwords</param>
        [AllowAnonymous]
        public ActionResult Reset(ManageMessageId? message, bool DisplayFields = false)
        {
            ModelState.Clear();

            // show the next set of fields required to reset their password
            if (DisplayFields)
                ViewBag.DisplayHiddenFields = true;

            if (TempData["model"] != null)
            {
                ResetPasswordModel model = TempData["model"] as ResetPasswordModel;

                ViewBag.UserQuestion = model.Question;
                ViewBag.Account = model.Account;
            }

            ViewBag.StatusMessage =
            message == ManageMessageId.PasswordResetUnsuccessful ? "Invalid answer to the question."
            : message == ManageMessageId.AccountDoesNotExist ? "The account does not exist."
            : message == ManageMessageId.EmptyInput ? "Input an Account Name."
            : "";

            return View();
        }



        /// <summary>
        /// POST: Determine if the user's answer matches their questions
        /// Prevent admins and locked accounts from resetting their passwords
        /// </summary>
        /// <param name="model">The reset password model</param>
        /// <param name="message">message to be displayed along the way</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Reset(ResetPasswordModel model, ManageMessageId? message)
        {
            // prevent user from accessing this link directly
            if (model == null)
                return RedirectToAction("IndirectAccess", "Error");
            
            if (model.Account == null)
                return RedirectToAction("Reset", new {Message = ManageMessageId.EmptyInput });


            bool UserExist = AccHelper.CheckUserExist(model.Account);
            
            // check to see if the user exist and make sure they are not an admin or are locked
            if (UserExist)
            {
                if (Roles.IsUserInRole(model.Account,"admin"))
                {
                    TempData["message"] = "Admin account, cannot be resetted";
                    return RedirectToAction("Index", "Home");
                }
                else if (Roles.IsUserInRole(model.Account, "locked"))
                {
                    TempData["message"] = "This account has been locked and forbidden from making changes.";
                    return RedirectToAction("Index", "Home");
                }

                // get their secret question
                model.Question = AccHelper.GetUserQuestion(model.Account);
                ViewBag.UserQuestion = model.Question; // fill the question textbox
                ViewBag.DisplayHiddenFields = true; // show the question / answer field

                TempData["model"] = model;  // storing the model server side to pass to the next Redirect

                if (model.Answer == null)
                {
                    // user successfully entered in an existing account name for first time
                    return RedirectToAction("Reset", new { DisplayFields = true });
                }
                else
                {
                    if (AccHelper.MatchAnswer(model.Account, model.Answer))
                    
                    {
                        // user successfully answered the question
                        TempData["resetPhaseOne"] = model.Account;
                        return RedirectToAction("SetNewPassword");
                    }
                    else
                    {
                        // incorrect answer to question
                        return RedirectToAction("Reset", new { Message = ManageMessageId.PasswordResetUnsuccessful, DisplayFields = true});
                    }
                }
            }
            else
            {
                // account does not exist
                return RedirectToAction("Reset", new {Message = ManageMessageId.AccountDoesNotExist });
            }
        }



        /// <summary>
        /// GET: Prompt the user for a new password
        /// </summary>
        [AllowAnonymous]
        public ActionResult SetNewPassword()
        {
            // resetPhaseOne is assigned when the user successfully answer their secret question
            // if it is not assigned, give an error
            if (TempData["resetPhaseOne"] == null)
            {
                return RedirectToAction("IndirectAccess", "Error");
            }
            else
            {
                // Going to next phase of resetting password
                TempData["resetPhaseTwo"] = TempData["resetPhaseOne"];
                ViewBag.Account = TempData["resetPhaseOne"];  // html display

                TempData["resetPhaseOne"] = null; // prevent user from backing and resubmitting
            }

            return View();
        }



        
        /// <summary>
        /// POST: Reset the users password
        /// redirect them to the main page afterwards
        /// </summary>
        /// <param name="model">data model</param>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetNewPassword(NewPasswordModel model)
        {
            // resetPhaseTwo is assigned when the user successfully submits validated matching passwords
            // if it is not assigned, give an error
            if (TempData["resetPhaseTwo"] == null)
            {
                return RedirectToAction("IndirectAccess", "Error");
            }
            else
            {
                string Account = TempData["resetPhaseTwo"].ToString();
                
                WebSecurity.ResetPassword(WebSecurity.GeneratePasswordResetToken(Account), model.NewPassword);
                WebSecurity.Login(Account, model.NewPassword);

                TempData["model"] = null;
                TempData["resetPhaseTwo"] = null;  // prevent user from backing and resubmitting
                TempData["message"] = "You have successfully changed your password.";

                return RedirectToAction("Index", "Home");
            }
        }



        /// <summary>
        /// Display page to register an account
        /// </summary>
        [AllowAnonymous]
        public ActionResult Register()
        {
            PopulateQuestionList();
            return View();
        }



        /// <summary>
        /// POST: Register an account
        /// </summary>
        /// <param name="model">register data model</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            PopulateQuestionList();

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(
                        model.UserName.Trim(), 
                        model.Password,
                        propertyValues: new { 
                            Question_ID = Convert.ToInt32(Request["qList"].ToString()),
                            Answer = model.User_Answer.Trim(),
                            State = "Active",   // default state
                            Reset = "False",     // default reset password -- here for future implementations if time permits
                        }
                    );

                    Roles.AddUserToRole(model.UserName.Trim(),"user");
                    WebSecurity.Login(model.UserName, model.Password);

                    TempData["message"] = "Welcome " + model.UserName + ". Your account has been successfully registered.";
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



    
        /// <summary>
        /// Display the options to edit their account
        /// </summary>
        /// <param name="message">message to be displayed</param>
        /// <param name="id">determine which options the user chose</param>
        public ActionResult Manage(ManageMessageId? message, string id)
        {

            if (id == null)
            {
                ViewBag.Parameter = "Manage";   // default page if there is no parameter
            }
            else
            {
                switch (id.ToLower())
                {
                    case "password":
                        ViewBag.Parameter = "Password"; // loads the partial Change Password Page
                        break;
                    case "question":
                        PopulateQuestionList();

                        ViewBag.UserQuestion = AccHelper.GetUserQuestion(User.Identity.Name);
                        ViewBag.Parameter = "Question"; // loads the partial Change Question Page
                        break;
                    default:
                        ViewBag.Parameter = "Manage";   //any unidentified parameter will go to the default manage page
                        break;
                }
            }

            // Message to be displayed based on whether the user has successfully changed the password/question
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.ChangePasswordUnSuccessful ? "The current password is incorrect or the new password is invalid."
                : message == ManageMessageId.ChangeQuestionSuccess ? "Your Secret Question and Answer has been changed."
                : message == ManageMessageId.ChangeQuestionUnsuccessful ? "The Answer to your current Secret Question is invalid."
                : message == ManageMessageId.ChangeError ? "An unexpected database error has occcurred. Please contact the administrator."
                : "";

            if (message == ManageMessageId.ChangePasswordSuccess || message == ManageMessageId.ChangeQuestionSuccess)
            {
                ViewBag.State = "Success";  // css coloring blue
            }
            else if (message == ManageMessageId.ChangePasswordUnSuccessful || message == ManageMessageId.ChangeQuestionUnsuccessful ||
                message == ManageMessageId.ChangeError)
            {
                ViewBag.State = "Error";    // css coloring red
            }

            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }




        /// <summary>
        /// POST: Attempt to change the users password based on the users input
        /// </summary>
        /// <param name="model">reset password model</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManagePasswordPost(LocalPasswordModel model)
        {
            bool HasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = HasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");

            if (HasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool ChangePasswordSucceeded;
                    try
                    {
                        ChangePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        ChangePasswordSucceeded = false;
                    }


                    if (ChangePasswordSucceeded)
                    {

                        return RedirectToAction("Manage/Password", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        return RedirectToAction("Manage/Password", new { Message = ManageMessageId.ChangePasswordUnSuccessful });
                    }

                }
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("Manage/Password", new { Message = ManageMessageId.ChangeError }); ;
        }




        /// <summary>
        /// POST: Attempt to change the users secret question
        /// </summary>
        /// <param name="model">secret question model</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageQuestionPost(ChangeQuestionModel model)
        {
    
            PopulateQuestionList();

            // Check to see if their inputted Answer matches the Answer in the database
            bool AnswerMatches = AccHelper.MatchAnswer(User.Identity.Name, model.Answer);
            
            if (AnswerMatches) 
            {
                AccHelper.UpdateUserQuestion(User.Identity.Name, Convert.ToInt32(Request["qList"].ToString()), model.NewAnswer);

                // Confirm if the New Answer is changed/saved in the database
                if (AccHelper.MatchAnswer(User.Identity.Name, model.NewAnswer))
                {
                    return RedirectToAction("Manage/Question", new { Message = ManageMessageId.ChangeQuestionSuccess });
                } 
                else 
                {
                    return RedirectToAction("Manage/Question", new { Message = ManageMessageId.ChangeError }); ;
                }
               
            }
            else
            {
                return RedirectToAction("Manage/Question", new { Message = ManageMessageId.ChangeQuestionUnsuccessful });
            }         
        }    


        #region Helpers
        // used to redirect user to another page after logging on
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            ChangePasswordUnSuccessful,
            ChangeQuestionSuccess,
            ChangeQuestionUnsuccessful,
            PasswordResetSuccess,
            PasswordResetUnsuccessful,
            AccountDoesNotExist,
            EmptyInput,
            ChangeError,
            LockedAccount
        }


        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
           
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
