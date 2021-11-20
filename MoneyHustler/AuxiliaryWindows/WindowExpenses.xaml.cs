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
        private List<ComboBoxItem> itemsForComboBoxOfExpesesPeriod;
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        
        public WindowExpenses()
        {
            InitializeComponent();
            itemsForComboBoxOfExpesesPeriod = new();
            itemsForComboBoxOfExpesesPeriod.Add(new ComboBoxItem { Content = "сегодня" });
            itemsForComboBoxOfExpesesPeriod.Add(new ComboBoxItem { Content = "прошедшая неделя" });
            itemsForComboBoxOfExpesesPeriod.Add(new ComboBoxItem { Content = "прошедший месяц" });
            itemsForComboBoxOfExpesesPeriod.Add(new ComboBoxItem { Content = "прошедший год" });
            SoundPlayer spZdarova = new SoundPlayer();
            spZdarova.SoundLocation = "Audio/zdarova.wav";
            spZdarova.LoadAsync();
            spZdarova.Play();
           
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
            SoundPlayer spDelete = new SoundPlayer();
            spDelete.SoundLocation = "Audio/nePonyal.wav";
            spDelete.LoadAsync();
            spDelete.Play();
            var button = (Button)sender;
            var expense = (Expense)button.DataContext;
            listOfExpensesView.Remove(expense);
            expense.Vault.Remove(expense);
            Storage.Save();
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

                    SoundPlayer spMaloDeneg = new();
                    spMaloDeneg.SoundLocation = "Audio/kavo.wav";
                    spMaloDeneg.Load();
                    spMaloDeneg.Play();

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
                    SoundPlayer spAuf = new();
                    spAuf.SoundLocation = "Audio/auf.wav";
                    spAuf.Load();
                    spAuf.Play();
                    MessageBox.Show("потратил");
                    UpdateIncomesView();
                    
                }
                catch (ArgumentException)
                {
                    TextBoxExpenseAmount.Text = string.Empty;
                    TextBoxExpenseComment.Text = string.Empty;
                    DatePickerExpenseDate.SelectedDate = DateTime.Today;
                    SoundPlayer spMaloDeneg = new();
                    spMaloDeneg.SoundLocation = "Audio/maloBabla.wav";
                    spMaloDeneg.Load();
                    spMaloDeneg.Play();
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

        private void ButtonViewClassificationExpenses_Click(object sender, RoutedEventArgs e)
        {
            if ((string)ButtonViewClassificationExpenses.Content == "Показать расходы по")
            {
                listViewForExpenses.IsEnabled = false;
                SoundPlayer spOnView = new SoundPlayer();
                spOnView.SoundLocation = "Audio/onView.wav";
                spOnView.LoadAsync();
                spOnView.Play();
                ButtonViewClassificationExpenses.Content = "К общему списку";
                StackPanelControlTemplateExpense.IsEnabled = false;
                ComboBoxExpensePerson.SelectedItem = null;
                ComboBoxExpenseVault.SelectedItem = null;
                ComboBoxExpenseType.SelectedItem = null;
                DatePickerExpenseDate.SelectedDate = null;
                ComboBoxOfClassificationExpenses.IsEnabled = true;
                ComboBoxClassExpenses.IsEnabled = true;

            }
            else if ((string)ButtonViewClassificationExpenses.Content == "К общему списку")
            {
                listViewForExpenses.IsEnabled = true;
                ButtonViewClassificationExpenses.Content = "Показать расходы по";
                StackPanelControlTemplateExpense.IsEnabled = true;
                ComboBoxExpensePerson.SelectedItem = Storage.Persons[0];
                ComboBoxExpenseVault.SelectedItem = Storage.Vaults[0];
                ComboBoxExpenseType.SelectedItem = Storage.ExpenseTypes[0];
                DatePickerExpenseDate.SelectedDate = DateTime.Now;
                ComboBoxOfClassificationExpenses.IsEnabled = false;
                ComboBoxClassExpenses.IsEnabled = false;
                ComboBoxOfClassificationExpenses.SelectedItem = null;
                ComboBoxClassExpenses.SelectedItem = null;
                UpdateIncomesView();
            }
        }

        private void ComboBoxOfClassificationExpenses_Selected(object sender, RoutedEventArgs e)
        {
            
            if (ComboBoxOfClassificationExpenses.SelectedItem == ComboBoxOfClassificationExpenses.Items[0])
            {
                ComboBoxClassExpenses.DisplayMemberPath = "Name";
                ComboBoxClassExpenses.ItemsSource = Storage.Vaults;
                ComboBoxClassExpenses.SelectedItem = Storage.Vaults[0];
                
            }

            else if (ComboBoxOfClassificationExpenses.SelectedItem == ComboBoxOfClassificationExpenses.Items[1])
            {
                ComboBoxClassExpenses.DisplayMemberPath = "Name";
                ComboBoxClassExpenses.ItemsSource = Storage.ExpenseTypes;
                ComboBoxClassExpenses.SelectedItem = Storage.ExpenseTypes[0];
                
            }

            else if (ComboBoxOfClassificationExpenses.SelectedItem == ComboBoxOfClassificationExpenses.Items[2])
            {
                ComboBoxClassExpenses.DisplayMemberPath = "Name";
                ComboBoxClassExpenses.ItemsSource = Storage.Persons;
                ComboBoxClassExpenses.SelectedItem = Storage.Persons[0];
                
            }
            else if (ComboBoxOfClassificationExpenses.SelectedItem == ComboBoxOfClassificationExpenses.Items[3])
            {
                ComboBoxClassExpenses.DisplayMemberPath = null;
                ComboBoxClassExpenses.ItemsSource = itemsForComboBoxOfExpesesPeriod;
                //ComboBoxClassExpenses.Items.Add(new ComboBoxItem { Content = "сегодня" });
                //ComboBoxClassExpenses.Items.Add(new ComboBoxItem { Content = "прошедшая неделя" });
                //ComboBoxClassExpenses.Items.Add(new ComboBoxItem { Content = "прошедший месяц" });
                //ComboBoxClassExpenses.Items.Add(new ComboBoxItem { Content = "прошедший год" });
                ComboBoxClassExpenses.SelectedItem = ComboBoxClassExpenses.Items[0];
            }
            else
            {
                return;
            }
        }

        private void ComboBoxClassExpenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxClassExpenses.ItemsSource == Storage.Persons)
            {
                listOfExpensesView.Clear();
                var allExpenses = Storage.GetAllExpences();
                foreach (var item in allExpenses)
                {
                    if (item.Person == ComboBoxClassExpenses.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }
                }
            }
            else if (ComboBoxClassExpenses.ItemsSource == Storage.Vaults)
            {
                listOfExpensesView.Clear();
                var allExpenses = Storage.GetAllExpences();
                foreach (var item in allExpenses)
                {
                    if (item.Vault == ComboBoxClassExpenses.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }
                }
            }
            else if (ComboBoxClassExpenses.ItemsSource == Storage.ExpenseTypes)
            {
                listOfExpensesView.Clear();
                var allExpenses = Storage.GetAllExpences();
                foreach (var item in allExpenses)
                {
                    if (item.Type == ComboBoxClassExpenses.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }
                }
            }
            else if (ComboBoxOfClassificationExpenses.SelectedItem == ComboBoxOfClassificationExpenses.Items[3])
            {
                listOfExpensesView.Clear();
                var allExpenses = Storage.GetAllExpences();

                foreach (var item in allExpenses)
                {
                    if (ComboBoxClassExpenses.SelectedItem == ComboBoxClassExpenses.Items[0])
                    {
                        if (DateTime.Today <= item.Date)
                            listOfExpensesView.Add(item);

                    }
                    else if (ComboBoxClassExpenses.SelectedItem == ComboBoxClassExpenses.Items[1])
                    {
                        if (DateTime.Today.AddDays(-7) < item.Date)
                            listOfExpensesView.Add(item);
                    }
                    else if (ComboBoxClassExpenses.SelectedItem == ComboBoxClassExpenses.Items[2])
                    {
                        if (DateTime.Today.AddMonths(-1) < item.Date)
                            listOfExpensesView.Add(item);
                    }
                    else if (ComboBoxClassExpenses.SelectedItem == ComboBoxClassExpenses.Items[3])
                    {
                        if (DateTime.Today.AddYears(-1) < item.Date)
                            listOfExpensesView.Add(item);
                    }
                }
            }
        }
    }
}
