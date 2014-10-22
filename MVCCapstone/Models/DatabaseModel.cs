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
        public UsersContext(): base("capstone")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<MemberShip> Membership { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<DBRoles> DbRoles { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Forum> Forum { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<BookGenre> BookGenre { get; set; }
        public DbSet<Bookmark> Bookmark { get; set; }
        public DbSet<Thread> Thread { get; set; }
        public DbSet<Post> Post { get; set; }
    }

    [Table("Question")]
    public class Question
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

    [Table("webpages_Membership")]
    public class MemberShip
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastPasswordFailureDate { get; set; }
    }

    [Table("webpages_Roles")]
    public class UserRole
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    [Table("webpages_UsersInRoles")]
    public class DBRoles
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        public int UserId { get; set; }

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
        public DateTime Published { get; set; }
        public string LanguageId { get; set; }
        public string ISBN { get; set; }
        public int ForumId { get; set; }
        public string Synopsis { get; set; }
        public string State { get; set; }
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

    [Table("BookGenre")]
    public class BookGenre
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BookGenreId { get; set; }
        public int BookId { get; set; }
        public int GenreId { get; set; }
    }


    [Table("Bookmark")]
    public class Bookmark
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BookmarkId { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime DateAdded { get; set; }
    }


    [Table("Thread")]
    public class Thread
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ThreadId { get; set; }
        public int ForumId { get; set; }
        public string Title { get; set; }
        public DateTime ThreadCreated { get; set; }
        public string ThreadCreator { get; set; }
        public DateTime LatestPost { get; set; }
        public string LatestPoster { get; set; }
        public string State { get; set; }
        public int TotalPost { get; set; }
        public int TotalView { get; set; }
    }

    [Table("Post")]
    public class Post
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int ThreadId { get; set; }
        public string PostContent { get; set; }
        public string ReplyTo { get; set; }
        public string ReplyPostContent { get; set; }
        public DateTime PostDate { get; set; }
        public string ModifiedDate { get; set; }
    }
    
}