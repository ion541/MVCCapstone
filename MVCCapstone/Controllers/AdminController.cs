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
    [ValidateInput(false)]
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
        // GET: /Admin/SelectImage
        [RoleAuthorize(Roles = "admin")]
        public ActionResult SelectImage()
        {
            DisplayImageModel model = new DisplayImageModel();
            model.images = db.Image.OrderByDescending(m => m.ImageId).ToList();
            return View(model);
        }


        public ActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        [RoleAuthorize(Roles = "admin")]
        public ActionResult UploadImage(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                try
                {
                    foreach (var file in files)
                    {
                        // verify that the user selected a file
                        if (file != null && file.ContentLength > 0)
                        {
                            // get the string to the path of the book images
                            string bookPath = BookHelper.GetServerPath();

                            // creates a unique name for the file
                            string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);

                            // get the path to where the images are stored
                            string basePath = Server.MapPath("~/" + bookPath);

                            // create the directory if it does not exist
                            if (System.IO.File.Exists(basePath))
                                System.IO.Directory.CreateDirectory(bookPath);

                            // Try and save the file directly to the server
                            file.SaveAs(Path.Combine(basePath, fileName));

                            // insert a record of the image path and return the id of the record
                            ViewBag.ImageId = BookHelper.InsertImageRecord(bookPath + fileName);
                        }
                    }
                }
                catch
                {
                    ViewBag.ErrorMessage = "An error has occurred while uploading the image.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "No Images were found";
            }
            return PartialView("_UploadImageResult");
        }




        //
        // GET: /Admin/Book
        [RoleAuthorize(Roles = "admin")]
        public ActionResult Book(string imageId)
        {
            BookManagementModel model = new BookManagementModel();

            if (imageId != null)
                model.Image = imageId;

            model.AvaialbleGenres = BookHelper.GetGenreList();
            model.Language = LanguageHelper.DisplayList();
            return View(model);
        }



 

        //
        // POST: /Admin/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(Roles = "admin")]
        public ActionResult Book(BookManagementModel model)
        {
            
            AddBookResultModel resultModel = new AddBookResultModel();
            resultModel.errors = new List<string>();

            if (!BookHelper.CheckISBN(model.ISBN))
                resultModel.errors.Add("The ISBN is already in the database.");

            if (model.PostedGenres.GenreId == null)
                resultModel.errors.Add("At least one genre must be selected.");

            if (model.isSeries == "Series" && model.Series == "Existing")
            {
                if (!BookHelper.CheckForForumIdExistence(Int32.Parse(model.ForumId)))
                    resultModel.errors.Add("The forum id inputted is not valid.");
            }

            DateTime day;
            if (!DateTime.TryParseExact(model.Published,"dd/MM/yyyy",CultureInfo.InvariantCulture, DateTimeStyles.None, out day))
                resultModel.errors.Add("The date published inputted is not valid.");

            // no errors, proceed to upload image to database
            if (resultModel.errors.Count() == 0)
            {
                if (model.isSeries == "Standalone")
                    model.ForumId = BookHelper.CreateNewForumId().ToString();
                else if (model.isSeries == "Series" && model.Series == "New")
                    model.ForumId = BookHelper.CreateNewForumId(model.SeriesTitle).ToString();

  
                string language = (Request["Language"] == null) ? "1" : Request["Language"].ToString();
  
                // insert the record of the books data into the database
                bool BookInserted = BookHelper.InsertBookRecord(model, language);
               
                
                if (BookInserted)
                {
                    int bookId = BookHelper.GetLastBookId();

                    // add all the selected genres for the book into the database
                    for (int i = 0; i < model.PostedGenres.GenreId.Count(); i++)
                        BookHelper.InsertBookGenre(bookId, Int32.Parse(model.PostedGenres.GenreId[i]));
                }
                else
                {
                    resultModel.errors.Add("An error has occurred while inserting the book into the database.");
                }

            }

            return PartialView("_AddBookResult", resultModel);
        }


        public PartialViewResult SeriesSearch(string seriesTitle)
        {
            SeriesListModel model = new SeriesListModel();
            model.seriesList = db.Forum.Where(m => m.SeriesTitle.Contains(seriesTitle)).OrderBy(m => m.ForumId).ToList();
            return PartialView("_SeriesResult", model);
        }
    }




    /// <summary>
    /// enumeration for messages throughout the admin controller
    /// </summary>
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
}
