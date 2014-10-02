using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using MVCCapstone.Models;

namespace MVCCapstone.Helpers
{
    public class AdminHelper
    {
        /// <summary>
        /// Get a Pagination collection of object based on the users input
        /// </summary>
        /// <param name="account">used to search for accounts in the database that resembles the value</param>
        /// <param name="page">used to display the page</param>
        /// <param name="display">used to display the number of items on the page</param>
        /// <param name="sortby">used to order by the field</param>
        /// <param name="dir">used to sort by ascending or descending</param>
        /// <param name="roleSelected"></param>
        /// <returns>IPagedList of the UserInfo class</returns>
        public static IPagedList<UserInfo> GenerateUserList(string account, int page, int display, string sortby, bool ascend, List<int> roleSelected = null)
        {
            UsersContext db = new UsersContext();

            if (roleSelected == null)
                roleSelected = new List<int>();

            // query containing the data based off of the inputs
            var userList = (from d in db.UserProfiles
                            join u in db.DbRoles on d.UserId equals u.UserId
                            join r in db.UserRoles on u.RoleId equals r.RoleId
                            where d.UserName.Contains(account) && roleSelected.Contains(r.RoleId)
                            select new UserInfo { UserId = d.UserId, UserName = d.UserName, RoleName = r.RoleName });


            // sort the query by the field and direction
            switch (sortby)
            {
                case "role":
                    userList = ((ascend) ? userList.OrderBy(u => u.UserName) : userList.OrderByDescending(u => u.RoleName));
                    break;

                case "account":
                    userList = ((ascend) ? userList.OrderBy(u => u.UserName) : userList.OrderByDescending(u => u.UserName));
                    break;

                case "id":
                default:
                    userList = ((ascend) ? userList.OrderBy(u => u.UserId) : userList.OrderByDescending(u => u.UserId));
                    break;
            }

            // convert the query into a pagination list
            IPagedList<UserInfo> orderedUserList = userList.ToPagedList(page, display) as IPagedList<UserInfo>;

            return orderedUserList;
        }
    }
}