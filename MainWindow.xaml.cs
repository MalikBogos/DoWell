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
using DoWell.Views;
using Microsoft.EntityFrameworkCore;

namespace DoWell
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel = null!;
        private DataGrid? _currentDataGrid;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainViewModel)DataContext;

            // Initialize after window is loaded
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeColorPickers();

            // Subscribe to columns changed event
            if (_viewModel != null)
            {
                _viewModel.ColumnsChanged += OnColumnsChanged;
            }
        }

        private void OnColumnsChanged(object? sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_currentDataGrid != null)
                {
                    UpdateDataGridColumns();
                }
            }));
        }

        private void InitializeColorPickers()
        {
            var colors = new[]
            {
                new { Name = "White", Color = "#FFFFFF" },
                new { Name = "Black", Color = "#000000" },
                new { Name = "Red", Color = "#FF0000" },
                new { Name = "Green", Color = "#00FF00" },
                new { Name = "Blue", Color = "#0000FF" },
                new { Name = "Yellow", Color = "#FFFF00" },
                new { Name = "Orange", Color = "#FFA500" },
                new { Name = "Purple", Color = "#800080" },
                new { Name = "Gray", Color = "#808080" },
                new { Name = "Light Blue", Color = "#ADD8E6" },
                new { Name = "Light Green", Color = "#90EE90" },
                new { Name = "Light Gray", Color = "#D3D3D3" },
                new { Name = "Pink", Color = "#FFC0CB" },
                new { Name = "Brown", Color = "#A52A2A" },
                new { Name = "Navy", Color = "#000080" }
            };

            BackgroundColorPicker.ItemsSource = colors;
            BackgroundColorPicker.SelectedIndex = 0;

            ForegroundColorPicker.ItemsSource = colors;
            ForegroundColorPicker.SelectedIndex = 1;
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _currentDataGrid = sender as DataGrid;

            // Only update columns if we have data
            if (_viewModel != null && _viewModel.GridData != null && _viewModel.GridData.Count > 0)
            {
                UpdateDataGridColumns();
            }
        }

        private void UpdateDataGridColumns()
        {
            try
            {
                if (_currentDataGrid == null || _viewModel == null ||
                    _viewModel.GridData == null || _viewModel.GridData.Count == 0)
                    return;

                _currentDataGrid.Columns.Clear();
                _currentDataGrid.ItemsSource = null;

                // Add row header column
                var rowHeaderColumn = new DataGridTextColumn
                {
                    Header = "",
                    Width = 40,
                    IsReadOnly = true,
                    Binding = new Binding("[0]")
                    {
                        Converter = new RowNumberConverter()
                    }
                };
                _currentDataGrid.Columns.Add(rowHeaderColumn);

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
                    _currentDataGrid.Columns.Add(column);
                }

                _currentDataGrid.ItemsSource = _viewModel.GridData;
            }
            catch (Exception ex)
            {
                _viewModel.StatusMessage = $"Error updating columns: {ex.Message}";
                _viewModel.IsStatusSuccess = false;
            }
        }

        private DataTemplate CreateCellTemplate(int columnIndex)
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(TextBlock));

            // Gebruik een MultiBinding met een converter voor veilige indexing
            var binding = new Binding($"[{columnIndex}]");
            binding.FallbackValue = new CellViewModel(new Cell());

            factory.SetBinding(TextBlock.TextProperty, new Binding($"[{columnIndex}].Value")
            {
                FallbackValue = "",
                TargetNullValue = ""
            });

            factory.SetBinding(TextBlock.BackgroundProperty, new Binding($"[{columnIndex}].BackgroundBrush")
            {
                FallbackValue = Brushes.White,
                TargetNullValue = Brushes.White
            });

            factory.SetBinding(TextBlock.ForegroundProperty, new Binding($"[{columnIndex}].ForegroundBrush")
            {
                FallbackValue = Brushes.Black,
                TargetNullValue = Brushes.Black
            });

            // Apply formatting met fallback values
            factory.SetBinding(TextBlock.FontWeightProperty, new Binding($"[{columnIndex}].IsBold")
            {
                Converter = new BoolToFontWeightConverter(),
                FallbackValue = FontWeights.Normal,
                TargetNullValue = FontWeights.Normal
            });

            factory.SetBinding(TextBlock.FontStyleProperty, new Binding($"[{columnIndex}].IsItalic")
            {
                Converter = new BoolToFontStyleConverter(),
                FallbackValue = FontStyles.Normal,
                TargetNullValue = FontStyles.Normal
            });

            factory.SetBinding(TextBlock.TextDecorationsProperty, new Binding($"[{columnIndex}].IsUnderline")
            {
                Converter = new BoolToTextDecorationConverter(),
                FallbackValue = null,
                TargetNullValue = null
            });

            factory.SetValue(TextBlock.PaddingProperty, new Thickness(2));

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
            factory.SetBinding(TextBox.BackgroundProperty, new Binding($"[{columnIndex}].BackgroundBrush"));
            factory.SetBinding(TextBox.ForegroundProperty, new Binding($"[{columnIndex}].ForegroundBrush"));

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
                // Auto-save after editing
                _viewModel.SaveChangesCommand.Execute(null);
            }
            catch (Exception ex)
            {
                _viewModel.StatusMessage = $"Error saving cell: {ex.Message}";
                _viewModel.IsStatusSuccess = false;
            }
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (_viewModel == null || _viewModel.GridData == null)
                    return;

                var dataGrid = sender as DataGrid;
                if (dataGrid?.CurrentCell.Column != null && dataGrid.CurrentCell.Item != null)
                {
                    var rowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentCell.Item);
                    var colIndex = dataGrid.CurrentCell.Column.DisplayIndex - 1; // -1 for row header

                    if (colIndex >= 0 && rowIndex >= 0 &&
                        rowIndex < _viewModel.GridData.Count &&
                        colIndex < _viewModel.GridData[rowIndex].Count)
                    {
                        _viewModel.SelectedCell = _viewModel.GridData[rowIndex][colIndex];

                        // Update formatting buttons only if they exist
                        if (BoldButton != null)
                            BoldButton.IsChecked = _viewModel.SelectedCell?.IsBold ?? false;
                        if (ItalicButton != null)
                            ItalicButton.IsChecked = _viewModel.SelectedCell?.IsItalic ?? false;
                        if (UnderlineButton != null)
                            UnderlineButton.IsChecked = _viewModel.SelectedCell?.IsUnderline ?? false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Selection error: {ex.Message}");
            }
        }

        // Formatting button event handlers
        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedCell != null)
                {
                    _viewModel.SelectedCell.IsBold = BoldButton.IsChecked ?? false;
                    _viewModel.SaveChangesCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying bold: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedCell != null)
                {
                    _viewModel.SelectedCell.IsItalic = ItalicButton.IsChecked ?? false;
                    _viewModel.SaveChangesCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying italic: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedCell != null)
                {
                    _viewModel.SelectedCell.IsUnderline = UnderlineButton.IsChecked ?? false;
                    _viewModel.SaveChangesCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying underline: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackgroundColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (BackgroundColorPicker.SelectedItem != null)
                {
                    dynamic selected = BackgroundColorPicker.SelectedItem;
                    _viewModel.SelectedBackgroundColor = selected.Color;
                    _viewModel.ApplyBackgroundColorCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying background color: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForegroundColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ForegroundColorPicker.SelectedItem != null)
                {
                    dynamic selected = ForegroundColorPicker.SelectedItem;
                    _viewModel.SelectedForegroundColor = selected.Color;
                    _viewModel.ApplyForegroundColorCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying foreground color: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TemplateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (TemplateComboBox.SelectedItem is FormatTemplate template)
                {
                    _viewModel.ApplyFormatTemplateCommand.Execute(template);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying template: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (TemplateComboBox.SelectedItem is FormatTemplate template)
            {
                _viewModel.ApplyFormatTemplateCommand.Execute(template);
            }
        }

        private void WorksheetTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Check if this is during initialization
                if (!IsLoaded || _viewModel == null)
                    return;

                if (e.AddedItems.Count > 0 && e.AddedItems[0] is Worksheet worksheet)
                {
                    _viewModel.SwitchWorksheetCommand.Execute(worksheet);

                    // Only update DataGrid if it exists
                    if (_currentDataGrid != null)
                    {
                        Dispatcher.BeginInvoke(new Action(() => UpdateDataGridColumns()));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error switching worksheet: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RenameWorksheet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is MenuItem menuItem && menuItem.DataContext is Worksheet worksheet)
                {
                    var dialog = new InputDialog("Rename Worksheet", "Enter new name:", worksheet.Name);
                    if (dialog.ShowDialog() == true)
                    {
                        worksheet.Name = dialog.ResponseText;
                        _viewModel.SaveChangesCommand.Execute(null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error renaming worksheet: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteWorksheet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.Worksheets.Count <= 1)
                {
                    MessageBox.Show("Cannot delete the last worksheet.", "Delete Worksheet",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this worksheet?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes && sender is MenuItem menuItem &&
                    menuItem.DataContext is Worksheet worksheet)
                {
                    _viewModel.Worksheets.Remove(worksheet);
                    // Implementation would include database removal
                    _viewModel.SaveChangesCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting worksheet: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FormatCell_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedCell != null)
                {
                    var dialog = new FormatCellDialog(_viewModel.SelectedCell);
                    if (dialog.ShowDialog() == true)
                    {
                        _viewModel.SaveChangesCommand.Execute(null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error formatting cell: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearContents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedCell != null)
                {
                    _viewModel.SelectedCell.Value = "";
                    _viewModel.SaveChangesCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing contents: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Do you want to save changes before exiting?",
                "Exit DoWell", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _viewModel.SaveWorkbookCommand.Execute(null);
                Application.Current.Shutdown();
            }
            else if (result == MessageBoxResult.No)
            {
                Application.Current.Shutdown();
            }
        }

        // Window closing event
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Unsubscribe from events
            if (_viewModel != null)
            {
                _viewModel.ColumnsChanged -= OnColumnsChanged;
            }

            var result = MessageBox.Show("Do you want to save changes before closing?",
                "Close DoWell", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _viewModel.SaveWorkbookCommand.Execute(null);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }

            base.OnClosing(e);
        }
    }

    // Converters for text formatting
    public class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is CellViewModel cell)
                return cell.Row.ToString();
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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

    public class BoolToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? TextDecorations.Underline : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null && ((TextDecorationCollection)value).Count > 0;
        }
    }
}