using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCCapstone.Models
{

        public class UsersContext : DbContext
        {
            public UsersContext()
                : base("DefaultConnection")
            {
            }

            public DbSet<UserProfile> UserProfiles { get; set; }
            public DbSet<QuestionT> Questions { get; set; }
            public DbSet<UserRole> UserRoles { get; set; }
            public DbSet<DBRoles> DbRoles { get; set; }
            public DbSet<Genre> Genres { get; set; }
            public DbSet<Language> Languages { get; set; }
        }

        [Table("Question")]
        public class QuestionT
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int Question_ID { get; set; }
            public string Value { get; set; }
        }

        [Table("UserProfile")]
        public class UserProfile
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int UserId { get; set; }
            public string UserName { get; set; }
            public int Question_ID { get; set; }
            public string Answer { get; set; }
        }

        [Table("Genre")]
        public class Genre
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int Genre_ID { get; set; }
            public string Value { get; set; }
        }

        [Table("Language")]
        public class Language
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int Language_ID { get; set; }
            public string Value { get; set; }
        }
    
}