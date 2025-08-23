using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoWell.Data;
using DoWell.Models;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace DoWell.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly DoWellContext _context;

        [ObservableProperty]
        private ObservableCollection<ObservableCollection<CellViewModel>> _gridData = new();

        [ObservableProperty]
        private int _rowCount = 10;

        [ObservableProperty]
        private int _columnCount = 10;

        [ObservableProperty]
        private CellViewModel? _selectedCell;

        public MainViewModel()
        {
            _context = new DoWellContext();
            LoadGridData();
        }

        private void LoadGridData()
        {
            try
            {
                // Load all cells from database
                var allCells = _context.Cells.ToList();

                GridData = new ObservableCollection<ObservableCollection<CellViewModel>>();

                for (int row = 0; row < RowCount; row++)
                {
                    var rowData = new ObservableCollection<CellViewModel>();
                    for (int col = 0; col < ColumnCount; col++)
                    {
                        var cell = allCells.FirstOrDefault(c => c.Row == row && c.Column == col)
                                  ?? new Cell { Row = row, Column = col };

                        rowData.Add(new CellViewModel(cell));
                    }
                    GridData.Add(rowData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading grid data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void AddRow()
        {
            try
            {
                var newRow = new ObservableCollection<CellViewModel>();
                for (int col = 0; col < ColumnCount; col++)
                {
                    var cell = new Cell
                    {
                        Row = RowCount,
                        Column = col
                    };
                    newRow.Add(new CellViewModel(cell));
                }

                GridData.Add(newRow);
                RowCount++;

                MessageBox.Show("Row added successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding row: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void RemoveRow()
        {
            if (GridData.Count <= 1) return;

            try
            {
                var result = MessageBox.Show("Are you sure you want to remove the last row?",
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Remove cells from database
                    var lastRowIndex = RowCount - 1;
                    var cellsToRemove = _context.Cells
                        .Where(c => c.Row == lastRowIndex)
                        .ToList();

                    _context.Cells.RemoveRange(cellsToRemove);
                    _context.SaveChanges();

                    GridData.RemoveAt(GridData.Count - 1);
                    RowCount--;

                    MessageBox.Show("Row removed successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing row: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void AddColumn()
        {
            try
            {
                for (int row = 0; row < RowCount; row++)
                {
                    var cell = new Cell
                    {
                        Row = row,
                        Column = ColumnCount
                    };
                    GridData[row].Add(new CellViewModel(cell));
                }

                ColumnCount++;

                MessageBox.Show("Column added successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding column: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void RemoveColumn()
        {
            if (ColumnCount <= 1) return;

            try
            {
                var result = MessageBox.Show("Are you sure you want to remove the last column?",
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Remove cells from database
                    var lastColIndex = ColumnCount - 1;
                    var cellsToRemove = _context.Cells
                        .Where(c => c.Column == lastColIndex)
                        .ToList();

                    _context.Cells.RemoveRange(cellsToRemove);
                    _context.SaveChanges();

                    foreach (var row in GridData)
                    {
                        row.RemoveAt(row.Count - 1);
                    }

                    ColumnCount--;

                    MessageBox.Show("Column removed successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing column: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void SaveChanges()
        {
            try
            {
                // Update or add cells
                foreach (var row in GridData)
                {
                    foreach (var cellVm in row)
                    {
                        cellVm.UpdateCell();
                        var cell = cellVm.GetCell();

                        if (string.IsNullOrEmpty(cell.Value) && cell.CellId == 0)
                            continue; // Skip empty new cells

                        var existingCell = _context.Cells
                            .FirstOrDefault(c => c.Row == cell.Row && c.Column == cell.Column);

                        if (existingCell == null)
                        {
                            if (!string.IsNullOrEmpty(cell.Value) || cell.IsBold || cell.IsItalic || cell.IsUnderline)
                            {
                                _context.Cells.Add(cell);
                            }
                        }
                        else
                        {
                            existingCell.Value = cell.Value;
                            existingCell.IsBold = cell.IsBold;
                            existingCell.IsItalic = cell.IsItalic;
                            existingCell.IsUnderline = cell.IsUnderline;
                        }
                    }
                }

                _context.SaveChanges();
                MessageBox.Show("Changes saved successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}