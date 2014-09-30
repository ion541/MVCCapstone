﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using MVCCapstone.Models;

namespace MVCCapstone.Models
{


    [Table("webpages_Roles")]
    public class UserRole
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    [Table("webpages_UsersInRoles")]
    public class DBRoles
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public int UserId { get; set; }

    }

    /// <summary>
    /// Helper class which provides functions / methods related to roles
    /// </summary>
    public class roleHelper
    {
        /// <summary>
        /// Get a list of roles available to be filtered
        /// </summary>
        /// <returns></returns>
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
        /// Get all the roles in the database as a list of strings
        /// </summary>
        /// <returns>a list containing the strings of roles</returns>
        public static List<string> GetRoleStringList()
        {
            UsersContext db = new UsersContext();

            var roles = db.UserRoles.OrderBy(u => u.RoleId).Select(u => u.RoleName).ToList();

            return roles;
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
        /// Get a list of all the role id's in the database
        /// </summary>
        /// <returns>a list of integers containing the role id</returns>
        public static List<int> GetAllRoleId()
        {
            UsersContext db = new UsersContext();

            List<int> roleIdList = (from r in db.UserRoles
                                    select r.RoleId).ToList();
            return roleIdList;
        }


    }

    public class UserInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
    }


    public class RoleList
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }


    public class AccountSearchViewModel
    {
        public IList<RoleList> AvailableRoles { get; set; }
        public IList<RoleList> SelectedRoles { get; set; }
        public PostedRoles PostedRoles { get; set; }
        public string hiddenId { get; set; }

        public AccountListModel AccountListModel { get; set; }

        public List<SelectListItem> DisplayList { get; set; }

        public IPagedList<UserInfo> PaginationUserInfoModel { get; set; }

        public EditUserModel UserRoles { get; set; }
       
    }

  
    public class PostedRoles
    {
        public string[] UserRoleIDs { get; set; }
    }

}