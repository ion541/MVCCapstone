using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCCapstone.Models;

namespace MVCCapstone.Helpers
{
    /// <summary>
    /// Helper class which provides functions / methods related to roles
    /// </summary>
    public class RoleHelper
    {

        /// <summary>
        /// Get a list of roles available to be filtered
        /// </summary>
        /// <returns>List of roles</returns>
        public static List<RoleList> GetRoleList()
        {
            UsersContext db = new UsersContext();

            var roles = (from u in db.UserRoles
                         select new RoleList { RoleId = u.RoleId, RoleName = u.RoleName }).OrderBy(u => u.RoleId);

            List<RoleList> roleList = new List<RoleList>();

            foreach (RoleList role in roles)
            {
                roleList.Add(role);
            }

            return roleList;
        }




        /// <summary>
        /// Get the string of the role the user is currently in
        /// </summary>
        /// <param name="UserId">the id of the user being searched for</param>
        /// <returns>a string containing the name of the role the user is in</returns>
        public static string GetUserCurrentRole(int UserId)
        {
            UsersContext db = new UsersContext();

            string currentRole = (from d in db.UserProfiles
                                  join u in db.DbRoles on d.UserId equals u.UserId
                                  join r in db.UserRoles on u.RoleId equals r.RoleId
                                  where d.UserId == UserId
                                  select r.RoleName).FirstOrDefault();
            return currentRole;

        }




        /// <summary>
        /// Get the roles the user is currently in
        /// </summary>
        /// <param name="UserId">the id of the user being searched for</param>
        /// <returns>a list of integers containing the ids of the roles the user is in</returns>
        public static List<int> GetUserRoles(int UserId)
        {
            UsersContext db = new UsersContext();

            List<int> userRoleList = (from d in db.UserProfiles
                                      join u in db.DbRoles on d.UserId equals u.UserId
                                      join r in db.UserRoles on u.RoleId equals r.RoleId
                                      where d.UserId == UserId
                                      select u.RoleId).ToList();

            return userRoleList;
        }




        /// <summary>
        /// Get all the role name in the role table
        /// </summary>
        /// <returns>List of string containing role names</returns>
        public static List<string> GetRoleNames()
        {
            UsersContext db = new UsersContext();

            return db.UserRoles.OrderBy(u => u.RoleId).Select(u => u.RoleName).ToList();
        }





        /// <summary>
        /// Get all the role ids in the role table
        /// </summary>
        /// <returns>List of int containing role ids</returns>
        public static List<int> GetRoleIds()
        {
            UsersContext db = new UsersContext();

            return db.DbRoles.Select(m => m.RoleId).ToList();
        }
    }
}