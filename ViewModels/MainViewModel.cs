// ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoWell.Data;
using DoWell.Models;
using DoWell.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

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

        [ObservableProperty]
        private Workbook? _currentWorkbook;

        [ObservableProperty]
        private ObservableCollection<FormatTemplate> _formatTemplates = new();

        [ObservableProperty]
        private string _selectedBackgroundColor = "#FFFFFF";

        [ObservableProperty]
        private string _selectedForegroundColor = "#000000";

        public event EventHandler? ColumnsChanged;

        public MainViewModel()
        {
            _context = new DoWellContext();
            _context.Database.EnsureCreated();
            InitializeWorkbook();
        }

        private void InitializeWorkbook()
        {
            try
            {
                CurrentWorkbook = _context.Workbooks
                    .Include(w => w.Cells)
                    .Include(w => w.FormatTemplates)
                    .FirstOrDefault();

                if (CurrentWorkbook == null)
                {
                    CurrentWorkbook = CreateNewWorkbook();
                }

                FormatTemplates = new ObservableCollection<FormatTemplate>(CurrentWorkbook.FormatTemplates);
                LoadWorkbookData();
                SetStatusMessage("Workbook loaded successfully", true);
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error loading workbook: {ex.Message}", false);
                MessageBox.Show($"Error initializing workbook: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Workbook CreateNewWorkbook()
        {
            var workbook = new Workbook
            {
                Name = "New Workbook",
                Author = Environment.UserName,
                CreatedDate = DateTime.Now,
                LastSavedDate = DateTime.Now
            };

            _context.Workbooks.Add(workbook);
            _context.SaveChanges();
            return workbook;
        }

        private void LoadWorkbookData()
        {
            try
            {
                if (CurrentWorkbook == null)
                {
                    SetStatusMessage("No workbook selected", false);
                    return;
                }

                var allCells = _context.Cells
                    .Where(c => c.WorkbookId == CurrentWorkbook.WorkbookId)
                    .Include(c => c.FormatTemplate)
                    .ToList();

                // Bepaal grid grootte van database
                if (allCells.Any())
                {
                    RowCount = Math.Max(allCells.Max(c => c.Row) + 1, 10);
                    ColumnCount = Math.Max(allCells.Max(c => c.Column) + 1, 10);
                }
                else
                {
                    RowCount = 10;
                    ColumnCount = 10;
                }

                GridData = new ObservableCollection<ObservableCollection<CellViewModel>>();

                for (int row = 0; row < RowCount; row++)
                {
                    var rowData = new ObservableCollection<CellViewModel>();
                    for (int col = 0; col < ColumnCount; col++)
                    {
                        var cell = allCells.FirstOrDefault(c => c.Row == row && c.Column == col)
                                  ?? new Cell
                                  {
                                      Row = row,
                                      Column = col,
                                      WorkbookId = CurrentWorkbook.WorkbookId,
                                      BackgroundColor = "#FFFFFF",
                                      ForegroundColor = "#000000"
                                  };

                        rowData.Add(new CellViewModel(cell));
                    }
                    GridData.Add(rowData);
                }

                SetStatusMessage($"Loaded workbook: {CurrentWorkbook.Name}", true);
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error loading workbook: {ex.Message}", false);
                MessageBox.Show($"Error loading workbook data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetStatusMessage(string message, bool isSuccess)
        {
            StatusMessage = message;
            IsStatusSuccess = isSuccess;

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
        private void NewWorkbook()
        {
            try
            {
                var result = MessageBox.Show("Save current workbook before creating new?",
                    "New Workbook", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveWorkbook();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }

                CurrentWorkbook = CreateNewWorkbook();
                InitializeWorkbook();
                SetStatusMessage("New workbook created", true);
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error creating new workbook: {ex.Message}", false);
            }
        }

        [RelayCommand]
        private void AddRow()
        {
            try
            {
                if (CurrentWorkbook == null)
                {
                    MessageBox.Show("Please select a workbook first.", "No Workbook",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newRow = new ObservableCollection<CellViewModel>();
                for (int col = 0; col < ColumnCount; col++)
                {
                    var cell = new Cell
                    {
                        Row = RowCount,
                        Column = col,
                        WorkbookId = CurrentWorkbook.WorkbookId,
                        BackgroundColor = "#FFFFFF",
                        ForegroundColor = "#000000"
                    };
                    newRow.Add(new CellViewModel(cell));
                }

                GridData.Add(newRow);
                RowCount++;

                SaveChangesToDatabase();
                SetStatusMessage("Row added successfully", true);
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error adding row: {ex.Message}", false);
                MessageBox.Show($"Error adding row: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (CurrentWorkbook == null)
                {
                    MessageBox.Show("Please select a workbook first.", "No Workbook",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to remove the last row?",
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var lastRowIndex = RowCount - 1;
                    var cellsToRemove = _context.Cells
                        .Where(c => c.WorkbookId == CurrentWorkbook.WorkbookId && c.Row == lastRowIndex)
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
                MessageBox.Show($"Error removing row: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void AddColumn()
        {
            try
            {
                if (CurrentWorkbook == null)
                {
                    MessageBox.Show("Please select a workbook first.", "No Workbook",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                for (int row = 0; row < RowCount; row++)
                {
                    var cell = new Cell
                    {
                        Row = row,
                        Column = ColumnCount,
                        WorkbookId = CurrentWorkbook.WorkbookId,
                        BackgroundColor = "#FFFFFF",
                        ForegroundColor = "#000000"
                    };
                    GridData[row].Add(new CellViewModel(cell));
                }

                ColumnCount++;

                SaveChangesToDatabase();
                ColumnsChanged?.Invoke(this, EventArgs.Empty);
                SetStatusMessage("Column added successfully", true);
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error adding column: {ex.Message}", false);
                MessageBox.Show($"Error adding column: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                if (CurrentWorkbook == null)
                {
                    MessageBox.Show("Please select a workbook first.", "No Workbook",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to remove the last column?",
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var lastColIndex = ColumnCount - 1;

                    // Create a new GridData collection instead of modifying the existing one
                    var newGridData = new ObservableCollection<ObservableCollection<CellViewModel>>();

                    for (int row = 0; row < RowCount; row++)
                    {
                        var newRow = new ObservableCollection<CellViewModel>();
                        for (int col = 0; col < lastColIndex; col++)
                        {
                            newRow.Add(GridData[row][col]);
                        }
                        newGridData.Add(newRow);
                    }

                    // Remove from database
                    var cellsToRemove = _context.Cells
                        .Where(c => c.WorkbookId == CurrentWorkbook.WorkbookId && c.Column == lastColIndex)
                        .ToList();

                    _context.Cells.RemoveRange(cellsToRemove);
                    _context.SaveChanges();

                    // Update column count first
                    ColumnCount--;

                    // Replace GridData entirely - this forces complete rebinding
                    GridData = newGridData;

                    // Trigger columns changed
                    ColumnsChanged?.Invoke(this, EventArgs.Empty);

                    SetStatusMessage("Column removed successfully", true);
                }
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error removing column: {ex.Message}", false);
                MessageBox.Show($"Error removing column: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void SaveWorkbook()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "DoWell Files (*.dwl)|*.dwl|JSON Files (*.json)|*.json",
                    DefaultExt = ".dwl",
                    FileName = CurrentWorkbook?.Name ?? "Workbook"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    SaveChangesToDatabase();

                    var exportData = new
                    {
                        Workbook = new
                        {
                            CurrentWorkbook.WorkbookId,
                            CurrentWorkbook.Name,
                            CurrentWorkbook.Author,
                            CurrentWorkbook.CreatedDate,
                            CurrentWorkbook.LastSavedDate
                        },
                        Cells = _context.Cells
                            .Where(c => c.WorkbookId == CurrentWorkbook!.WorkbookId)
                            .Select(c => new
                            {
                                c.CellId,
                                c.Row,
                                c.Column,
                                c.Value,
                                c.IsBold,
                                c.IsItalic,
                                c.IsUnderline,
                                c.BackgroundColor,
                                c.ForegroundColor,
                                c.WorkbookId,
                                c.FormatTemplateId
                            })
                            .ToList(),
                        Templates = _context.FormatTemplates
                            .Where(ft => ft.WorkbookId == CurrentWorkbook!.WorkbookId)
                            .Select(ft => new
                            {
                                ft.FormatTemplateId,
                                ft.Name,
                                ft.IsBold,
                                ft.IsItalic,
                                ft.IsUnderline,
                                ft.BackgroundColor,
                                ft.ForegroundColor,
                                ft.FontFamily,
                                ft.FontSize,
                                ft.WorkbookId
                            })
                            .ToList()
                    };

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                    };

                    var json = JsonSerializer.Serialize(exportData, options);
                    File.WriteAllText(saveDialog.FileName, json);

                    if (CurrentWorkbook != null)
                    {
                        CurrentWorkbook.FilePath = saveDialog.FileName;
                        CurrentWorkbook.LastSavedDate = DateTime.Now;
                        _context.SaveChanges();
                    }

                    SetStatusMessage($"Workbook saved to {saveDialog.FileName}", true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving workbook: {ex.Message}", "Save Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                SetStatusMessage($"Error saving workbook: {ex.Message}", false);
            }
        }

        [RelayCommand]
        private void OpenWorkbook()
        {
            try
            {
                var openDialog = new OpenFileDialog
                {
                    Filter = "DoWell Files (*.dwl)|*.dwl|JSON Files (*.json)|*.json",
                    DefaultExt = ".dwl"
                };

                if (openDialog.ShowDialog() == true)
                {
                    var result = MessageBox.Show(
                        "This will replace the current workbook. Save changes first?",
                        "Open Workbook",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        SaveWorkbook();
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }

                    var json = File.ReadAllText(openDialog.FileName);

                    MessageBox.Show(
                        $"File opened successfully!\n\nFile: {Path.GetFileName(openDialog.FileName)}\n\n" +
                        "Note: Full import functionality would deserialize the JSON and load it into the database.",
                        "Open File",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    SetStatusMessage($"Opened: {openDialog.FileName}", true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening workbook: {ex.Message}", "Open Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                SetStatusMessage($"Error opening workbook: {ex.Message}", false);
            }
        }

        [RelayCommand]
        private void ApplyFormatTemplate(FormatTemplate template)
        {
            try
            {
                if (SelectedCell != null && template != null)
                {
                    SelectedCell.ApplyTemplateCommand.Execute(template);
                    SaveChangesToDatabase();
                    SetStatusMessage($"Applied template: {template.Name}", true);
                }
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error applying template: {ex.Message}", false);
            }
        }

        [RelayCommand]
        private void CreateFormatTemplate()
        {
            try
            {
                if (SelectedCell == null)
                {
                    MessageBox.Show("Please select a cell first to create a template from its format.",
                        "No Cell Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (CurrentWorkbook == null)
                {
                    MessageBox.Show("No workbook is currently loaded.", "No Workbook",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new InputDialog("New Format Template", "Enter template name:", "Custom Style");
                if (dialog.ShowDialog() == true)
                {
                    var template = new FormatTemplate
                    {
                        Name = dialog.ResponseText,
                        WorkbookId = CurrentWorkbook.WorkbookId,
                        IsBold = SelectedCell.IsBold,
                        IsItalic = SelectedCell.IsItalic,
                        IsUnderline = SelectedCell.IsUnderline,
                        BackgroundColor = SelectedCell.BackgroundColor,
                        ForegroundColor = SelectedCell.ForegroundColor
                    };

                    _context.FormatTemplates.Add(template);
                    _context.SaveChanges();
                    FormatTemplates.Add(template);

                    SetStatusMessage($"Created template: {template.Name}", true);
                }
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error creating template: {ex.Message}", false);
                MessageBox.Show($"Error creating template: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ApplyBackgroundColor()
        {
            try
            {
                if (SelectedCell != null)
                {
                    SelectedCell.SetBackgroundColorCommand.Execute(SelectedBackgroundColor);
                    SaveChangesToDatabase();
                    SetStatusMessage($"Applied background color: {SelectedBackgroundColor}", true);
                }
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error applying background color: {ex.Message}", false);
            }
        }

        [RelayCommand]
        private void ApplyForegroundColor()
        {
            try
            {
                if (SelectedCell != null)
                {
                    SelectedCell.SetForegroundColorCommand.Execute(SelectedForegroundColor);
                    SaveChangesToDatabase();
                    SetStatusMessage($"Applied foreground color: {SelectedForegroundColor}", true);
                }
            }
            catch (Exception ex)
            {
                SetStatusMessage($"Error applying foreground color: {ex.Message}", false);
            }
        }

        [RelayCommand]
        private void ShowAbout()
        {
            MessageBox.Show(
                "DoWell - Excel Clone\nVersion 1.0\n\n" +
                "A simple spreadsheet application with basic Excel-like functionality.\n\n" +
                "Features:\n" +
                "• Cell formatting (Bold, Italic, Underline)\n" +
                "• Background and foreground colors\n" +
                "• Format templates\n" +
                "• Save/Load workbooks\n\n" +
                "© 2025 DoWell Project",
                "About DoWell",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void SaveChangesToDatabase()
        {
            if (CurrentWorkbook == null) return;

            foreach (var row in GridData)
            {
                foreach (var cellVm in row)
                {
                    cellVm.UpdateCell();
                    var cell = cellVm.GetCell();

                    var existingCell = _context.Cells
                        .FirstOrDefault(c => c.WorkbookId == CurrentWorkbook.WorkbookId
                            && c.Row == cell.Row && c.Column == cell.Column);

                    if (existingCell == null)
                    {
                        if (!string.IsNullOrEmpty(cell.Value) || cell.IsBold || cell.IsItalic ||
                            cell.IsUnderline || cell.BackgroundColor != "#FFFFFF" || cell.ForegroundColor != "#000000")
                        {
                            cell.WorkbookId = CurrentWorkbook.WorkbookId;
                            _context.Cells.Add(cell);
                        }
                    }
                    else
                    {
                        existingCell.Value = cell.Value;
                        existingCell.IsBold = cell.IsBold;
                        existingCell.IsItalic = cell.IsItalic;
                        existingCell.IsUnderline = cell.IsUnderline;
                        existingCell.BackgroundColor = cell.BackgroundColor;
                        existingCell.ForegroundColor = cell.ForegroundColor;
                        existingCell.FormatTemplateId = cell.FormatTemplateId;
                    }
                }
            }

            if (CurrentWorkbook != null)
            {
                CurrentWorkbook.LastSavedDate = DateTime.Now;
            }
            _context.SaveChanges();
        }
    }
}