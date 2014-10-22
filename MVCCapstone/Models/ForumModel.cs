using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCCapstone.Models
{

    public class ThreadModel
    {
        public List<SharedWith> sharedWith { get; set; }
        public int ForumId { get; set; }
        public string bookTitle { get; set; }
        public bool series { get; set; }
        public List<ThreadViewModel> threadList { get; set; }
    }

    public class ThreadViewModel
    {
        public int ThreadId { get; set; }
        public int ForumId { get; set; }
        public string Title { get; set; }
        public DateTime ThreadCreated { get; set; }
        public string ThreadCreator { get; set; }
        public DateTime LatestPost { get; set; }
        public string LatestPoster { get; set; }
        public string State { get; set; }
        public int TotalPost { get; set; }
        public int TotalView { get; set; }
        public string DateString { get; set; }
    }

    public class PostModel
    {
        public int forumId { get; set; }
        public int threadId { get; set; }
        public string threadTitle { get; set; }
        public List<PostViewModel> postList { get; set; }
    }

    public class PostViewModel
    {
        public int postId { get; set; }
        public string userName { get; set; }
        public int totalPost { get; set; }
        public DateTime memberSince { get; set; }
        public string postContent { get; set; }
        public string replyTo { get; set; }
        public string replyPostContent {get; set;}
        public DateTime datePosted { get; set; }
        public bool editPost { get; set; }
        public string dateModified { get; set; }
    }



    public class SharedWith
    {
        public int bookid { get; set; }
        public string title { get; set; }
    }

    public class CreateThreadModel
    {
        public string title { get; set; }
        public int forumid { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name="Title")]
        public string threadTitle { get; set; }

        public CreatePostModel post { get; set; }
    }

    public class CreatePostModel
    {
        [Required]
        [StringLength(7000, ErrorMessage = "The {0} must be at a maximum of {1} characters")]
        [Display(Name = "Post")]
        public string content { get; set; }

        public bool showPostSection { get; set; }
        public bool showReply { get; set; }
        public string replyTo { get; set; }
        public string replyContent { get; set; }
        public int threadId { get; set; }
    }
}