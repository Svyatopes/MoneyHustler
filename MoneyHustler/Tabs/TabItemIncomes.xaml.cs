using MoneyHustler.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoneyHustler.Tabs
{
    /// <summary>
    /// Interaction logic for TabItemIncomes.xaml
    /// </summary>
    public partial class TabItemIncomes : TabItem
    {
        private ObservableCollection<Income> listOfIncomesView;
        private Storage _storageInstance = Storage.GetInstance();

        private DateTime _dateStartForView;
        private DateTime _dateEndForView;

        private Income _income;
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Descending;
        public TabItemIncomes()
        {
            InitializeComponent();
            listOfIncomesView = new ObservableCollection<Income>(Storage.GetAllIncomes());
            listViewForIncomes.ItemsSource = listOfIncomesView;
            _dateStartForView = DateTime.MinValue;
            _dateEndForView = DateTime.MaxValue;

            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomePerson, _storageInstance.Persons);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomeVault, _storageInstance.Vaults);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomeType, _storageInstance.IncomeTypes);

            DatePickerIncomeDate.SelectedDate = DateTime.Now;
            Sort("Date", _lastDirection);
        }

        private void SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBox comboBox, IEnumerable source) //
        {
            comboBox.ItemsSource = source;
            comboBox.SelectedIndex = 0;
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

        private void ButtonEditIncome_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button == null)
            {
                return;
            }
            var income = (Income)button.DataContext;

            ChangeEnabledDisplayAndButtonAddContentInModEdit("Сохраните", false);

            ComboBoxIncomePerson.SelectedItem = income.Person;
            ComboBoxIncomeVault.SelectedItem = income.Vault;
            ComboBoxIncomeType.SelectedItem = income.Type;
            DatePickerIncomeDate.SelectedDate = income.Date;
            TextBoxIncomeComment.Text = income.Comment;
            TextBoxIncomeAmount.Text = income.Amount.ToString();
            _income = income;

        }

        private void ChangeEnabledDisplayAndButtonAddContentInModEdit(string buttonAddEditContent, bool isEnabled)
        {
            ButtonAddEditIncome.Content = buttonAddEditContent;
            listViewForIncomes.IsEnabled = isEnabled;
            StackPanelFilterIncomes.IsEnabled = isEnabled;
            StackPanelSelectDateIncomesOnDisplay.IsEnabled = isEnabled;
        }

        //TODO: Transfer how to remove in right way
        private void ButtonDeleteIncome_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var income = (Income)button.DataContext;

            if (income.Vault.GetBalance() - income.Amount < 0)
            {
                MessageBox.Show("Вы не можете удалить этот доход, так как не могли бы совершить некоторые покупки");
                return;
            }

            listOfIncomesView.Remove(income);
            income.Vault.Remove(income);
            Storage.Save();
        }

        private void ButtonAddEditIncome_Click(object sender, RoutedEventArgs e)
        {

            var personName = ComboBoxIncomePerson.Text;
            if (string.IsNullOrEmpty(personName))
            {
                MessageBox.Show("Вы должны заполнить персону");
                return;
            }
            var incomeTypeName = ComboBoxIncomeType.Text;
            if (string.IsNullOrEmpty(incomeTypeName))
            {
                MessageBox.Show("Вы должны заполнить категорию расхода");
                return;
            }

            var person = Storage.GetOrCreatePersonByName(personName);
            var incomeType = Storage.GetOrCreateIncomeTypeByName(incomeTypeName);
            Card selectedVault = (Card)ComboBoxIncomeVault.SelectedItem;
            if ((string)ButtonAddEditIncome.Content == "Сохраните")
            {
                ChangeEnabledDisplayAndButtonAddContentInModEdit("Добавить", true);

                decimal different;
                decimal currentBalanceOfVault = _income.Vault.GetBalance();
                
                if (_income.Vault != selectedVault) //если кошель поменялся
                {
                    different = currentBalanceOfVault - _income.Amount;
                    //смотрим не станет ли баланс меньше нуля при удалении того расхода из прежнего кошелька
                    if (different < 0)
                    {
                        MessageBox.Show($"Вы не можете убрать этот доход с '{_income.Vault.Name}', так как баланс уйдёт в минус");
                        return;
                    }
                }
                else //если кошель остался тем же
                {
                    different = currentBalanceOfVault - (_income.Amount - Convert.ToDecimal(TextBoxIncomeAmount.Text));
                    //смотрим не стала ли разница между старой суммой и новой обращать баланс в ноль
                    //допустим доход был 500, стал 200, то отнимаем от баланса (500 - 200 =) 300
                    if (different < 0)
                    {
                        MessageBox.Show($"Вы не можете уменьшить сумму дохода, выше чем {currentBalanceOfVault}") ;
                        return;
                    }
                } //если все условия соблюдены, то запишем новый доход

                
                _income.Amount = Convert.ToDecimal(TextBoxIncomeAmount.Text);
                _income.Type = incomeType;
                _income.Date = (DateTime)DatePickerIncomeDate.SelectedDate;
                _income.Person = person;
                _income.Comment = TextBoxIncomeComment.Text;

                if (_income.Vault != selectedVault) 
                {
                    _income.Vault.Remove(_income);
                    selectedVault.IncreaseBalance(_income);
                }
            }
            else if ((string)ButtonAddEditIncome.Content == "Добавить")
            {
                Income newIncome = new Income
                (
                   Convert.ToDecimal(TextBoxIncomeAmount.Text),
                   (DateTime)DatePickerIncomeDate.SelectedDate,
                   person,
                   TextBoxIncomeComment.Text,
                   incomeType
                );

                selectedVault.IncreaseBalance(newIncome);
                
                //MessageBox.Show("поднял");
            }
            Storage.Save();
            SetDefaultInAddEditArea();
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void SetDefaultInAddEditArea()
        {
            TextBoxIncomeAmount.Text = string.Empty;
            TextBoxIncomeComment.Text = string.Empty;
            DatePickerIncomeDate.SelectedDate = DateTime.Today;
        }

        private enum ComboBoxFilterItems
        {
            Vault,
            IncomeType,
            Persons
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

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal n = 0;
            if (!Decimal.TryParse(TextBoxIncomeAmount.Text, out n) && n >= 0)
            {
                ButtonAddEditIncome.IsEnabled = false;
            }
            else
            {
                ButtonAddEditIncome.IsEnabled = true;
            }
        }

        private void ButtonViewClassificationIncomes_Click(object sender, RoutedEventArgs e)
        {
            if ((string)ButtonViewClassificationIncomes.Content == "Показать доходы по")
            {
                //TODO: отдельный метод
                ButtonViewClassificationIncomes.Content = "К общему списку";
                SetIsEnabledForItemsOnStackPanel(true);
                DatePickerIncomeDate.SelectedDate = null;


            }
            else if ((string)ButtonViewClassificationIncomes.Content == "К общему списку")
            {
                //TODO: отдельный метод
                ButtonViewClassificationIncomes.Content = "Показать доходы по";
                SetIsEnabledForItemsOnStackPanel(false);
                DatePickerIncomeDate.SelectedDate = DateTime.Now;
                ComboBoxFilterIncomes.SelectedItem = null;
                ComboBoxItemOfFilter.SelectedItem = null;
            }
            UpdateIncomesViewAndClearAddEditArea();
        }
        private void SetIsEnabledForItemsOnStackPanel(bool isEnable)
        {
            ComboBoxFilterIncomes.IsEnabled = isEnable;
            ComboBoxItemOfFilter.IsEnabled = isEnable;
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

        private void ComboBoxClassIncomes_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                    ChangePeriodOnDisplay(DateTime.MinValue);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastIncomes.Today:
                    ChangePeriodOnDisplay(DateTime.Now.Date);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastIncomes.LastWeek:
                    ChangePeriodOnDisplay(DateTime.Now.AddDays(-7).Date);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastIncomes.LastMonth:
                    ChangePeriodOnDisplay(DateTime.Now.AddMonths(-1).Date);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastIncomes.ChooseYourself:
                    StackPanelSelectDateIncomesOnDisplay.Visibility = Visibility.Visible;
                    StackPanelSelectDateIncomesOnDisplay.IsEnabled = true;
                    break;
                default:
                    return;
            }
        }

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

        private void ChangePeriodOnDisplay(DateTime startDate)
        {
            StackPanelSelectDateIncomesOnDisplay.Visibility = Visibility.Hidden;
            StackPanelSelectDateIncomesOnDisplay.IsEnabled = false;
            _dateStartForView = startDate;
            _dateEndForView = (startDate == DateTime.MinValue) ? DateTime.MaxValue : DateTime.Now;
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void TabItemIncomes_Selected(object sender, RoutedEventArgs e)
        {
            UpdateIncomesViewAndClearAddEditArea();
        }
    }
}
