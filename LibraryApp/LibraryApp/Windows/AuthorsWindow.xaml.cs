using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Data.Models;

namespace LibraryApp.Windows
{
    public partial class AuthorsWindow : Window
    {
        private LibraryContext _context;

        public AuthorsWindow(LibraryContext context)
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
                await LoadAuthorsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task LoadAuthorsAsync()
        {
            var authors = await _context.Authors.ToListAsync();
            AuthorsGrid.ItemsSource = authors;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new AuthorEditWindow(_context);
                window.Owner = this;
                if (window.ShowDialog() == true)
                {
                    _ = LoadAuthorsAsync();
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
                if (AuthorsGrid.SelectedItem is Author selectedAuthor)
                {
                    var window = new AuthorEditWindow(_context, selectedAuthor);
                    window.Owner = this;
                    if (window.ShowDialog() == true)
                    {
                        _ = LoadAuthorsAsync();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите автора для редактирования", "Информация", 
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
                if (AuthorsGrid.SelectedItem is Author selectedAuthor)
                {
                    var result = MessageBox.Show($"Удалить автора {selectedAuthor.FullName}?", 
                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        _context.Authors.Remove(selectedAuthor);
                        await _context.SaveChangesAsync();
                        await LoadAuthorsAsync();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите автора для удаления", "Информация", 
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
