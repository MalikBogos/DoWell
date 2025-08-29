using System.Windows;
using DoWell.ViewModels;

namespace DoWell.Views
{
    public partial class FindDialog : Window
    {
        private MainViewModel _viewModel;
        private int _currentRow = 0;
        private int _currentCol = 0;

        public FindDialog(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            SearchTextBox.Focus();
        }

        private void FindNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                ResultTextBlock.Text = "Please enter search text.";
                return;
            }

            var searchText = SearchTextBox.Text;
            var matchCase = MatchCaseCheckBox.IsChecked ?? false;
            var found = false;

            // Start searching from current position
            for (int row = _currentRow; row < _viewModel.RowCount && !found; row++)
            {
                int startCol = (row == _currentRow) ? _currentCol : 0;

                for (int col = startCol; col < _viewModel.ColumnCount; col++)
                {
                    var cellValue = _viewModel.GridData[row][col].Value;

                    if (string.IsNullOrEmpty(cellValue))
                        continue;

                    bool matches = matchCase
                        ? cellValue.Contains(searchText)
                        : cellValue.ToLower().Contains(searchText.ToLower());

                    if (matches)
                    {
                        // Found a match
                        _viewModel.SelectedCell = _viewModel.GridData[row][col];
                        ResultTextBlock.Text = $"Found at Row {row + 1}, Column {GetColumnName(col)}";

                        // Update position for next search
                        _currentCol = col + 1;
                        if (_currentCol >= _viewModel.ColumnCount)
                        {
                            _currentCol = 0;
                            _currentRow = row + 1;
                        }
                        else
                        {
                            _currentRow = row;
                        }

                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                ResultTextBlock.Text = "No more matches found. Search will restart from beginning.";
                _currentRow = 0;
                _currentCol = 0;
            }
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}