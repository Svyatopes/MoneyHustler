using MoneyHustler.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary>
    /// Interaction logic for WindowAddEditCredit.xaml
    /// </summary>
    public partial class WindowAddEditCredit : Window
    {
        private Credit _credit;
        public WindowAddEditCredit()
        {
            InitializeComponent();
            ComboBoxCards.ItemsSource = Storage.Vaults;
            ComboBoxPerson.ItemsSource = Storage.Persons;
        }

        public WindowAddEditCredit(Credit credit)
        {
            InitializeComponent();

            _credit = credit;

            TextBoxName.Text = _credit.Name;

            LabelValue.Content = Convert.ToString(_credit.Value);

            DatePickerDayOpen.SelectedDate = _credit.DayOpen;
            DatePickerDayClose.SelectedDate = _credit.DayClose;

            TextBoxPercent.Text = Convert.ToString(_credit.Percent);


            ComboBoxCards.ItemsSource = Storage.Vaults;
            ComboBoxCards.SelectedItem = _credit.BindedCard;

            ComboBoxPerson.ItemsSource = Storage.Persons;
            ComboBoxPerson.SelectedItem = _credit.Person;


        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            if (TextBoxValueWithoutPercent.Text == String.Empty)
            {
                MessageBox.Show("You need to enter the value!");
                return;
            }

            decimal enteredValue = 0;
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

            decimal enteredPercent = 0;
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


            var enteredVault = (Card)ComboBoxCards.SelectedItem;

            if (_credit == null)
            {
               

                _credit = new Credit(TextBoxName.Text, Convert.ToDouble(TextBoxPercent.Text), null, enteredValue, (Person)ComboBoxPerson.SelectedItem, (Card)ComboBoxCards.SelectedItem, (
                    DateTime)DatePickerDayClose.SelectedDate, (DateTime)DatePickerDayOpen.SelectedDate);
                Storage.Credits.Add(_credit);

            }
            else
            {
                _credit.Name = TextBoxName.Text;
                _credit.Percent = Convert.ToDouble(TextBoxPercent.Text);
                _credit.ValuewithoutPercent = enteredValue;
                _credit.Value = (decimal?)LabelValue.Content;
                _credit.BindedCard = (Card)ComboBoxCards.SelectedItem;
                _credit.DayOpen = (DateTime)DatePickerDayOpen.SelectedDate;
                _credit.DayClose = (DateTime)DatePickerDayClose.SelectedDate;
                _credit.Person = (Person)ComboBoxPerson.SelectedItem;
            }

            Storage.Save(); 
            this.Close();
        }
    }
}
