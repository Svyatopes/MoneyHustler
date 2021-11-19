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
        public WindowMyFamily()
        {
            
            InitializeComponent();
            _persons = new ObservableCollection<Person>(Storage.Persons);
            ListViewPersonsDisplay.ItemsSource = _persons;
        }

        private void ButtonAddPersonClick(object sender, RoutedEventArgs e)
        {
            WindowAddEditMyFamily WindowAddEditMyFamily = new();
            WindowAddEditMyFamily.ShowDialog();
            UpdatePersonsView();
            Storage.Save();

        }

        private void ButtonEditClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var person = (Person)button.DataContext;
            WindowAddEditMyFamily WindowAddEditMyFamily = new(person);
            WindowAddEditMyFamily.ShowDialog();
            UpdatePersonsView();
            Storage.Save();

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

        private void UpdatePersonsView()
        {
            _persons.Clear();
            var allPersons = Storage.Persons;
            foreach (var income in allPersons)
            {
                _persons.Add(income);
            }
        }
    }
}