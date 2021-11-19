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
            Person.ItemsSource = Storage.Persons;
            Person.SelectedItem = Storage.Persons[0];
            Vault.ItemsSource = Storage.Vaults;
            Vault.SelectedItem = Storage.Vaults[0];
            TypeComboBox.ItemsSource = Storage.ExpenseTypes;
            TypeComboBox.SelectedItem = Storage.ExpenseTypes[0];
            DatePick.SelectedDate = DateTime.Now;
            listViewForExpenses.ItemsSource = listOfExpensesView;
            
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("понял");
            var button = (Button)sender;
            var expense = (Expense)button.DataContext;
            listOfExpensesView.Remove(expense);
            expense.Vault.Remove(expense);
            Storage.Save();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            
            if ((string)((Button)e.OriginalSource).Content == "Изменить")
            {
                var button = (Button)sender;
                var expense = (Expense)button.DataContext;
                ((Button)e.OriginalSource).Content = "Сохранить";
                AddButton.Content = "Сохраните!";
                AddButton.IsEnabled = false;
                Person.SelectedItem = expense.Person;
                Vault.SelectedItem = expense.Vault;
                TypeComboBox.SelectedItem = expense.Type;
                DatePick.SelectedDate = expense.Date;
                Comment.Text = expense.Comment;
                Amount.Text = expense.Amount.ToString();
            }
            else if ((string)((Button)e.OriginalSource).Content == "Сохранить")
            {
                
                var button = (Button)sender;
                var expense = (Expense)button.DataContext;
                
                ((Button)e.OriginalSource).Content = "Изменить";
                AddButton.IsEnabled = true;
                AddButton.Content = "Add";

                try
                {
                    expense.Amount = Convert.ToDecimal(Amount.Text);
                    expense.Comment = Comment.Text;
                    expense.Date = (DateTime)DatePick.SelectedDate;
                    expense.Person = (Person)Person.SelectedItem;
                    expense.Type = (ExpenseType)TypeComboBox.SelectedItem;

                    if (expense.Vault != (MoneyVault)Vault.SelectedItem)
                    {
                        expense.Vault.Remove(expense);
                        expense.Vault = (MoneyVault)Vault.SelectedItem;
                        expense.Vault.DecreaseBalance(
                            new Expense
                            (
                          Convert.ToDecimal(Amount.Text),
                          (DateTime)DatePick.SelectedDate,
                          (Person)Person.SelectedItem,
                          Comment.Text,
                          (ExpenseType)TypeComboBox.SelectedItem
                            )
                        );
                    }
                    UpdateIncomesView();
                    MessageBox.Show("ок");
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("не согласны\n понял???");
                }
            }
            

        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Expense newExpense = new Expense
            (
               Convert.ToDecimal(Amount.Text),
               (DateTime)DatePick.SelectedDate,
               (Person)Person.SelectedItem,
               Comment.Text,
               (ExpenseType)TypeComboBox.SelectedItem
            );
            try
            {
                ((MoneyVault)Vault.SelectedItem).DecreaseBalance(newExpense);
                MessageBox.Show("потратил\nок");
                UpdateIncomesView();
            }
            catch (ArgumentException)
            {
                MessageBox.Show("не потратил\nок");
            }
            
        }

       

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal n = 0;
            if (!Decimal.TryParse(Amount.Text, out n))
            {
                AddButton.IsEnabled = false;
                AddButton.Content = "Зайди правильно";
                Amount.Background = Brushes.Yellow;
                
            }
            else if((string)AddButton.Content != "Сохраните!")
            {
                AddButton.IsEnabled = true;
                Amount.Background = Brushes.White;
                AddButton.Content = "Add";
            }  
        }

        private void UpdateIncomesView()
        {
            Amount.Text = "Сумма";
            Comment.Text = "Комментарий";
            DatePick.SelectedDate = DateTime.Today;
            listOfExpensesView.Clear();
            var allExpenses = Storage.GetAllExpences();
            foreach (var item in allExpenses)
            {
                listOfExpensesView.Add(item);
            }
        }
    }
}
