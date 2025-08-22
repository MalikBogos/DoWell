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
        private ObservableCollection<Worksheet> _worksheets = new();

        [ObservableProperty]
        private Worksheet? _selectedWorksheet;

        [ObservableProperty]
        private ObservableCollection<ObservableCollection<CellViewModel>> _gridData = new();

        [ObservableProperty]
        private int _rowCount;

        [ObservableProperty]
        private int _columnCount;

        [ObservableProperty]
        private CellViewModel? _selectedCell;

        [ObservableProperty]
        private string _searchText = string.Empty;

        public MainViewModel()
        {
            _context = new DoWellContext();
            LoadWorksheets();
        }

        private void LoadWorksheets()
        {
            try
            {
                var worksheets = _context.Worksheets
                    .Include(w => w.Cells)
                    .ToList();

                Worksheets = new ObservableCollection<Worksheet>(worksheets);

                if (Worksheets.Any())
                {
                    SelectedWorksheet = Worksheets.First();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading worksheets: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        partial void OnSelectedWorksheetChanged(Worksheet? value)
        {
            if (value != null)
            {
                LoadGridData();
            }
        }

        private void LoadGridData()
        {
            if (SelectedWorksheet == null) return;

            try
            {
                RowCount = SelectedWorksheet.RowCount;
                ColumnCount = SelectedWorksheet.ColumnCount;

                GridData = new ObservableCollection<ObservableCollection<CellViewModel>>();

                for (int row = 0; row < RowCount; row++)
                {
                    var rowData = new ObservableCollection<CellViewModel>();
                    for (int col = 0; col < ColumnCount; col++)
                    {
                        var cell = SelectedWorksheet.Cells
                            .FirstOrDefault(c => c.Row == row && c.Column == col)
                            ?? new Cell { Row = row, Column = col, WorksheetId = SelectedWorksheet.WorksheetId };

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
            if (SelectedWorksheet == null) return;

            try
            {
                var newRow = new ObservableCollection<CellViewModel>();
                for (int col = 0; col < ColumnCount; col++)
                {
                    var cell = new Cell
                    {
                        Row = RowCount,
                        Column = col,
                        WorksheetId = SelectedWorksheet.WorksheetId
                    };
                    newRow.Add(new CellViewModel(cell));
                }

                GridData.Add(newRow);
                RowCount++;
                SelectedWorksheet.RowCount = RowCount;

                SaveChanges();
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
            if (SelectedWorksheet == null || GridData.Count <= 1) return;

            try
            {
                var result = MessageBox.Show("Are you sure you want to remove the last row?",
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Remove cells from database
                    var lastRowIndex = RowCount - 1;
                    var cellsToRemove = _context.Cells
                        .Where(c => c.WorksheetId == SelectedWorksheet.WorksheetId && c.Row == lastRowIndex)
                        .ToList();

                    _context.Cells.RemoveRange(cellsToRemove);

                    GridData.RemoveAt(GridData.Count - 1);
                    RowCount--;
                    SelectedWorksheet.RowCount = RowCount;

                    SaveChanges();
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
            if (SelectedWorksheet == null) return;

            try
            {
                for (int row = 0; row < RowCount; row++)
                {
                    var cell = new Cell
                    {
                        Row = row,
                        Column = ColumnCount,
                        WorksheetId = SelectedWorksheet.WorksheetId
                    };
                    GridData[row].Add(new CellViewModel(cell));
                }

                ColumnCount++;
                SelectedWorksheet.ColumnCount = ColumnCount;

                SaveChanges();
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
            if (SelectedWorksheet == null || ColumnCount <= 1) return;

            try
            {
                var result = MessageBox.Show("Are you sure you want to remove the last column?",
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Remove cells from database
                    var lastColIndex = ColumnCount - 1;
                    var cellsToRemove = _context.Cells
                        .Where(c => c.WorksheetId == SelectedWorksheet.WorksheetId && c.Column == lastColIndex)
                        .ToList();

                    _context.Cells.RemoveRange(cellsToRemove);

                    foreach (var row in GridData)
                    {
                        row.RemoveAt(row.Count - 1);
                    }

                    ColumnCount--;
                    SelectedWorksheet.ColumnCount = ColumnCount;

                    SaveChanges();
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

                        var existingCell = _context.Cells.Local
                            .FirstOrDefault(c => c.CellId == cell.CellId);

                        if (existingCell == null && cell.CellId == 0)
                        {
                            _context.Cells.Add(cell);
                        }
                        else if (existingCell == null)
                        {
                            _context.Cells.Update(cell);
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

        [RelayCommand]
        private void SearchCells()
        {
            if (string.IsNullOrWhiteSpace(SearchText) || GridData == null) return;

            try
            {
                var found = false;
                foreach (var row in GridData)
                {
                    foreach (var cell in row)
                    {
                        if (cell.Value?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            SelectedCell = cell;
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }

                if (!found)
                {
                    MessageBox.Show($"'{SearchText}' not found", "Search Result",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during search: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void NewWorkbook()
        {
            try
            {
                var newWorkbook = new Workbook
                {
                    Name = $"New Workbook {DateTime.Now:HH-mm-ss}",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                var newWorksheet = new Worksheet
                {
                    Name = "Sheet1",
                    RowCount = 10,
                    ColumnCount = 10,
                    Workbook = newWorkbook
                };

                newWorkbook.Worksheets.Add(newWorksheet);

                _context.Workbooks.Add(newWorkbook);
                _context.SaveChanges();

                LoadWorksheets();
                SelectedWorksheet = newWorksheet;

                MessageBox.Show("New workbook created!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating workbook: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Open()
        {
            try
            {
                // In een echte applicatie zou je hier een file dialog openen
                // Voor nu laden we gewoon de workbooks opnieuw
                LoadWorksheets();
                MessageBox.Show("Workbooks refreshed from database", "Open",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}