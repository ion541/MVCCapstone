using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        [Required]
        [Display(Name = "Book Title")]
        public string BookTitle { get; set; }

        [Required]
        [Display(Name = "Author")]
        public string Author { get; set; }

        [RegularExpression("([0-9]+)", ErrorMessage = "Numbers Only")]
        [StringLength(13, ErrorMessage = "The {0} must be {2} characters long.", MinimumLength = 13)]
        [Required]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; }


        [Display (Name ="Image")]
        public string Image { get; set; }

        public IList<GenreList> AvaialbleGenres { get; set; }
        public IList<GenreList> SelectedGenre { get; set; }
        public PostedGenres PostedGenres { get; set; }

        [Required]
        [Display(Name = "Publisher")]
        public string Publisher { get; set; }


        [Required]
        [RegularExpression("^(3[01]|[12][0-9]|0[1-9])/(1[0-2]|0[1-9])/[0-9]{4}$", ErrorMessage = "Must be in the format of dd/mm/yyyy")]
        [Display(Name = "Date Published")]
        public string Published { get; set; }

        [Display(Name = "Language")]
        public List<SelectListItem> Language { get; set; }

        [RegularExpression("([0-9]+)", ErrorMessage = "Numbers Only")]
        [Display(Name = "Forum Id")]
        public string ForumId { get; set; }
    }

    public class PostedGenres
    {
        public string[] GenreId { get; set; }
    }
    public class GenreList
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GenreId { get; set; }
        public string Genre { get; set; }
    }
}