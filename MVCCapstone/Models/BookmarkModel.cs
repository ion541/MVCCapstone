using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCCapstone.Models
{

    public class BookmarkDisplayModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime DateAdded { get; set; }
       
    }
    
}