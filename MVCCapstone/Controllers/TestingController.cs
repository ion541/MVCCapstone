using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MVCCapstone.Models;
using PagedList;
using MVCCapstone.Helpers;

namespace MVCCapstone.Controllers
{
    public class TestingController : Controller
    {
        /*
        private UsersContext db = new UsersContext();

        [NonAction]
        private IPagedList<UserInfo> GenerateUserList(string account, int page, int display, List<int> roleSelected = null)
        {
            if (roleSelected == null)
                roleSelected = new List<int>();
           
            IPagedList<UserInfo> userList = (from d in db.UserProfiles
                                             join u in db.DbRoles on d.UserId equals u.UserId
                                             join r in db.UserRoles on u.RoleId equals r.RoleId
                                             where d.UserName.Contains(account) && roleSelected.Contains(r.RoleId)
                                             select new UserInfo { UserId = d.UserId, UserName = d.UserName, RoleName = r.RoleName })
                                                    .OrderBy(v => v.UserId).ToPagedList(page, display) as IPagedList<MVCCapstone.Models.UserInfo>;
            return userList;
        }

        /// <summary>
        /// Create a list of numbers used for selecting the numbers of items to be displayed on the pagination
        /// </summary>
        /// <param name="display">the current selected item</param>
        /// <returns>List of select list items</returns>
        [NonAction]
        private List<SelectListItem> DisplayList(int display)
        {
            string selectedNumber = display.ToString();

            // numbers selected on a whim
            string[] displayNumbers = { "5", "10", "15", "25", "50", "100" };
            List<SelectListItem> DisplayList = new List<SelectListItem>();

            // in the event the user inputs in their own number
            if (!displayNumbers.Contains(selectedNumber))
                DisplayList.Add(new SelectListItem { Text = selectedNumber, Value = selectedNumber, Selected = true });
     

            foreach (string number in displayNumbers)
            {
                if (number == selectedNumber)
                {
                    // this was the item that is selected
                    DisplayList.Add(new SelectListItem { Text = number, Value = number, Selected = true });
                }
                else
                {
                    DisplayList.Add(new SelectListItem { Text = number, Value = number });
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




        //
        // GET: /Admin/Account
        [RoleAuthorize(Roles = "admin")]
        public ActionResult Account(ManageMessageId? message, string account = "", int page = 1, int display = 10)
        {
        
            ViewBag.RoleChangeMessage = (message == ManageMessageId.EditRolesSuccessful) ? "The user role has been updated successfully." :
                message == ManageMessageId.EditRolesUnsuccessful ? "An issue has occurred, the user role was not changed. Pleaes contact the developer with steps on how to recreate the problem." :
                "";

            // default set the roles selected to nothing
            List<int> roleList = null;
            if (Request.QueryString["roles"] != null)
            {
                // split the roles parameter and only include valid integers
                ViewBag.RoleSelected = Request.QueryString["roles"].Split('-').Select(n => { int e; return Int32.TryParse(n, out e) ? e : -1; }).ToList(); 
            }
            else
            {
                ViewBag.RoleSelected = new List<int> { 0, 1, 2, 3, 4, 5, };
            }

            roleList = ViewBag.RoleSelected;

            // model used to store information and display on the view
            AccountSearchViewModel model = new AccountSearchViewModel();

            model.PaginationUserInfoModel = GenerateUserList(account, page, display, roleList);
            model.AvailableRoles = RoleHelper.GetRoleList();       // get the roles available to search for in the database
            model.DisplayList = DisplayList(display);   // populate the number of users to display for pagination

            // Check to see if the Edit parameter exists
            if (Request.QueryString["userId"] != null)
            {
                int userId;
                // see if the input is an integer
                if (int.TryParse(Request.QueryString["userId"], out userId))
                {
                    // check to see if the user id exists
                    if (AccHelper.CheckUserExist(userId))
                    {
                        ViewBag.displayEditSection = true;

                        EditUserModel userRole = new EditUserModel();

                        userRole.Account = AccHelper.GetUserName(userId);
                        userRole.UserId = userId;
                        userRole.CurrentRole = RoleHelper.GetUserCurrentRole(userId);
                        userRole.SelectRole = db.UserRoles.OrderBy(u => u.RoleId).Select(u => u.RoleName).ToList();
                        
                        // used for the search form memory
                        userRole.hiddenAccount = account;
                        userRole.hiddenDisplay = display;
                        userRole.hiddenPage = page;
                        userRole.hiddenRoleId = Request.QueryString["roles"].ToString();

                        model.UserRoles = userRole;

                        //model.UserRoles.UserRoles = RoleHelper.GetUserRoles(userId);
                        ViewBag.UserRoles = RoleHelper.GetUserRoles(userId);
                    }
                    else
                    {
                        ViewBag.displayErrorMessage = "The user inputted does not exist";
                    }
                }
                else
                {
                    ViewBag.displayErrorMessage = "An invalid user id format has be inputted.";
                }
            }


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
         
            // user has posted to the form to update a user's role
            if (model.UserRoles != null)
            {
                ManageMessageId statusMessage;

                Roles.RemoveUserFromRoles(model.UserRoles.Account, Roles.GetRolesForUser(model.UserRoles.Account));
                Roles.AddUserToRoles(model.UserRoles.Account, model.UserRoles.SelectRole.ToArray());

                statusMessage = Roles.IsUserInRole(model.UserRoles.Account, model.UserRoles.SelectRole[0].ToString()) ? ManageMessageId.EditRolesSuccessful : ManageMessageId.EditRolesUnsuccessful;
         

                return RedirectToAction("Account", "Testing", new
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
                // user has posted to the form to update the list of users

                string roleIdSelected = "";

                if (postedRoles.UserRoleIDs != null)
                {
                    roleIdSelected = postedRoles.UserRoleIDs[0];
                    for (int i = 1; i < postedRoles.UserRoleIDs.Count(); i++)
                        roleIdSelected = roleIdSelected + "-" + postedRoles.UserRoleIDs[i];
                }

                return RedirectToAction("Account", "Testing", new
                {
                    roles = roleIdSelected,
                    account = model.AccountListModel.AccountName,
                    display = Request["DisplayValue"].ToString()
                });
            }
         
        }


        //
        // GET: /Admin/Book
        [RoleAuthorize(Roles = "admin")]
        public ActionResult Book()
        {
            return View();
        }
    
    */
    }



}
