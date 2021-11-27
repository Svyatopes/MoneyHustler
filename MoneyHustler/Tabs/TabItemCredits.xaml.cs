using MoneyHustler.Models;
using System;
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
        private void ChnageIsEnabledProperty(bool isEnabled)
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
            ChangeVisibilityColumns(true, new ObservableCollection<ColumnDefinition> {ColumnLabelsEditSave, ColumnTextBoxEditSave});
            ChnageIsEnabledProperty(true);
        }

        private void ButtonEditCreditClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;
            ChangeVisibilityColumns(true, new ObservableCollection<ColumnDefinition> { ColumnLabelsEditSave, ColumnTextBoxEditSave });
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
            foreach (var income in allCredits)
            {
                listOfCreditsView.Add(income);
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
            var expenseType = _storageInstance.ExpenseTypes.FirstOrDefault(item => item.Name == "Кредит");
            if (expenseType == null)
            {
                expenseType = new ExpenseType() { Name = "Кредит" };
                _storageInstance.ExpenseTypes.Add(expenseType);
            }
            credit.PayMonthlyPayment(expenseType);
            UpdateCreditsView();


            Storage.Save();


        }


        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {

            ChangeVisibilityColumns(false, new ObservableCollection<ColumnDefinition> { ColumnLabelsEditSave, ColumnTextBoxEditSave }); ;
            UpdateCreditsView();

        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            decimal enteredValue;
            double enteredPercent;
            
            if (TextBoxValueWithoutPercent.IsEnabled == true)
            {
                if (TextBoxValueWithoutPercent.Text == String.Empty)
                {
                    MessageBox.Show("You need to enter the value!");
                    return;
                }

                var findSameCredit = _storageInstance.Credits.FirstOrDefault(item => item.Name == TextBoxName.Text.Trim());
                if (findSameCredit == null)
                {
                    MessageBox.Show("A credit with the same name already exists");
                    return;
                }

                if (!decimal.TryParse(TextBoxValueWithoutPercent.Text, out enteredValue))
                {
                    MessageBox.Show("You entered some invalid string to amount field!");
                    return;
                }

                if (enteredValue < 0)
                {
                    MessageBox.Show("Amount can't be less than zero.");
                    return;
                }

                if (ComboBoxCards.SelectedItem == null)
                {
                    MessageBox.Show("You need to choose card!");
                    return;
                }

                if (DatePickerDayClose.SelectedDate < DatePickerDayOpen.SelectedDate)
                {
                    MessageBox.Show("The closing date cannot be earlier than the opening date!");
                    return;
                }

                if (DatePickerDayClose.SelectedDate.Value.Month == DatePickerDayOpen.SelectedDate.Value.Month && DatePickerDayClose.SelectedDate.Value.Year == DatePickerDayOpen.SelectedDate.Value.Year)
                {
                    MessageBox.Show("You cannot take a loan and pay it off in the same month");
                    return;
                }

                if (!double.TryParse(TextBoxPercent.Text, out enteredPercent))
                {
                    MessageBox.Show("You entered some invalid string to percent field!");
                    return;
                }
                if (enteredPercent < 0)
                {
                    MessageBox.Show("Percent can't be less than zero.");
                    return;
                }
            }

            enteredValue = Convert.ToDecimal(TextBoxValueWithoutPercent.Text);
            enteredPercent = Convert.ToDouble(TextBoxPercent.Text);

            string enteredName = TextBoxName.Text.Trim();
            Person enteredPerson = (Person)ComboBoxPerson.SelectedItem;
            Card enteredCard = (Card)ComboBoxCards.SelectedItem;

            if (_credit == null)
            {
                DateTime entredOpenDate = (DateTime)DatePickerDayClose.SelectedDate;
                DateTime entredCloseDate = (DateTime)DatePickerDayOpen.SelectedDate;
                _credit = new Credit(enteredName, enteredPercent, null, enteredValue, enteredPerson, enteredCard, entredCloseDate, entredOpenDate);
                _storageInstance.Credits.Add(_credit);

            }
            else
            {
                _credit.Name = enteredName;
                _credit.BindedCard = enteredCard;
                _credit.Person = enteredPerson;
            }

            Storage.Save();

            ChangeVisibilityColumns(false, new ObservableCollection<ColumnDefinition> { ColumnLabelsEditSave, ColumnTextBoxEditSave });
            UpdateCreditsView();
        }



        private void ChangeVisibilityColumns(bool visible, ObservableCollection<ColumnDefinition> columns)
        {
            if (visible)
            {
                foreach (var item in columns)
                {
                    item.Width = new GridLength(20, GridUnitType.Star);
                }
                ButtonAdd.IsEnabled = false;
            }
            else
            {
                foreach (var item in columns)
                {
                    item.Width = new GridLength(0, GridUnitType.Star);
                }
                ButtonAdd.IsEnabled = true;
            }
        }


        private void ButtonPayOnceItemClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;
            _credit = credit;
            ChangeVisibilityColumns(true, new ObservableCollection<ColumnDefinition> { ColumnTextBoxOncePay, ColumnLabelsOncePay });
        }

        private void ButtonOncePayBackClick(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityColumns(false, new ObservableCollection<ColumnDefinition> { ColumnTextBoxOncePay, ColumnLabelsOncePay });
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

            var expenseType = _storageInstance.ExpenseTypes.FirstOrDefault(item => item.Name == "Кредит");
            if (expenseType == null)
            {
                expenseType = new ExpenseType() { Name = "Кредит" };
                _storageInstance.ExpenseTypes.Add(expenseType);
            }

            _credit.PayOneTimePayment(enteredValue, expenseType);

            ChangeVisibilityColumns(false, new ObservableCollection<ColumnDefinition> { ColumnTextBoxOncePay, ColumnLabelsOncePay });
            UpdateCreditsView();

            Storage.Save();
        }

        private void TabItemCreditSelected(object sender, RoutedEventArgs e)
        {
            UpdateCreditsView();
        }
    }
}
