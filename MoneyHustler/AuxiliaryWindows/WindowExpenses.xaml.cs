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

        private ObservableCollection<Expense> listOfExpensesView;
        public WindowExpenses()
        {
            InitializeComponent();
            initExpenses();
            listOfExpensesView = new ObservableCollection<Expense>(Storage.GetAllExpences());
            Person.ItemsSource = Storage.Persons;
            Person.SelectedItem = Storage.Persons[0];
            Vault.ItemsSource = Storage.Vaults;
            Vault.SelectedItem = Storage.Vaults[0];
            TypeComboBox.ItemsSource = Storage.ExpenseTypes;
            TypeComboBox.SelectedItem = Storage.ExpenseTypes[0];
            DatePick.SelectedDate = DateTime.Now;
            //listBox1.ItemsSource = listOfExpensesView;
            
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
            List<UIElement> listUI = new();

            listUI.Add(new DatePicker { SelectedDate = DatePick.SelectedDate, IsEnabled = false });
            listUI.Add( new TextBox { Text = Amount.Text, IsEnabled = false });
            listUI.Add(CreateNewComboBox(Person));
            listUI.Add(CreateNewComboBox(Vault));
            listUI.Add( new TextBox { Text = Comment.Text, IsEnabled = false });
            listUI.Add( CreateNewComboBox(TypeComboBox));

            StackPanel stackPanel = CreateStackPanel(listUI);

            editButtons.Add(new Button { Content = "Edit" });
            editButtons[editButtons.Count - 1].Click += Edit_Click;
            deleteButtons.Add(new Button { Content = "Delete" });
            deleteButtons[deleteButtons.Count - 1].Click += Delete_Click;

            stackPanel.Children.Add(editButtons[editButtons.Count - 1]);
            stackPanel.Children.Add(deleteButtons[editButtons.Count - 1]);
            listBox1.Items.Add(stackPanel);

            Expense expense = new Expense
            (
                Convert.ToDecimal(Amount.Text),
                (DateTime)DatePick.SelectedDate,
                (Person)Person.SelectedItem,
                Comment.Text,
                (ExpenseType)TypeComboBox.SelectedItem
            );

            foreach(MoneyVault item in Storage.Vaults)
            {
                if (item == Vault.SelectedItem)
                {
                    item.DecreaseBalance(expense);
                    break;
                }
            }
            

        }

        private ComboBox CreateNewComboBox(ComboBox box)
        {
            ComboBox comboBox = new ComboBox { IsEnabled = false, DisplayMemberPath = "Name"};
            comboBox.SelectedItem = box.SelectedItem;
            comboBox.ItemsSource = box.ItemsSource;
            return comboBox;
        }

        private StackPanel CreateStackPanel( List<UIElement> uIElements)
        {
            StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            foreach (UIElement item in uIElements)
            {
                stackPanel.Children.Add(item);
            }

            return stackPanel;
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal n = 0;
            if (!Decimal.TryParse(Amount.Text, out n))
            {
                AddButton.IsEnabled = false;
                AddButton.Content = "Зайди правильно";
                Amount.Background = Brushes.Red;
                
            }
            else
            {
                AddButton.IsEnabled = true;
                Amount.Background = Brushes.White;
                AddButton.Content = "Add";
            }  
        }

        private void initExpenses()
        {
            listOfExpensesView = new ObservableCollection<Expense>(Storage.GetAllExpences());

            foreach (Expense item in listOfExpensesView)
            {
                List<UIElement> listUI = new();

                listUI.Add(new DatePicker { SelectedDate = item.Date, IsEnabled = false });
                listUI.Add(new TextBox { Text = Convert.ToString(item.Amount), IsEnabled = false });
                listUI.Add(new ComboBox { ItemsSource = Storage.Persons, DisplayMemberPath = "Name", SelectedItem = item.Person, IsEnabled = false });
                listUI.Add(new ComboBox { ItemsSource = Storage.Vaults, DisplayMemberPath = "Name", SelectedItem = item.Vault, IsEnabled = false });
                listUI.Add(new TextBox { Text = item.Comment, IsEnabled = false });
                listUI.Add(new ComboBox { ItemsSource = Storage.ExpenseTypes, DisplayMemberPath = "Name", SelectedItem = item.Type, IsEnabled = false });

                StackPanel stackPanel = CreateStackPanel(listUI);

                editButtons.Add(new Button { Content = "Edit" });
                editButtons[editButtons.Count - 1].Click += Edit_Click;
                deleteButtons.Add(new Button { Content = "Delete" });
                deleteButtons[deleteButtons.Count - 1].Click += Delete_Click;

                stackPanel.Children.Add(editButtons[editButtons.Count - 1]);
                stackPanel.Children.Add(deleteButtons[editButtons.Count - 1]);
                listBox1.Items.Add(stackPanel);
            }
            
        }
    }
}
