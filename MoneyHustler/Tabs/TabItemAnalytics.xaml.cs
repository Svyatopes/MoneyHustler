using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MoneyHustler.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for TabItemAnalytics.xaml
    /// </summary>
    public partial class TabItemAnalytics : TabItem
    {
        public SeriesCollection SeriesCollection { get; set; }
        private Storage _storageInstance = Storage.GetInstance();
        private ObservableCollection<ExpenseType> listOfExpensTypeseView;
        private ObservableCollection<Expense> listOfExpensesView;
        public TabItemAnalytics()
        {
            InitializeComponent();  
            CreateDiagram();
            DataContext = this;

        }

        public void CreateDiagram(string period = "За все время")
        {
            listOfExpensTypeseView = new ObservableCollection<ExpenseType>(_storageInstance.ExpenseTypes);
            listOfExpensesView = new ObservableCollection<Expense>(Storage.GetAllExpenses());
            DateTime pickedPeriod = new DateTime();

            switch (period)
            {
                case "За месяц":
                    pickedPeriod = DateTime.Today;
                    LabelPeriod.Content = $"Граффик трат за {pickedPeriod:dd MMMM yyyy}";
                    break;
                case "За три месяца":
                    pickedPeriod = DateTime.Today.AddMonths(-2);
                    LabelPeriod.Content = $"Граффик трат с {pickedPeriod:dd MMMM yyyy} по {DateTime.Today:dd MMMM yyyy}";
                    break;

                case "За шесть месяцев":
                    pickedPeriod = DateTime.Today.AddMonths(-5);
                    LabelPeriod.Content = $"Граффик трат с {pickedPeriod:dd MMMM yyyy} по {DateTime.Today:dd MMMM yyyy}";
                    break;
                default:
                    pickedPeriod = DateTime.UnixEpoch;
                    LabelPeriod.Content = $"Граффик трат за все время";
                    break;
            }
            if (SeriesCollection == null)
            {
                SeriesCollection = new SeriesCollection();
                
            }
            
            foreach (var item in listOfExpensTypeseView)
            {
                var SumeExpensesDependingOnType = from i in listOfExpensesView where i.Type == item && i.Date.Month >= pickedPeriod.Month select i;
                var value = Math.Round(SumeExpensesDependingOnType.Sum(n => n.Amount));
                if (value == 0)
                {
                    continue;
                }
                SeriesCollection.Add(new PieSeries { Title = item.Name, Values = new ChartValues<ObservableValue> { new ObservableValue((double)value) }, DataLabels = true}); ;
            }

            Chart.Update();
        }
        private void ComboBoxPeriodItemSelected(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            var period = selectedItem.Content.ToString();
            SeriesCollection.Clear();
            CreateDiagram(period);
            Chart.Update();

        }
    }
}
