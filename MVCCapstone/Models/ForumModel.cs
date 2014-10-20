using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCCapstone.Models
{
    public class ForumModel
    {

    }

    public class ThreadModel
    {
        public List<SharedWith> sharedWith { get; set; }
        public int bookid { get; set; }
        public string bookTitle { get; set; }
        public bool series { get; set; }
        public List<Thread> threadList { get; set; }
    }

    public class SharedWith
    {
        public int bookid { get; set; }
        public string title { get; set; }
    }

    public class CreateThreadModel
    {
        public string title { get; set; }
        public int bookid { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name="Title")]
        public string threadTitle { get; set; }

        public CreatePostModel post { get; set; }
    }

    public class CreatePostModel
    {
        [Required]
        [StringLength(8000, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "Post")]
        public string content { get; set; }
    }
}