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
            Storage.Persons = new List<Person>();
            Storage.Persons.Add(new Person() { Name = "Sasha"});
            Storage.Persons.Add(new Person() { Name = "Pasha"});
            InitializeComponent();
            _persons = new ObservableCollection<Person>(Storage.Persons);
            ListViewPersonsDisplay.ItemsSource = _persons; 
        }

        private void ButtonAddPersonClick(object sender, RoutedEventArgs e)
        {
            WindowAddEditMyFamily WindowAddEditMyFamily = new();
            WindowAddEditMyFamily.ShowDialog();
        }

        private void ButtonEditClick(object sender, RoutedEventArgs e)
        {
            WindowAddEditMyFamily WindowAddEditMyFamily = new();
            WindowAddEditMyFamily.ShowDialog();
        }

        private void ButtonDeleteClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var person = (Person)button.DataContext;
            if (Storage.GetAllIncomes().Any(item => item.Person == person)|| Storage.GetAllExpences().Any(item => item.Person == person))
            {
                MessageBox.Show("You can't remove this person, cause this name is already used in incomes/expenses");
                return;
            }
            Storage.Persons.Remove(person);
            _persons.Remove(person);
        }
    }
}
