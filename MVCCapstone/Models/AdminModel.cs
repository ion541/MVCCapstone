﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;

namespace MVCCapstone.Models
{

    // model used to get user account name on admin account management page
    public class UserInfoModel
    {
        [Editable(false)]
        [StringLength(256, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "Account Name")]
        public int UserId { get; set; }

        public List<UserRole> UserRoles { get; set; }
    }

    // model used to display account list info
    public class AccountListModel
    {
        [RegularExpression("([a-zA-Z0-9_-]+)", ErrorMessage = "Only letters, numbers, and characters '-' and '_' are allowed.")]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        public string Display { get; set; }
    }

    // model used to display individla account info
    public class AccountDisplayModel
    {
        [RegularExpression("([0-9]+)", ErrorMessage = "Numbers Only")]
        [Display(Name = "Account ID")]
        public string UserId { get; set; }

        [StringLength(20, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "Account Name")]
        public string UserName { get; set; }
    }

    // model used to get users input to select user
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

    // model used to display a users id, name, role selected and roles available
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

    // model used to store a books information when adding to database
    public class BookDetailsModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "Book Title")]
        public string BookTitle { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "Author")]
        public string Author { get; set; }

        [Display(Name = "Synopsis")]
        [StringLength(7500, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        public string Synopsis { get; set; }

        [RegularExpression("^(([0-9]{10})|([0-9]{13}))$", ErrorMessage = "Must be either 10 or 13 digits")]
        [Required]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; }


        [Display (Name ="Image")]
        public string Image { get; set; }

        public IList<GenreList> AvaialbleGenres { get; set; }
        public IList<GenreList> SelectedGenre { get; set; }
        public PostedGenres PostedGenres { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "Publisher")]
        public string Publisher { get; set; }

        [Required]
        [RegularExpression("^(3[01]|[12][0-9]|0[1-9])/(1[0-2]|0[1-9])/[0-9]{4}$", ErrorMessage = "Must be in the format of dd/mm/yyyy")]
        [Display(Name = "Date Published")]
        public string Published { get; set; }

        [Display(Name = "Language")]
        public List<SelectListItem> Language { get; set; }

        public string isSeries { get; set; }

        [Display(Name = "Series")]
        public string Series { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "Series Title")]
        public string SeriesTitle { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Numbers Only")]
        [Display(Name = "Series Id")]
        public string ForumId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Numbers Only")]
        [Display(Name = "Series Id")]
        public string NewForumId { get; set; }

        public string State { get; set; }
    }

    // genres posted when adding a book
    public class PostedGenres
    {
        public string[] GenreId { get; set; }
    }
    
    // display list of genres from database
    public class GenreList
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GenreId { get; set; }
        public string Genre { get; set; }
    }

    // display list of forum series in database
    public class SeriesListModel
    {
        public List<Forum> seriesList { get; set; }
    }

    // display list of books
    public class BookResultModel
    {
        public List<string> errors { get; set; }
        public int bookId { get; set; }
    }

    // display list of images
    public class DisplayImageModel
    {
        public List<Image> images { get; set; } 
    }

    // model used to query for books in database
    public class ManageBookModel
    {
        [StringLength(50, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "Title of Book")]
        public string titleSearch { get; set; }

        [StringLength(50, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "Author of Book")]
        public string authorSearch { get; set; }


    }

    // model passed to the view of the search result
    public class ManageBookSearchModel
    {
        public List<Book> bookResults { get; set; }
    }


    // model used to edit book information
    public class EditBookModel : BookDetailsModel
    {
        public bool displayEdit { get; set; }

        public int BookId { get; set; }
        public string LanguageId { get; set; }
        public List<int> BookGenre { get; set; }

        public bool isStandalone { get; set; }
        public string SeriesTitleDisplay { get; set; }

        [Display(Name = "Forum Action")]
        public string ForumAction { get; set; }

        /// <summary>
        /// Known asp.net bug which prevents defaulted selected item
        /// Solution is to use another list with another name (Language => LanguageDisplay)
        /// </summary>
        public List<SelectListItem> LanguageDisplay { get; set; }
    }

    // model displayed when changing a books image
    public class EditImageModel
    {
        public int bookId { get; set; }
        public bool hasImage { get; set; }

        [Display(Name="Current Image")]
        public string bookImage { get; set; }
        [Display(Name = "Select Image")]
        public List<Image> images { get; set; } 
    }


    /// <summary>
    /// Used to store information relevant to a user before deleting
    /// </summary>
    public class DeleteBookPromptModel
    {
        public bool display { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public int bookid { get; set; }

        public bool isStandalone { get; set; }
        public int postCount { get; set; }
        public int threadCount { get; set; }
    }
    
}