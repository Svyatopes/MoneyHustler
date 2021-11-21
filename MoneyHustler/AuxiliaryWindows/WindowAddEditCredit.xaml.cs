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
        private Storage _storageInstance = Storage.GetInstance();

        private Credit _credit;
        public WindowAddEditCredit()
        {
            InitializeComponent();
            ComboBoxCards.ItemsSource = _storageInstance.Vaults;
            ComboBoxPerson.ItemsSource = _storageInstance.Persons;
        }

        public WindowAddEditCredit(Credit credit)
        {
            InitializeComponent();

            _credit = credit;

            if (_credit != null)
            {
                TextBoxValueWithoutPercent.IsEnabled = false;
                DatePickerDayOpen.IsEnabled = false;
                DatePickerDayClose.IsEnabled = false;
                TextBoxPercent.IsEnabled = false;
            }
            else
            {
                TextBoxValueWithoutPercent.IsEnabled = true;
                DatePickerDayOpen.IsEnabled = true;
                DatePickerDayClose.IsEnabled = true;
                TextBoxPercent.IsEnabled = true;
            }

            TextBoxName.Text = _credit.Name;

            TextBoxValueWithoutPercent.Text = Convert.ToString(_credit.ValuewithoutPercent);

            DatePickerDayOpen.SelectedDate = _credit.DayOpen;
            DatePickerDayClose.SelectedDate = _credit.DayClose;

            TextBoxPercent.Text = Convert.ToString(_credit.Percent);


            ComboBoxCards.ItemsSource = _storageInstance.Vaults;
            ComboBoxCards.SelectedItem = _credit.BindedCard;

            ComboBoxPerson.ItemsSource = _storageInstance.Persons;
            ComboBoxPerson.SelectedItem = _credit.Person;


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
            this.Close();
        }
    }
}
