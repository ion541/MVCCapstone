using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCCapstone.Models
{
    public class ReviewModel
    {
        public List<Review> reviewList { get; set; }
    }

    public class CreateReviewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        public string title { get; set; }

        [Required]
        [StringLength(7500, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        public string content { get; set; }
    }
}