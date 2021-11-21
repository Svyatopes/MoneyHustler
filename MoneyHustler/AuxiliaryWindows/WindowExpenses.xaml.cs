using System;
using System.Collections;
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
        private Storage _storageInstance = Storage.GetInstance();

        private Expense _expense;
        private DateTime _dateStartForView;
        private DateTime _dateEndForView;

        private ObservableCollection<Expense> listOfExpensesView;
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Descending;
        
        SoundPlayer spZdarova = new SoundPlayer();
        SoundPlayer spMaloDeneg = new();
        SoundPlayer spAuf = new();
        SoundPlayer spOnView = new SoundPlayer();
        
        //TODO: сделать более понятные названия x:Name

        private void LoadAudio() //метод загрузки звукового сопровождения
        {
            spZdarova.SoundLocation = "Audio/zdarova.wav";
            spZdarova.LoadAsync();
            spMaloDeneg.SoundLocation = "Audio/kavo.wav";
            spMaloDeneg.Load();
            spAuf.SoundLocation = "Audio/auf.wav";
            spAuf.Load();
            spOnView.SoundLocation = "Audio/onView.wav";
            spOnView.LoadAsync();
        }

        public WindowExpenses()
        {
            InitializeComponent();
            LoadAudio();
            
            spZdarova.Play();

            listOfExpensesView = new ObservableCollection<Expense>(Storage.GetAllExpences());
            listViewForExpenses.ItemsSource = listOfExpensesView;
            _dateStartForView = DateTime.Now.AddYears(-20);
            _dateEndForView = DateTime.Now;
            //TODO:ГОТОВО перевести на SetItemSourceAndSelectedIndexToZero 
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpensePerson, _storageInstance.Persons);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseVault, _storageInstance.Vaults);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseType, _storageInstance.ExpenseTypes);

            DatePickerExpenseDate.SelectedDate = DateTime.Now;
            SortExpenses("Date", _lastDirection);
        }

        private void SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBox comboBox, IEnumerable source) //
        {
            if (source is null)
            {
                comboBox.SelectedItem = null;
                return;
            }
            comboBox.ItemsSource = source;
            comboBox.SelectedIndex = 0;
        }

        private void ButtonDeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            SoundPlayer spDelete = new() { SoundLocation = "Audio/nePonyal.wav" };
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

            //TODO: попробовать вынести в отдельный метод все изменения View
            ButtonAddEditExpense.Content = "Сохраните";
            listViewForExpenses.IsEnabled = false;
            StackPanelOnlyExpense.IsEnabled = false;
            StackPanelSelectDateOnDisplay.IsEnabled = false;
            ComboBoxExpensePerson.SelectedItem = expense.Person;
            ComboBoxExpenseVault.SelectedItem = expense.Vault;
            ComboBoxExpenseType.SelectedItem = expense.Type;
            DatePickerExpenseDate.SelectedDate = expense.Date;
            TextBoxExpenseComment.Text = expense.Comment;
            TextBoxExpenseAmount.Text = expense.Amount.ToString();
            _expense = expense;
        }

        private void ButtonAddEditExpense_Click(object sender, RoutedEventArgs e)
        {

            if ((string)ButtonAddEditExpense.Content == "Сохраните")
            {
                listViewForExpenses.IsEnabled = true;
                StackPanelOnlyExpense.IsEnabled = true;
                StackPanelSelectDateOnDisplay.IsEnabled = true;
                ButtonAddEditExpense.Content = "Добавить";

                MoneyVault newVault = (MoneyVault)ComboBoxExpenseVault.SelectedItem;
                //записываем кошель для проверки баланса по датам и сумме расхода

                //TODO: проверять isEnable текстбокса, куда вводили сумму
                decimal balanceOnSelectDay = newVault.GetBalanceOnDate((DateTime)DatePickerExpenseDate.SelectedDate);
                //расчитываем баланс на выбранный в календаре день

                if (_expense.Date > (DateTime)DatePickerExpenseDate.SelectedDate //если дата до изменения старше даты после
                && Convert.ToDecimal(TextBoxExpenseAmount.Text) > balanceOnSelectDay //и введённая сумма больше суммы на тот день
                || (_expense.Date < (DateTime)DatePickerExpenseDate.SelectedDate && //если изменяемая дата раньше новой и
                   (_expense.Amount + balanceOnSelectDay) < Convert.ToDecimal(TextBoxExpenseAmount.Text))) //сумма на будущий день + изменяемая меньше введённой
                {
                    TextBoxExpenseAmount.Text = string.Empty;
                    TextBoxExpenseComment.Text = string.Empty;
                    DatePickerExpenseDate.SelectedDate = DateTime.Today;
                    
                    
                    spMaloDeneg.Play();

                    MessageBox.Show("На этом счету недостаточно средств на выбранную дату", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //TODO: проверка достаточно ли средств на данную дату чтобы сделать расход
                //(учитывая изменение суммы:
                // 1) если сумма выросла, может не хватить средств
                // 2) мы должны учитывать что баланс на ту дату изменится когда мы поменяем этот расход
                // 2.1) если мы получаем баланс на дату 5000, а нужно 5500 расход сделать не факт, что мы не можем, потому что
                // при переносе расхода вперед по датам баланс увеличится
                _expense.Amount = Convert.ToDecimal(TextBoxExpenseAmount.Text);
                _expense.Comment = TextBoxExpenseComment.Text;
                _expense.Date = (DateTime)DatePickerExpenseDate.SelectedDate;
                _expense.Person = (Person)ComboBoxExpensePerson.SelectedItem;
                _expense.Type = (ExpenseType)ComboBoxExpenseType.SelectedItem;
                _expense.Vault = (MoneyVault)ComboBoxExpenseVault.SelectedItem;

                TextBoxExpenseAmount.Text = string.Empty;
                TextBoxExpenseComment.Text = string.Empty;
                DatePickerExpenseDate.SelectedDate = DateTime.Today;

                UpdateIncomesViewAndClearAddEditArea();
                MessageBox.Show("ок");
   
            }

            else if ((string)ButtonAddEditExpense.Content == "Добавить")
            {
                Expense newExpense = new Expense
                (
                   Convert.ToDecimal(TextBoxExpenseAmount.Text),
                   (DateTime)DatePickerExpenseDate.SelectedDate,
                   (Person)ComboBoxExpensePerson.SelectedItem,
                   TextBoxExpenseComment.Text,
                   (ExpenseType)ComboBoxExpenseType.SelectedItem
                );
                decimal balanceOnSelectDay = ((MoneyVault)ComboBoxExpenseVault.SelectedItem).
                    GetBalanceOnDate((DateTime)DatePickerExpenseDate.SelectedDate);
                //расчитываем баланс на выбранный в календаре день
                if (newExpense.Amount > balanceOnSelectDay)
                {
                    TextBoxExpenseAmount.Text = string.Empty;
                    TextBoxExpenseComment.Text = string.Empty;
                    DatePickerExpenseDate.SelectedDate = DateTime.Today;

                    spMaloDeneg.Play();
                    MessageBox.Show("На выбранном счёте не достаточно средств на выбранную дату", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    ((MoneyVault)ComboBoxExpenseVault.SelectedItem).DecreaseBalance(newExpense);
                    
                    
                    spAuf.Play();
                    MessageBox.Show("потратил");
                    UpdateIncomesViewAndClearAddEditArea();

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
                ButtonAddEditExpense.IsEnabled = false;
                //TextBoxExpenseAmount.Background = Brushes.HotPink;

            }
            else
            {
                ButtonAddEditExpense.IsEnabled = true;
                //TextBoxExpenseAmount.Background = Brushes.Yellow;
            }
        }

        private void UpdateIncomesViewAndClearAddEditArea()
        {
            TextBoxExpenseAmount.Text = string.Empty;
            TextBoxExpenseComment.Text = string.Empty;
            DatePickerExpenseDate.SelectedDate = DateTime.Today;
            listOfExpensesView.Clear();

            if (0 == ComboBoxOfClassificationExpenses.SelectedIndex)
            {
                //TODO: void UpdateExpenseViewList( можно ли вставить )
                var allExpenses = Storage.GetAllExpences();
                foreach (Expense item in allExpenses)
                {
                    if (item.Date >= _dateStartForView && item.Date <= _dateEndForView
                        && item.Vault == ComboBoxClassExpenses.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }

                }
            }
            else if (1 == ComboBoxOfClassificationExpenses.SelectedIndex)
            {
                var allExpenses = Storage.GetAllExpences();
                foreach (Expense item in allExpenses)
                {
                    if (item.Date >= _dateStartForView && item.Date <= _dateEndForView
                        && item.Type == ComboBoxClassExpenses.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }

                }
            }
            else if (2 == ComboBoxOfClassificationExpenses.SelectedIndex)
            {
                var allExpenses = Storage.GetAllExpences();
                foreach (Expense item in allExpenses)
                {
                    if (item.Date >= _dateStartForView && item.Date <= _dateEndForView
                        && item.Person == ComboBoxClassExpenses.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }

                }
            }
            else 
            {
                var allExpenses = Storage.GetAllExpences();
                foreach (Expense item in allExpenses)
                {
                    if (item.Date >= _dateStartForView && item.Date <= _dateEndForView)
                    {
                        listOfExpensesView.Add(item);
                    }

                }

            }
            SortExpenses("Date", _lastDirection);

        }
        #region RegionForSort
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
        #endregion
        private void ButtonViewClassificationExpenses_Click(object sender, RoutedEventArgs e)
        {
            if ((string)ButtonViewClassificationExpenses.Content == "Показать расходы по")
            {
               
                spOnView.Play();
                //TODO: отдельный метод
                ButtonViewClassificationExpenses.Content = "К общему списку";
                SetIsEnabledForItemsOnStackPanel(true);
                //StackPanelControlTemplateExpense.IsEnabled = false;//
                SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpensePerson, null);
                SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseVault, null);
                SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseType, null);
                //ComboBoxExpensePerson.SelectedItem = null;
                //ComboBoxExpenseVault.SelectedItem = null;
                //ComboBoxExpenseType.SelectedItem = null;
                DatePickerExpenseDate.SelectedDate = null;
                //ComboBoxOfClassificationExpenses.IsEnabled = true;//
                //ComboBoxClassExpenses.IsEnabled = true;//

            }
            else if ((string)ButtonViewClassificationExpenses.Content == "К общему списку")
            {
                //TODO: отдельный метод
                ButtonViewClassificationExpenses.Content = "Показать расходы по";
                SetIsEnabledForItemsOnStackPanel(false);
                //StackPanelControlTemplateExpense.IsEnabled = true;//
                SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpensePerson, _storageInstance.Persons);
                SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseVault, _storageInstance.Vaults);
                SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseType, _storageInstance.ExpenseTypes);
                //ComboBoxExpensePerson.SelectedItem = Storage.Persons[0];
                //ComboBoxExpenseVault.SelectedItem = Storage.Vaults[0];
                //ComboBoxExpenseType.SelectedItem = Storage.ExpenseTypes[0];
                DatePickerExpenseDate.SelectedDate = DateTime.Now;
                //ComboBoxOfClassificationExpenses.IsEnabled = false;//
                //ComboBoxClassExpenses.IsEnabled = false;//
                ComboBoxOfClassificationExpenses.SelectedItem = null;
                ComboBoxClassExpenses.SelectedItem = null;
                UpdateIncomesViewAndClearAddEditArea();
            }
        }

        private void ComboBoxOfClassificationExpenses_Selected(object sender, RoutedEventArgs e)
        {

            if (ComboBoxOfClassificationExpenses.SelectedIndex == 0)
            {
                ComboBoxClassExpenses.ItemsSource = _storageInstance.Vaults;
                ComboBoxClassExpenses.SelectedItem = _storageInstance.Vaults[0];

            }

            else if (ComboBoxOfClassificationExpenses.SelectedIndex == 1)
            {
                ComboBoxClassExpenses.ItemsSource = _storageInstance.ExpenseTypes;
                ComboBoxClassExpenses.SelectedItem = _storageInstance.ExpenseTypes[0];

            }

            else if (ComboBoxOfClassificationExpenses.SelectedIndex == 2)
            {
                ComboBoxClassExpenses.ItemsSource = _storageInstance.Persons;
                ComboBoxClassExpenses.SelectedItem = _storageInstance.Persons[0];
            }
            
            else
            {
                return;
            }
        }

        private void ComboBoxClassExpenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void ComboBoxSelectPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOfExpensesView == null)
            {
                return;
            }

            if (ComboBoxSelectPeriod.SelectedIndex == 0)
            {               
                ChangePeriodOnDisplay(4000);
            }

            else if (ComboBoxSelectPeriod.SelectedIndex == 1)
            {
                ChangePeriodOnDisplay(0);               
            }

            else if (ComboBoxSelectPeriod.SelectedIndex == 2)
            {
                ChangePeriodOnDisplay(7);
            }
            else if (ComboBoxSelectPeriod.SelectedIndex == 3)
            {
                ChangePeriodOnDisplay(30);
            }

            else if (ComboBoxSelectPeriod.SelectedIndex == 4)
            {
                StackPanelSelectDateOnDisplay.Visibility = Visibility.Visible;
                StackPanelSelectDateOnDisplay.IsEnabled = true;
                DatePickerSelectStartPeriodOrDayExpenses.IsEnabled = true;

            }         
        }

        private void DatePickerSelectEndPeriodExpenses_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _dateEndForView = (DateTime)DatePickerSelectEndPeriodExpenses.SelectedDate;
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void DatePickerSelectStartPeriodOrDayExpenses_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _dateStartForView = (DateTime)DatePickerSelectStartPeriodOrDayExpenses.SelectedDate;
            DatePickerSelectEndPeriodExpenses.IsEnabled = true;
            DatePickerSelectEndPeriodExpenses.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddYears(-10), _dateStartForView));
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void ChangePeriodOnDisplay(int inputDays)
        {
            StackPanelSelectDateOnDisplay.Visibility = Visibility.Hidden;
            StackPanelSelectDateOnDisplay.IsEnabled = false;
            _dateStartForView = DateTime.Today.AddDays(-inputDays);
            _dateEndForView = DateTime.Now;
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void SetIsEnabledForItemsOnStackPanel(bool isEnable)
        {
            StackPanelControlTemplateExpense.IsEnabled = !isEnable;
            ComboBoxOfClassificationExpenses.IsEnabled = isEnable;
            ComboBoxClassExpenses.IsEnabled = isEnable;
        }
    }
}
