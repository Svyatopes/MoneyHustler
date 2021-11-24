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
            _dateStartForView = DateTime.Now.AddYears(-20);
            _dateEndForView = DateTime.Now;
            
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomePerson, _storageInstance.Persons);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomeVault, _storageInstance.Vaults);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxIncomeType, _storageInstance.IncomeTypes);

            DatePickerIncomeDate.SelectedDate = DateTime.Now;
            Sort("Date", _lastDirection);
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

            //TODO: попробовать вынести в отдельный метод все изменения View
            ButtonAddEditIncome.Content = "Сохраните";
            listViewForIncomes.IsEnabled = false;
            StackPanelFilterIncomes.IsEnabled = false;
            StackPanelSelectDateIncomesOnDisplay.IsEnabled = false;
            ComboBoxIncomePerson.SelectedItem = income.Person;
            ComboBoxIncomeVault.SelectedItem = income.Vault;
            ComboBoxIncomeType.SelectedItem = income.Type;
            DatePickerIncomeDate.SelectedDate = income.Date;
            TextBoxIncomeComment.Text = income.Comment;
            TextBoxIncomeAmount.Text = income.Amount.ToString();
            _income = income;

        }


        //TODO: Transfer how to remove in right way
        private void ButtonDeleteIncome_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var income = (Income)button.DataContext;
            
            if (income.Amount > income.Vault.GetBalance())
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

            if ((string)ButtonAddEditIncome.Content == "Сохраните")
            {
                listViewForIncomes.IsEnabled = true;
                StackPanelFilterIncomes.IsEnabled = true;
                StackPanelSelectDateIncomesOnDisplay.IsEnabled = true;
                ButtonAddEditIncome.Content = "Добавить";

                MoneyVault newVault = (MoneyVault)ComboBoxIncomeVault.SelectedItem;
                _income.Amount = Convert.ToDecimal(TextBoxIncomeAmount.Text);
                _income.Type = (IncomeType)ComboBoxIncomeType.SelectedItem;
                _income.Date = (DateTime)DatePickerIncomeDate.SelectedDate;
                _income.Person = (Person)ComboBoxIncomePerson.SelectedItem;
                _income.Comment = TextBoxIncomeComment.Text;

                if (_income.Vault != newVault)
                {
                    _income.Vault.Remove(_income);
                    newVault.IncreaseBalance(_income);
                }
                
            }
            else if ((string)ButtonAddEditIncome.Content == "Добавить")
            {
                Income newIncome = new Income
                (
                   Convert.ToDecimal(TextBoxIncomeAmount.Text),
                   (DateTime)DatePickerIncomeDate.SelectedDate,
                   (Person)ComboBoxIncomePerson.SelectedItem,
                   TextBoxIncomeComment.Text,
                   (IncomeType)ComboBoxIncomeType.SelectedItem
                );

                ((MoneyVault)ComboBoxIncomeVault.SelectedItem).IncreaseBalance(newIncome);
                Storage.Save();

                MessageBox.Show("поднял");
                UpdateIncomesViewAndClearAddEditArea();

            }
        }

        private void UpdateIncomesViewAndClearAddEditArea()
        {
            TextBoxIncomeAmount.Text = string.Empty;
            TextBoxIncomeComment.Text = string.Empty;
            DatePickerIncomeDate.SelectedDate = DateTime.Today;
            listOfIncomesView.Clear();

            if (0 == ComboBoxOfClassificationIncomes.SelectedIndex)
            {
                //TODO: void UpdateExpenseViewList( можно ли вставить )
                var allIncomes = Storage.GetAllIncomes();
                foreach (Income item in allIncomes)
                {
                    if (item.Date >= _dateStartForView && item.Date <= _dateEndForView
                        && item.Vault == ComboBoxClassIncomes.SelectedItem)
                    {
                        listOfIncomesView.Add(item);
                    }

                }
            }
            else if (1 == ComboBoxOfClassificationIncomes.SelectedIndex)
            {
                var allIncomes = Storage.GetAllIncomes();
                foreach (Income item in allIncomes)
                {
                    if (item.Date >= _dateStartForView && item.Date <= _dateEndForView
                        && item.Type == ComboBoxClassIncomes.SelectedItem)
                    {
                        listOfIncomesView.Add(item);
                    }

                }
            }
            else if (2 == ComboBoxOfClassificationIncomes.SelectedIndex)
            {
                var allIncomes = Storage.GetAllIncomes();
                foreach (Income item in allIncomes)
                {
                    if (item.Date >= _dateStartForView && item.Date <= _dateEndForView
                        && item.Person == ComboBoxClassIncomes.SelectedItem)
                    {
                        listOfIncomesView.Add(item);
                    }

                }
            }
            else
            {
                var allIncomes = Storage.GetAllIncomes();
                foreach (Income item in allIncomes)
                {
                    if (item.Date >= _dateStartForView && item.Date <= _dateEndForView)
                    {
                        listOfIncomesView.Add(item);
                    }

                }

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
                ComboBoxOfClassificationIncomes.SelectedItem = null;
                ComboBoxClassIncomes.SelectedItem = null;
                UpdateIncomesViewAndClearAddEditArea();
            }
        }

        private void ComboBoxOfClassificationIncomes_Selected(object sender, RoutedEventArgs e)
        {
            
            if (ComboBoxOfClassificationIncomes.SelectedIndex == 0)
            {
                ComboBoxClassIncomes.ItemsSource = _storageInstance.Vaults;
                ComboBoxClassIncomes.SelectedItem = _storageInstance.Vaults[0];

            }

            else if (ComboBoxOfClassificationIncomes.SelectedIndex == 1)
            { 
                
                ComboBoxClassIncomes.ItemsSource = _storageInstance.IncomeTypes;
                ComboBoxClassIncomes.SelectedItem = _storageInstance.IncomeTypes[0];

            }

            else if (ComboBoxOfClassificationIncomes.SelectedIndex == 2)
            {
                ComboBoxClassIncomes.ItemsSource = _storageInstance.Persons;
                ComboBoxClassIncomes.SelectedItem = _storageInstance.Persons[0];
            }

            else
            {
                return;
            }
        }

        private void ComboBoxClassIncomes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void ComboBoxSelectPeriodIncomes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOfIncomesView == null)
            {
                return;
            }

            if (ComboBoxSelectPeriodIncomes.SelectedIndex == 0)
            {
                ChangePeriodOnDisplay(4000);
            }

            else if (ComboBoxSelectPeriodIncomes.SelectedIndex == 1)
            {
                ChangePeriodOnDisplay(0);
            }

            else if (ComboBoxSelectPeriodIncomes.SelectedIndex == 2)
            {
                ChangePeriodOnDisplay(7);
            }
            else if (ComboBoxSelectPeriodIncomes.SelectedIndex == 3)
            {
                ChangePeriodOnDisplay(30);
            }

            else if (ComboBoxSelectPeriodIncomes.SelectedIndex == 4)
            {
                StackPanelSelectDateIncomesOnDisplay.Visibility = Visibility.Visible;
                StackPanelSelectDateIncomesOnDisplay.IsEnabled = true;
                DatePickerSelectStartPeriodOrDayIncomes.IsEnabled = true;

            }
        }

        private void DatePickerSelectEndPeriodIncomes_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _dateEndForView = (DateTime)DatePickerSelectEndPeriodIncomes.SelectedDate;
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void DatePickerSelectStartPeriodOrDayIncomes_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((DateTime)DatePickerSelectStartPeriodOrDayIncomes.SelectedDate
                > _dateEndForView)
            {
                _dateEndForView = ((DateTime)DatePickerSelectStartPeriodOrDayIncomes.SelectedDate).AddDays(1);
                DatePickerSelectEndPeriodIncomes.SelectedDate = _dateEndForView;
            }

            _dateStartForView = (DateTime)(DatePickerSelectStartPeriodOrDayIncomes.SelectedDate);
            DatePickerSelectEndPeriodIncomes.IsEnabled = true;
            DatePickerSelectEndPeriodIncomes.BlackoutDates.Clear();
            DatePickerSelectEndPeriodIncomes.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddYears(-10), _dateStartForView));
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void ChangePeriodOnDisplay(int inputDays)
        {
            StackPanelSelectDateIncomesOnDisplay.Visibility = Visibility.Hidden;
            StackPanelSelectDateIncomesOnDisplay.IsEnabled = false;
            _dateStartForView = DateTime.Today.AddDays(-inputDays);
            _dateEndForView = DateTime.Now;
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void SetIsEnabledForItemsOnStackPanel(bool isEnable)
        {
            ComboBoxOfClassificationIncomes.IsEnabled = isEnable;
            ComboBoxClassIncomes.IsEnabled = isEnable;
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            UpdateIncomesViewAndClearAddEditArea();
        }

        private void TabItemIncome_GotFocus(object sender, RoutedEventArgs e)
        {
            if (true)
            {

            }
            UpdateIncomesViewAndClearAddEditArea();
        }
    }
}
