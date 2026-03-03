using System;
using System.Windows;
using LibraryApp.Data;
using LibraryApp.Data.Models;

namespace LibraryApp.Windows
{
    public partial class GenreEditWindow : Window
    {
        private LibraryContext _context;
        private Genre _genre;
        private bool _isEditMode;

        public GenreEditWindow(LibraryContext context, Genre genre = null)
        {
            try
            {
                InitializeComponent();
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _genre = genre ?? new Genre();
                _isEditMode = genre != null;

                if (_isEditMode)
                {
                    Title = "Редактирование жанра";
                    LoadGenreData();
                }
                else
                {
                    Title = "Добавление жанра";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}");
            }
        }

        private void LoadGenreData()
        {
            NameBox.Text = _genre.Name;
            DescriptionBox.Text = _genre.Description;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameBox.Text))
                {
                    MessageBox.Show("Введите название жанра", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    NameBox.Focus();
                    return;
                }

                _genre.Name = NameBox.Text.Trim();
                _genre.Description = DescriptionBox.Text?.Trim() ?? "";

                if (!_isEditMode)
                {
                    _context.Genres.Add(_genre);
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}