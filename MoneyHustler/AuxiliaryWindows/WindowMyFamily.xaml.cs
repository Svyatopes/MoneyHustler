﻿using System;
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
        private Storage _instance = Storage.GetInstance();

        private ObservableCollection<Person> _persons;
        private Person _personToRename;
        public WindowMyFamily()
        {

            InitializeComponent();
            _persons = new ObservableCollection<Person>(_instance.Persons);
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
            _instance.Persons.Remove(person);
            _persons.Remove(person);
            Storage.Save();
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
            ListViewPersonsDisplay.IsEnabled = false;
            TextBoxEnterMemberName.Text = person.Name;
            SetButtonEnabledAndVisibility(ButtonAddNewMember, false);
            SetButtonEnabledAndVisibility(ButtonRenameFinallyExistingMember, true);
            LabelAddFamilyMembers.Visibility = Visibility.Hidden;
            LabelEditFamilyMembers.Visibility = Visibility.Visible;
            LabelEditFamilyMembers.Content = "Переименовать участника: \n" + person.Name;


           
            _personToRename = person;
        }
        private void ButtonRenameFinallyExistingMemberClick(object sender, RoutedEventArgs e) 
        {
            
            string enteredPerson = TextBoxEnterMemberName.Text.Trim();

            if (TextBoxEnterMemberName.Text == String.Empty)
            {
                MessageBox.Show("Введите имя пользователя!");
                return;
            }


            if (_instance.Persons.Any(item => item.Name == enteredPerson))
            {
                MessageBox.Show("Такое имя уже существует");
                return;
            }


            if (_personToRename == null)
            {
                _personToRename = new Person { Name = enteredPerson };
                _instance.Persons.Add(_personToRename);

            }
            else
            {
                _personToRename.Name = enteredPerson;
            }
            TextBoxEnterMemberName.Text = String.Empty;
            SetButtonEnabledAndVisibility(ButtonAddNewMember, true);
            SetButtonEnabledAndVisibility(ButtonRenameFinallyExistingMember, false);
            UpdatePersonsView();
            ListViewPersonsDisplay.IsEnabled = true;
            LabelAddFamilyMembers.Visibility = Visibility.Visible;
            LabelEditFamilyMembers.Visibility = Visibility.Hidden;
            Storage.Save();


        }

        private void ButtonAddNewMemberClick(object sender, RoutedEventArgs e) 
        {

            string enteredPerson = TextBoxEnterMemberName.Text.Trim();

            if (TextBoxEnterMemberName.Text == String.Empty)
            {
                MessageBox.Show("Введите имя пользователя!");
                return;
            }


            if (_instance.Persons.Any(item => item.Name == enteredPerson))
            {
                MessageBox.Show("Такое имя уже существует");
                return;
            }

            _instance.Persons.Add(new Person { Name = enteredPerson });
            UpdatePersonsView();
            Storage.Save();

        }

        private void UpdatePersonsView()
        {
            TextBoxEnterMemberName.Text = String.Empty;
            _persons.Clear();
            var allPersons = _instance.Persons;
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