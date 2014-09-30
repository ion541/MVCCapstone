using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MVCCapstone.Models
{


    public class UserInfoModel
    {
        [Editable(false)]
        [Display(Name = "Account Name")]
        public int UserId { get; set; }


        public List<UserRole> UserRoles { get; set; }
    }


    public class AccountListModel
    {
        [RegularExpression("([a-zA-Z0-9_-]+)", ErrorMessage = "Only letters, numbers, and characters '-' and '_' are allowed.")]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        public string Display { get; set; }
    }



    public class AccountDisplayModel
    {
        [RegularExpression("([0-9]+)", ErrorMessage = "Numbers Only")]
        [Display(Name = "Account ID")]
        public string UserId { get; set; }

        [Display(Name = "Account Name")]
        public string UserName { get; set; }
    }


    public class AccountManagementModel
    {
        [Required]
        [RegularExpression("([0-9]+)", ErrorMessage = "Numbers Only")]
        [Display(Name = "Account ID")]
        public int AccountID { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }


    public class BookManagementModel
    {


    }
}