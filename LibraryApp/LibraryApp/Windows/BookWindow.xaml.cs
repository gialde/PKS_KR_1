using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Data;
using LibraryApp.Data.Models;

namespace LibraryApp.Windows
{
    public partial class BookWindow : Window
    {
        private LibraryContext _context;
        private Book _book;
        private bool _isEditMode;

        public BookWindow(LibraryContext context, Book book = null)
        {
            try
            {
                InitializeComponent();
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _book = book ?? new Book();
                _isEditMode = book != null;

                if (_isEditMode)
                {
                    TitleText.Text = "Редактирование книги";
                    Title = "Редактирование книги";
                    LoadBookData();
                }
                else
                {
                    TitleText.Text = "Добавление книги";
                    Title = "Добавление книги";
                }
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
                AuthorCombo.ItemsSource = await _context.Authors.ToListAsync();
                GenreCombo.ItemsSource = await _context.Genres.ToListAsync();

                // Если редактирование, выбираем нужные значения
                if (_isEditMode)
                {
                    AuthorCombo.SelectedItem = await _context.Authors.FindAsync(_book.AuthorId);
                    GenreCombo.SelectedItem = await _context.Genres.FindAsync(_book.GenreId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadBookData()
        {
            TitleBox.Text = _book.Title;
            YearBox.Text = _book.PublishYear.ToString();
            IsbnBox.Text = _book.ISBN;
            QuantityBox.Text = _book.QuantityInStock.ToString();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(TitleBox.Text))
                {
                    MessageBox.Show("Введите название книги", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    TitleBox.Focus();
                    return;
                }

                if (AuthorCombo.SelectedItem == null)
                {
                    MessageBox.Show("Выберите автора", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    AuthorCombo.Focus();
                    return;
                }

                if (GenreCombo.SelectedItem == null)
                {
                    MessageBox.Show("Выберите жанр", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    GenreCombo.Focus();
                    return;
                }

                if (!int.TryParse(YearBox.Text, out int year) || year < 1000 || year > 2100)
                {
                    MessageBox.Show("Введите корректный год (1000-2100)", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    YearBox.Focus();
                    return;
                }

                if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Введите корректное количество", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    QuantityBox.Focus();
                    return;
                }

                // Заполняем данные
                _book.Title = TitleBox.Text.Trim();
                _book.PublishYear = year;
                _book.ISBN = IsbnBox.Text?.Trim() ?? "";
                _book.QuantityInStock = quantity;
                _book.AuthorId = ((Author)AuthorCombo.SelectedItem).Id;
                _book.GenreId = ((Genre)GenreCombo.SelectedItem).Id;

                // Сохраняем
                if (!_isEditMode)
                {
                    _context.Books.Add(_book);
                }

                _context.SaveChanges();
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}\n\n{ex.InnerException?.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}