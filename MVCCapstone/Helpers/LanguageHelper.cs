using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCCapstone.Models;

namespace MVCCapstone.Helpers
{
    public class LanguageHelper
    {
        public static List<SelectListItem> DisplayList()
        {
            UsersContext db = new UsersContext();
            // numbers selected on a whim
            var displaylist = db.Languages.ToList();

            List<SelectListItem> DisplayList = new List<SelectListItem>();

            // loop through each number in the configured list above and add it to the list with its value
            foreach (Language language in displaylist)
            {
  
                    DisplayList.Add(new SelectListItem { Text = language.Value, Value = language.Language_ID.ToString() });
            }

            return DisplayList;
        }
    }
}