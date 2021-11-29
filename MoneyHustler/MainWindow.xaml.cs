using MoneyHustler.Models;
using System.Windows;

namespace MoneyHustler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Storage.Load();

            InitializeComponent();

        }

    }
}
