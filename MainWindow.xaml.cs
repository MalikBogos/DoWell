// MainWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DoWell.Data;
using DoWell.Models;
using DoWell.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DoWell
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel = null!;
        private DoWellContext _context = null!;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainViewModel)DataContext;
            _context = new DoWellContext();
            InitializeDataGrid();
            LoadUsersGrid();
        }

        private void InitializeDataGrid()
        {
            try
            {
                // Subscribe to GridData changes
                _viewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(_viewModel.GridData))
                    {
                        UpdateDataGridColumns();
                    }
                };

                UpdateDataGridColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing grid: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateDataGridColumns()
        {
            if (_viewModel.GridData == null || _viewModel.GridData.Count == 0) return;

            try
            {
                MainDataGrid.Columns.Clear();
                MainDataGrid.ItemsSource = null;

                // Add row header column
                var rowHeaderColumn = new DataGridTextColumn
                {
                    Header = "",
                    Width = 40,
                    IsReadOnly = true,
                    Binding = new Binding("Row") { StringFormat = "{0}" }
                };
                MainDataGrid.Columns.Add(rowHeaderColumn);

                // Add data columns
                for (int col = 0; col < _viewModel.ColumnCount; col++)
                {
                    var column = new DataGridTemplateColumn
                    {
                        Header = GetColumnName(col),
                        Width = new DataGridLength(100),
                        CellTemplate = CreateCellTemplate(col),
                        CellEditingTemplate = CreateCellEditingTemplate(col)
                    };
                    MainDataGrid.Columns.Add(column);
                }

                MainDataGrid.ItemsSource = _viewModel.GridData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating columns: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DataTemplate CreateCellTemplate(int columnIndex)
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new Binding($"[{columnIndex}].Value"));

            // Apply formatting
            var boldBinding = new Binding($"[{columnIndex}].IsBold");
            var italicBinding = new Binding($"[{columnIndex}].IsItalic");
            var underlineBinding = new Binding($"[{columnIndex}].IsUnderline");

            factory.SetBinding(TextBlock.FontWeightProperty, new Binding($"[{columnIndex}].IsBold")
            {
                Converter = new BoolToFontWeightConverter()
            });
            factory.SetBinding(TextBlock.FontStyleProperty, new Binding($"[{columnIndex}].IsItalic")
            {
                Converter = new BoolToFontStyleConverter()
            });

            template.VisualTree = factory;
            return template;
        }

        private DataTemplate CreateCellEditingTemplate(int columnIndex)
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(TextBox));
            factory.SetBinding(TextBox.TextProperty, new Binding($"[{columnIndex}].Value")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            template.VisualTree = factory;
            return template;
        }

        private string GetColumnName(int columnIndex)
        {
            string columnName = "";
            while (columnIndex >= 0)
            {
                columnName = (char)('A' + columnIndex % 26) + columnName;
                columnIndex = columnIndex / 26 - 1;
            }
            return columnName;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                // The cell value will be automatically updated through binding
                // We can save changes here if needed
                _viewModel.SaveChangesCommand.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving cell: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (MainDataGrid.CurrentCell.Column != null && MainDataGrid.CurrentCell.Item != null)
                {
                    var rowIndex = MainDataGrid.Items.IndexOf(MainDataGrid.CurrentCell.Item);
                    var colIndex = MainDataGrid.CurrentCell.Column.DisplayIndex - 1; // -1 for row header

                    if (colIndex >= 0 && rowIndex >= 0 &&
                        rowIndex < _viewModel.GridData.Count &&
                        colIndex < _viewModel.GridData[rowIndex].Count)
                    {
                        _viewModel.SelectedCell = _viewModel.GridData[rowIndex][colIndex];

                        // Update formatting buttons
                        BoldButton.IsChecked = _viewModel.SelectedCell?.IsBold ?? false;
                        ItalicButton.IsChecked = _viewModel.SelectedCell?.IsItalic ?? false;
                        UnderlineButton.IsChecked = _viewModel.SelectedCell?.IsUnderline ?? false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Silently handle selection errors
                Console.WriteLine($"Selection error: {ex.Message}");
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DoWell Excel Clone\nVersion 1.0\n\nA simple spreadsheet application built with WPF and Entity Framework.",
                "About DoWell", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Guide_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Quick Guide:\n\n" +
                "• Click on any cell to edit its content\n" +
                "• Use the toolbar buttons to add/remove rows and columns\n" +
                "• Select a cell and use B/I/U buttons to format text\n" +
                "• Right-click for context menu options\n" +
                "• Use the search box to find content in cells\n" +
                "• Switch between tabs to manage data",
                "Quick Guide", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FormatCell_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedCell != null)
            {
                // Show formatting options
                _viewModel.SelectedCell.IsBold = !_viewModel.SelectedCell.IsBold;
                _viewModel.SaveChangesCommand.Execute(null);

                // Update the formatting buttons
                BoldButton.IsChecked = _viewModel.SelectedCell.IsBold;

                MessageBox.Show("Cell formatting toggled!", "Format", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a cell first", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearCell_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedCell != null)
            {
                _viewModel.SelectedCell.Value = "";
                _viewModel.SelectedCell.IsBold = false;
                _viewModel.SelectedCell.IsItalic = false;
                _viewModel.SelectedCell.IsUnderline = false;
                _viewModel.SaveChangesCommand.Execute(null);
            }
        }

        private void RenameWorksheet_Click(object sender, RoutedEventArgs e)
        {
            // Simplified rename - in production would use input dialog
            if (_viewModel.SelectedWorksheet != null)
            {
                _viewModel.SelectedWorksheet.Name = "Renamed_" + DateTime.Now.Ticks;
                _viewModel.SaveChangesCommand.Execute(null);
            }
        }

        private void DeleteWorksheet_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Delete functionality would be implemented here", "Delete",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DuplicateWorksheet_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Duplicate functionality would be implemented here", "Duplicate",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = (TableSelector.SelectedItem as ComboBoxItem)?.Content.ToString();
                switch (selected)
                {
                    case "Workbooks":
                        ManagementGrid.ItemsSource = _context.Workbooks.ToList();
                        break;
                    case "Worksheets":
                        ManagementGrid.ItemsSource = _context.Worksheets.ToList();
                        break;
                    case "Users":
                        ManagementGrid.ItemsSource = _context.Users.ToList();
                        break;
                    case "Cells":
                        ManagementGrid.ItemsSource = _context.Cells.Take(100).ToList(); // Limit for performance
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUsersGrid()
        {
            try
            {
                UsersGrid.ItemsSource = _context.Users.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterUsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filterText = UserFilter.Text.ToLower();
                if (string.IsNullOrWhiteSpace(filterText))
                {
                    UsersGrid.ItemsSource = _context.Users.ToList();
                }
                else
                {
                    // Using LINQ to filter users
                    var filteredUsers = _context.Users
                        .Where(u => u.Username.ToLower().Contains(filterText))
                        .ToList();
                    UsersGrid.ItemsSource = filteredUsers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering users: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Formatting button event handlers
        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedCell != null)
            {
                _viewModel.SelectedCell.IsBold = BoldButton.IsChecked ?? false;
                _viewModel.SaveChangesCommand.Execute(null);
            }
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedCell != null)
            {
                _viewModel.SelectedCell.IsItalic = ItalicButton.IsChecked ?? false;
                _viewModel.SaveChangesCommand.Execute(null);
            }
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedCell != null)
            {
                _viewModel.SelectedCell.IsUnderline = UnderlineButton.IsChecked ?? false;
                _viewModel.SaveChangesCommand.Execute(null);
            }
        }
    }

    // Converters for text formatting
    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return FontWeights.Bold.Equals(value);
        }
    }

    public class BoolToFontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? FontStyles.Italic : FontStyles.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return FontStyles.Italic.Equals(value);
        }
    }
}