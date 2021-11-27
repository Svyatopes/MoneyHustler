using MoneyHustler.Models;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoneyHustler.Tabs
{
    /// <summary>
    /// Interaction logic for TabItemExpenses.xaml
    /// </summary>
    public partial class TabItemExpenses : TabItem
    {
        private Storage _storageInstance;

        private Expense _expense;
        private DateTime _dateStartForView;
        private DateTime _dateEndForView;

        private ObservableCollection<Expense> listOfExpensesView;
        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirection;

        SoundPlayer soundAccompainementZdarova;
        SoundPlayer soundAccompainementMaloDeneg;
        SoundPlayer soundAccompainementAuf;
        SoundPlayer soundAccompainementOnView;
        SoundPlayer soundAccompainementDelete;


        //TODO: сделать более понятные названия x:Name

        private void LoadAudio() //метод загрузки звукового сопровождения
        {
            soundAccompainementZdarova = new SoundPlayer();
            soundAccompainementZdarova.SoundLocation = "Audio/zdarova.wav";
            soundAccompainementMaloDeneg = new SoundPlayer();
            soundAccompainementMaloDeneg.SoundLocation = "Audio/kavo.wav";
            soundAccompainementAuf = new SoundPlayer();
            soundAccompainementAuf.SoundLocation = "Audio/auf.wav";
            soundAccompainementOnView = new SoundPlayer();
            soundAccompainementOnView.SoundLocation = "Audio/onView.wav";
            soundAccompainementDelete = new SoundPlayer();
            soundAccompainementDelete.SoundLocation = "Audio/nePonyal.wav";
        }

        public TabItemExpenses()
        {
            InitializeComponent();

            LoadAudio();
            soundAccompainementZdarova.Play();
            _storageInstance = Storage.GetInstance();

            listOfExpensesView = new ObservableCollection<Expense>(Storage.GetAllExpences());
            listViewForExpenses.ItemsSource = listOfExpensesView;
            _dateStartForView = DateTime.MinValue;
            _dateEndForView = DateTime.MaxValue;

            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpensePerson, _storageInstance.Persons);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseVault, _storageInstance.Vaults);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseType, _storageInstance.ExpenseTypes);
            DatePickerChooseDateOfExpense.SelectedDate = DateTime.Now;

            _lastHeaderClicked = null;
            _lastDirection = ListSortDirection.Descending;
            SortExpenses("Date", _lastDirection);
        }

        #region Sort
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

        #region Buttons
        private void ButtonDeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            soundAccompainementDelete.Play();

            var button = (Button)sender;
            var expense = (Expense)button.DataContext;

            listOfExpensesView.Remove(expense);
            expense.Vault.Remove(expense);
            Storage.Save();
        }

        private void ButtonEditExpense_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var expense = (Expense)button.DataContext;

            ChangeStateListAreaAndSetButtonAddEditContent("Сохраните", false);

            //переносим данные в стек панель для записи расхода, теперь там будем редактировать расход
            ChangeAddEditViewState(expense);
            _expense = expense; //записываем в поле ссылку на расход
        }

        private void ButtonAddEditExpense_Click(object sender, RoutedEventArgs e)
        {
            var personName = ComboBoxExpensePerson.Text.Trim();
            if (string.IsNullOrWhiteSpace(personName))
            {
                MessageBox.Show("Вы должны заполнить персону");
                return;
            }
            var expenseTypeName = ComboBoxExpenseType.Text.Trim();
            if (string.IsNullOrWhiteSpace(expenseTypeName))
            {
                MessageBox.Show("Вы должны заполнить категорию расхода");
                return;
            }

            var person = Storage.GetOrCreatePersonByName(personName);
            var expenseType = Storage.GetOrCreateExpenseTypeByName(expenseTypeName);
            var chooseVaultForEditExpense = (MoneyVault)ComboBoxExpenseVault.SelectedItem;

            DateTime chooseDateOfExpense = (DateTime)DatePickerChooseDateOfExpense.SelectedDate;

            if (!ExpenseAmountTryParse(TextBoxExpenseAmount.Text, out decimal expenseAmount))
            {
                MessageBox.Show("Введите корректно сумму расхода");
                return;
            }

            if ((string)ButtonAddEditExpense.Content == "Сохраните")
            {
                if (!CheckingPossipilityOfEditingExpense(chooseVaultForEditExpense, chooseDateOfExpense))
                {
                    var userAnswer = MessageBox.Show("Ваш баланс может уйти в минус. Хотите продолжить?", "Предупреждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    soundAccompainementMaloDeneg.Play();
                    if (userAnswer == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                _expense.Amount = expenseAmount;
                _expense.Comment = TextBoxExpenseComment.Text.Trim();
                _expense.Date = chooseDateOfExpense;
                _expense.Person = person;
                _expense.Type = expenseType;
                _expense.Vault = chooseVaultForEditExpense;
                UpdateExpensesViewAndClearAddEditArea(); //иначе не обновляет
                ChangeStateListAreaAndSetButtonAddEditContent("Добавить", true);
                MessageBox.Show("ок");
            }

            else if ((string)ButtonAddEditExpense.Content == "Добавить")
            {
                decimal balanceOnSelectDay = chooseVaultForEditExpense.GetBalanceOnDate(chooseDateOfExpense);
                //расчитываем баланс на выбранный в календаре день
                if (expenseAmount > balanceOnSelectDay //в прошлом может и было много денег
                    || expenseAmount > chooseVaultForEditExpense.GetBalance())
                //но не факт, что сейчас я могу себе такое позволить
                {
                    var userAnswer = MessageBox.Show("Ваш баланс может уйти в минус. Хотите продолжить?", "Предупреждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    soundAccompainementMaloDeneg.Play();
                    if (userAnswer == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                Expense newExpense = new Expense
                (
                   expenseAmount,
                   chooseDateOfExpense,
                   person,
                   TextBoxExpenseComment.Text.Trim(),
                   expenseType
                );

                DecreaseBalanceOfSelectedVaultType(chooseVaultForEditExpense, newExpense);
                listOfExpensesView.Add(newExpense);
                soundAccompainementAuf.Play();
                MessageBox.Show("потратил");
            }
            Storage.Save();
            SetDefaultInAddEditArea();
        }

        private void ButtonEnableFilter_Click(object sender, RoutedEventArgs e)
        {
            if ((string)ButtonEnableFilter.Content == "Показать расходы по")
            {

                soundAccompainementOnView.Play();
                //TODO: отдельный метод
                ButtonEnableFilter.Content = "К общему списку";
                SetIsEnabledForItemsOnStackPanel(true);
                DatePickerChooseDateOfExpense.SelectedDate = null;

            }
            else if ((string)ButtonEnableFilter.Content == "К общему списку")
            {
                //TODO: отдельный метод
                ButtonEnableFilter.Content = "Показать расходы по";
                SetIsEnabledForItemsOnStackPanel(false);
                DatePickerChooseDateOfExpense.SelectedDate = DateTime.Now;
                ComboBoxFilterList.SelectedItem = null;
                ComboBoxItemOfFilterList.SelectedItem = null;
            }
            UpdateExpensesViewAndClearAddEditArea();
        }

        #endregion

        #region TextBoxes
        private bool ExpenseAmountTryParse(string expenseAmountInString, out decimal expenseAmount)
        {
            return Decimal.TryParse(expenseAmountInString, out expenseAmount) && expenseAmount >= 0;
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ExpenseAmountTryParse(TextBoxExpenseAmount.Text, out decimal n))
                ButtonAddEditExpense.IsEnabled = false;

            else
                ButtonAddEditExpense.IsEnabled = true;
        }
        #endregion

        #region ComboBoxes
        private void SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBox comboBox, IEnumerable source) //
        {
            comboBox.ItemsSource = source;
            comboBox.SelectedIndex = 0;
        }
        private enum ComboBoxFilterItems
        {
            Vault,
            ExpenseType,
            Persons
        }

        private void ComboBoxOfClassificationExpenses_Selected(object sender, RoutedEventArgs e)
        {
            switch (ComboBoxFilterList.SelectedIndex)
            {
                case (int)ComboBoxFilterItems.Vault:
                    SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxItemOfFilterList, _storageInstance.Vaults);
                    break;
                case (int)ComboBoxFilterItems.ExpenseType:
                    SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxItemOfFilterList, _storageInstance.ExpenseTypes);
                    break;
                case (int)ComboBoxFilterItems.Persons:
                    SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxItemOfFilterList, _storageInstance.Persons);
                    break;
                default:
                    return;
            }
        }

        private void ComboBoxClassExpenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateExpensesViewAndClearAddEditArea();
        }

        private enum ItemsOfComboBoxSelectPeriodLastExpenses
        {
            AllTime,
            Today,
            LastWeek,
            LastMonth,
            ChooseYourself
        }

        private void ComboBoxSelectPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOfExpensesView == null)
            {
                return;
            }

            switch (ComboBoxSelectPeriodLastExpenses.SelectedIndex)
            {
                case (int)ItemsOfComboBoxSelectPeriodLastExpenses.AllTime:
                    ChangeFilterByDatesInListView(DateTime.MinValue);
                    ChangeStateAndVisibilityStackPanelSelectDateIncomesOnDisplay(false);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastExpenses.Today:
                    ChangeFilterByDatesInListView(DateTime.Now.Date);
                    ChangeStateAndVisibilityStackPanelSelectDateIncomesOnDisplay(false);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastExpenses.LastWeek:
                    ChangeFilterByDatesInListView(DateTime.Now.AddDays(-7).Date);
                    ChangeStateAndVisibilityStackPanelSelectDateIncomesOnDisplay(false);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastExpenses.LastMonth:
                    ChangeFilterByDatesInListView(DateTime.Now.AddMonths(-1).Date);
                    ChangeStateAndVisibilityStackPanelSelectDateIncomesOnDisplay(false);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastExpenses.ChooseYourself:
                    ChangeStateAndVisibilityStackPanelSelectDateIncomesOnDisplay(true);
                    break;
                default:
                    return;
            }
        }
        #endregion

        #region DatePickers
        private void DatePickerSelectStartPeriodOrDayExpenses_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime dateInPickerSelectStartPeriod = (DateTime)DatePickerSelectStartPeriodOrDayExpenses.SelectedDate;
            if (dateInPickerSelectStartPeriod > _dateEndForView)
            {
                _dateEndForView = DateTime.MaxValue;
                DatePickerSelectEndPeriodExpenses.SelectedDate = _dateEndForView;
            }
            _dateStartForView = dateInPickerSelectStartPeriod;
            DatePickerSelectEndPeriodExpenses.BlackoutDates.Clear();
            DatePickerSelectEndPeriodExpenses.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, _dateStartForView));
            UpdateExpensesViewAndClearAddEditArea();
        }

        private void DatePickerSelectEndPeriodExpenses_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _dateEndForView = (DateTime)DatePickerSelectEndPeriodExpenses.SelectedDate;
            UpdateExpensesViewAndClearAddEditArea();
        }

        private void ChangeStateAndVisibilityStackPanelSelectDateIncomesOnDisplay(bool isEnableAndVisible)
        {
            switch (isEnableAndVisible)
            {
                case true:
                    StackPanelSelectDateExpensesOnDisplay.Visibility = Visibility.Visible;
                    break;
                case false:
                    StackPanelSelectDateExpensesOnDisplay.Visibility = Visibility.Hidden;
                    break;
            }
            StackPanelSelectDateExpensesOnDisplay.IsEnabled = isEnableAndVisible;
        }

        private void ChangeFilterByDatesInListView(DateTime startDate)
        {
            _dateStartForView = startDate;
            _dateEndForView = (startDate == DateTime.MinValue) ? DateTime.MaxValue : DateTime.Now;
            UpdateExpensesViewAndClearAddEditArea();
        }
        #endregion

        #region WorkWithModel
        private void DecreaseBalanceOfSelectedVaultType(MoneyVault chooseVaultForEditExpense, Expense newExpense)
        {
            switch (chooseVaultForEditExpense)
            {
                case Card:
                    ((Card)chooseVaultForEditExpense).DecreaseBalance(newExpense);
                    break;
                case OnlyTopDeposit:
                    ((OnlyTopDeposit)chooseVaultForEditExpense).DecreaseBalance(newExpense);
                    break;
                case Deposit:
                    ((Deposit)chooseVaultForEditExpense).DecreaseBalance(newExpense);
                    break;
                default:
                    return;
            }
        }


        #endregion

        #region StatesIemsAndView

        private bool CheckingPossipilityOfEditingExpense(MoneyVault vaultForEditExpense, DateTime chooseDateOfExpense)
        {
            decimal balanceOnSelectDay = vaultForEditExpense.GetBalanceOnDate(chooseDateOfExpense);

            if (_expense.Date >= chooseDateOfExpense &&
                Convert.ToDecimal(TextBoxExpenseAmount.Text) > balanceOnSelectDay //и введённая сумма больше суммы на тот день
            || _expense.Date < chooseDateOfExpense && //если изменяемая дата раньше новой и
               (_expense.Amount + balanceOnSelectDay) < Convert.ToDecimal(TextBoxExpenseAmount.Text))//сумма на будущий день + изменяемая меньше введённой
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private void ChangeAddEditViewState(Expense expense)
        {
            ComboBoxExpensePerson.SelectedItem = expense.Person;
            ComboBoxExpenseVault.SelectedItem = expense.Vault;
            ComboBoxExpenseType.SelectedItem = expense.Type;
            DatePickerChooseDateOfExpense.SelectedDate = expense.Date;
            TextBoxExpenseComment.Text = expense.Comment;
            TextBoxExpenseAmount.Text = expense.Amount.ToString();
        }

        private void ChangeStateListAreaAndSetButtonAddEditContent(string buttonAddEditContent, bool isEnabled)
        {
            ButtonAddEditExpense.Content = buttonAddEditContent;
            listViewForExpenses.IsEnabled = isEnabled;
            StackPanelFilterExpense.IsEnabled = isEnabled;
            StackPanelSelectDateExpensesOnDisplay.IsEnabled = isEnabled;
        }

        private void SetDefaultInAddEditArea()
        {
            TextBoxExpenseAmount.Text = string.Empty;
            TextBoxExpenseComment.Text = string.Empty;
            DatePickerChooseDateOfExpense.SelectedDate = DateTime.Today;
        }
        private void SetIsEnabledForItemsOnStackPanel(bool isEnable)
        {
            ComboBoxFilterList.IsEnabled = isEnable;
            ComboBoxItemOfFilterList.IsEnabled = isEnable;
        }

        private void UpdateExpensesViewAndClearAddEditArea()
        {
            listOfExpensesView.Clear();

            var allExpenses = Storage.GetAllExpences().Where(item => item.Date >= _dateStartForView && item.Date <= _dateEndForView);
            if (ComboBoxFilterList.SelectedIndex == (int)ComboBoxFilterItems.Vault)
            {
                foreach (Expense item in allExpenses)
                {
                    if (item.Vault == ComboBoxItemOfFilterList.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }
                }
            }
            else if (ComboBoxFilterList.SelectedIndex == (int)ComboBoxFilterItems.ExpenseType)
            {
                foreach (Expense item in allExpenses)
                {
                    if (item.Type == ComboBoxItemOfFilterList.SelectedItem)
                    {
                        listOfExpensesView.Add(item);
                    }

                }
            }
            else if (ComboBoxFilterList.SelectedIndex == (int)ComboBoxFilterItems.Persons)
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

        private void TabItemExpenses_Selected(object sender, RoutedEventArgs e)
        {
            UpdateExpensesViewAndClearAddEditArea();
        }

        #endregion

    }
}
