using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using MVCCapstone.Models;
using PagedList;

namespace MVCCapstone.Helpers
{
    public class ReviewHelper
    {

        public static IPagedList<ReviewModel> GetReviews(int bookid, string sortby, int page, int display)
        {
            UsersContext db = new UsersContext();
            if (page <= 0)
                page = 1;

            // query containing the data based off of the inputs
            List<ReviewModel> reviewList = (from b in db.Book
                              join r in db.Review on b.BookId equals r.BookId
                              join u in db.UserProfiles on r.UserId equals u.UserId
                              where b.BookId == bookid
                              select new ReviewModel
                              {
                                  reviewId = r.ReviewId,
                                  bookId = b.BookId,
                                  bookTitle = b.Title,
                                  author = u.UserName,
                                  recommend = r.Recommended,
                                  reviewTitle = r.Title,
                                  reviewContent = r.Content,
                                  reviewCreated = r.DateCreated,
                                  reviewLastModified = r.DateModified,
                              }).ToList();

            foreach (ReviewModel review in reviewList)
            {
                review.rateTotal = db.ReviewRate.Where(m => m.ReviewId == review.reviewId).Count();
                review.rateUp = db.ReviewRate.Where(m => m.ReviewId == review.reviewId && m.Rate == "up").Count();
            }

            switch (sortby)
            {
                case "notrecommended":
                    reviewList = reviewList.Where(m => m.recommend == "no").OrderByDescending(m => m.rateTotal).ToList();
                    break;

                case "recommended":
                    reviewList = reviewList.Where(m => m.recommend == "yes").OrderByDescending(m => m.rateTotal).ToList();
                    break;
                case "new":
                    reviewList = reviewList.OrderByDescending(m => m.reviewLastModified).ToList();
                    break;
                case "popular":
                default:
                    reviewList = reviewList.OrderByDescending(m => m.rateTotal).ToList();
                    break;
            }

            IPagedList<ReviewModel> orderedReviewList = reviewList.ToPagedList(page, display) as IPagedList<ReviewModel>;

             return orderedReviewList;
            
        }
        // class used to find and replace tags
        public class AcceptedTags
        {
            public string searchFor, replaceStart, replaceEnd;
            public AcceptedTags(string search, string start, string end)
            {
                this.searchFor = search;
                this.replaceStart = start;
                this.replaceEnd = end;
            }
        }

        
        /// <summary>
        /// Remove every occurence of html tags and replace squared bracket tags with html tags
        /// 
        /// Validate the string and make sure that the html tags are well formed
        /// </summary>
        /// <param name="content">the content to be filtered / validated</param>
        /// <param name="isPreview">If set the true, the stack of tags will not be shown when filtering fails</param>
        /// <returns>raw html string</returns>
        public static string ReviewFilter(string rawContent, bool isPreview = false)
        {

            string errorString = "<strong>Error, the tags being used are not well-formed.</strong> <br />";


            if (String.IsNullOrEmpty(rawContent)) 
                return "";

            // keep a copy of the raw content stripped of html tags in case of an error
            rawContent = Regex.Replace(rawContent, @"<[^>]+>|&nbsp;", "").Trim();

            // attempt to replace all square bracket tags with html tags
            string content = rawContent;

            List<AcceptedTags> listTags = new List<AcceptedTags>();

            listTags.Add(new AcceptedTags("b", "<b>", "</b>"));
            listTags.Add(new AcceptedTags("i", "<i>", "</i>"));
            listTags.Add(new AcceptedTags("u", "<u>", "</u>"));
            listTags.Add(new AcceptedTags("s", "<s>", "</s>"));
            listTags.Add(new AcceptedTags("h1", "<h2>", "</h2>"));
            listTags.Add(new AcceptedTags("h2", "<h3>", "</h3>"));
            listTags.Add(new AcceptedTags("blockquote", "<blockquote>", "</blockquote>"));
            listTags.Add(new AcceptedTags("sp", "<span class=\"spoiler\">", "</span>"));


            foreach (AcceptedTags tags in listTags)
            {
                content = content.Replace("[" + tags.searchFor + "]", tags.replaceStart);
                content = content.Replace("[/" + tags.searchFor + "]", tags.replaceEnd);
            }

            bool IsValidHTML = true;

            string decoded = HttpUtility.HtmlDecode(content);
            string process = "";
            int tabTracker = 0;

            // splits the html by the closing character of a tag
            string[] splitHTML = decoded.Split('>');

            // array that contains only tags
            string[] tagClean = cleanTags(splitHTML);
            try
            {

                Stack<string> tagStack = new Stack<string>();
                foreach (string tag in tagClean)
                {

                    // determine if the tag is a closing tag or open tag
                    if (tag[0] == '/')
                    {
                        string tagStackPop = tagStack.Pop(); // open tag that was removed from the stack
                        string closeTag = tag.Substring(1, tag.Length - 1); // close tag without the /
                        tabTracker--;


                        // compares the previous open tag with the current close tag
                        if (tagStackPop != closeTag)
                        {
                            // the open tag does not match the closing tag
                            IsValidHTML = false;
                            break;
                        }

                        process += "<span style='margin-right:" + tabTracker * 35 + "px;'></span>[" + tag + "] <br />";
                    }
                    else
                    {
                        // a open tag was found
                        tagStack.Push(tag);

                        process += "<span style='margin-right:" + tabTracker * 35 + "px'></span>[" + tag + "] <br />";
                        tabTracker++;

                    }


                }

                if (IsValidHTML && tagStack.Count == 0)
                    return content;

                // show the stack of tags if it is a preview
                if (isPreview)
                    return errorString + "<strong>At the end of this trace is where the problem tag is found.</strong> <br /> " + process
                       + "<br />" + rawContent;

                // if there is an error while filtering, and is in an actual review, only display the error and the html before filtering
                return errorString + rawContent;
            }
            catch (InvalidOperationException)
            {
                // will occur when ended tags are discovered when the stack is empty
                if (isPreview)
                    return errorString + "<strong>At the end of this trace is where the problem tag is found.</strong> <br /> " + process
                       + "<br />" + rawContent;

                // if there is an error while filtering, and is in an actual review, only display the error and the html before filtering
                return errorString + rawContent;
            }
            catch (Exception)
            {
                return "An error has occurred while converting the tags. Please contact the administrator. + <br />" + rawContent;
            }

        }

        /// <summary>
        /// Takes an array that contains a row with strings and its tag
        /// Splits the string and remove its attribute so that only the tag remains which is then added into a new array
        /// </summary>
        /// <param name="arrTagsAndStrings">Array that contains a tag and strings</param>
        /// <returns>an array of only tags</returns>
        private static string[] cleanTags(string[] arrTagsAndStrings)
        {

            int index = 0;

            // last line will be empty
            string[] arrCleanTagArray = new string[arrTagsAndStrings.Length - 1];

            for (int i = 0; i < arrTagsAndStrings.Count() - 1; i++)
            {
                string tag = arrTagsAndStrings[i].Substring(arrTagsAndStrings[i].IndexOf('<') + 1);


                int space = tag.IndexOf(' ');
                if (space >= 0)
                {
                    tag = tag.Substring(0, space);
                }

                arrCleanTagArray[index++] = tag.ToLower();
            }
            return arrCleanTagArray;
        }


        /// <summary>
        /// Gets the data from the review and set and format it to the model's structure
        /// </summary>
        /// <param name="review">the review which contains the data</param>
        /// <param name="isPreview"> the review model is to be seen in a preview mode</param>
        /// <param name="filterContent">filter the reviews content</param>
        /// <returns>A ReviewModel</returns>
        public static ReviewModel SetReviewModel(Review review, bool isPreview, bool filterContent)
        {

            UsersContext db = new UsersContext();

            ReviewModel model = new ReviewModel();
            model.reviewId = review.ReviewId;


            // filter the content  which will replace square tags with html tags
            if (filterContent)
            {
                model.reviewContent = ReviewHelper.ReviewFilter(review.Content);
            }
            else
            {
                model.reviewContent = review.Content;
            }
            model.reviewTitle = review.Title;
            model.author = AccHelper.GetUserName(review.UserId);
            model.recommend = review.Recommended;

            if (review.Recommended == "yes")
            {
                model.isRecommended = true;
            }
            else
            {
                model.isRecommended = false;
            }

            model.isPreview = isPreview;
            model.bookId = review.BookId;
            model.rateUp = db.ReviewRate.Where(m => m.ReviewId == review.ReviewId && m.Rate == "up").Count();
            model.rateTotal = db.ReviewRate.Where(m => m.ReviewId == review.ReviewId && m.Rate == "down").Count() + model.rateUp;
         

            model.lastModified = "Written On: " + review.DateCreated.ToShortDateString();

            // display date modified instead if the date created and last modified is different
            if (review.DateCreated.Ticks != review.DateModified.Ticks)
                model.lastModified = "Last Modified On: " + review.DateModified.ToShortDateString();

            return model;
        }

        /// <summary>
        /// Determines if a review with the id exists.
        /// Returns a boolean indicating the existence of the review
        /// </summary>
        /// <param name="id">id to be searched</param>
        /// <returns>boolean</returns>
        public static bool ReviewIdValid(int? id)
        {
            if (id.HasValue)
            {
                UsersContext db = new UsersContext();
                if (db.Review.Find(id.Value) != null)
                    return true;
            }

            return false;
            
        }

    }
}