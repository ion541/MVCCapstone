using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MVCCapstone.Helpers
{
    public class ReviewHelper
    {
        public static string ReviewFilter(string content)
        {
            if (String.IsNullOrEmpty(content)) 
                return "";

            content = Regex.Replace(content, @"<[^>]+>|&nbsp;", "").Trim(); // remove all html tags
            content =  Regex.Replace(content, @"\s{2,}", " "); // replace double spaces with one space

            // {look for,  replace with}
            string[,] acceptedTags = {  
                                        {"b", "b"},
                                        {"i", "i"},
                                        {"u", "u"},
                                        {"s", "strike"},
                                        {"h1", "h2"},
                                        {"h2", "h3"},
                                        {"blockquote", "blockquote"},
                                        {"sp", "span class=\"spoiler\""},
                                    };

            for (int i = 0; i < acceptedTags.GetLength(0); i++)
            {

                switch (acceptedTags[i, 0])
                {
                    case "spoiler":
                        content = content.Replace("[" + acceptedTags[i, 0] + "]", "<" + acceptedTags[i, 1] + ">");
                        content = content.Replace("[/" + acceptedTags[i, 0] + "]", "</span>");
                        break;

                    default:
                        content = content.Replace("[" + acceptedTags[i, 0] + "]", "<" + acceptedTags[i, 1] + ">");
                        content = content.Replace("[/" + acceptedTags[i, 0] + "]", "</" + acceptedTags[i, 1] + ">");
                        break;
                }
            }    

            return content;
        }
    }
}