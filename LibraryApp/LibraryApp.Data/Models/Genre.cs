using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Data.Models
{
    public class Genre
    {
        public Genre()
        {
            Books = new List<Book>();
        }

        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        // Навигационное свойство
        public ICollection<Book> Books { get; set; }
    }
}