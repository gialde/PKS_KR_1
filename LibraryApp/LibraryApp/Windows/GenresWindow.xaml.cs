using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Data.Models;

namespace LibraryApp.Windows
{
    public partial class GenresWindow : Window
    {
        private LibraryContext _context;

        public GenresWindow(LibraryContext context)
        {
            try
            {
                InitializeComponent();
                _context = context;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}");
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadGenresAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task LoadGenresAsync()
        {
            var genres = await _context.Genres.ToListAsync();
            GenresGrid.ItemsSource = genres;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new GenreEditWindow(_context);
                window.Owner = this;
                if (window.ShowDialog() == true)
                {
                    _ = LoadGenresAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GenresGrid.SelectedItem is Genre selectedGenre)
                {
                    var window = new GenreEditWindow(_context, selectedGenre);
                    window.Owner = this;
                    if (window.ShowDialog() == true)
                    {
                        _ = LoadGenresAsync();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите жанр для редактирования", "Информация", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GenresGrid.SelectedItem is Genre selectedGenre)
                {
                    var result = MessageBox.Show($"Удалить жанр '{selectedGenre.Name}'?", 
                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        _context.Genres.Remove(selectedGenre);
                        await _context.SaveChangesAsync();
                        await LoadGenresAsync();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите жанр для удаления", "Информация", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
            }
        }
    }
}