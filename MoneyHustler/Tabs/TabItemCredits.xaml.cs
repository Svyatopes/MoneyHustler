using MoneyHustler.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace MoneyHustler.Tabs
{
    /// <summary>
    /// Interaction logic for TabItemCredits.xaml
    /// </summary>
    public partial class TabItemCredits : TabItem
    {
        private Storage _storageInstance = Storage.GetInstance();

        private ObservableCollection<Credit> listOfCreditsView;
        private Credit _credit;
        public TabItemCredits()
        {
            if (_storageInstance.Credits == null)
            {
                _storageInstance.Credits = new List<Credit> { };
            }
            InitializeComponent();
            listOfCreditsView = new ObservableCollection<Credit>(_storageInstance.Credits);
            listViewForCredits.ItemsSource = listOfCreditsView;

        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(listViewForCredits.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private void ButtonAddCreditClick(object sender, RoutedEventArgs e)
        {
            AuxiliaryWindows.WindowAddEditCredit windowIncomes = new();
            windowIncomes.ShowDialog();
            UpdateCreditsView();
        }

        private void ButtonEditCreditClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;

            AuxiliaryWindows.WindowAddEditCredit windowCredits = new(credit);
            windowCredits.ShowDialog();
            UpdateCreditsView();
            Storage.Save();

        }

        private void UpdateCreditsView()
        {
            listOfCreditsView.Clear();
            var allCredits = _storageInstance.Credits;
            foreach (var income in allCredits)
            {
                listOfCreditsView.Add(income);
            }
        }

        private void ButtonRemoveCreditItemClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;
            listOfCreditsView.Remove(credit);
            _storageInstance.Credits.Remove(credit);
            Storage.Save();
        }

        private void ButtonPayItemClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;

            if (credit.Value == 0)
            {
                MessageBox.Show("Пошел нахуй дибил блядь");
                return;
            }

            credit.PayMonthlyPayment();
            UpdateCreditsView();


            Storage.Save();


        }

    }
}
