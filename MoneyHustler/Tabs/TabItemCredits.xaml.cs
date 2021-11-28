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
        public TabItemCredits()
        {
            _storageInstance = Storage.GetInstance();
            InitializeComponent();
            listOfCreditsView = new ObservableCollection<Credit>(_storageInstance.Credits);
            listViewForCredits.ItemsSource = listOfCreditsView;

            ComboBoxCards.ItemsSource = _storageInstance.Vaults;
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


                ComboBoxCards.ItemsSource = _storageInstance.Vaults;
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
            UIHelpers.ChangeVisibilityColumns(new ObservableCollection<ColumnDefinition> { ColumnLabelsEditSave, ColumnTextBoxEditSave }, 20);
            ButtonAdd.IsEnabled = false;
            ChangeButtonIsEnabledProperty(true);
        }

        private void ButtonEditCreditClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;
            UIHelpers.ChangeVisibilityColumns( new ObservableCollection<ColumnDefinition> { ColumnLabelsEditSave, ColumnTextBoxEditSave }, 20);
            ButtonAdd.IsEnabled = false;
            _credit = credit;

            TextBoxName.Text = _credit.Name;

            TextBoxValueWithoutPercent.Text = Convert.ToString(_credit.InitialAmount);

            DatePickerDayOpen.SelectedDate = _credit.OpenDate;
            DatePickerDayClose.SelectedDate = _credit.CloseDate;

            TextBoxPercent.Text = Convert.ToString(_credit.Percent);


            ComboBoxCards.ItemsSource = _storageInstance.Vaults;
            ComboBoxCards.SelectedItem = _credit.BindedCard;

            ComboBoxPerson.ItemsSource = _storageInstance.Persons;
            ComboBoxPerson.SelectedItem = _credit.Person;
            UpdateCreditsView();
            Storage.Save();

        }

        private void UpdateCreditsView()
        {
            listOfCreditsView.Clear();
            var allCredits = _storageInstance.Credits;
            foreach (Credit credit in allCredits)
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

            UIHelpers.ChangeVisibilityColumns( new ObservableCollection<ColumnDefinition> { ColumnLabelsEditSave, ColumnTextBoxEditSave }, 0);
            ButtonAdd.IsEnabled = true;
            UpdateCreditsView();

        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            decimal enteredValue;
            decimal enteredPercent;


            if (TextBoxValueWithoutPercent.Text == String.Empty)
            {
                MessageBox.Show("Вам нужно ввести число!");
                return;
            }

            if (!decimal.TryParse(TextBoxValueWithoutPercent.Text, out enteredValue))
            {
                MessageBox.Show("Вы ввели недопустимую сумму кредита!");
                return;
            }

            if (enteredValue < 0)
            {
                MessageBox.Show("Кредит не может быть отрицательным!");
                return;
            }

            if (ComboBoxCards.SelectedItem == null)
            {
                MessageBox.Show("Вам нужно выбрать карту!");
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

            if (!decimal.TryParse(TextBoxPercent.Text, out enteredPercent))
            {
                MessageBox.Show("Вы ввели неккортектный процент!");
                return;
            }
            if (enteredPercent < 0)
            {
                MessageBox.Show("Процент не может быть отрицательным!");
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

            UIHelpers.ChangeVisibilityColumns(new ObservableCollection<ColumnDefinition> { ColumnLabelsEditSave, ColumnTextBoxEditSave }, 0);
            ButtonAdd.IsEnabled = true;
            UpdateCreditsView();
        }

        private void ButtonPayOnceItemClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;
            _credit = credit;
            UIHelpers.ChangeVisibilityColumns(new ObservableCollection<ColumnDefinition> { ColumnTextBoxOncePay, ColumnLabelsOncePay }, 20);
            ButtonAdd.IsEnabled = false;
        }

        private void ButtonOncePayBackClick(object sender, RoutedEventArgs e)
        {
            UIHelpers.ChangeVisibilityColumns(new ObservableCollection<ColumnDefinition> { ColumnTextBoxOncePay, ColumnLabelsOncePay }, 0);
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

            if (TextBoxOncePay.Text == String.Empty)
            {
                MessageBox.Show("You need to enter the value!");
                return;
            }


            if (!decimal.TryParse(TextBoxOncePay.Text, out enteredValue))
            {
                MessageBox.Show("You entered some invalid string to amount field!");
                return;
            }

            if (enteredValue < 0)
            {
                MessageBox.Show("Amount can't be less than zero.");
                return;
            }

            if (enteredValue > _credit.Amount)
            {
                MessageBox.Show("The payment amount exceeds the loan amount");
                return;
            }

            var expenseType = Storage.GetOrCreateExpenseTypeByName("Кредит");

            _credit.PayOneTimePayment(enteredValue, expenseType);

            UIHelpers.ChangeVisibilityColumns(new ObservableCollection<ColumnDefinition> { ColumnTextBoxOncePay, ColumnLabelsOncePay }, 0);
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
