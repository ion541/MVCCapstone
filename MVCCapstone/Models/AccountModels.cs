using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Linq;

namespace MVCCapstone.Models
{

    // model used to change the users current password on account management page
    public class LocalPasswordModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The New Password and Confirmation Password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    // model used to change password when resetting
    public class NewPasswordModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The New Password and Confirmation Password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    // login model
    public class LoginModel
    {
        [Required]
        [RegularExpression("([a-zA-Z0-9_-]+)", ErrorMessage = "Only letters, numbers, and characters '-' and '_' are allowed.")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    // register model
    public class RegisterModel
    {
        [Required]
        [RegularExpression("([a-zA-Z0-9_-]+)", ErrorMessage = "Only letters, numbers, and characters '-' and '_' are allowed.")]
        [StringLength(20, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The Password and Confirmation Password do not match.")]
        public string ConfirmPassword { get; set; }

        public string User_Permission { get; set; }

        [Display(Name = "Secret Question")]
        public string User_Question { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Secret Answer")]
        public string User_Answer { get; set; }
    }

    // model used to change users secret question on account management page
    public class ChangeQuestionModel
    {

        [Required]
        [ReadOnly(true)]
        [Display(Name = "Current Secret Question")]
        public string Question { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Current Question Answer")]
        public string Answer { get; set; }

        [Required]
        [Display(Name = "New Secret Question")]
        public string NewQuestion { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "New Secret Answer")]
        public string NewAnswer { get; set; }

        [Display(Name = "Confirm Secret Answer")]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 3)]
        [Compare("NewAnswer", ErrorMessage = "The Secret Answer and Confirmation Answer do not match.")]
        public string NewAnswerConfirm { get; set; }
    }


    // model used to prompt user for their account / secret question / answer
    public class ResetPasswordModel
    {
        [Required]
        [RegularExpression("([a-zA-Z0-9_-]+)", ErrorMessage = "Only letters, numbers, and characters '-' and '_' are allowed.")]
        [Display(Name = "Account Name")]
        public string Account { get; set;}
   
        [Display(Name = "Question")]
        public string Question { get; set; }

        [Display(Name = "Answer")]
        public string Answer { get; set; }
    }


    // admin model used to display the account on the admin page
    public class EditUserModel
    {
        [Display(Name="User Id")]
        public int UserId {get; set;}

        [Display(Name = "Account Name")]
        public string Account { get; set;}

        public string CurrentRole { get; set; }

        [Display(Name = "Roles")]
        public List<string> SelectRole { get; set; }

        public string hiddenAccount { get; set; }
        public int hiddenPage { get; set; }
        public string hiddenRoleId { get; set; }
        public int hiddenDisplay { get; set; }

    }

}
