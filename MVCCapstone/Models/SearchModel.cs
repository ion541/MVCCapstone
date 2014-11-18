using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
using System.Web.Mvc;

namespace MVCCapstone.Models
{
    // model used for querying in a search
    public class SearchModel
    {
        public SearchQuery query { get; set; }
        public List<Book> books { get; set; }

        public IList<GenreList> availableGenres { get; set; }
        public IList<GenreList> selectedGenres { get; set; }
        public PostedGenres postedGenres { get; set; }

        public List<SelectListItem> availableLanguage { get; set; }
    }

    // model used to store users input for querying
    public class SearchQuery
    {
        [Display(Name = "Title")]
        public string title { get; set; }

        [Display(Name = "Author")]
        public string author { get; set; }

        [Display(Name = "Published")]
        public string yearFrom { get; set; }
        public string yearTo { get; set; }

        [Display(Name = "Genre")]
        public string genre { get; set; }

        [Display(Name = "Publisher")]
        public string publisher { get; set; }

        [Display(Name = "ISBN")]
        public string isbn { get; set; }

        [Display(Name = "Language")]
        public string language { get; set; }
    }

    // display list of books
    public class SearchResult
    {
        public List<Book> books { get; set; }
    }
}