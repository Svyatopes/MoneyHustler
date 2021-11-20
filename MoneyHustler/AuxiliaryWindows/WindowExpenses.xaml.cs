using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Media;
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
        SoundPlayer sp = new SoundPlayer();
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        public WindowExpenses()
        {
            InitializeComponent();
            //SoundPlayer sp = new SoundPlayer();
            
            
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

        private void ButtonDeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var expense = (Expense)button.DataContext;
            listOfExpensesView.Remove(expense);
            expense.Vault.Remove(expense);
            Storage.Save();
            //sp.SoundLocation = "C:/Users/Anton/Downloads/pushistiyEbalnik.wav";
            //sp.Load();
            //sp.Play();
        }

        private void ButtonEditExpense_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button == null)
            {
                return;
            }
            var expense = (Expense)button.DataContext;

            ButtonAddExpense.Content = "Сохраните";
            listViewForExpenses.IsEnabled = false;
            ComboBoxExpensePerson.SelectedItem = expense.Person;
            ComboBoxExpenseVault.SelectedItem = expense.Vault;
            ComboBoxExpenseType.SelectedItem = expense.Type;
            DatePickerExpenseDate.SelectedDate = expense.Date;
            TextBoxExpenseComment.Text = expense.Comment;
            TextBoxExpenseAmount.Text = expense.Amount.ToString();
            _expense = expense;
        }

        private void ButtonAddExpense_Click(object sender, RoutedEventArgs e)
        {
            
            if ((string)ButtonAddExpense.Content == "Сохраните")
            {
                listViewForExpenses.IsEnabled = true;
                ButtonAddExpense.Content = "Добавить";
                //var button = (Button)sender;
                //if (button == null)
                //{
                //    return;
                //}
                //var expense = (Expense)button.DataContext;
                try
                {
                    _expense.Amount = Convert.ToDecimal(TextBoxExpenseAmount.Text);
                    _expense.Comment = TextBoxExpenseComment.Text;
                    _expense.Date = (DateTime)DatePickerExpenseDate.SelectedDate;
                    _expense.Person = (Person)ComboBoxExpensePerson.SelectedItem;
                    _expense.Type = (ExpenseType)ComboBoxExpenseType.SelectedItem;

                    // if (_expense.Vault != (MoneyVault)ComboBoxExpenseVault.SelectedItem)
                    // {
                    _expense.Vault.Remove(_expense);
                    _expense.Vault = (MoneyVault)ComboBoxExpenseVault.SelectedItem;
                    _expense.Vault.DecreaseBalance(
                        new Expense
                        (
                      Convert.ToDecimal(TextBoxExpenseAmount.Text),
                      (DateTime)DatePickerExpenseDate.SelectedDate,
                      (Person)ComboBoxExpensePerson.SelectedItem,
                      TextBoxExpenseComment.Text,
                      (ExpenseType)ComboBoxExpenseType.SelectedItem
                        )
                    );
                    // }
                    UpdateIncomesView();
                    MessageBox.Show("ок");
                }
                catch (ArgumentException)
                {
                    TextBoxExpenseAmount.Text = string.Empty;
                    TextBoxExpenseComment.Text = string.Empty;
                    DatePickerExpenseDate.SelectedDate = DateTime.Today;
                    MessageBox.Show("На выбранном счёте не достаточно средств", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            else if ((string)ButtonAddExpense.Content == "Добавить")
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
                    MessageBox.Show("потратил");
                    UpdateIncomesView();
                }
                catch (ArgumentException)
                {
                    TextBoxExpenseAmount.Text = string.Empty;
                    TextBoxExpenseComment.Text = string.Empty;
                    DatePickerExpenseDate.SelectedDate = DateTime.Today;
                    MessageBox.Show("На выбранном счёте не достаточно средств", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal n = 0;
            if (!Decimal.TryParse(TextBoxExpenseAmount.Text, out n))
            {
                ButtonAddExpense.IsEnabled = false;
                //TextBoxExpenseAmount.Background = Brushes.HotPink;
                
            }
            else
            {
                ButtonAddExpense.IsEnabled = true;
                //TextBoxExpenseAmount.Background = Brushes.Yellow;
            }
        }

        private void UpdateIncomesView()
        {
            TextBoxExpenseAmount.Text = string.Empty;
            TextBoxExpenseComment.Text = string.Empty;
            DatePickerExpenseDate.SelectedDate = DateTime.Today;
            listOfExpensesView.Clear();
            var allExpenses = Storage.GetAllExpences();
            foreach (var item in allExpenses)
            {
                listOfExpensesView.Add(item);
            }
        }

        private void GridViewColumnHeaderExpenses_ClickedOnHeader(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked == null)
            {
                return;
            }

            if ((string)headerClicked.Content == "Удалить" || (string)headerClicked.Content == "Изменить")
            {
                return;
            }
            ListSortDirection direction;


            if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                if (headerClicked != _lastHeaderClicked)
                {
                    direction = ListSortDirection.Ascending;
                }
                else
                {
                    if (_lastDirection == ListSortDirection.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                    }
                }

                var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                SortExpenses(sortBy, direction);

                if (direction == ListSortDirection.Ascending)
                {
                    headerClicked.Column.HeaderTemplate =
                      Resources["HeaderTemplateArrowUp"] as DataTemplate;
                }
                else
                {
                    headerClicked.Column.HeaderTemplate =
                      Resources["HeaderTemplateArrowDown"] as DataTemplate;
                }

                // Remove arrow from previously sorted header
                if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                {
                    _lastHeaderClicked.Column.HeaderTemplate = null;
                }

                _lastHeaderClicked = headerClicked;
                _lastDirection = direction;
            }

        }
        private void SortExpenses(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(listViewForExpenses.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}
