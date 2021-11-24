using MoneyHustler.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoneyHustler.Tabs
{
    /// <summary>
    /// Interaction logic for TabItemExpenses.xaml
    /// </summary>
    public partial class TabItemExpenses : TabItem
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

        public TabItemExpenses()
        {
            InitializeComponent();
            LoadAudio();

            spZdarova.Play();

            listOfExpensesView = new ObservableCollection<Expense>(Storage.GetAllExpences());
            listViewForExpenses.ItemsSource = listOfExpensesView;
            _dateStartForView = DateTime.MinValue;
            _dateEndForView = DateTime.MaxValue;


            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpensePerson, _storageInstance.Persons);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseVault, _storageInstance.Vaults);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseType, _storageInstance.ExpenseTypes);

            DatePickerExpenseDate.SelectedDate = DateTime.Now;
            SortExpenses("Date", _lastDirection);
        }

        private void SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBox comboBox, IEnumerable source) //
        {
            comboBox.ItemsSource = source;
            comboBox.SelectedIndex = 0;
        }

        private void ChangeEnabledDisplayAndButtonContentInModEdit(string buttonAddEditContent, bool isEnabled)
        {
            ButtonAddEditExpense.Content = buttonAddEditContent;
            listViewForExpenses.IsEnabled = isEnabled;
            StackPanelOnlyExpense.IsEnabled = isEnabled;
            StackPanelSelectDateOnDisplay.IsEnabled = isEnabled;
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

            ChangeEnabledDisplayAndButtonContentInModEdit("Сохраните", false);

            //переносим данные в стек панель для записи расхода, теперь там будем редактировать расход
            ComboBoxExpensePerson.SelectedItem = expense.Person;
            ComboBoxExpenseVault.SelectedItem = expense.Vault;
            ComboBoxExpenseType.SelectedItem = expense.Type;
            DatePickerExpenseDate.SelectedDate = expense.Date;
            TextBoxExpenseComment.Text = expense.Comment;
            TextBoxExpenseAmount.Text = expense.Amount.ToString();
            _expense = expense; //записываем в поле ссылку на расход
        }

        private void EditExpense(MoneyVault vaultForDecreaseBalance)
        {
            //записываем кошель для проверки баланса по датам и сумме расхода
            //нужно выполнить проверку, является ли кошелёк картой, депозитом или депозитом с ограничением снятия
            //она выполнится в методе мэни волта

            decimal balanceOnSelectDay = vaultForDecreaseBalance.GetBalanceOnDate((DateTime)DatePickerExpenseDate.SelectedDate);
            //расчитываем баланс на выбранный в календаре день

            //тут для действия, если кошелёк - карта или депозит
            //в случае депозита с ограничением. нужно смотреть на его манибокс
            //нужно отловить все нежелательные случаи
            if (_expense.Date >= (DateTime)DatePickerExpenseDate.SelectedDate &&
                Convert.ToDecimal(TextBoxExpenseAmount.Text) > balanceOnSelectDay //и введённая сумма больше суммы на тот день
            || _expense.Date < (DateTime)DatePickerExpenseDate.SelectedDate && //если изменяемая дата раньше новой и
               (_expense.Amount + balanceOnSelectDay) < Convert.ToDecimal(TextBoxExpenseAmount.Text))//сумма на будущий день + изменяемая меньше введённой
            {
                UpdateIncomesViewAndClearAddEditArea();

                spMaloDeneg.Play();

                MessageBox.Show("На этом счету недостаточно средств на выбранную дату", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _expense.Amount = Convert.ToDecimal(TextBoxExpenseAmount.Text);
            _expense.Comment = TextBoxExpenseComment.Text;
            _expense.Date = (DateTime)DatePickerExpenseDate.SelectedDate;
            _expense.Person = (Person)ComboBoxExpensePerson.SelectedItem;
            _expense.Type = (ExpenseType)ComboBoxExpenseType.SelectedItem;
            _expense.Vault = (Card)ComboBoxExpenseVault.SelectedItem;
            Storage.Save();
            MessageBox.Show("ок");

        }

        private void ButtonAddEditExpense_Click(object sender, RoutedEventArgs e)
        {
            var personName = ComboBoxExpensePerson.Text;
            if (string.IsNullOrEmpty(personName))
            {
                MessageBox.Show("Вы должны заполнить персону");
                return;
            }
            var expenseTypeName = ComboBoxExpenseType.Text;
            if (string.IsNullOrEmpty(expenseTypeName))
            {
                MessageBox.Show("Вы должны заполнить категорию расхода");
                return;
            }



            if ((string)ButtonAddEditExpense.Content == "Сохраните")
            {
                ChangeEnabledDisplayAndButtonContentInModEdit("Добавить", true);

                ComboboxIsEditable(); //проверяем есть ли в комбобоксах новое имя или категория и, если да, записываем в сторэдж

                EditExpense((MoneyVault)ComboBoxExpenseVault.SelectedItem);

                UpdateIncomesViewAndClearAddEditArea();

            }

            else if ((string)ButtonAddEditExpense.Content == "Добавить")
            {

                decimal balanceOnSelectDay = ((MoneyVault)ComboBoxExpenseVault.SelectedItem).
                    GetBalanceOnDate((DateTime)DatePickerExpenseDate.SelectedDate);
                //расчитываем баланс на выбранный в календаре день
                if (Convert.ToDecimal(TextBoxExpenseAmount.Text) > balanceOnSelectDay //в прошлом может и было много денег
                    || (Convert.ToDecimal(TextBoxExpenseAmount.Text) > ((MoneyVault)ComboBoxExpenseVault.SelectedItem).GetBalance()))
                //но не факт, что сейчас я могу себе такое позволить
                {
                    UpdateIncomesViewAndClearAddEditArea();

                    spMaloDeneg.Play();
                    MessageBox.Show("На выбранном счёте не достаточно средств на выбранную дату", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                } //ОТДЕЛЬНЫЙ МЕТОД ВЕРНЁТ БУЛЬКУ И ПРИМЕТ


                var person = Storage.GetOrCreatePersonByName(personName);
                var expenseType = Storage.GetOrCreateExpenseTypeByName(expenseTypeName);

                Expense newExpense = new Expense
                (
                   Convert.ToDecimal(TextBoxExpenseAmount.Text),
                   (DateTime)DatePickerExpenseDate.SelectedDate,
                   person,
                   TextBoxExpenseComment.Text,
                   expenseType
                );

                ((MoneyVault)ComboBoxExpenseVault.SelectedItem).DecreaseBalance(newExpense);

                spAuf.Play();
                Storage.Save();
                MessageBox.Show("потратил");
                UpdateIncomesViewAndClearAddEditArea();

            }
        }



        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal n = 0;
            if (!Decimal.TryParse(TextBoxExpenseAmount.Text, out n) && n > 0)
                ButtonAddEditExpense.IsEnabled = false;

            else
                ButtonAddEditExpense.IsEnabled = true;

        }

        private void UpdateIncomesViewAndClearAddEditArea()
        {
            TextBoxExpenseAmount.Text = string.Empty;
            TextBoxExpenseComment.Text = string.Empty;
            DatePickerExpenseDate.SelectedDate = DateTime.Today;
            listOfExpensesView.Clear();

            var allExpenses = Storage.GetAllExpences().Where(item => item.Date >= _dateStartForView && item.Date <= _dateEndForView);
            if (ComboBoxFilterList.SelectedIndex == 0)
            {
                //TODO: void UpdateExpenseViewList( можно ли вставить )
                foreach (Expense item in allExpenses)
                {
                    if (item.Vault == ComboBoxItemOfFilterList.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }
                }
            }
            else if (ComboBoxFilterList.SelectedIndex == 1)
            {
                foreach (Expense item in allExpenses)
                {
                    if (item.Type == ComboBoxItemOfFilterList.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }

                }
            }
            else if (ComboBoxFilterList.SelectedIndex == 2)
            {
                foreach (Expense item in allExpenses)
                {
                    if (item.Person == ComboBoxItemOfFilterList.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }

                }
            }
            else
            {
                foreach (Expense item in allExpenses)
                {
                    listOfExpensesView.Add(item);

                }

            }
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpensePerson, _storageInstance.Persons);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseVault, _storageInstance.Vaults);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseType, _storageInstance.ExpenseTypes);
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
                DatePickerExpenseDate.SelectedDate = null;

            }
            else if ((string)ButtonViewClassificationExpenses.Content == "К общему списку")
            {
                //TODO: отдельный метод
                ButtonViewClassificationExpenses.Content = "Показать расходы по";
                SetIsEnabledForItemsOnStackPanel(false);
                DatePickerExpenseDate.SelectedDate = DateTime.Now;
                ComboBoxFilterList.SelectedItem = null;
                ComboBoxItemOfFilterList.SelectedItem = null;
                UpdateIncomesViewAndClearAddEditArea();
            }
        }

        private void ComboBoxOfClassificationExpenses_Selected(object sender, RoutedEventArgs e)
        {

            if (ComboBoxFilterList.SelectedIndex == 0)
            {
                ComboBoxItemOfFilterList.ItemsSource = _storageInstance.Vaults;
                ComboBoxItemOfFilterList.SelectedItem = _storageInstance.Vaults[0];

            }

            else if (ComboBoxFilterList.SelectedIndex == 1)
            {
                ComboBoxItemOfFilterList.ItemsSource = _storageInstance.ExpenseTypes;
                ComboBoxItemOfFilterList.SelectedItem = _storageInstance.ExpenseTypes[0];

            }

            else if (ComboBoxFilterList.SelectedIndex == 2)
            {
                ComboBoxItemOfFilterList.ItemsSource = _storageInstance.Persons;
                ComboBoxItemOfFilterList.SelectedItem = _storageInstance.Persons[0];
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
                ChangePeriodOnDisplay(DateTime.MinValue);
            }

            else if (ComboBoxSelectPeriod.SelectedIndex == 1)
            {
                ChangePeriodOnDisplay(DateTime.Now.Date);
            }

            else if (ComboBoxSelectPeriod.SelectedIndex == 2)
            {
                ChangePeriodOnDisplay(DateTime.Now.AddDays(-7).Date);
            }
            else if (ComboBoxSelectPeriod.SelectedIndex == 3)
            {
                ChangePeriodOnDisplay(DateTime.Now.AddMonths(-1).Date);
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
            if ((DateTime)DatePickerSelectStartPeriodOrDayExpenses.SelectedDate > _dateEndForView)
            {
                _dateEndForView = ((DateTime)DatePickerSelectStartPeriodOrDayExpenses.SelectedDate).AddDays(1);
                DatePickerSelectEndPeriodExpenses.SelectedDate = _dateEndForView;
            }
            _dateStartForView = (DateTime)(DatePickerSelectStartPeriodOrDayExpenses.SelectedDate);
            DatePickerSelectEndPeriodExpenses.IsEnabled = true;
            DatePickerSelectEndPeriodExpenses.BlackoutDates.Clear();
            DatePickerSelectEndPeriodExpenses.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, _dateStartForView));
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void ChangePeriodOnDisplay(DateTime startDate)
        {
            StackPanelSelectDateOnDisplay.Visibility = Visibility.Hidden;
            StackPanelSelectDateOnDisplay.IsEnabled = false;
            _dateStartForView = startDate;
            _dateEndForView = DateTime.Now;
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void SetIsEnabledForItemsOnStackPanel(bool isEnable)
        {
            ComboBoxFilterList.IsEnabled = isEnable;
            ComboBoxItemOfFilterList.IsEnabled = isEnable;
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void ComboboxIsEditable()
        {

            if (!(_storageInstance.Persons.Any(item => item.Name == ComboBoxExpensePerson.Text)))
            {
                Person newPerson = new Person { Name = ComboBoxExpensePerson.Text };
                _storageInstance.Persons.Add(newPerson);
                ComboBoxExpensePerson.SelectedItem = newPerson;
            }

            if (!(_storageInstance.ExpenseTypes.Any(item => item.Name == ComboBoxExpenseType.Text)))
            {
                ExpenseType newExpenseType = new ExpenseType { Name = ComboBoxExpenseType.Text };
                _storageInstance.ExpenseTypes.Add(newExpenseType);
                ComboBoxExpenseType.SelectedItem = newExpenseType;
            }
        }
    }
}
