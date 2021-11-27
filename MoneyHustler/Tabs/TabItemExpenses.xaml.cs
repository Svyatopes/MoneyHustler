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

        SoundPlayer[] soundAccompainement = new SoundPlayer[5];

        SoundPlayer soundAccompainementZdarova = new SoundPlayer();
        SoundPlayer soundAccompainementMaloDeneg = new SoundPlayer();
        SoundPlayer soundAccompainementAuf = new SoundPlayer();
        SoundPlayer soundAccompainementOnView = new SoundPlayer();
        SoundPlayer soundAccompainementDelete = new SoundPlayer();


        //TODO: сделать более понятные названия x:Name

        private void LoadAudio() //метод загрузки звукового сопровождения
        {
            soundAccompainementZdarova.SoundLocation = "Audio/zdarova.wav";
            soundAccompainementZdarova.LoadAsync();
            soundAccompainementMaloDeneg.SoundLocation = "Audio/kavo.wav";
            soundAccompainementMaloDeneg.LoadAsync();
            soundAccompainementAuf.SoundLocation = "Audio/auf.wav";
            soundAccompainementAuf.LoadAsync();
            soundAccompainementOnView.SoundLocation = "Audio/onView.wav";
            soundAccompainementOnView.LoadAsync();
            soundAccompainementDelete.SoundLocation = "Audio/nePonyal.wav";
            soundAccompainementDelete.LoadAsync();
        }

        public TabItemExpenses()
        {
            InitializeComponent();

            LoadAudio();
            soundAccompainementZdarova.Play();

            listOfExpensesView = new ObservableCollection<Expense>(Storage.GetAllExpences());
            listViewForExpenses.ItemsSource = listOfExpensesView;
            _dateStartForView = DateTime.MinValue;
            _dateEndForView = DateTime.MaxValue;

            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpensePerson, _storageInstance.Persons);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseVault, _storageInstance.Vaults);
            SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBoxExpenseType, _storageInstance.ExpenseTypes);

            DatePickerChooseDateOfExpense.SelectedDate = DateTime.Now;
            SortExpenses("Date", _lastDirection);
        }

        private void SetItemSourceAndSelectedIndexToZeroOrSelectedItem(ComboBox comboBox, IEnumerable source) //
        {
            comboBox.ItemsSource = source;
            comboBox.SelectedIndex = 0;
        }

        private void ButtonDeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            soundAccompainementDelete.Play();

            var button = (Button)sender;
            var expense = (Expense)button.DataContext;

            listOfExpensesView.Remove(expense);
            expense.Vault.Remove(expense);
            Storage.Save();
        }
        private void ChangeEnabledDisplayAndButtonContentInModEdit(string buttonAddEditContent, bool isEnabled)
        {
            ButtonAddEditExpense.Content = buttonAddEditContent;
            listViewForExpenses.IsEnabled = isEnabled;
            StackPanelOnlyExpense.IsEnabled = isEnabled;
            StackPanelSelectDateOnDisplay.IsEnabled = isEnabled;
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
            DatePickerChooseDateOfExpense.SelectedDate = expense.Date;
            TextBoxExpenseComment.Text = expense.Comment;
            TextBoxExpenseAmount.Text = expense.Amount.ToString();
            _expense = expense; //записываем в поле ссылку на расход
        }

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

            var person = Storage.GetOrCreatePersonByName(personName);
            var expenseType = Storage.GetOrCreateExpenseTypeByName(expenseTypeName);
            Card chooseVaultFoeEditExpense = (Card)ComboBoxExpenseVault.SelectedItem;
            DateTime chooseDateOfExpense = (DateTime)DatePickerChooseDateOfExpense.SelectedDate;
            if ((string)ButtonAddEditExpense.Content == "Сохраните")
            {
                ChangeEnabledDisplayAndButtonContentInModEdit("Добавить", true);
                
                if (CheckingPossipilityOfEditingExpense(chooseVaultFoeEditExpense, chooseDateOfExpense))
                {
                    _expense.Amount = Convert.ToDecimal(TextBoxExpenseAmount.Text);
                    _expense.Comment = TextBoxExpenseComment.Text;
                    _expense.Date = (DateTime)DatePickerChooseDateOfExpense.SelectedDate;
                    _expense.Person = person;
                    _expense.Type = expenseType;
                    _expense.Vault = chooseVaultFoeEditExpense;
                    // MessageBox.Show("ок");
                }
                else
                {
                    soundAccompainementMaloDeneg.Play();
                    //MessageBox.Show("На этом счету недостаточно средств на выбранную дату", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }

            else if ((string)ButtonAddEditExpense.Content == "Добавить")
            {
                decimal balanceOnSelectDay = chooseVaultFoeEditExpense.GetBalanceOnDate(chooseDateOfExpense);
                //расчитываем баланс на выбранный в календаре день
                if (Convert.ToDecimal(TextBoxExpenseAmount.Text) > balanceOnSelectDay //в прошлом может и было много денег
                    || Convert.ToDecimal(TextBoxExpenseAmount.Text) > chooseVaultFoeEditExpense.GetBalance())
                //но не факт, что сейчас я могу себе такое позволить
                {
                    soundAccompainementMaloDeneg.Play();
                    // MessageBox.Show("На выбранном счёте не достаточно средств на выбранную дату", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Expense newExpense = new Expense
                (
                   Convert.ToDecimal(TextBoxExpenseAmount.Text),
                   chooseDateOfExpense,
                   person,
                   TextBoxExpenseComment.Text,
                   expenseType
                );

                chooseVaultFoeEditExpense.DecreaseBalance(newExpense);
                soundAccompainementAuf.Play();
                //  MessageBox.Show("потратил");
            }
            Storage.Save();
            SetDefaultInAddEditArea();
            UpdateExpensesViewAndClearAddEditArea();
        }



        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal n = 0;
            if (!Decimal.TryParse(TextBoxExpenseAmount.Text, out n) && n > 0)
                ButtonAddEditExpense.IsEnabled = false;

            else
                ButtonAddEditExpense.IsEnabled = true;
        }

        private void SetDefaultInAddEditArea()
        {
            TextBoxExpenseAmount.Text = string.Empty;
            TextBoxExpenseComment.Text = string.Empty;
            DatePickerChooseDateOfExpense.SelectedDate = DateTime.Today;
        }


        private enum ComboBoxFilterItems
        {
            Vault,
            ExpenseType,
            Persons
        }

        private void UpdateExpensesViewAndClearAddEditArea()
        {
            //отдельный метод начало
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
            //конец отдельного метода и сюда же сортировку или вообще убрать отовсюду кроме инициализации
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


        private void SetIsEnabledForItemsOnStackPanel(bool isEnable)
        {
            ComboBoxFilterList.IsEnabled = isEnable;
            ComboBoxItemOfFilterList.IsEnabled = isEnable;
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
                UpdateExpensesViewAndClearAddEditArea();
            }
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
                    ChangePeriodExpensesOnDisplay(DateTime.MinValue);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastExpenses.Today:
                    ChangePeriodExpensesOnDisplay(DateTime.Now.Date);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastExpenses.LastWeek:
                    ChangePeriodExpensesOnDisplay(DateTime.Now.AddDays(-7).Date);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastExpenses.LastMonth:
                    ChangePeriodExpensesOnDisplay(DateTime.Now.AddMonths(-1).Date);
                    break;
                case (int)ItemsOfComboBoxSelectPeriodLastExpenses.ChooseYourself:
                    StackPanelSelectDateOnDisplay.Visibility = Visibility.Visible;
                    StackPanelSelectDateOnDisplay.IsEnabled = true;
                    break;
                default:
                    return;
            }
        }

        private void DatePickerSelectStartPeriodOrDayExpenses_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime DateInPickerSelectStartPeriod = (DateTime)DatePickerSelectStartPeriodOrDayExpenses.SelectedDate;
            if (DateInPickerSelectStartPeriod > _dateEndForView)
            {
                _dateEndForView = DateTime.MaxValue;
                DatePickerSelectEndPeriodExpenses.SelectedDate = _dateEndForView;
            }
            _dateStartForView = DateInPickerSelectStartPeriod;
            DatePickerSelectEndPeriodExpenses.BlackoutDates.Clear();
            DatePickerSelectEndPeriodExpenses.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, _dateStartForView));
            UpdateExpensesViewAndClearAddEditArea();
        }

        private void DatePickerSelectEndPeriodExpenses_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _dateEndForView = (DateTime)DatePickerSelectEndPeriodExpenses.SelectedDate;
            UpdateExpensesViewAndClearAddEditArea();
        }

        private void ChangePeriodExpensesOnDisplay(DateTime startDate)
        {
            StackPanelSelectDateOnDisplay.Visibility = Visibility.Hidden;
            StackPanelSelectDateOnDisplay.IsEnabled = false;
            _dateStartForView = startDate;
            _dateEndForView = (startDate == DateTime.MinValue) ? DateTime.MaxValue : DateTime.Now;
            UpdateExpensesViewAndClearAddEditArea();
        }


        private void TabItemExpenses_Selected(object sender, RoutedEventArgs e)
        {
            UpdateExpensesViewAndClearAddEditArea();
        }
    }
}
