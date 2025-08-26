// ViewModels/CellViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoWell.Models;
using System.Windows.Media;

namespace DoWell.ViewModels
{
    public partial class CellViewModel : ViewModelBase
    {
        private readonly Cell _cell;

        [ObservableProperty]
        private string _value;

        [ObservableProperty]
        private bool _isBold;

        [ObservableProperty]
        private bool _isItalic;

        [ObservableProperty]
        private bool _isUnderline;

        [ObservableProperty]
        private string _backgroundColor;

        [ObservableProperty]
        private string _foregroundColor;

        [ObservableProperty]
        private int _row;

        [ObservableProperty]
        private int _column;

        [ObservableProperty]
        private FormatTemplate? _selectedTemplate;

        public Brush BackgroundBrush => GetBrushFromHex(BackgroundColor);
        public Brush ForegroundBrush => GetBrushFromHex(ForegroundColor);

        public CellViewModel(Cell cell)
        {
            _cell = cell ?? new Cell();
            _value = _cell.Value;
            _isBold = _cell.IsBold;
            _isItalic = _cell.IsItalic;
            _isUnderline = _cell.IsUnderline;
            _backgroundColor = _cell.BackgroundColor;
            _foregroundColor = _cell.ForegroundColor;
            _row = _cell.Row;
            _column = _cell.Column;
            _selectedTemplate = _cell.FormatTemplate;
        }

        public Cell GetCell() => _cell;

        public void UpdateCell()
        {
            _cell.Value = Value;
            _cell.IsBold = IsBold;
            _cell.IsItalic = IsItalic;
            _cell.IsUnderline = IsUnderline;
            _cell.BackgroundColor = BackgroundColor;
            _cell.ForegroundColor = ForegroundColor;
            _cell.FormatTemplate = SelectedTemplate;
            _cell.FormatTemplateId = SelectedTemplate?.FormatTemplateId;
        }

        [RelayCommand]
        private void ToggleBold()
        {
            IsBold = !IsBold;
            UpdateCell();
        }

        [RelayCommand]
        private void ToggleItalic()
        {
            IsItalic = !IsItalic;
            UpdateCell();
        }

        [RelayCommand]
        private void ToggleUnderline()
        {
            IsUnderline = !IsUnderline;
            UpdateCell();
        }

        [RelayCommand]
        private void SetBackgroundColor(string color)
        {
            BackgroundColor = color;
            OnPropertyChanged(nameof(BackgroundBrush));
            UpdateCell();
        }

        [RelayCommand]
        private void SetForegroundColor(string color)
        {
            ForegroundColor = color;
            OnPropertyChanged(nameof(ForegroundBrush));
            UpdateCell();
        }

        [RelayCommand]
        private void ApplyTemplate(FormatTemplate? template)
        {
            if (template != null)
            {
                SelectedTemplate = template;
                IsBold = template.IsBold;
                IsItalic = template.IsItalic;
                IsUnderline = template.IsUnderline;
                BackgroundColor = template.BackgroundColor;
                ForegroundColor = template.ForegroundColor;
                OnPropertyChanged(nameof(BackgroundBrush));
                OnPropertyChanged(nameof(ForegroundBrush));
                UpdateCell();
            }
        }

        private Brush GetBrushFromHex(string hexColor)
        {
            try
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
            }
            catch
            {
                return hexColor == "#000000" ? Brushes.Black : Brushes.White;
            }
        }
    }
}