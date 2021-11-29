using MoneyHustler.Helpers;
using MoneyHustler.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace MoneyHustler.Tabs
{
    /// <summary>
    /// Interaction logic for TabItemCredits.xaml
    /// </summary>
    public partial class TabItemCredits : TabItem
    {
        private Storage _storageInstance;

        private ObservableCollection<Credit> listOfCreditsView;
        private Credit _credit;
        private ObservableCollection<ColumnDefinition> columnsSaveEdit;
        private ObservableCollection<ColumnDefinition> columnsOncePay;
        private const double ColumnVisibilityOn = 20;
        private const double ColumnVisibilityOff = 0;

        public TabItemCredits()
        {
            _storageInstance = Storage.GetInstance();
            InitializeComponent();
            columnsSaveEdit = new ObservableCollection<ColumnDefinition>();
            columnsOncePay = new ObservableCollection<ColumnDefinition>();
            columnsSaveEdit.Add(ColumnLabelsEditSave);
            columnsSaveEdit.Add(ColumnTextBoxEditSave);
            columnsOncePay.Add(ColumnLabelsOncePay);
            columnsOncePay.Add(ColumnTextBoxOncePay);
            listOfCreditsView = new ObservableCollection<Credit>(_storageInstance.Credits);
            listViewForCredits.ItemsSource = listOfCreditsView;
            ComboBoxCards.ItemsSource = _storageInstance.Vaults.Where(item => item.GetType() == typeof(Card));
            ComboBoxPerson.ItemsSource = _storageInstance.Persons;


        }
        private void ChangeButtonIsEnabledProperty(bool isEnabled)
        {
            if (isEnabled)
            {
                TextBoxValueWithoutPercent.IsEnabled = true;
                DatePickerDayOpen.IsEnabled = true;
                DatePickerDayClose.IsEnabled = true;
                TextBoxPercent.IsEnabled = true;

                TextBoxName.Text = null;

                TextBoxValueWithoutPercent.Text = null;

                DatePickerDayOpen.SelectedDate = null;
                DatePickerDayClose.SelectedDate = null;

                TextBoxPercent.Text = null;


                ComboBoxCards.ItemsSource = _storageInstance.Vaults.Where(item => item.GetType() == typeof(Card));
                ComboBoxCards.SelectedItem = null;

                ComboBoxPerson.ItemsSource = _storageInstance.Persons;
                ComboBoxPerson.SelectedItem = null;
            }
            else
            {

                TextBoxValueWithoutPercent.IsEnabled = false;
                DatePickerDayOpen.IsEnabled = false;
                DatePickerDayClose.IsEnabled = false;
                TextBoxPercent.IsEnabled = false;
            }
        }
        private void ButtonAddCreditClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            _credit = null;
            UIHelpers.ChangeWidthGridColumns(columnsSaveEdit, ColumnVisibilityOn);
            ButtonAdd.IsEnabled = false;
            ChangeButtonIsEnabledProperty(true);
        }

        private void ButtonEditCreditClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;
            UIHelpers.ChangeWidthGridColumns(columnsSaveEdit, ColumnVisibilityOn);
            ButtonAdd.IsEnabled = false;
            ChangeButtonIsEnabledProperty(false);
            _credit = credit;

            TextBoxName.Text = _credit.Name;

            TextBoxValueWithoutPercent.Text = Convert.ToString(_credit.InitialAmount);

            DatePickerDayOpen.SelectedDate = _credit.OpenDate;
            DatePickerDayClose.SelectedDate = _credit.CloseDate;

            TextBoxPercent.Text = Convert.ToString(_credit.Percent);


            ComboBoxCards.ItemsSource = _storageInstance.Vaults.Where(item => item.GetType() == typeof(Card));
            ComboBoxCards.SelectedItem = _credit.BindedCard;

            ComboBoxPerson.ItemsSource = _storageInstance.Persons;
            ComboBoxPerson.SelectedItem = _credit.Person;
            UpdateCreditsView();
            Storage.Save();

        }

        private void UpdateCreditsView()
        {
            listOfCreditsView.Clear();
            foreach (Credit credit in _storageInstance.Credits)
            {
                listOfCreditsView.Add(credit);
            }
        }

        private void ButtonRemoveCreditItemClick(object sender, RoutedEventArgs e)
        {
            var userAnswer = MessageBox.Show("Точно удалить?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (userAnswer == MessageBoxResult.Yes)
            {
                var button = (Button)sender;
                var credit = (Credit)button.DataContext;
                listOfCreditsView.Remove(credit);
                _storageInstance.Credits.Remove(credit);
                Storage.Save();
            }
        }

        private void ButtonPayItemClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;

            if (credit.Amount == 0)
            {
                MessageBox.Show("Кредит уже оплачен, молодец!*Звуки салюта*");
                return;
            }

            ExpenseType expenseType = Storage.GetOrCreateExpenseTypeByName("Кредит");
            credit.PayMonthlyPayment(expenseType);
            UpdateCreditsView();


            Storage.Save();


        }


        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {

            UIHelpers.ChangeWidthGridColumns(columnsSaveEdit, ColumnVisibilityOff);
            ButtonAdd.IsEnabled = true;
            UpdateCreditsView();

        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            decimal enteredValue;
            decimal enteredPercent;

            if ((TextBoxValueWithoutPercent.Text == String.Empty) || (!decimal.TryParse(TextBoxValueWithoutPercent.Text, out enteredValue)) || (enteredValue < 0))
            {
                MessageBox.Show("Вы ввели недопустимую сумму кредита!");
                return;
            }

            if (ComboBoxCards.SelectedItem == null)
            {
                MessageBox.Show("Вам нужно выбрать карту!");
                return;
            }

            if (DatePickerDayOpen.SelectedDate == null)
            {
                MessageBox.Show("Вам нужно выбрать дату открытия!");
                return;
            }

            if (DatePickerDayClose.SelectedDate == null)
            {
                MessageBox.Show("Вам нужно выбрать дату закрытия!");
                return;
            }

            if (DatePickerDayClose.SelectedDate < DatePickerDayOpen.SelectedDate)
            {
                MessageBox.Show("Дата закрытия должна быть не раньше даты открытия!");
                return;
            }

            if (DatePickerDayClose.SelectedDate.Value.Month == DatePickerDayOpen.SelectedDate.Value.Month && DatePickerDayClose.SelectedDate.Value.Year == DatePickerDayOpen.SelectedDate.Value.Year)
            {
                MessageBox.Show("Нельзя Открыть кредит меньше чем на один месяц!");
                return;
            }

            if ((!decimal.TryParse(TextBoxPercent.Text, out enteredPercent)) || (enteredPercent < 0))
            {
                MessageBox.Show("Вы ввели неккортектный процент!");
                return;
            }

            enteredValue = Convert.ToDecimal(TextBoxValueWithoutPercent.Text);
            enteredPercent = Convert.ToDecimal(TextBoxPercent.Text);

            string enteredName = TextBoxName.Text.Trim();
            Person enteredPerson = (Person)ComboBoxPerson.SelectedItem;
            Card enteredCard = (Card)ComboBoxCards.SelectedItem;

            if (_credit == null)
            {
                var findSameCredit = _storageInstance.Credits.FirstOrDefault(item => item.Name == enteredName);
                if (findSameCredit != null)
                {
                    MessageBox.Show("Кредит с таким именем уже существует!");
                    return;
                }
                DateTime entredCloseDate = (DateTime)DatePickerDayClose.SelectedDate;
                DateTime entredOpenDate = (DateTime)DatePickerDayOpen.SelectedDate;
                _credit = new Credit(enteredName, enteredPercent, null, enteredValue, enteredPerson, enteredCard, entredCloseDate, entredOpenDate);
                _storageInstance.Credits.Add(_credit);

            }
            else
            {
                if (_credit.Name != enteredName.Trim())
                {
                    var findSameCredit = _storageInstance.Credits.FirstOrDefault(item => item.Name == enteredName);
                    if (findSameCredit != null)
                    {
                        MessageBox.Show("Кредит с таким именем уже существует!");
                        return;
                    }
                }
                _credit.Name = enteredName.Trim();
                _credit.BindedCard = enteredCard;
                _credit.Person = enteredPerson;
            }

            Storage.Save();

            UIHelpers.ChangeWidthGridColumns(columnsSaveEdit, ColumnVisibilityOff);
            ButtonAdd.IsEnabled = true;
            UpdateCreditsView();
        }

        private void ButtonPayOnceItemClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;
            _credit = credit;
            UIHelpers.ChangeWidthGridColumns(columnsOncePay, ColumnVisibilityOn);
            ButtonAdd.IsEnabled = false;
        }

        private void ButtonOncePayBackClick(object sender, RoutedEventArgs e)
        {
            UIHelpers.ChangeWidthGridColumns(columnsOncePay, ColumnVisibilityOff);
            ButtonAdd.IsEnabled = true;
        }

        private void ButtonOncePaySaveClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;


            decimal enteredValue = 0;

            if (_credit.Amount == 0)
            {
                MessageBox.Show("Кредит уже оплачен, молодец!*Звуки салюта*");
                return;
            }

            if ((TextBoxOncePay.Text == String.Empty) || (!decimal.TryParse(TextBoxOncePay.Text, out enteredValue)) || (enteredValue < 0))
            {
                MessageBox.Show("Введено некорректное число!");
                return;
            }

            if (enteredValue > _credit.Amount)
            {
                MessageBox.Show("Сумма платежа больше чем остаток долга по кредиту");
                return;
            }


            var expenseType = Storage.GetOrCreateExpenseTypeByName("Кредит");

            _credit.PayOneTimePayment(enteredValue, expenseType);

            UIHelpers.ChangeWidthGridColumns(columnsOncePay, ColumnVisibilityOff);
            ButtonAdd.IsEnabled = true;
            UpdateCreditsView();

            Storage.Save();
        }

        private void TabItemCreditSelected(object sender, RoutedEventArgs e)
        {
            UpdateCreditsView();
        }
    }
}
