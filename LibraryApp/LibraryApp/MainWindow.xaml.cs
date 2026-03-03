using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Data.Models;
using LibraryApp.Windows;

namespace LibraryApp
{
    public partial class MainWindow : Window
    {
        private LibraryContext _context;
        private List<Book> _allBooks;

        public MainWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
            _allBooks = new List<Book>();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Подключение к базе данных...";
                await _context.Database.EnsureCreatedAsync();
                await LoadDataAsync();
                StatusText.Text = "Готово";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                StatusText.Text = "Ошибка";
            }
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            _allBooks = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToListAsync();

            var genres = await _context.Genres.ToListAsync();
            genres.Insert(0, new Genre { Id = 0, Name = "Все жанры" });
            GenreFilterCombo.ItemsSource = genres;
            GenreFilterCombo.SelectedIndex = 0;

            BooksGrid.ItemsSource = _allBooks;
            BooksCountText.Text = $"Всего книг: {_allBooks.Count}";
        }

        private void ApplyFilter()
        {
            if (_allBooks == null) return;

            var filteredBooks = _allBooks.AsEnumerable();

            string searchText = SearchBox.Text?.ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredBooks = filteredBooks.Where(b => 
                    b.Title.ToLower().Contains(searchText));
            }

            if (GenreFilterCombo.SelectedItem is Genre selectedGenre && selectedGenre.Id > 0)
            {
                filteredBooks = filteredBooks.Where(b => b.GenreId == selectedGenre.Id);
            }

            BooksGrid.ItemsSource = filteredBooks.ToList();
            BooksCountText.Text = $"Всего книг: {filteredBooks.Count()}";
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ClearFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            GenreFilterCombo.SelectedIndex = 0;
        }

        private void AddBookBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new BookWindow(_context);
            window.Owner = this;
            window.ShowDialog();
        }

        private void EditBookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BooksGrid.SelectedItem is Book selectedBook)
            {
                var window = new BookWindow(_context, selectedBook);
                window.Owner = this;
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите книгу для редактирования");
            }
        }

        private async void DeleteBookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BooksGrid.SelectedItem is Book selectedBook)
            {
                var result = MessageBox.Show($"Удалить книгу '{selectedBook.Title}'?", 
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    _context.Books.Remove(selectedBook);
                    await _context.SaveChangesAsync();
                    await LoadDataAsync();
                }
            }
        }

        private void ManageAuthorsBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new AuthorsWindow(_context);
            window.Owner = this;
            window.ShowDialog();
        }

        private void ManageGenresBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new GenresWindow(_context);
            window.Owner = this;
            window.ShowDialog();
        }
    }
}