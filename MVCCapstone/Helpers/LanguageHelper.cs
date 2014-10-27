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
        /// <summary>
        /// Generates a select list of languages from the database and returns it
        /// </summary>
        /// <param name="selectedItem">the default item to select</param>
        /// <returns>a list of select select list items of languages</returns>
        public static List<SelectListItem> DisplayList(string selectedItem = "")
        {

            UsersContext db = new UsersContext();

            int selectedId = -1;
            if (selectedItem != "") Int32.TryParse(selectedItem, out selectedId);

            var displaylist = db.Languages.ToList();

            List<SelectListItem> DisplayList = new List<SelectListItem>();

            // loop through each number in the configured list above and add it to the list with its value
            foreach (Language language in displaylist)
            {
                if (language.Language_ID == selectedId)
                {
                    DisplayList.Add(new SelectListItem { Text = language.Value, Value = language.Language_ID.ToString(), Selected = true });
                }
                else
                {
                    DisplayList.Add(new SelectListItem { Text = language.Value, Value = language.Language_ID.ToString(), Selected = false });
                }
            }

            return DisplayList;
        }
    }
}