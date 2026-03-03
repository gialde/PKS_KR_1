using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Data.Models
{
    public class Author
    {
        public Author()
        {
            Books = new List<Book>();
        }

        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        // ВАЖНО: DateTime вместо DateOnly
        public DateTime BirthDate { get; set; }
        
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;
        
        public ICollection<Book> Books { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
    }
}