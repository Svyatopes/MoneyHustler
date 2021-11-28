using MoneyHustler.Helpers;
using MoneyHustler.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;



namespace MoneyHustler.Tabs
{
    /// <summary>
    /// Interaction logic for TabItemMyFamily.xaml
    /// </summary>
    public partial class TabItemMyFamily : TabItem
    {
        private Storage _storageInstance;

        private ObservableCollection<Person> _persons;

        private Person _personToRename;

        public TabItemMyFamily()
        {
            InitializeComponent();
            _storageInstance = Storage.GetInstance();
            _persons = new ObservableCollection<Person>(_storageInstance.Persons);
            ListViewPersonsDisplay.ItemsSource = _persons;
        }

        private void TabItemFamily_Selected(object sender, RoutedEventArgs e)
        {
            UpdatePersonsView();
        }
        private void UpdatePersonsView()
        {
            _persons.Clear();
            var allPersons = _storageInstance.Persons;
            foreach (var income in allPersons)
            {
                _persons.Add(income);
            }
        }


        private void ButtonDeleteClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var person = (Person)button.DataContext;
            if (Storage.IsPesonUsedInVaults(person))
            {
                MessageBox.Show("Этого человека убрать нельзя");
                return;
            }
            _storageInstance.Persons.Remove(person);
            _persons.Remove(person);
            Storage.Save();
        }
        private void SetPersonLabelsForEditing(string name)
        {
            ListViewPersonsDisplay.IsEnabled = false;
            TextBoxEnterMemberName.Text = name;

            UIHelpers.SetButtonEnabledAndVisibility(ButtonAddNewMember, false);
            UIHelpers.SetButtonEnabledAndVisibility(ButtonRenameFinallyExistingMember, true);

            LabelAddFamilyMembers.Visibility = Visibility.Hidden;
            LabelEditFamilyMembers.Visibility = Visibility.Visible;

            LabelEditFamilyMembers.Content = $"Переименовать участника: \n{ name}";
        }

        private void SetPersonLabelsAfterEditing()
        {
            TextBoxEnterMemberName.Text = String.Empty;

            UIHelpers.SetButtonEnabledAndVisibility(ButtonAddNewMember, true);
            UIHelpers.SetButtonEnabledAndVisibility(ButtonRenameFinallyExistingMember, false);

            ListViewPersonsDisplay.IsEnabled = true;
            LabelAddFamilyMembers.Visibility = Visibility.Visible;
            LabelEditFamilyMembers.Visibility = Visibility.Hidden;
        }
        private void ButtonRenameExistingMemberClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var person = (Person)button.DataContext;

            SetPersonLabelsForEditing(person.Name);

            _personToRename = person;
        }
        private void ButtonRenameFinallyExistingMemberClick(object sender, RoutedEventArgs e)
        {

            string enteredPerson = TextBoxEnterMemberName.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredPerson))
            {
                MessageBox.Show("Введите имя пользователя!");
                return;
            }


            if (Storage.CheckIfPersonExist(enteredPerson))
            {
                MessageBox.Show("Такое имя уже существует");
                return;
            }


            if (_personToRename == null)
            {
                _personToRename = new Person { Name = enteredPerson };
                _storageInstance.Persons.Add(_personToRename);

            }
            else
            {
                _personToRename.Name = enteredPerson;
            }
            SetPersonLabelsAfterEditing();
            UpdatePersonsView();
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


            if (Storage.CheckIfPersonExist(enteredPerson))
            {
                MessageBox.Show("Такое имя уже существует");
                return;
            }

            _storageInstance.Persons.Add(new Person { Name = enteredPerson });
            _persons.Add(new Person { Name = enteredPerson });
            Storage.Save();

        }




    }
}
