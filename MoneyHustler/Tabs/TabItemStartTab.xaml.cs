using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MoneyHustler.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MoneyHustler.Tabs
{
    /// <summary>
    /// Interaction logic for TabItemStartTab.xaml
    /// </summary>
    public partial class TabItemStartTab : TabItem
    {
        public SeriesCollection SeriesCollection { get; set; }
        private Storage _storageInstance = Storage.GetInstance();
        private ObservableCollection<ExpenseType> _listOfExpenseTypesView;
        private ObservableCollection<Expense> _listOfExpensesView;
        public TabItemStartTab()
        {
            InitializeComponent();
            CreateDiagram();
            DataContext = this;
        }

        public void CreateDiagram()
        {
            _listOfExpenseTypesView = new ObservableCollection<ExpenseType>(_storageInstance.ExpenseTypes);
            _listOfExpensesView = new ObservableCollection<Expense>(Storage.GetAllExpenses());
            DateTime pickedPeriod = DateTime.Today;

            if (SeriesCollection == null)
            {
                SeriesCollection = new SeriesCollection();
            }

            foreach (var item in _listOfExpenseTypesView)
            {
                var sumExpensesDependingOnType = from i in _listOfExpensesView where i.Type == item && i.Date.Month >= pickedPeriod.Month select i;
                var value = Math.Round(sumExpensesDependingOnType.Sum(n => n.Amount));
                if (value == 0)
                {
                    continue;
                }
                SeriesCollection.Add(new PieSeries { Title = item.Name, Values = new ChartValues<ObservableValue> { new ObservableValue((double)value) }, DataLabels = true }); ;
            }

            Chart.Update();
        }

        private void ButtonExitClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
