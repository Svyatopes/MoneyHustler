using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MoneyHustler.Models;

namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary> 
    /// Interaction logic for WindowMyFamily.xaml 
    /// </summary> 
    public partial class WindowMyFamily : Window
    {
        private ObservableCollection<Person> _persons;
        private Person _personToRename;
        public WindowMyFamily()
        {
            
            InitializeComponent();
            _persons = new ObservableCollection<Person>(Storage.Persons);
            ListViewPersonsDisplay.ItemsSource = _persons;
        }
        private void ButtonDeleteClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var person = (Person)button.DataContext;
            if (Storage.GetAllIncomes().Any(item => item.Person == person) || Storage.GetAllExpences().Any(item => item.Person == person))
            {
                MessageBox.Show("You can't remove this person, cause this name is already used in incomes/expenses");
                return;
            }
            Storage.Persons.Remove(person);
            _persons.Remove(person);
            Storage.Save();
        }
        private void SetPersonsNameLabelsForEditing(string name) 
        {
            LabelAddFamilyMembers.Content = $"Переименовать: {name}";
            LabelEnterPersonsName.Content = "Введите новое имя";

            SetButtonEnabledAndVisibility(ButtonRenameFinallyExistingMember, true);
            SetButtonEnabledAndVisibility(ButtonAddNewMember, false);

            TextBoxEnterMemberName.Text = string.Empty;


        }
        private void SetPersonsNameLabelsForAdding()
        {
            LabelAddFamilyMembers.Content = "Добавить участника: ";
            LabelEnterPersonsName.Content = "Введите новое имя";

            SetButtonEnabledAndVisibility(ButtonRenameFinallyExistingMember, false);
            SetButtonEnabledAndVisibility(ButtonAddNewMember, true);
            TextBoxEnterMemberName.Text = string.Empty;
        }
        private void SetButtonEnabledAndVisibility(Button button, bool enabled)
        {
            if (enabled)
            {
                button.Visibility = Visibility.Visible;
                button.IsEnabled = true;
            }
            else
            {
                button.Visibility = Visibility.Hidden;
                button.IsEnabled = false;
            }
        }
        private void ButtonRenameExistingMemberClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var person = (Person)button.DataContext;

            SetPersonsNameLabelsForEditing(person.Name);
            _personToRename = person;
        }
        private void ButtonRenameFinallyExistingMemberClick(object sender, RoutedEventArgs e) //хуйня какая то, проверить с Антоном
        {
            string enteredPerson = TextBoxEnterMemberName.Text.Trim();

            if (TextBoxEnterMemberName.Text == String.Empty)
            {
                MessageBox.Show("Введите имя пользователя!");
                return;
            }


            if (Storage.Persons.Any(item => item.Name == enteredPerson))
            {
                MessageBox.Show("Такое имя уже существует");
                return;
            }


            if (_personToRename == null)
            {
                _personToRename = new Person { Name = enteredPerson };
                Storage.Persons.Add(_personToRename);

            }
            else
            {
                _personToRename.Name = enteredPerson;
            }

            this.Close(); //надо как то эту строчку поменять
            Storage.Save();
        }

        private void ButtonAddNewMemberClick(object sender, RoutedEventArgs e) // надо сделать по красоте
        {

            string enteredPerson = TextBoxEnterMemberName.Text.Trim();

            if (TextBoxEnterMemberName.Text == String.Empty)
            {
                MessageBox.Show("Введите имя пользователя!");
                return;
            }


            if (Storage.Persons.Any(item => item.Name == enteredPerson))
            {
                MessageBox.Show("Такое имя уже существует");
                return;
            }


            if (_personToRename == null)
            {
                _personToRename = new Person { Name = enteredPerson };
                Storage.Persons.Add(_personToRename);

            }
            else
            {
                _personToRename.Name = enteredPerson;
            }

            this.Close();
            UpdatePersonsView();
            Storage.Save();

        }

        
        



        private void UpdatePersonsView()
        {
            _persons.Clear();
            var allPersons = Storage.Persons;
            foreach (var income in allPersons)
            {
                _persons.Add(income);
            }
        }

        private void ButtonToMainScreenClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        
    }
}