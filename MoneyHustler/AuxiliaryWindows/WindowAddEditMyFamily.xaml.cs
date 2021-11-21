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

//namespace MoneyHustler.AuxiliaryWindows
////{
////    /// <summary> 
////    /// Interaction logic for WindowAddEditMyFamily.xaml 
////    /// </summary> 
////    //public partial class WindowAddEditMyFamily : Window
////    //{
////    //    private Person _person;

////    //    public WindowAddEditMyFamily()
////    //    {
////    //        InitializeComponent();
////    //    }

////    //    public WindowAddEditMyFamily(Person person)
////    //    {
////    //        InitializeComponent();

////    //        _person = person;
////    //        TextBoxChangesInput.Text = _person.Name;
////    //    }

////    //    private void ButtonSaveClick(object sender, RoutedEventArgs e)
////    //    {
////    //        string enteredPerson = TextBoxChangesInput.Text.Trim();

////    //        if (TextBoxChangesInput.Text == String.Empty)
////    //        {
////    //            MessageBox.Show("Введите имя пользователя!");
////    //            return;
////    //        }


////    //        if (Storage.Persons.Any(item => item.Name == enteredPerson))
////    //        {
////    //            MessageBox.Show("Такое имя уже существует");
////    //            return;
////    //        }


////    //        if (_person == null)
////    //        {
////    //            _person = new Person { Name = enteredPerson };
////    //            Storage.Persons.Add(_person);

////    //        }
////    //        else
////    //        {
////    //            _person.Name = enteredPerson;
////    //        }

////    //        this.Close();
////    //    }
////    }
////}