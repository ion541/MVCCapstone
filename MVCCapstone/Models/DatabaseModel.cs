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
            public DbSet<Forum> Forum { get; set; }
            public DbSet<Book> Book { get; set; }
            public DbSet<Image> Image { get; set; }
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

        [Table("Forum")]
        public class Forum
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int ForumId { get; set; }
        }

        [Table("Book")]
        public class Book
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int BookId { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string ImageId { get; set; }
            public string Publisher { get; set; }
            public string Published { get; set; }
            public string LanguageId { get; set; }
            public string ISBN { get; set; }
            public string ForumId { get; set; }
        }

        [Table("Image")]
        public class Image
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int ImageId { get; set; }
            public string Path { get; set; }
            public DateTime DateAdded { get; set; }
        }
    
}