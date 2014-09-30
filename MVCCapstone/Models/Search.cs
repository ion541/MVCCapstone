using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCCapstone.Models
{
    [Table("Genre")]
    public class GenreT
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Genre_ID  { get; set; }
        public string Value { get; set; }
    }

    [Table("Language")]
    public class LanguageT
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Language_ID { get; set; }
        public string Value { get; set; }
    }

    public class SearchDBContext : DbContext
    {
        public SearchDBContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<GenreT> Genres { get; set; }
        public DbSet<LanguageT> Languages { get; set; }
    }
}