using System;
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