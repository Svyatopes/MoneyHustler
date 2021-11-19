using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MoneyHustler.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary>
    /// Interaction logic for WindowCategories.xaml
    /// </summary>
    public partial class WindowCategories : Window
    {
        private ObservableCollection<IncomeType> _incomeTypes;

        private ObservableCollection<ExpenseType> _expenseTypes;

        private IncomeType _incomeTypeToRename;
        private ExpenseType _expenseTypeToRename;

        public WindowCategories()
        {
            InitializeComponent();

            _incomeTypes = new ObservableCollection<IncomeType>(Storage.IncomeTypes);
            _expenseTypes = new ObservableCollection<ExpenseType>(Storage.ExpenseTypes);

            ListViewIncomes.ItemsSource = _incomeTypes;
            ListViewExpenses.ItemsSource = _expenseTypes;

            SetIncomeLabelsForAdding();
            SetExpenseLabelsForAdding();
        }

        private void ButtonRemoveIncomeCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var incomeType = (IncomeType)button.DataContext;

            if (Storage.GetAllIncomes().Any(item => item.Type == incomeType))
            {
                MessageBox.Show("Эта категория используется вами!");
                return;
            }    

            Storage.IncomeTypes.Remove(incomeType);
            _incomeTypes.Remove(incomeType);

            Storage.Save();
        }

        private void ButtonRemoveExpenseCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var expenseType = (ExpenseType)button.DataContext;

            if (Storage.GetAllExpences().Any(item => item.Type == expenseType))
            {
                MessageBox.Show("Эта категория используется вами!");
                return;
            }

            Storage.ExpenseTypes.Remove(expenseType);
            _expenseTypes.Remove(expenseType);

            Storage.Save();
        }

        private void SetIncomeLabelsForEditing(string name)
        {
            LabelAddIncomeCategories.Content = $"Переименовать: {name}";
            LabelEnterIncomeCategories.Content = "Введите новое название: ";


            ButtonRenameFinallyIncomeCategory.IsEnabled = true;
            ButtonRenameFinallyIncomeCategory.Visibility = Visibility.Visible;
            ButtonAddIncomeCategory.IsEnabled = false;
            ButtonAddIncomeCategory.Visibility = Visibility.Hidden;
            TextBoxEnterIncomeCategory.Text = string.Empty;
        }

        private void SetIncomeLabelsForAdding()
        {
            LabelAddIncomeCategories.Content = "Добавить категорию: ";
            LabelEnterIncomeCategories.Content = "Введите название категории: ";

            ButtonRenameFinallyIncomeCategory.IsEnabled = false;
            ButtonRenameFinallyIncomeCategory.Visibility = Visibility.Hidden;
            ButtonAddIncomeCategory.IsEnabled = true;
            ButtonAddIncomeCategory.Visibility = Visibility.Visible;
            TextBoxEnterIncomeCategory.Text = string.Empty;
        }

        private void SetExpenseLabelsForEditing(string name)
        {
            LabelAddExpenseCategories.Content = $"Переименовать: {name}";
            LabelEnterExpenseCategories.Content = "Введите новое название: ";


            ButtonRenameFinallyExpenseCategory.IsEnabled = true;
            ButtonRenameFinallyExpenseCategory.Visibility = Visibility.Visible;
            ButtonAddExpenseCategory.IsEnabled = false;
            ButtonAddExpenseCategory.Visibility = Visibility.Hidden;
            TextBoxEnterExpenseCategory.Text = string.Empty;
        }

        private void SetExpenseLabelsForAdding()
        {
            LabelAddExpenseCategories.Content = "Добавить категорию: ";
            LabelEnterExpenseCategories.Content = "Введите название категории: ";

            ButtonRenameFinallyExpenseCategory.IsEnabled = false;
            ButtonRenameFinallyExpenseCategory.Visibility = Visibility.Hidden;
            ButtonAddExpenseCategory.IsEnabled = true;
            ButtonAddExpenseCategory.Visibility = Visibility.Visible;
            TextBoxEnterExpenseCategory.Text = string.Empty;
        }

        private void ButtonRenameIncomeCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var incomeType = (IncomeType)button.DataContext;

            SetIncomeLabelsForEditing(incomeType.Name);
            _incomeTypeToRename = incomeType;
        }

        

        private void ButtonRenameExpenseCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var expenseType = (ExpenseType)button.DataContext;

            SetExpenseLabelsForEditing(expenseType.Name);
            _expenseTypeToRename = expenseType;
        }


        private void ButtonRenameFinallyIncomeCategoryClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxEnterIncomeCategory.Text))
            {
                return;
            }

            var enteredIncomeTypeName = TextBoxEnterIncomeCategory.Text.Trim();

            if (Storage.IncomeTypes.Any(item => item.Name == enteredIncomeTypeName))
            {
                MessageBox.Show("Категория с таким именем уже существует!");
                return;
            }

            _incomeTypeToRename.Name = enteredIncomeTypeName;

            _incomeTypes.Clear();
            foreach (IncomeType type in Storage.IncomeTypes)
            {
                _incomeTypes.Add(type);
            }

            MessageBox.Show("Успешно переименовано!");
            SetIncomeLabelsForAdding();

            Storage.Save();

        }

        private void ButtonRenameFinallyExpenseCategoryClick(object sender, RoutedEventArgs e)
        {
            if (TextBoxEnterExpenseCategory.Text.Length == 0)
            {
                return;
            }

            var enteredExpenseTypeName = TextBoxEnterExpenseCategory.Text.Trim();

            if (Storage.ExpenseTypes.Any(item => item.Name == enteredExpenseTypeName))
            {
                MessageBox.Show("Категория с таким именем уже существует!");
                return;
            }

            _expenseTypeToRename.Name = enteredExpenseTypeName;

            _expenseTypes.Clear();
            foreach (ExpenseType type in Storage.ExpenseTypes)
            {
                _expenseTypes.Add(type);
            }

            MessageBox.Show("Успешно переименовано!");
            SetExpenseLabelsForAdding();

            Storage.Save();
        }


        private void ButtonAddIncomeCategoryClick(object sender, RoutedEventArgs e)
        {
            if (TextBoxEnterIncomeCategory.Text.Length == 0) return;
            if (_incomeTypes.Any(item => item.Name == TextBoxEnterIncomeCategory.Text))
            {
                MessageBox.Show("Такая категория уже существует!");
                return;
            }

            Storage.IncomeTypes.Add(new IncomeType() { Name = TextBoxEnterIncomeCategory.Text });
            _incomeTypes.Add(new IncomeType() { Name = TextBoxEnterIncomeCategory.Text });
            TextBoxEnterIncomeCategory.Text = string.Empty;

            Storage.Save();
        }

        private void ButtonAddExpenseCategoryClick(object sender, RoutedEventArgs e)
        {
            if (TextBoxEnterExpenseCategory.Text.Length == 0) return;
            if (_incomeTypes.Any(item => item.Name == TextBoxEnterExpenseCategory.Text))
            {
                MessageBox.Show("Такая категория уже существует!");
                return;
            }
            Storage.ExpenseTypes.Add(new ExpenseType() { Name = TextBoxEnterExpenseCategory.Text });
            _expenseTypes.Add(new ExpenseType() { Name = TextBoxEnterExpenseCategory.Text });
            TextBoxEnterExpenseCategory.Text = string.Empty;

            Storage.Save();
        }

        private void ButtonToMainScreenClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonToMainScreenExpensesClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

