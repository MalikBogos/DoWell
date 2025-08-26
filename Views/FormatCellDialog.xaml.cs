using System.Windows;
using System.Windows.Media;
using DoWell.ViewModels;

namespace DoWell.Views
{
    public partial class FormatCellDialog : Window
    {
        private CellViewModel _cellViewModel;

        public FormatCellDialog(CellViewModel cellViewModel)
        {
            InitializeComponent();
            _cellViewModel = cellViewModel;
            DataContext = _cellViewModel;
            InitializeColorComboBoxes();
        }

        private void InitializeColorComboBoxes()
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
                new { Name = "Light Gray", Color = "#D3D3D3" }
            };

            BackgroundColorCombo.ItemsSource = colors;
            ForegroundColorCombo.ItemsSource = colors;

            // Set selected items based on current colors
            foreach (dynamic color in colors)
            {
                if (color.Color == _cellViewModel.BackgroundColor)
                    BackgroundColorCombo.SelectedItem = color;
                if (color.Color == _cellViewModel.ForegroundColor)
                    ForegroundColorCombo.SelectedItem = color;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Apply selected colors
            if (BackgroundColorCombo.SelectedItem != null)
            {
                dynamic selected = BackgroundColorCombo.SelectedItem;
                _cellViewModel.BackgroundColor = selected.Color;
            }

            if (ForegroundColorCombo.SelectedItem != null)
            {
                dynamic selected = ForegroundColorCombo.SelectedItem;
                _cellViewModel.ForegroundColor = selected.Color;
            }

            _cellViewModel.UpdateCell();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void PreviewUpdate(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            PreviewText.FontWeight = BoldCheckBox.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;
            PreviewText.FontStyle = ItalicCheckBox.IsChecked == true ? FontStyles.Italic : FontStyles.Normal;
            PreviewText.TextDecorations = UnderlineCheckBox.IsChecked == true ? TextDecorations.Underline : null;

            if (BackgroundColorCombo.SelectedItem != null)
            {
                dynamic bgColor = BackgroundColorCombo.SelectedItem;
                PreviewBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor.Color));
            }

            if (ForegroundColorCombo.SelectedItem != null)
            {
                dynamic fgColor = ForegroundColorCombo.SelectedItem;
                PreviewText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fgColor.Color));
            }
        }
    }
}