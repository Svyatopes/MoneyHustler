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
        private Storage _storageInstance = Storage.GetInstance();

        private ObservableCollection<Credit> listOfCreditsView;
        private Credit _credit;
        public TabItemCredits()
        {
            if (_storageInstance.Credits == null)
            {
                _storageInstance.Credits = new List<Credit> { };
            }
            InitializeComponent();
            listOfCreditsView = new ObservableCollection<Credit>(_storageInstance.Credits);
            listViewForCredits.ItemsSource = listOfCreditsView;

            ComboBoxCards.ItemsSource = _storageInstance.Vaults;
            ComboBoxPerson.ItemsSource = _storageInstance.Persons;

            ChangeVisibilityOfGridAddEditVault(false);


        }

        private void ButtonAddCreditClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            _credit = null;
            ChangeVisibilityOfGridAddEditVault(true);


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

        private void ButtonEditCreditClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;
            ChangeVisibilityOfGridAddEditVault(true);
            _credit = credit;


            TextBoxValueWithoutPercent.IsEnabled = false;
            DatePickerDayOpen.IsEnabled = false;
            DatePickerDayClose.IsEnabled = false;
            TextBoxPercent.IsEnabled = false;


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

            credit.PayMonthlyPayment();
            UpdateCreditsView();


            Storage.Save();


        }


        private void ButtonBackClick(object sender, RoutedEventArgs e)
        {

            ChangeVisibilityOfGridAddEditVault(false);
            UpdateCreditsView();

        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {

            decimal enteredValue = 0;
            decimal enteredPercent = 0;
            if (TextBoxValueWithoutPercent.IsEnabled == true)
            {
                if (TextBoxValueWithoutPercent.Text == String.Empty)
                {
                    MessageBox.Show("You need to enter the value!");
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

                if (DatePickerDayClose.SelectedDate.Value.Month == DatePickerDayOpen.SelectedDate.Value.Month)
                {
                    MessageBox.Show("You cannot take a loan and pay it off in the same month");
                    return;
                }


                if (!decimal.TryParse(TextBoxPercent.Text, out enteredPercent))
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



            var enteredVault = (Card)ComboBoxCards.SelectedItem;

            if (_credit == null)
            {


                _credit = new Credit(TextBoxName.Text, Convert.ToDouble(TextBoxPercent.Text), null, enteredValue, (Person)ComboBoxPerson.SelectedItem, (Card)ComboBoxCards.SelectedItem, (
                    DateTime)DatePickerDayClose.SelectedDate, (DateTime)DatePickerDayOpen.SelectedDate);
                _storageInstance.Credits.Add(_credit);

            }
            else
            {

                _credit.Name = TextBoxName.Text;
                _credit.BindedCard = (Card)ComboBoxCards.SelectedItem;
                _credit.Person = (Person)ComboBoxPerson.SelectedItem;
            }

            Storage.Save();

            ChangeVisibilityOfGridAddEditVault(false);
            UpdateCreditsView();
        }



        private void ChangeVisibilityOfGridAddEditVault(bool visible)
        {
            if (visible)
            {
                ColumnLabels.Width = new GridLength(20, GridUnitType.Star);
                ColumnTextBox.Width = new GridLength(20, GridUnitType.Star);
                LabelName.Visibility = Visibility.Visible;
                LabelValueWithoutPercent.Visibility = Visibility.Visible;
                LabelOpenDate.Visibility = Visibility.Visible;
                LabelCloseDate.Visibility = Visibility.Visible;
                LabelPercent.Visibility = Visibility.Visible;
                LabelCards.Visibility = Visibility.Visible;
                LabelPerson.Visibility = Visibility.Visible;

                TextBoxName.Visibility = Visibility.Visible;
                TextBoxValueWithoutPercent.Visibility = Visibility.Visible;
                DatePickerDayOpen.Visibility = Visibility.Visible;
                DatePickerDayClose.Visibility = Visibility.Visible;
                TextBoxPercent.Visibility = Visibility.Visible;
                ComboBoxCards.Visibility = Visibility.Visible;
                ComboBoxPerson.Visibility = Visibility.Visible;

                ButtonSave.Visibility = Visibility.Visible;
                ButtonBack.Visibility = Visibility.Visible;


                ButtonAdd.IsEnabled = false;
            }
            else
            {
                ColumnLabels.Width = new GridLength(0, GridUnitType.Star);
                ColumnTextBox.Width = new GridLength(0, GridUnitType.Star);

                LabelName.Visibility = Visibility.Hidden;
                LabelValueWithoutPercent.Visibility = Visibility.Hidden;
                LabelOpenDate.Visibility = Visibility.Hidden;
                LabelCloseDate.Visibility = Visibility.Hidden;
                LabelPercent.Visibility = Visibility.Hidden;
                LabelCards.Visibility = Visibility.Hidden;
                LabelPerson.Visibility = Visibility.Hidden;

                TextBoxName.Visibility = Visibility.Hidden;
                TextBoxValueWithoutPercent.Visibility = Visibility.Hidden;
                DatePickerDayOpen.Visibility = Visibility.Hidden;
                DatePickerDayClose.Visibility = Visibility.Hidden;
                TextBoxPercent.Visibility = Visibility.Hidden;
                ComboBoxCards.Visibility = Visibility.Hidden;
                ComboBoxPerson.Visibility = Visibility.Hidden;

                ButtonSave.Visibility = Visibility.Hidden;
                ButtonBack.Visibility = Visibility.Hidden;

                ButtonAdd.IsEnabled = true;
            }
        }


        private void ChangeVisibilityOncePaymentClick(bool visible)
        {
            if (visible)
            {
                ColumnLabels.Width = new GridLength(20, GridUnitType.Star);
                ColumnTextBox.Width = new GridLength(20, GridUnitType.Star);

                LabelOncePay.Visibility = Visibility.Visible;
                TextBoxOncePay.Visibility = Visibility.Visible;

                ButtonOncePayBack.Visibility = Visibility.Visible;
                ButtonOncePaySave.Visibility = Visibility.Visible;

                ButtonAdd.IsEnabled = false;
            }
            else
            {
                ColumnLabels.Width = new GridLength(0, GridUnitType.Star);
                ColumnTextBox.Width = new GridLength(0, GridUnitType.Star);

                LabelOncePay.Visibility = Visibility.Hidden;
                TextBoxOncePay.Visibility = Visibility.Hidden;

                ButtonOncePayBack.Visibility = Visibility.Hidden;
                ButtonOncePaySave.Visibility = Visibility.Hidden;

                ButtonAdd.IsEnabled = true;
            }
        }

        private void ButtonPayOnceItemClick(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityOncePaymentClick(true);
        }

        private void ButtonOncePayBackClick(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityOncePaymentClick(false);
        }

        private void ButtonOncePaySaveClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var credit = (Credit)button.DataContext;
            decimal enteredValue = 0;

            if (credit.Amount == 0)
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

            if (enteredValue > credit.Amount)
            {
                MessageBox.Show("The payment amount exceeds the loan amount");
                return;
            }


            credit.PayOneTimePayment(enteredValue);
            ChangeVisibilityOncePaymentClick(false);
            UpdateCreditsView();

            Storage.Save();
        }
    }
}
