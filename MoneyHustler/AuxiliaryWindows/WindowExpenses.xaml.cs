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
using MoneyHustler.Models;

namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary>
    /// Interaction logic for WindowExpenses.xaml
    /// </summary>
    public partial class WindowExpenses : Window
    {
        List<Button> editButtons = new List<Button>();
        List<Button> deleteButtons = new List<Button>();
        List<ComboBoxItem> typesExpense = new List<ComboBoxItem>();
        List<MoneyVault> vaults = new();
        List<ExpenseType> expenseTypes = new();
        List<Person> people = new();
        Binding namePeople = new Binding { };

        private void TestInitComponents()
        {
            Card halva = new Card("Halva", 1000m, 1.5m);
            Card sber = new Card("SberBank", 5000m, 0.5m);

            Person ivan = new() { Name = "Иван" };
            Person petya = new() { Name = "Пётр" };

            ExpenseType food = new() { Name = "Хавка" };
            ExpenseType sport = new() { Name = "Качалка" };

            vaults.Add(sber); vaults.Add(halva);

            expenseTypes.Add(food); expenseTypes.Add(sport);

            people.Add(ivan); people.Add(petya);
        }
        public WindowExpenses()
        {
            TestInitComponents();
            InitializeComponent();
            PersonInit();
            VaultInit();
            TypeInit();
            
            
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            StackPanel sp = (StackPanel)((Button)e.OriginalSource).Parent;

            listBox1.Items.Remove(sp);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            
            if ((string)((Button)e.OriginalSource).Content == "Edit")
            {
                AddButton.IsEnabled = false;
                ((Button)e.OriginalSource).Content = "Save";
                if(((Button)e.OriginalSource).Parent is StackPanel)
                {
                    StackPanel sp = (StackPanel)((Button)e.OriginalSource).Parent;

                    foreach (UIElement item in sp.Children)
                    {
                        item.IsEnabled = true;
                    }
                }
            }
            else if ((string)((Button)e.OriginalSource).Content == "Save")
            {
                AddButton.IsEnabled = true;
                ((Button)e.OriginalSource).Content = "Edit";
                if (((Button)e.OriginalSource).Parent is StackPanel)
                {
                    StackPanel sp = (StackPanel)((Button)e.OriginalSource).Parent;

                    foreach (UIElement item in sp.Children)
                    {
                        if (!(item is Button))
                            item.IsEnabled = false;   
                    }
                }
            }


        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            
            TextBox amountBox = new TextBox { Text = Amount.Text, IsEnabled = false };
            ComboBox personBox = CreateNewComboBox(Person, Person.Text);
            ComboBox vaultBox = CreateNewComboBox(Vault, Vault.Text);
            TextBox commentBox = new TextBox { Text = Comment.Text, IsEnabled = false };
            ComboBox typeBox = CreateNewComboBox(TypeComboBox, TypeComboBox.Text); 
            DatePicker datePicker = new DatePicker {  SelectedDate = DatePick.SelectedDate, IsEnabled = false };

            StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            stackPanel.Children.Add(datePicker);
            stackPanel.Children.Add(amountBox);
            stackPanel.Children.Add(personBox);
            stackPanel.Children.Add(vaultBox);
            stackPanel.Children.Add(commentBox);
            stackPanel.Children.Add(typeBox);

            editButtons.Add(new Button { Content = "Edit" });
            editButtons[editButtons.Count - 1].Click += Edit_Click;
            deleteButtons.Add(new Button { Content = "Delete" });
            deleteButtons[deleteButtons.Count - 1].Click += Delete_Click;

            stackPanel.Children.Add(editButtons[editButtons.Count - 1]);
            stackPanel.Children.Add(deleteButtons[editButtons.Count - 1]);
            listBox1.Items.Add(stackPanel);

            Expense expense = new Expense(
                Convert.ToDecimal(Amount.Text),
                (DateTime)DatePick.SelectedDate,
                people[0],
                "насвай",
                new ExpenseType { Name = "В ротик" }
                );


        }


        private void TypeInit( )
        {
            bool isSelected = true;
            foreach (ExpenseType item in expenseTypes)
            {
                TypeComboBox.Items.Add(new ComboBoxItem { Content = item.Name, IsSelected = isSelected });
                isSelected = false;
            }
        }

        private void PersonInit()
        {

            bool isSelected = true;
            foreach (Person item in people)
            {
                Person.Items.Add(new ComboBoxItem {  Content = item.Name, IsSelected = isSelected });
                isSelected = false;
            }
        }

        private void VaultInit()
        {
            bool isSelected = true;
            foreach (MoneyVault item in vaults)
            {
                Vault.Items.Add(new ComboBoxItem { Content = item.Name, IsSelected = isSelected });
                isSelected = false;
            }
        }

        private ComboBox CreateNewComboBox(ComboBox box, string contentSelectedItem)
        {
            ComboBox comboBox = new ComboBox { IsEnabled = false};
            bool isSelected = false;
            foreach (ComboBoxItem item in box.Items)
            {
                if ((string)item.Content == contentSelectedItem)
                {
                    isSelected = true;
                }
                comboBox.Items.Add(new ComboBoxItem { Content = item.Content, IsSelected = isSelected });
                isSelected = false;
            }
            return comboBox;
        }

        

    }
}
