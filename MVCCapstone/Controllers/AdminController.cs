using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MVCCapstone.Models;
using PagedList;
using MVCCapstone.Helpers;
using System.Globalization;
using System.IO;

namespace MVCCapstone.Controllers
{
    public class AdminController : Controller
    {

        private UsersContext db = new UsersContext();


        /// <summary>
        /// Create a list of numbers used for selecting the numbers of items to be displayed on the pagination
        /// </summary>
        /// <param name="display">the current selected item</param>
        /// <returns>List of select list items</returns>
        [NonAction]
        private List<SelectListItem> DisplayList(int display)
        {

            // numbers selected on a whim
            List<int> displaylist = new List<int> { 5, 10, 15, 25, 50, 100 };

            // if the selected display number is not in the list above, add it and then sort
            if (!displaylist.Contains(display))
            {
                displaylist.Add(display);
                displaylist.Sort();
            }
        
            List<SelectListItem> DisplayList = new List<SelectListItem>();

            // loop through each number in the configured list above and add it to the list with its value
            foreach (int number in displaylist)
            {
                string num = number.ToString();

                // check to see if the instance of the integer is the selected number
                if (number == display)
                {
                    // mark the item so that it is selected on the view
                    DisplayList.Add(new SelectListItem { Text = num, Value = num, Selected = true });
                }
                else
                {
                    DisplayList.Add(new SelectListItem { Text = num, Value = num });
                }
            }

            return DisplayList;
        }



        //
        // GET: /Admin/
        [RoleAuthorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View();
        }




       
        // GET: /Admin/Account
        [RoleAuthorize(Roles = "admin")]
        public ActionResult Account(ManageMessageId? message, string account = "", int page = 1, int display = 10, string sort = "id", bool asc = true)
        {

            if (Request.QueryString["roles"] != null)
            {
                // split the roles parameter and only include valid integers
                ViewBag.RoleSelected = Request.QueryString["roles"].Split('-').Select(n => { int e; return Int32.TryParse(n, out e) ? e : -1; }).ToList();
            }
            else
            {
                // if no roles were selected, select them all by default
                ViewBag.RoleSelected = RoleHelper.GetRoleIds();
            }

            // used to search for the selected roles
            List<int>  roleList = ViewBag.RoleSelected;

            // model used to store information and display on the view
            AccountSearchViewModel model = new AccountSearchViewModel();

            model.PaginationUserInfoModel = AdminHelper.GenerateUserList(account, page, display, sort, asc, roleList);
            model.AvailableRoles = RoleHelper.GetRoleList();       // get the roles available to search for in the database
            model.DisplayList = DisplayList(display);   // populate the number of users to display for pagination
            model.hiddenId = ""; // default empty for the search / filter form memory which will be posted as well

            // Check to see if the Edit parameter exists
            if (Request.QueryString["userId"] != null)
            {
                int userId;
                // see if the user id inputted is an integer
                if (int.TryParse(Request.QueryString["userId"], out userId))
                {
                    // check to see if the user id exists
                    if (AccHelper.CheckUserExist(userId))
                    {
                        if (User.Identity.Name == AccHelper.GetUserName(userId))
                        {
                            message = ManageMessageId.EditOwnRole;
                        }
                        else
                        {
                            ViewBag.displayEditSection = true;

                            EditUserModel userRole = new EditUserModel();

                            userRole.Account = AccHelper.GetUserName(userId);
                            userRole.UserId = userId;
                            userRole.CurrentRole = RoleHelper.GetUserCurrentRole(userId);
                            userRole.SelectRole = RoleHelper.GetRoleNames() ;

                            // data posted in the role update form will retain the values for the search / query form
                            userRole.hiddenAccount = account;
                            userRole.hiddenDisplay = display;
                            userRole.hiddenPage = page;
                            userRole.hiddenRoleId = (Request.QueryString["roles"] != null) ? Request.QueryString["roles"].ToString() : "";
                          
                            model.UserRoles = userRole;
                            model.hiddenId = userId.ToString();
                        }
                    }
                    else
                    {
                            ViewBag.displayErrorMessage = "The user id does not exist";
                    }
                }
                else
                {
                    ViewBag.displayErrorMessage = "An invalid user id has been inputted.";
                }
            }

            ViewBag.RoleChangeMessage =     message == ManageMessageId.EditOwnRole ? "You are forbidden from changing your own role." :
                                            message == ManageMessageId.EditRolesSuccessful ? "The user role has been updated successfully." :
                                            message == ManageMessageId.EditRolesUnsuccessful ? "An issue has occurred, the user role was not changed. Pleaes contact the developer with steps on how to recreate the problem." :
                                            "";

            // if there was a posted account name, set the value in the textbox
            if (Request.QueryString["account"] != null)
            {
                ViewBag.AccountNameFieldValue = Request.QueryString["account"].ToString();
            }

            return View(model);
        }




        //
        // Post: /Admin/Account
        [HttpPost]
        [RoleAuthorize(Roles = "admin")]
        public ActionResult Account(AccountSearchViewModel model, PostedRoles postedRoles)
        {

            // check to see which form posted
            // currently only two forms which post different models
            if (model.UserRoles != null)
            {
                // the user has posted the model to update a users information
                ManageMessageId statusMessage;

                // remove the user from the current role assigned
                Roles.RemoveUserFromRoles(model.UserRoles.Account, Roles.GetRolesForUser(model.UserRoles.Account));
                // assign the user to the selected role
                Roles.AddUserToRoles(model.UserRoles.Account, model.UserRoles.SelectRole.ToArray());

                // message to to see if the user is now in the role posted
                statusMessage = Roles.IsUserInRole(model.UserRoles.Account, model.UserRoles.SelectRole[0].ToString()) ? ManageMessageId.EditRolesSuccessful : ManageMessageId.EditRolesUnsuccessful;


                return RedirectToAction("Account", "Admin", new
                {
                    userId = model.UserRoles.UserId,
                    roles = model.UserRoles.hiddenRoleId,
                    account = model.UserRoles.hiddenAccount,
                    page = model.UserRoles.hiddenPage,
                    display = model.UserRoles.hiddenDisplay,
                    message = statusMessage
                });
            }
            else
            {
                // user has posted to the form to update the list of users to be displayed

                string roleIdSelected = ""; // used as a URL parameter

                // break down the role id posted and seperate them using "-"
                if (postedRoles.UserRoleIDs != null)
                {
                    roleIdSelected = postedRoles.UserRoleIDs[0];
                    for (int i = 1; i < postedRoles.UserRoleIDs.Count(); i++)
                        roleIdSelected = roleIdSelected + "-" + postedRoles.UserRoleIDs[i];
                }

                // see if the id selected in the model to update a users information exist
                if (Request["hiddenId"] != "0") // 0 is a default value representing no user was selected to edit
                {
                    return RedirectToAction("Account", "Admin", new
                    {
                        userId = Request["hiddenId"].ToString(),    // redirect with the id which will populate the other update user form
                        roles = roleIdSelected,
                        account = model.AccountListModel.AccountName,
                        display = Request["DisplayValue"].ToString()
                    });
                }
                else
                {
                    return RedirectToAction("Account", "Admin", new
                    {
                        roles = roleIdSelected,
                        account = model.AccountListModel.AccountName,
                        display = Request["DisplayValue"].ToString()
                    });
                }
            
            }

        }


        //
        // GET: /Admin/Book
        [RoleAuthorize(Roles = "admin")]
        public ActionResult Book(ManageMessageId? message)
        {

            BookManagementModel model = new BookManagementModel();
            if (TempData["model"] != null)
                model = TempData["model"] as BookManagementModel;


            ViewBag.Message =   message == ManageMessageId.ForumIdDoesNotExist ? "The Inputted Forum Id does not exist" :
                                message == ManageMessageId.ForumIdNotValid ? "The Forum Id inputted was not valid." :
                                message == ManageMessageId.InvalidDate ? "The date inputted is not valid." :
                                message == ManageMessageId.UploadingImageError ? "An error has occurred while updating the image." :
                                message == ManageMessageId.SuccessfulInsert ? "The book has been successfully added into the database." :
                                message == ManageMessageId.ISBNInDatabase ? "The ISBN inputted is already in the database." :
                                message == ManageMessageId.UnsuccessfulInsert ? "The book was not successfully inserted into the database." :
                                message == ManageMessageId.TestingTrue ? "Test condition is true" :
                                message == ManageMessageId.TestingFalse ? "Testing condition is false" :
                                "";

            model.AvaialbleGenres = BookHelper.GetGenreList();
            model.Language = LanguageHelper.DisplayList();
            return View(model);
        }



        //
        // POST: /Admin/Book
        [HttpPost]
        [RoleAuthorize(Roles = "admin")]
        public ActionResult Book(BookManagementModel model, PostedGenres postedGenres, HttpPostedFileBase Image = null)
        {

            TempData["model"] = model;  // pass the model data back to the next action

            // ISBN must be unique, see if it is
            if (!BookHelper.CheckISBN(model.ISBN))
            {
                return RedirectToAction("Book", "Admin", new { message = ManageMessageId.ISBNInDatabase });
            }

            string imageId = null;
            if (Image != null)
            {
                try
                {
                    // get the string to the path of the book images
                    string bookPath = BookHelper.GetServerPath();

                    // creates a unique name for the file
                    string fileName =   Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Image.FileName);
                    // get the path to where the images are stored
                    string basePath = Server.MapPath("~/" + bookPath);

                    // create the directory if it does not exist
                    if (System.IO.File.Exists(basePath))
                        System.IO.Directory.CreateDirectory(bookPath);

                    // Try and save the file directly to the server
                    Image.SaveAs(Path.Combine(basePath, fileName));
                  
                    // insert a record of the image path and return the id of the record
                    imageId = BookHelper.InsertImageRecord(bookPath + fileName);
                }
                catch
                {
                    RedirectToAction("Book", "Admin", new { message = ManageMessageId.UploadingImageError });
                }
            }


            int forum_id;

            if (model.ForumId == null)
            {
               // no forum id provided, create a new forum id for this book
                model.ForumId = BookHelper.CreateNewForumId().ToString();
            }
            else
            {
                forum_id = Int32.Parse(model.ForumId);
                bool ForumIdExist = BookHelper.CheckForForumIdExistence(forum_id);

                // check to see if the inputted forum id exists
                if (!ForumIdExist)
                {
                    return RedirectToAction("Book", "Admin", new { message = ManageMessageId.ForumIdDoesNotExist });
                }
            }

            DateTime day;
            if (!DateTime.TryParseExact(model.Published,"dd/MM/yyyy",CultureInfo.InvariantCulture, DateTimeStyles.None, out day)) {
                return RedirectToAction("Book", "Admin", new {message = ManageMessageId.InvalidDate });
            }
  

            string language = Request["LanguageId"].ToString();
         
            // at this point, the image was uploaded if it existed and the forum id has been valid / generated
            bool BookInserted = BookHelper.InsertBookRecord(model, language, imageId);
            string bookId = BookHelper.GetLastBookId().ToString();
            
            
            if (postedGenres.GenreId != null)
            {
                for (int i = 0; i < postedGenres.GenreId.Count(); i++)
                {
                    BookHelper.InsertBookGenre(bookId, postedGenres.GenreId[i]);
                }
            }
            else
            {
                BookHelper.InsertBookGenreDefault(bookId);
            }


            if (BookInserted)
            {
                // if we made it here, then nothing went wrong
                return RedirectToAction("Book", "Admin", new { message = ManageMessageId.SuccessfulInsert });
            }
            else
            {
                return RedirectToAction("Book", "Admin", new { message = ManageMessageId.UnsuccessfulInsert });
            }
             
        }


    }



    public enum ManageMessageId
    {
        EditRolesSuccessful,
        EditRolesUnsuccessful,
        EditOwnRole,
        ForumIdNotValid,
        ForumIdDoesNotExist,
        InvalidDate,
        SuccessfulInsert,
        UnsuccessfulInsert,
        UploadingImageError,
        ISBNInDatabase,
        TestingTrue,
        TestingFalse
    }

    /// <summary>
    /// Attribute that inherits the Authorize attribute.
    /// 
    /// If the user is not logged in, it will redirect the user to the log in page.
    /// 
    /// If the user is logged in, it will redirect the user to the  error page instead of always redirecting to the Log in page in the event of unauthorization
    /// The process above is repeated when the user logs in and doesn't have the authority to access it.
    /// </summary>
    public class RoleAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // test to see if the user is logged in
            if(filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // redirect to error page
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "UnauthorizedAccess" }));
            }
            else
            {
                // redirect to login page
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
