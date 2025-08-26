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

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainViewModel)DataContext;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeColorPickers();

            if (_viewModel != null)
            {
                _viewModel.ColumnsChanged += OnColumnsChanged;
            }
        }

        private void OnColumnsChanged(object? sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateDataGridColumns();
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
            if (_viewModel != null && _viewModel.GridData != null && _viewModel.GridData.Count > 0)
            {
                UpdateDataGridColumns();
            }
        }

        private void UpdateDataGridColumns()
        {
            try
            {
                if (_viewModel == null || _viewModel.GridData == null || _viewModel.GridData.Count == 0)
                    return;

                MainDataGrid.Columns.Clear();
                MainDataGrid.ItemsSource = null;

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
                MainDataGrid.Columns.Add(rowHeaderColumn);

                // Add data columns - ALLEEN voor bestaande kolommen
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
                _viewModel.StatusMessage = $"Error updating columns: {ex.Message}";
                _viewModel.IsStatusSuccess = false;
            }
        }

        private DataTemplate CreateCellTemplate(int columnIndex)
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(TextBlock));

            // Direct binding naar specifieke kolom met fallback values
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

            // Direct binding naar de specifieke kolom index met Path
            factory.SetBinding(TextBox.TextProperty, new Binding($"[{columnIndex}].Value")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay,
                FallbackValue = "",
                TargetNullValue = ""
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

        // Event handlers
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
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

    // Eenvoudige converters voor text formatting
    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return (bool)value ? FontWeights.Bold : FontWeights.Normal;
            }
            catch
            {
                return FontWeights.Normal;
            }
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
            try
            {
                return (bool)value ? FontStyles.Italic : FontStyles.Normal;
            }
            catch
            {
                return FontStyles.Normal;
            }
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
            try
            {
                return (bool)value ? TextDecorations.Underline : null;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null && ((TextDecorationCollection)value).Count > 0;
        }
    }

    public class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is System.Collections.ObjectModel.ObservableCollection<CellViewModel> row && row.Count > 0)
                {
                    return (row[0].Row + 1).ToString();
                }
                return "1";
            }
            catch
            {
                return "1";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}