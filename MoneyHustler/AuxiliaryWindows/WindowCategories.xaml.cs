using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MoneyHustler.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary>
    /// Interaction logic for WindowCategories.xaml
    /// </summary>
    public partial class WindowCategories : Window
    {
        private ObservableCollection<IncomeType> _incomeTypes;

        private ObservableCollection<ExpenseType> _expenseTypes;

        public WindowCategories()
        {
            InitializeComponent();

            _incomeTypes = new ObservableCollection<IncomeType>(Storage.IncomeTypes);
            _expenseTypes = new ObservableCollection<ExpenseType>(Storage.ExpenseTypes);

            ListViewIncomes.ItemsSource = _incomeTypes;
            ListViewExpenses.ItemsSource = _expenseTypes;
        }

        private void ButtonRemoveIncomeCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var incomeType = (IncomeType)button.DataContext;

            if (Storage.GetAllIncomes().Any(item => item.Type == incomeType))
            {
                MessageBox.Show("Эта категория используется вами!");
                return;
            }    

            Storage.IncomeTypes.Remove(incomeType);
            _incomeTypes.Remove(incomeType);
           
        }

        private void ButtonRemoveExpenseCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var expenseType = (IncomeType)button.DataContext;

            if (Storage.GetAllIncomes().Any(item => item.Type == expenseType))
            {
                MessageBox.Show("Эта категория используется вами!");
                return;
            }

            Storage.IncomeTypes.Remove(expenseType);
            _incomeTypes.Remove(expenseType);
        }

        private void ButtonRenameIncomeCategoryClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonRenameExpenseCategoryClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAddIncomeCategory_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxEnterIncomeCategory.Text.Length == 0) return;
            if (_incomeTypes.Any(item => item.Name == TextBoxEnterIncomeCategory.Text))
            {
                MessageBox.Show("Такая категория уже существует!");
                return;
            }

            Storage.IncomeTypes.Add(new IncomeType() { Name = TextBoxEnterIncomeCategory.Text });
            _incomeTypes.Add(new IncomeType() { Name = TextBoxEnterIncomeCategory.Text });
        }

        private void ButtonAddExpenseCategory_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxEnterExpenseCategory.Text.Length == 0) return;
            if (_incomeTypes.Any(item => item.Name == TextBoxEnterExpenseCategory.Text))
            {
                MessageBox.Show("Такая категория уже существует!");
                return;
            }
            Storage.ExpenseTypes.Add(new ExpenseType() { Name = TextBoxEnterExpenseCategory.Text });
            _expenseTypes.Add(new ExpenseType() { Name = TextBoxEnterExpenseCategory.Text });
        }
    }
}

