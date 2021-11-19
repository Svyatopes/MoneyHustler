﻿using System;
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
using System.Windows.Shapes;
using MoneyHustler.Models;

namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary>
    /// Interaction logic for WindowExpenses.xaml
    /// </summary>
    public partial class WindowExpenses : Window
    {

        private Expense _expense;

        private ObservableCollection<Expense> listOfExpensesView;
        public WindowExpenses()
        {
            InitializeComponent();
            listOfExpensesView = new ObservableCollection<Expense>(Storage.GetAllExpences());
            ComboBoxExpensePerson.ItemsSource = Storage.Persons;
            ComboBoxExpensePerson.SelectedItem = Storage.Persons[0];
            ComboBoxExpenseVault.ItemsSource = Storage.Vaults;
            ComboBoxExpenseVault.SelectedItem = Storage.Vaults[0];
            ComboBoxExpenseType.ItemsSource = Storage.ExpenseTypes;
            ComboBoxExpenseType.SelectedItem = Storage.ExpenseTypes[0];
            DatePickerExpenseDate.SelectedDate = DateTime.Now;
            listViewForExpenses.ItemsSource = listOfExpensesView;

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var expense = (Expense)button.DataContext;
            listOfExpensesView.Remove(expense);
            expense.Vault.Remove(expense);
            Storage.Save();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button == null)
            {
                return;
            }
            var expense = (Expense)button.DataContext;

            if ((string)button.Content == "Изменить")
            {
                AddButton.Content = "Сохраните";
                listViewForExpenses.IsEnabled = false;
                AddButton.IsEnabled = false;
                ComboBoxExpensePerson.SelectedItem = expense.Person;
                ComboBoxExpenseVault.SelectedItem = expense.Vault;
                ComboBoxExpenseType.SelectedItem = expense.Type;
                DatePickerExpenseDate.SelectedDate = expense.Date;
                TextBoxExpenseComment.Text = expense.Comment;
                TextBoxExpenseAmount.Text = expense.Amount.ToString();
            }
            else if ((string)((Button)e.OriginalSource).Content == "Сохранить")
            {

                button.Content = "Изменить";
                AddButton.IsEnabled = true;
                AddButton.Content = "Add";

                try
                {
                    expense.Amount = Convert.ToDecimal(TextBoxExpenseAmount.Text);
                    expense.Comment = TextBoxExpenseComment.Text;
                    expense.Date = (DateTime)DatePickerExpenseDate.SelectedDate;
                    expense.Person = (Person)ComboBoxExpensePerson.SelectedItem;
                    expense.Type = (ExpenseType)ComboBoxExpenseType.SelectedItem;

                    if (expense.Vault != (MoneyVault)ComboBoxExpenseVault.SelectedItem)
                    {
                        expense.Vault.Remove(expense);
                        expense.Vault = (MoneyVault)ComboBoxExpenseVault.SelectedItem;
                        expense.Vault.DecreaseBalance(
                            new Expense
                            (
                          Convert.ToDecimal(TextBoxExpenseAmount.Text),
                          (DateTime)DatePickerExpenseDate.SelectedDate,
                          (Person)ComboBoxExpensePerson.SelectedItem,
                          TextBoxExpenseComment.Text,
                          (ExpenseType)ComboBoxExpenseType.SelectedItem
                            )
                        );
                    }
                    UpdateIncomesView();
                    MessageBox.Show("ок");
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("не согласны\n понял???", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Expense newExpense = new Expense
            (
               Convert.ToDecimal(TextBoxExpenseAmount.Text),
               (DateTime)DatePickerExpenseDate.SelectedDate,
               (Person)ComboBoxExpensePerson.SelectedItem,
               TextBoxExpenseComment.Text,
               (ExpenseType)ComboBoxExpenseType.SelectedItem
            );
            try
            {
                ((MoneyVault)ComboBoxExpenseVault.SelectedItem).DecreaseBalance(newExpense);
                MessageBox.Show("потратил\nок");
                UpdateIncomesView();
            }
            catch (ArgumentException)
            {
                MessageBox.Show("не потратил\nок", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }



        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal n = 0;
            if (!Decimal.TryParse(TextBoxExpenseAmount.Text, out n))
            {
                AddButton.IsEnabled = false;
                AddButton.Content = "Зайди правильно";
                TextBoxExpenseAmount.Background = Brushes.Yellow;

            }
            else if ((string)AddButton.Content != "Сохраните!")
            {
                AddButton.IsEnabled = true;
                TextBoxExpenseAmount.Background = Brushes.White;
                AddButton.Content = "Add";
            }
        }

        private void UpdateIncomesView()
        {
            TextBoxExpenseAmount.Text = "Сумма";
            TextBoxExpenseComment.Text = "Комментарий";
            DatePickerExpenseDate.SelectedDate = DateTime.Today;
            listOfExpensesView.Clear();
            var allExpenses = Storage.GetAllExpences();
            foreach (var item in allExpenses)
            {
                listOfExpensesView.Add(item);
            }
        }
    }
}
