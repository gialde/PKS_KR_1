using System;
using System.Windows;
using LibraryApp.Data;
using LibraryApp.Data.Models;

namespace LibraryApp.Windows
{
    public partial class AuthorEditWindow : Window
    {
        private LibraryContext _context;
        private Author _author;
        private bool _isEditMode;

        public AuthorEditWindow(LibraryContext context, Author author = null)
        {
            try
            {
                InitializeComponent();
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _author = author ?? new Author();
                _isEditMode = author != null;

                if (_isEditMode)
                {
                    Title = "Редактирование автора";
                    LoadAuthorData();
                }
                else
                {
                    Title = "Добавление автора";
                    BirthDatePicker.SelectedDate = DateTime.Today.AddYears(-30);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void LoadAuthorData()
        {
            FirstNameBox.Text = _author.FirstName;
            LastNameBox.Text = _author.LastName;
            CountryBox.Text = _author.Country;
            BirthDatePicker.SelectedDate = _author.BirthDate;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(FirstNameBox.Text))
                {
                    MessageBox.Show("Введите имя", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    FirstNameBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(LastNameBox.Text))
                {
                    MessageBox.Show("Введите фамилию", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    LastNameBox.Focus();
                    return;
                }

                if (BirthDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату рождения", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    BirthDatePicker.Focus();
                    return;
                }

                // Заполняем данные - просто сохраняем дату как есть
                _author.FirstName = FirstNameBox.Text.Trim();
                _author.LastName = LastNameBox.Text.Trim();
                _author.Country = CountryBox.Text?.Trim() ?? "";
                _author.BirthDate = BirthDatePicker.SelectedDate.Value;

                // Сохраняем в базу
                if (!_isEditMode)
                {
                    _context.Authors.Add(_author);
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