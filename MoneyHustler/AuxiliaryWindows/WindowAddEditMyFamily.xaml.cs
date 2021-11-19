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
    /// Interaction logic for WindowAddEditMyFamily.xaml 
    /// </summary> 
    public partial class WindowAddEditMyFamily : Window
    {
        private Person _person;

        public WindowAddEditMyFamily()
        {
            InitializeComponent();
        }

        public WindowAddEditMyFamily(Person person)
        {
            InitializeComponent();

            _person = person;
            TextBoxChangesInput.Text = _person.Name;
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            string enteredPerson = TextBoxChangesInput.Text;

            if (TextBoxChangesInput.Text == String.Empty)
            {
                MessageBox.Show("Введите имя пользователя!");
                return;
            }

            if (enteredPerson.Contains("0") || enteredPerson.Contains("1") || enteredPerson.Contains("2") ||
                enteredPerson.Contains("3") || enteredPerson.Contains("4") || enteredPerson.Contains("5") ||
                enteredPerson.Contains("6") || enteredPerson.Contains("7") || enteredPerson.Contains("8") ||
                enteredPerson.Contains("9") || enteredPerson.Contains(" "))
            {
                MessageBox.Show("Имя не должно содержать цифр и пробелов!");
                return;
            }

            if (Char.IsLower(enteredPerson[0]))
            {
                MessageBox.Show("Имя должно начинаться с большой буквы!");
                return;
            }

            for (int i = 1; i < enteredPerson.Length; i++)
            {
                if (Char.IsUpper(enteredPerson[i]))
                {
                    MessageBox.Show("Имя не может содержать заглавные буквы, кроме первой");
                    return;
                }
            }

            foreach (Person item in Storage.Persons)
            {
                if (item.Name == enteredPerson)
                {
                    MessageBox.Show("Такое имя уже существует");
                    return;
                }
            }

            if (_person == null)
            {
                _person = new Person { Name = enteredPerson };
                Storage.Persons.Add(_person);

            }
            else
            {
                _person.Name = enteredPerson;
            }

            this.Close();
        }
    }
}