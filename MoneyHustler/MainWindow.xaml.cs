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
            InitialTestFillStorage();
        }

        private void InitialTestFillStorage()
        {
            Storage.Vaults = new List<MoneyVault>();
            Storage.Persons = new List<Person>();
            Storage.IncomeTypes = new List<IncomeType>();
            Storage.ExpenseTypes = new List<ExpenseType>();


            var iam = new Person() { Name = "I'am" };
            Storage.Persons.Add(iam);

            var mother = new Person() { Name = "mother" };
            Storage.Persons.Add(mother);

            var father = new Person() { Name = "father" };
            Storage.Persons.Add(father);

            var myBox = new Card("MyBox", 100, 0);
            Storage.Vaults.Add(myBox);

            var myCard = new Card("myCard", 500, 1);
            Storage.Vaults.Add(myCard);

            var incomeTypeFood = new IncomeType() { Name = "Food" };
            Storage.IncomeTypes.Add(incomeTypeFood);
            
            var incomeTypeClothes = new IncomeType() { Name = "Clothes" };
            Storage.IncomeTypes.Add(incomeTypeClothes);

            var incomeTypePet = new IncomeType() { Name = "Pet" };
            Storage.IncomeTypes.Add(incomeTypePet);

            myBox.IncreaseBalance(new Income(10000, DateTime.Now, iam, "from selling TV", incomeTypeClothes));
            myBox.IncreaseBalance(new Income(500, DateTime.Now.AddDays(-10), father, "Feed", incomeTypeFood));
            myBox.IncreaseBalance(new Income(300, DateTime.Now.AddDays(-2), mother, "for my shooes", incomeTypeClothes));
            myBox.IncreaseBalance(new Income(200, DateTime.Now.AddDays(-15), iam, "from cat", incomeTypePet));

            myCard.IncreaseBalance(new Income(1000, DateTime.Now.AddDays(-4), mother, "for food", incomeTypeFood));
            myCard.IncreaseBalance(new Income(750, DateTime.Now.AddDays(-8), iam, "from cat", incomeTypePet));

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

        private void ButtonAnalytics_Click(object sender, RoutedEventArgs e)
        {
            WindowAnalytics windowAnalytics = new();
            windowAnalytics.ShowDialog();
        }
    }
}
