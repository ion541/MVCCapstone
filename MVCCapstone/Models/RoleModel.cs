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

    // model used to display a users role
    public class UserInfo
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
    }

    // list of roles in database
    public class RoleList
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    // posted user roles
    public class PostedRoles
    {
        public string[] UserRoleIDs { get; set; }
    }

}