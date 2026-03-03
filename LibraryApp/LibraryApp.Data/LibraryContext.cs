using Microsoft.EntityFrameworkCore;
using LibraryApp.Data.Models;

namespace LibraryApp.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=LibraryDB;Username=postgres;Password=1512");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                
                entity.Property(b => b.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(b => b.ISBN)
                    .HasMaxLength(20);
                
                entity.Property(b => b.QuantityInStock)
                    .IsRequired()
                    .HasDefaultValueSql("1");
                
                entity.HasOne(b => b.Author)
                    .WithMany(a => a.Books)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(b => b.Genre)
                    .WithMany(g => g.Books)
                    .HasForeignKey(b => b.GenreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Настройка Author
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(a => a.Id);
                
                entity.Property(a => a.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(a => a.LastName)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(a => a.Country)
                    .HasMaxLength(100);
                
                // Ничего особенного для BirthDate - оставляем как есть
            });

            // Настройка Genre
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                
                entity.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(g => g.Description)
                    .HasMaxLength(500);
            });
        }
    }
}