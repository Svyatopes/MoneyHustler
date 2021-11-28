using MoneyHustler.Models;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoneyHustler.Tabs
{
    /// <summary>
    /// Interaction logic for TabItemIncomes.xaml
    /// </summary>
    public partial class TabItemIncomes : TabItem
    {
        private ObservableCollection<Income> listOfIncomesView;
        private Storage _storageInstance;

        private DateTime _dateStartForView;
        private DateTime _dateEndForView;

        private Income _income;
        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirection;
        public TabItemIncomes()
        {
            InitializeComponent();

            _storageInstance = Storage.GetInstance();
            listOfIncomesView = new ObservableCollection<Income>(Storage.GetAllIncomes());
            listViewForIncomes.ItemsSource = listOfIncomesView;
            _dateStartForView = DateTime.MinValue;
            _dateEndForView = DateTime.MaxValue;

            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomePerson, _storageInstance.Persons);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomeVault, _storageInstance.Vaults);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomeType, _storageInstance.IncomeTypes);
            DatePickerIncomeDate.SelectedDate = DateTime.Now;

            _lastHeaderClicked = null;
            _lastDirection = ListSortDirection.Descending;

            Sort("Date", _lastDirection);
        }


        #region Sort
        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
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

                Sort(sortBy, direction);

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

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(listViewForIncomes.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
        #endregion


        #region Buttons

        
        private void ButtonEditIncome_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var income = (Income)button.DataContext;

            ChangeStateListAreaAndSetButtonAddEditContent("Сохраните", false);
            ChangeAddEditViewState(income);
            _income = income;

        }

        private void ButtonDeleteIncome_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var income = (Income)button.DataContext;

            if (income.Vault.GetBalance() - income.Amount < 0)
            {
                var userAnswer = MessageBox.Show("Вы точно хотите удалить этот доход? Ваш баланс может стать меньше нуля...", "Удаление дохода",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (userAnswer == MessageBoxResult.No)
                {
                    return;
                }
            }

            listOfIncomesView.Remove(income);
            income.Vault.Remove(income);
            Storage.Save();
        }

        private void ButtonAddEditIncome_Click(object sender, RoutedEventArgs e)
        {

            var personName = ComboBoxIncomePerson.Text.Trim();
            if (string.IsNullOrWhiteSpace(personName))
            {
                MessageBox.Show("Вы должны заполнить персону");
                return;
            }
            var incomeTypeName = ComboBoxIncomeType.Text.Trim();
            if (string.IsNullOrWhiteSpace(incomeTypeName))
            {
                MessageBox.Show("Вы должны заполнить категорию расхода");
                return;
            }

            var person = Storage.GetOrCreatePersonByName(personName);
            var incomeType = Storage.GetOrCreateIncomeTypeByName(incomeTypeName);
            MoneyVault selectedVault = (MoneyVault)ComboBoxIncomeVault.SelectedItem;
            if (!IncomeAmountTryParse(TextBoxIncomeAmount.Text, out decimal incomeAmount))
            {
                MessageBox.Show("Введите корректно сумму дохода");
                return;
            }
            if ((string)ButtonAddEditIncome.Content == "Сохраните")
            {
                decimal currentBalanceOfVault = _income.Vault.GetBalance();

                if (_income.Vault != selectedVault) //если кошель поменялся
                {
                    var different = currentBalanceOfVault - _income.Amount;
                    //смотрим не станет ли баланс меньше нуля при удалении того расхода из прежнего кошелька
                    if (different < 0)
                    {
                        var userAnswer = MessageBox.Show($"Вы точно хотите перенести этот доход с '{_income.Vault.Name}'? " +
                            $"Баланс может стать меньше 0...", "Перенос дохода", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (userAnswer == MessageBoxResult.No)
                        {
                            return;
                        }

                    }
                }
                else //если кошель остался тем же
                {
                    var different = currentBalanceOfVault - (_income.Amount - Convert.ToDecimal(TextBoxIncomeAmount.Text));
                    //смотрим не стала ли разница между старой суммой и новой обращать баланс в ноль
                    //допустим доход был 500, стал 200, то отнимаем от баланса (500 - 200 =) 300
                    if (different < 0)
                    {
                        var userAnswer = MessageBox.Show($"Вы точно хотите уменьшить доход? Ваш баланс станет равным {different}",
                            "Уменьшение суммы дохода", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (userAnswer == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                } //если все условия соблюдены, то запишем новый доход

                _income.Amount = incomeAmount;
                _income.Type = incomeType;
                _income.Date = (DateTime)DatePickerIncomeDate.SelectedDate;
                _income.Person = person;
                _income.Comment = TextBoxIncomeComment.Text.Trim();

                if (_income.Vault != selectedVault)
                {
                    _income.Vault.Remove(_income);
                    listOfIncomesView.Remove(_income);
                    IncreaseBalanceOfSelectedVaultType(selectedVault, _income);
                    //selectedVault.IncreaseBalance(_income);
                    listOfIncomesView.Add(_income);
                }
                UpdateIncomesViewAndClearAddEditArea(); //иначе не обновляется
                ChangeStateListAreaAndSetButtonAddEditContent("Добавить", true);

            }
            else if ((string)ButtonAddEditIncome.Content == "Добавить")
            {
                Income newIncome = new Income
                (
                   incomeAmount,
                   (DateTime)DatePickerIncomeDate.SelectedDate,
                   person,
                   TextBoxIncomeComment.Text.Trim(),
                   incomeType
                );
                IncreaseBalanceOfSelectedVaultType(selectedVault, newIncome);
                //selectedVault.IncreaseBalance(newIncome);
                listOfIncomesView.Add(newIncome);
                MessageBox.Show("поднял");
            }
            Storage.Save();
            SetDefaultInAddEditArea();
        }

        private void ButtonEnableDisableFilters_Click(object sender, RoutedEventArgs e)
        {
            if ((string)ButtonEnableDisableFilters.Content == "Показать доходы по")
            {
                ButtonEnableDisableFilters.Content = "К общему списку";
                SetIsEnabledForItemsOnStackPanel(true);
                DatePickerIncomeDate.SelectedDate = null;
            }
            else if ((string)ButtonEnableDisableFilters.Content == "К общему списку")
            {
                ButtonEnableDisableFilters.Content = "Показать доходы по";
                SetIsEnabledForItemsOnStackPanel(false);
                DatePickerIncomeDate.SelectedDate = DateTime.Now;
                ComboBoxFilterIncomes.SelectedItem = null;
                ComboBoxItemOfFilter.SelectedItem = null;
            }
            UpdateIncomesViewAndClearAddEditArea();
        }

        #endregion


        #region TextBoxes

        private bool IncomeAmountTryParse(string incomeAmountInString, out decimal incomeAmount)
        {
            return Decimal.TryParse(incomeAmountInString, out incomeAmount) && incomeAmount >= 0;
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal incomeAmount = 0;
            if (!IncomeAmountTryParse(TextBoxIncomeAmount.Text, out incomeAmount))
            {
                ButtonAddEditIncome.IsEnabled = false;
            }
            else
            {
                ButtonAddEditIncome.IsEnabled = true;
            }
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
            IncomeType,
            Persons
        }
        private void ComboBoxFilterIncomes_Selected(object sender, RoutedEventArgs e)
        {

            switch (ComboBoxFilterIncomes.SelectedIndex)
            {
                case (int)ComboBoxFilterItems.Vault:
                    SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxItemOfFilter, _storageInstance.Vaults);
                    break;
                case (int)ComboBoxFilterItems.IncomeType:
                    SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxItemOfFilter, _storageInstance.IncomeTypes);
                    break;
                case (int)ComboBoxFilterItems.Persons:
                    SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxItemOfFilter, _storageInstance.Persons);
                    break;
                default:
                    return;
            }

        }

        private void ComboBoxItemFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateIncomesViewAndClearAddEditArea();
        }

        private enum ItemsOfComboBoxSelectPeriodLastIncomes
        {
            AllTime,
            Today,
            LastWeek,
            LastMonth,
            ChooseYourself
        }

        private void ComboBoxSelectPeriodIncomes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOfIncomesView == null)
            {
                return;
            }

            switch (ComboBoxSelectPeriodIncomes.SelectedIndex)
            {
                case (int)ItemsOfComboBoxSelectPeriodLastIncomes.AllTime:
                    ChangeFilterByDatesInListView(DateTime.MinValue);
                    ChangeStateAndVisibilityStackPanelSelectDateOnDisplay(false);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastIncomes.Today:
                    ChangeFilterByDatesInListView(DateTime.Now.Date);
                    ChangeStateAndVisibilityStackPanelSelectDateOnDisplay(false);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastIncomes.LastWeek:
                    ChangeFilterByDatesInListView(DateTime.Now.AddDays(-7).Date);
                    ChangeStateAndVisibilityStackPanelSelectDateOnDisplay(false);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastIncomes.LastMonth:
                    ChangeFilterByDatesInListView(DateTime.Now.AddMonths(-1).Date);
                    ChangeStateAndVisibilityStackPanelSelectDateOnDisplay(false);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastIncomes.ChooseYourself:
                    ChangeStateAndVisibilityStackPanelSelectDateOnDisplay(true);
                    break;
                default:
                    return;
            }
        }

        #endregion


        #region DatePickers
        private void DatePickerSelectEndPeriodIncomes_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _dateEndForView = (DateTime)DatePickerSelectEndPeriodIncomes.SelectedDate;
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void DatePickerSelectStartPeriodOrDayIncomes_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime dateInPickerSelectStartPeriod = (DateTime)DatePickerSelectStartPeriodOrDayIncomes.SelectedDate;
            if (dateInPickerSelectStartPeriod > _dateEndForView)
            {
                _dateEndForView = DateTime.MaxValue;
                DatePickerSelectEndPeriodIncomes.SelectedDate = _dateEndForView;
            }

            _dateStartForView = dateInPickerSelectStartPeriod;
            DatePickerSelectEndPeriodIncomes.BlackoutDates.Clear();
            DatePickerSelectEndPeriodIncomes.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, _dateStartForView));
            UpdateIncomesViewAndClearAddEditArea();
        }
        #endregion

        #region WorkWithModel
        private void IncreaseBalanceOfSelectedVaultType(MoneyVault chooseVaultForEditIncome, Income newIncome)
        {
            switch (chooseVaultForEditIncome)
            {
                case Card:
                    ((Card)chooseVaultForEditIncome).IncreaseBalance(newIncome);
                    break;
                case OnlyTopDeposit:
                    ((OnlyTopDeposit)chooseVaultForEditIncome).IncreaseBalance(newIncome);
                    break;
                case Deposit:
                    ((Deposit)chooseVaultForEditIncome).IncreaseBalance(newIncome);
                    break;
                default:
                    return;
            }
        }
        #endregion

        #region StatesAndViewItems

        private void ChangeStateListAreaAndSetButtonAddEditContent(string buttonAddEditContent, bool isEnabled)
        {
            ButtonAddEditIncome.Content = buttonAddEditContent;
            listViewForIncomes.IsEnabled = isEnabled;
            StackPanelFilterIncomes.IsEnabled = isEnabled;
            StackPanelSelectDateIncomesOnDisplay.IsEnabled = isEnabled;
        }


        private void SetDefaultInAddEditArea()
        {
            TextBoxIncomeAmount.Text = string.Empty;
            TextBoxIncomeComment.Text = string.Empty;
            DatePickerIncomeDate.SelectedDate = DateTime.Today;
        }
        private void ChangeAddEditViewState(Income income)
        {
            ComboBoxIncomePerson.SelectedItem = income.Person;
            ComboBoxIncomeVault.SelectedItem = income.Vault;
            ComboBoxIncomeType.SelectedItem = income.Type;
            DatePickerIncomeDate.SelectedDate = income.Date;
            TextBoxIncomeComment.Text = income.Comment;
            TextBoxIncomeAmount.Text = income.Amount.ToString();
        }
        private void UpdateIncomesViewAndClearAddEditArea()
        {
            listOfIncomesView.Clear();
            var allIncomes = Storage.GetAllIncomes().Where(item => item.Date >= _dateStartForView && item.Date <= _dateEndForView);
            if (ComboBoxFilterIncomes.SelectedIndex == (int)ComboBoxFilterItems.Vault)
            {

                foreach (Income item in allIncomes)
                {
                    if (item.Vault == ComboBoxItemOfFilter.SelectedItem)
                        listOfIncomesView.Add(item);
                }
            }
            else if (ComboBoxFilterIncomes.SelectedIndex == (int)ComboBoxFilterItems.IncomeType)
            {

                foreach (Income item in allIncomes)
                {
                    if (item.Type == ComboBoxItemOfFilter.SelectedItem)
                        listOfIncomesView.Add(item);
                }
            }
            else if (ComboBoxFilterIncomes.SelectedIndex == (int)ComboBoxFilterItems.Persons)
            {
                foreach (Income item in allIncomes)
                {
                    if (item.Person == ComboBoxItemOfFilter.SelectedItem)
                        listOfIncomesView.Add(item);
                }
            }
            else
            {

                foreach (Income item in allIncomes)
                    listOfIncomesView.Add(item);
            }
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomePerson, _storageInstance.Persons);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomeVault, _storageInstance.Vaults);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomeType, _storageInstance.IncomeTypes);
            Sort("Date", _lastDirection);
        }

        private void ChangeStateAndVisibilityStackPanelSelectDateOnDisplay(bool isEnableAndVisible)
        {
            if (isEnableAndVisible)
                StackPanelSelectDateIncomesOnDisplay.Visibility = Visibility.Visible;
            else
                StackPanelSelectDateIncomesOnDisplay.Visibility = Visibility.Hidden;

            StackPanelSelectDateIncomesOnDisplay.IsEnabled = isEnableAndVisible;
        }
        private void ChangeFilterByDatesInListView(DateTime startDate)
        {
            _dateStartForView = startDate;
            _dateEndForView = (startDate == DateTime.MinValue) ? DateTime.MaxValue : DateTime.Now;
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void TabItemIncomes_Selected(object sender, RoutedEventArgs e)
        {
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void SetIsEnabledForItemsOnStackPanel(bool isEnable)
        {
            ComboBoxFilterIncomes.IsEnabled = isEnable;
            ComboBoxItemOfFilter.IsEnabled = isEnable;
        }

        #endregion
    }
}
