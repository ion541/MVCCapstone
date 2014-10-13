using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCCapstone.Models
{
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
    }

    



}