// ViewModels/CellViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoWell.Models;

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
        private int _row;

        [ObservableProperty]
        private int _column;

        public CellViewModel(Cell cell)
        {
            _cell = cell ?? new Cell();
            _value = _cell.Value;
            _isBold = _cell.IsBold;
            _isItalic = _cell.IsItalic;
            _isUnderline = _cell.IsUnderline;
            _row = _cell.Row;
            _column = _cell.Column;
        }

        public Cell GetCell() => _cell;

        public void UpdateCell()
        {
            _cell.Value = Value;
            _cell.IsBold = IsBold;
            _cell.IsItalic = IsItalic;
            _cell.IsUnderline = IsUnderline;
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
    }
}