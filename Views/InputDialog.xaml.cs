using System.Windows;

namespace DoWell.Views
{
    public partial class InputDialog : Window
    {
        public string ResponseText { get; set; }

        public InputDialog(string title, string question, string defaultAnswer = "")
        {
            InitializeComponent();
            Title = title;
            QuestionLabel.Content = question;
            ResponseTextBox.Text = defaultAnswer;
            ResponseTextBox.SelectAll();
            ResponseTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = ResponseTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}