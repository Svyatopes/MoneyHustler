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
        private List<Income> _incomes;
        private ObservableCollection<Income> sd;
        public WindowIncomes()
        {
            InitializeComponent();
            _incomes = new List<Income>()
            {
                new Income(500,DateTime.Now,new Person(){Name="Бабуля"}, string.Empty,new Card("TestCard",50,0),new IncomeType () {Name="На пирожное"}),
                new Income(600,DateTime.Now,new Person(){Name="Дедуля"}, "Some",new Card("TestCard",50,0),new IncomeType () {Name="На пирожное"}),
                new Income(700,DateTime.Now,new Person(){Name="Я"}, "Chtoto",new Card("TestCard",50,0),new IncomeType () {Name="На пирожное"}),
            };
            sd = new ObservableCollection<Income>(_incomes);
            listViewForIncomes.ItemsSource = sd;
        }

        private void ButtonRemoveIncomeItemClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var income = (Income)button.DataContext;
            sd.Remove(income);
            int stop = 1;
        }
    }
}
