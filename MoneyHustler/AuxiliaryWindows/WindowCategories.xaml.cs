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

namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary>
    /// Interaction logic for WindowCategories.xaml
    /// </summary>
    public partial class WindowCategories : Window
    {
        public WindowCategories()
        {
            InitializeComponent();
            FillExpensesList();
            FillIncomesList();
        }

        private void FillIncomesList()
        {
            ListViewIncomes.Items.Clear();
            foreach (IncomeType incomeType in Storage.IncomeTypes)
            {
                ListViewIncomes.Items.Add(incomeType.Name);
            }
        }

        private void FillExpensesList()
        {
            ListViewExpenses.Items.Clear();
            foreach (ExpenseType expenseType in Storage.ExpenseTypes)
            {
                ListViewExpenses.Items.Add(expenseType.Name);
            }
        }

        private void ButtonRemoveIncomeCategoryClick(object sender, RoutedEventArgs e)
        {

           // if (Storage.IncomeTypes.Contains())
        }

        private void ButtonRemoveExpenseCategoryClick(object sender, RoutedEventArgs e)
        {

            //if (Storage.IncomeTypes.Contains())
        }

    }
}

