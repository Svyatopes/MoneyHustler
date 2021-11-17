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
using System.Windows.Shapes;

namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary>
    /// Interaction logic for WindowAddEditIncome.xaml
    /// </summary>
    public partial class WindowAddEditIncome : Window
    {
        private Income _income;
        public WindowAddEditIncome()
        {
            InitializeComponent();

            ComboBoxIncomeTypes.ItemsSource = Storage.IncomeTypes;
            ComboBoxVaults.ItemsSource = Storage.Vaults;
            DatePickerIncomeDate.SelectedDate = DateTime.Now;
            ComboBoxPersons.ItemsSource = Storage.Persons;
        }

        public WindowAddEditIncome(Income income)
        {
            InitializeComponent();

            _income = income;

            TextBoxAmount.Text = _income.Amount.ToString();

            ComboBoxIncomeTypes.ItemsSource = Storage.IncomeTypes;
            ComboBoxIncomeTypes.SelectedItem = _income.Type;

            ComboBoxVaults.ItemsSource = Storage.Vaults;
            ComboBoxVaults.SelectedItem = _income.Vault;

            DatePickerIncomeDate.SelectedDate = _income.Date;

            ComboBoxPersons.ItemsSource = Storage.Persons;
            ComboBoxPersons.SelectedItem = _income.Person;

            TextBoxComment.Text = _income.Comment;
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            if (TextBoxAmount.Text == String.Empty)
            {
                MessageBox.Show("You need to enter the Amount!");
                return;
            }

            decimal enteredAmount = 0;
            if (!decimal.TryParse(TextBoxAmount.Text, out enteredAmount))
            {
                MessageBox.Show("You entered some invalid string to amount field!");
                return;
            }

            if (ComboBoxIncomeTypes.SelectedItem == null)
            {
                MessageBox.Show("You need to choose IncomeType!");
                return;
            }

            if (ComboBoxVaults.SelectedItem == null)
            {
                MessageBox.Show("You need to choose your Vault!");
                return;
            }

            if (ComboBoxPersons.SelectedItem == null)
            {
                MessageBox.Show("You need to choose Person!");
                return;
            }

            var enteredVault = (MoneyVault)ComboBoxVaults.SelectedItem;

            if (_income == null)
            {
                _income = new Income(enteredAmount, (DateTime)DatePickerIncomeDate.SelectedDate,
                    (Person)ComboBoxPersons.SelectedItem, TextBoxComment.Text,
                    (IncomeType)ComboBoxIncomeTypes.SelectedItem);
                enteredVault.IncreaseBalance(_income);

            }
            else
            {
                _income.Amount = enteredAmount;
                _income.Type = (IncomeType)ComboBoxIncomeTypes.SelectedItem;
                _income.Date = (DateTime)DatePickerIncomeDate.SelectedDate;
                _income.Person = (Person)ComboBoxPersons.SelectedItem;
                _income.Comment = TextBoxComment.Text;

                if (_income.Vault != enteredVault)
                {
                    _income.Vault.Remove(_income);
                    enteredVault.IncreaseBalance(_income);
                }
            }

            this.Close();
        }
    }
}
