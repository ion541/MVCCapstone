using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCCapstone.Models;

namespace MVCCapstone.Helpers
{
    /// <summary>
    /// Helper class which provides functions / methods related to accounts
    /// </summary>
    public class AccHelper
    {

        /// <summary>
        /// Get the user name by the User Id
        /// </summary>
        /// <param name="userId">the id of the user</param>
        /// <returns>username</returns>
        public static string GetUserName(int userId)
        {
            UsersContext db = new UsersContext();

            string userName = (from u in db.UserProfiles
                               where u.UserId == userId
                               select u.UserName).First().ToString();
            return userName;
        }

        /// <summary>
        /// Get the secret question in the database of the selected user
        /// </summary>
        /// <param name="userName">the user name</param>
        public static string GetUserQuestion(string userName)
        {
            UsersContext db = new UsersContext();

            string question = (from u in db.UserProfiles
                               join q in db.Questions on u.Question_ID equals q.Question_ID
                               where u.UserName == userName
                               select q.Value).First().ToString();
            return question;
        }

        /// <summary>
        /// Returns a boolean depending on whether the posted Answer matches the Answer stored in the database of the user
        /// </summary>
        /// <param name="userName">The user to be checked</param>
        /// <param name="userAnswer">The posted Answer</param>
        /// <returns>true if the Answer matches otherwise false</returns>
        public static bool MatchAnswer(string userName, string userAnswer)
        {
            UsersContext db = new UsersContext();

            int count = (from u in db.UserProfiles
                         where u.UserName == userName && u.Answer == userAnswer.Trim()
                         select u.UserName).Count();

            return count == 1 ? true : false;
        }

        /// <summary>
        /// Updates the user name Secret Question and Answer
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        public static void UpdateUserQuestion(string userName, int id, string answer)
        {
            UsersContext db = new UsersContext();

            UserProfile user =
                (from u in db.UserProfiles
                 where u.UserName == userName
                 select u).First();

            user.Question_ID = id;
            user.Answer = answer;
            db.SaveChanges();
        }

        /// <summary>
        /// Check to see if the account exist within the database
        /// </summary>
        /// <param name="userName">The name of the account </param>
        /// <returns>true if exist otherwise false</returns>
        public static bool CheckUserExist(string userName)
        {
            UsersContext db = new UsersContext();

            int count = (from u in db.UserProfiles
                         where u.UserName == userName.Trim()
                         select u.UserName).Count();
            return count == 1 ? true : false;
        }

        /// <summary>
        /// Check to see if the account exist within the database
        /// </summary>
        /// <param name="userName">The id of the account </param>
        /// <returns>true if exist otherwise false</returns>
        public static bool CheckUserExist(int userId)
        {
            UsersContext db = new UsersContext();

            int count = (from u in db.UserProfiles
                         where u.UserId == userId
                         select u.UserName).Count();
            return count == 1 ? true : false;
        }
    }
}