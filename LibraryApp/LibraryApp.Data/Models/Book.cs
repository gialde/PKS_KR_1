using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Data.Models
{
    public class Book
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public int PublishYear { get; set; }
        
        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;
        
        public int QuantityInStock { get; set; }
        
        // Внешние ключи
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        
        // Навигационные свойства
        public Author Author { get; set; } = null!;
        public Genre Genre { get; set; } = null!;
        
        // Для отображения в DataGrid
        public string AuthorName => Author?.FullName ?? "Неизвестно";
        public string GenreName => Genre?.Name ?? "Неизвестно";
    }
}