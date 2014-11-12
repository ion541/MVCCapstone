using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;

namespace MVCCapstone.Models
{
    public class ReviewListModel
    {
        public int bookId { get; set; }
        public string bookTitle { get; set; }
        public IPagedList<ReviewModel> reviewList { get; set; }
    }

    // model used to create a review
    public class ReviewModel
    {
        public int bookId { get; set; }
        public string bookTitle { get; set; }
        public int reviewId { get; set; }

        [Required(ErrorMessage="You must set a title for your review")]
        [StringLength(50, ErrorMessage = "Your Review Title must be at a maximum of {1} characters")]
        [Display(Name = "Your Review Title")]
        public string reviewTitle { get; set; }

        [Required(ErrorMessage="Your review must not be empty")]
        [StringLength(7500, ErrorMessage = "Your review must be at a maximum of {1} characters")]
        [Display(Name = "Your Review")]
        public string reviewContent { get; set; }

        [Required(ErrorMessage="Your must select an option")]
        [Display(Name = "Recommend This Book?")]
        public string recommend { get; set; }

        public bool isRecommended { get; set; }
        public bool isPreview { get; set; }
        public string author { get; set; }
        public DateTime reviewCreated { get; set; }
        public DateTime reviewLastModified { get; set; }

        public string lastModified { get; set; }
        public int rateUp { get; set; }
        public int rateTotal { get; set; }
    }

    public class ReviewList
    {
        public int bookId { get; set; }
        public string bookTitle { get; set; }
        public int reviewId { get; set; }
        public int ratings { get; set; }
        public string author { get; set; }
        public bool isRecommended { get; set; }
        public DateTime reviewCreated { get; set; }
        public DateTime reviewLastModified { get; set; }
    }
}