using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MoneyHustler.Models;

namespace MoneyHustler
{
    /// <summary>
    /// Interaction logic for WindowIncomes.xaml
    /// </summary>
    public partial class WindowIncomes : Window
    {
        private ObservableCollection<Income> listOfIncomesView;
        public WindowIncomes()
        {
            InitializeComponent();
            listOfIncomesView = new ObservableCollection<Income>(Storage.GetAllIncomes());
            listViewForIncomes.ItemsSource = listOfIncomesView;
        }



        private void ButtonRemoveIncomeItemClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var income = (Income)button.DataContext;
            listOfIncomesView.Remove(income);

            income.Vault.Remove(income);
            //TODO: serialize
        }
    }
}
