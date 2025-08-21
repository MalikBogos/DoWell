using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DoWell
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }
        public event PropertyChangedEventHandler? PropertyChanged;

    }
}