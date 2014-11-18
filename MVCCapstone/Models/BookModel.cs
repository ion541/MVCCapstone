using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace MVCCapstone.Models
{
    // used for the home page to display a list of book
    public class HomePageModel
    {
        public IPagedList<BookDisplayModel> BookList { get; set; }
    }

    // model used to display a books information, latest forum post and reviews
    public class BookDisplayModel
    {
        public string BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ImagePath { get; set; }
        public string Publisher { get; set; }
        public string Published { get; set; }
        public string Language { get; set; }
        public string Genre { get; set; }
        public string ISBN { get; set; }
        public string Synopsis { get; set; }
        public string State { get; set; }
        public int ForumId { get; set; }
        public IPagedList<ThreadModel> ThreadList { get; set; }
        public string DiscussionTitle { get; set; }
        public bool IsSeries { get; set; }
        public string SeriesTitle { get; set; }
        public IPagedList<ReviewModel> ReviewList { get; set; }

    }
}