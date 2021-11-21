using MoneyHustler.AuxiliaryWindows;
using MoneyHustler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoneyHustler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Storage.Load();
        }

        private void ButtonIncomes_Click(object sender, RoutedEventArgs e)
        {
            WindowIncomes windowIncomes = new();
            windowIncomes.ShowDialog();
        }

        private void ButtonExpenses_Click(object sender, RoutedEventArgs e)
        {
            WindowExpenses windowExpences = new();
            windowExpences.ShowDialog();
        }

        private void ButtonCategories_Click(object sender, RoutedEventArgs e)
        {
            WindowCategories windowCategories = new();
            windowCategories.ShowDialog();
        }

        private void ButtonMyFamily_Click(object sender, RoutedEventArgs e)
        {
            WindowMyFamily windowMyFamily = new();
            windowMyFamily.ShowDialog();
        }

        private void ButtonMoneyVaults_Click(object sender, RoutedEventArgs e)
        {
            WindowMoneyVaults moneyVaults = new();
            moneyVaults.ShowDialog();
        }


        private void ButtonAnalytics_Click(object sender, RoutedEventArgs e)
        {
            WindowAnalytics windowAnalytics = new();
            windowAnalytics.ShowDialog();
        }
        private void ButtonCredit_Click(object sender, RoutedEventArgs e)
        {
            WindowCredits windowCredits = new();
            windowCredits.ShowDialog();
        }
    }
}
