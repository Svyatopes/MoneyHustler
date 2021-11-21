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

            var wolf = new Person() { Name = "волк" };
            Storage.Persons.Add(wolf);

            var myBox = new Card("MyBox", 100, 0);
            Storage.Vaults.Add(myBox);

            var myCard = new Card("myCard", 500, 1);
            Storage.Vaults.Add(myCard);

            var visa = new Card("Visa", 100000, 10);
            Storage.Vaults.Add(visa);

            var visa2 = new Card("хуIsa", 8000000, 40);
            Storage.Vaults.Add(visa2);

            var incomeTypeFood = new IncomeType() { Name = "Food" };
            Storage.IncomeTypes.Add(incomeTypeFood);
            
            var incomeTypeClothes = new IncomeType() { Name = "Clothes" };
            Storage.IncomeTypes.Add(incomeTypeClothes);

            var incomeTypePet = new IncomeType() { Name = "Pet" };
            Storage.IncomeTypes.Add(incomeTypePet);

            var expenseDrugs = new ExpenseType() { Name = "Наркота" };
            Storage.ExpenseTypes.Add(expenseDrugs);

            var expenseWhores = new ExpenseType() { Name = "Шлюхи" };
            Storage.ExpenseTypes.Add(expenseWhores);

            var expenseGuns = new ExpenseType() { Name = "Стволы" };
            Storage.ExpenseTypes.Add(expenseGuns);

            var expenseForWolf = new ExpenseType() { Name = "не цирк" };
            Storage.ExpenseTypes.Add(expenseGuns);

            var expenseHairStyle = new ExpenseType() { Name = "Барбишоп" };
            Storage.ExpenseTypes.Add(expenseHairStyle);


            myBox.IncreaseBalance(new Income(10000, DateTime.Now, iam, "from selling TV", incomeTypeClothes));
            myBox.IncreaseBalance(new Income(500, DateTime.Now.AddDays(-10), father, "Feed", incomeTypeFood));
            myBox.IncreaseBalance(new Income(300, DateTime.Now.AddDays(-2), mother, "for my shooes", incomeTypeClothes));
            myBox.IncreaseBalance(new Income(200, DateTime.Now.AddDays(-15), iam, "from cat", incomeTypePet));

            myCard.IncreaseBalance(new Income(1000, DateTime.Now.AddDays(-4), mother, "for food", incomeTypeFood));
            myCard.IncreaseBalance(new Income(750, DateTime.Now.AddDays(-8), iam, "from cat", incomeTypePet));

            visa.DecreaseBalance(new Expense(3000, DateTime.Now, iam, "объебусь", expenseDrugs));
            visa.DecreaseBalance(new Expense(10000, DateTime.Now.AddDays(-1), iam, "поебусь", expenseWhores));
            visa.DecreaseBalance(new Expense(50000, DateTime.Now.AddDays(-2), iam, "застрелюсь", expenseGuns));
            visa2.DecreaseBalance(new Expense(3000, DateTime.Now.AddDays(-2), father, "сходил в этот ваш", expenseHairStyle));
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
