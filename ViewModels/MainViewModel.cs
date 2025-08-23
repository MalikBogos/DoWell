using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoWell.Data;
using DoWell.Models;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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

        [ObservableProperty]
        private string _statusMessage = "Ready";

        [ObservableProperty]
        private bool _isStatusSuccess = true;

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

                // Determine actual grid size from database
                if (allCells.Any())
                {
                    RowCount = Math.Max(allCells.Max(c => c.Row) + 1, 10);
                    ColumnCount = Math.Max(allCells.Max(c => c.Column) + 1, 10);
                }

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

                SetStatusMessage("Data loaded successfully", true);
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error loading data: {ex.Message}", false);
            }
        }

        private void SetStatusMessage(string message, bool isSuccess)
        {
            StatusMessage = message;
            IsStatusSuccess = isSuccess;

            // Auto-clear success messages after 3 seconds
            if (isSuccess)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    StatusMessage = "Ready";
                });
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

                // Save to database immediately
                SaveChangesToDatabase();
                SetStatusMessage("Row added successfully", true);
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error adding row: {ex.Message}", false);
            }
        }

        [RelayCommand]
        private void RemoveRow()
        {
            if (GridData.Count <= 1)
            {
                SetStatusMessage("Cannot remove last row", false);
                return;
            }

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

                    SetStatusMessage("Row removed successfully", true);
                }
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error removing row: {ex.Message}", false);
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

                // Force UI update by creating new collection
                var tempData = new ObservableCollection<ObservableCollection<CellViewModel>>(GridData);
                GridData = tempData;

                // Save to database immediately
                SaveChangesToDatabase();
                SetStatusMessage("Column added successfully", true);
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error adding column: {ex.Message}", false);
            }
        }

        [RelayCommand]
        private void RemoveColumn()
        {
            if (ColumnCount <= 1)
            {
                SetStatusMessage("Cannot remove last column", false);
                return;
            }

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

                    // Force UI update
                    var tempData = new ObservableCollection<ObservableCollection<CellViewModel>>(GridData);
                    GridData = tempData;

                    SetStatusMessage("Column removed successfully", true);
                }
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error removing column: {ex.Message}", false);
            }
        }

        [RelayCommand]
        private void SaveChanges()
        {
            try
            {
                SaveChangesToDatabase();
                SetStatusMessage("Changes saved successfully", true);
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error saving changes: {ex.Message}", false);
            }
        }

        private void SaveChangesToDatabase()
        {
            // Update or add cells
            foreach (var row in GridData)
            {
                foreach (var cellVm in row)
                {
                    cellVm.UpdateCell();
                    var cell = cellVm.GetCell();

                    var existingCell = _context.Cells
                        .FirstOrDefault(c => c.Row == cell.Row && c.Column == cell.Column);

                    if (existingCell == null)
                    {
                        // Only add cells that have content or formatting
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
        }
    }
}