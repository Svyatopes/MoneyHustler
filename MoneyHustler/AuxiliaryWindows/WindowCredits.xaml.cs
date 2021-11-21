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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MoneyHustler.Models;

namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary>
    /// Interaction logic for WindowCredits.xaml
    /// </summary>
    public partial class WindowCredits : Window
    {
        private ObservableCollection<Credit> listOfCreditsView;
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        public WindowCredits()
        {
            if (Storage.Credits == null)
            {
                Storage.Credits = new List<Credit> { };
            }
            InitializeComponent();
            listOfCreditsView = new ObservableCollection<Credit>(Storage.Credits);
            listViewForCredits.ItemsSource = listOfCreditsView;

        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked == null)
            {
                return;
            }

            if ((string)headerClicked.Content == "Удалить" || (string)headerClicked.Content == "Изменить")
            {
                return;
            }
            ListSortDirection direction;


            if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                if (headerClicked != _lastHeaderClicked)
                {
                    direction = ListSortDirection.Ascending;
                }
                else
                {
                    if (_lastDirection == ListSortDirection.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                    }
                }

                var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                Sort(sortBy, direction);

                if (direction == ListSortDirection.Ascending)
                {
                    headerClicked.Column.HeaderTemplate =
                      Resources["HeaderTemplateArrowUp"] as DataTemplate;
                }
                else
                {
                    headerClicked.Column.HeaderTemplate =
                      Resources["HeaderTemplateArrowDown"] as DataTemplate;
                }


                if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                {
                    _lastHeaderClicked.Column.HeaderTemplate = null;
                }

                _lastHeaderClicked = headerClicked;
                _lastDirection = direction;
            }

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
            var allCredits = Storage.Credits;
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
            Storage.Credits.Remove(credit);
            Storage.Save();
        }

        private void ButtonPayItemClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;

            credit.PayMonthlyPayment();
            UpdateCreditsView();
            Storage.Save();
        }
    }
}
