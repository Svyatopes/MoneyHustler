using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MoneyHustler.Models;

namespace MoneyHustler
{
    /// <summary>
    /// Interaction logic for WindowIncomes.xaml
    /// </summary>
    public partial class WindowIncomes : Window
    {
        private ObservableCollection<Income> listOfIncomesView;
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        public WindowIncomes()
        {
            InitializeComponent();
            listOfIncomesView = new ObservableCollection<Income>(Storage.GetAllIncomes());
            listViewForIncomes.ItemsSource = listOfIncomesView;
        }

        void GridViewColumnHeaderClickedHandler(object sender,
                                            RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if ((string)headerClicked.Content == "Удалить" || (string)headerClicked.Content == "Изменить")
            {
                return;
            }
            ListSortDirection direction;

            if (headerClicked != null)
            {
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

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(listViewForIncomes.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }



        private void ButtonEditIncomeClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var income = (Income)button.DataContext;

            AuxiliaryWindows.WindowAddEditIncome windowIncomes = new(income);
            windowIncomes.ShowDialog();
            UpdateIncomesView();

            //TODO: serialize

        }


        //TODO: Transfer how to remove in right way
        private void ButtonRemoveIncomeItemClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var income = (Income)button.DataContext;
            listOfIncomesView.Remove(income);
            income.Vault.Remove(income);
            //TODO: serialize
        }

        private void ButtonAddIncomeClick(object sender, RoutedEventArgs e)
        {
            AuxiliaryWindows.WindowAddEditIncome windowIncomes = new();
            windowIncomes.ShowDialog();

            UpdateIncomesView();
            //TODO: serialize

        }

        private void UpdateIncomesView()
        {
            listOfIncomesView.Clear();
            var allIncomes = Storage.GetAllIncomes();
            foreach (var income in allIncomes)
            {
                listOfIncomesView.Add(income);
            }
        }


    }
}
