using MoneyHustler.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using MoneyHustler.Helpers;

namespace MoneyHustler.Tabs
{
    /// <summary>
    /// Interaction logic for TabItemCategories.xaml
    /// </summary>
    public partial class TabItemCategories : TabItem
    {
        private Storage _storageInstance;

        private ObservableCollection<IncomeType> _incomeTypes;

        private ObservableCollection<ExpenseType> _expenseTypes;

        private IncomeType _incomeTypeToRename;
        private ExpenseType _expenseTypeToRename;

        public TabItemCategories()
        {
            InitializeComponent();

            _storageInstance = Storage.GetInstance();

            _incomeTypes = new ObservableCollection<IncomeType>(_storageInstance.IncomeTypes);
            _expenseTypes = new ObservableCollection<ExpenseType>(_storageInstance.ExpenseTypes);

            ListViewIncomes.ItemsSource = _incomeTypes;
            ListViewExpenses.ItemsSource = _expenseTypes;

            SetIncomeLabelsForAdding();
            SetExpenseLabelsForAdding();
        }      

        #region Incomes

        private void ButtonRemoveIncomeCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var incomeType = (IncomeType)button.DataContext;

            if (Storage.IsIncomeTypeUsedInVaults(incomeType))
            {
                MessageBox.Show("Эта категория используется вами!");
                return;
            }

            _storageInstance.IncomeTypes.Remove(incomeType);
            _incomeTypes.Remove(incomeType);

            Storage.Save();
        }


        private void SetIncomeLabelsForEditing(string name)
        {
            LabelAddIncomeCategories.Content = $"Переименовать: {name}";
            LabelEnterIncomeCategories.Content = "Введите новое название: ";

            UIHelpers.SetButtonEnabledAndVisibility(ButtonRenameFinallyIncomeCategory, true);
            UIHelpers.SetButtonEnabledAndVisibility(ButtonAddIncomeCategory, false);

            TextBoxEnterIncomeCategory.Text = string.Empty;
        }

        private void SetIncomeLabelsForAdding()
        {
            LabelAddIncomeCategories.Content = "Добавить категорию: ";
            LabelEnterIncomeCategories.Content = "Введите название категории: ";

            UIHelpers.SetButtonEnabledAndVisibility(ButtonRenameFinallyIncomeCategory, false);
            UIHelpers.SetButtonEnabledAndVisibility(ButtonAddIncomeCategory, true);

            TextBoxEnterIncomeCategory.Text = string.Empty;
        } 

        private void ButtonRenameIncomeCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var incomeType = (IncomeType)button.DataContext;

            SetIncomeLabelsForEditing(incomeType.Name);
            _incomeTypeToRename = incomeType;
        }

        private void ButtonRenameFinallyIncomeCategoryClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxEnterIncomeCategory.Text))
            {
                return;
            }

            var enteredIncomeTypeName = TextBoxEnterIncomeCategory.Text.Trim();

            if (_storageInstance.IncomeTypes.Any(item => item.Name == enteredIncomeTypeName))
            {
                MessageBox.Show("Категория с таким именем уже существует!");
                return;
            }

            _incomeTypeToRename.Name = enteredIncomeTypeName;

            _incomeTypes.Clear();
            foreach (IncomeType type in _storageInstance.IncomeTypes)
            {
                _incomeTypes.Add(type);
            }

            MessageBox.Show("Успешно переименовано!");
            SetIncomeLabelsForAdding();

            Storage.Save();

        }


        private void ButtonAddIncomeCategoryClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxEnterIncomeCategory.Text)) return;

            var enteredIncomeTypeName = TextBoxEnterIncomeCategory.Text.Trim();

            if (_storageInstance.IncomeTypes.Any(item => item.Name == enteredIncomeTypeName))
            {
                MessageBox.Show("Такая категория уже существует!");
                return;
            }
            var incomeType = new IncomeType() { Name = enteredIncomeTypeName };

            _storageInstance.IncomeTypes.Add(incomeType);
            _incomeTypes.Add(incomeType);

            TextBoxEnterIncomeCategory.Text = string.Empty;

            Storage.Save();
        }

        #endregion
        #region Expenses

        private void ButtonRemoveExpenseCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var expenseType = (ExpenseType)button.DataContext;

            if (Storage.IsExpenseTypeUsedInVaults(expenseType))
            {
                MessageBox.Show("Эта категория используется вами!");
                return;
            }

            _storageInstance.ExpenseTypes.Remove(expenseType);
            _expenseTypes.Remove(expenseType);

            Storage.Save();
        }

        private void SetExpenseLabelsForEditing(string name)
        {
            LabelAddExpenseCategories.Content = $"Переименовать: {name}";
            LabelEnterExpenseCategories.Content = "Введите новое название: ";

            UIHelpers.SetButtonEnabledAndVisibility(ButtonRenameFinallyExpenseCategory, true);
            UIHelpers.SetButtonEnabledAndVisibility(ButtonAddExpenseCategory, false);

            TextBoxEnterExpenseCategory.Text = string.Empty;
        }

        private void SetExpenseLabelsForAdding()
        {
            LabelAddExpenseCategories.Content = "Добавить категорию: ";
            LabelEnterExpenseCategories.Content = "Введите название категории: ";

            UIHelpers.SetButtonEnabledAndVisibility(ButtonRenameFinallyExpenseCategory, false);
            UIHelpers.SetButtonEnabledAndVisibility(ButtonAddExpenseCategory, true);

            TextBoxEnterExpenseCategory.Text = string.Empty;
        }

        private void ButtonRenameExpenseCategoryClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var expenseType = (ExpenseType)button.DataContext;

            SetExpenseLabelsForEditing(expenseType.Name);
            _expenseTypeToRename = expenseType;
        }

        private void ButtonRenameFinallyExpenseCategoryClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxEnterExpenseCategory.Text))
            {
                return;
            }

            var enteredExpenseTypeName = TextBoxEnterExpenseCategory.Text.Trim();

            if (_storageInstance.ExpenseTypes.Any(item => item.Name == enteredExpenseTypeName))
            {
                MessageBox.Show("Категория с таким именем уже существует!");
                return;
            }

            _expenseTypeToRename.Name = enteredExpenseTypeName;

            _expenseTypes.Clear();
            foreach (ExpenseType type in _storageInstance.ExpenseTypes)
            {
                _expenseTypes.Add(type);
            }

            MessageBox.Show("Успешно переименовано!");
            SetExpenseLabelsForAdding();

            Storage.Save();
        }

        private void ButtonAddExpenseCategoryClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxEnterExpenseCategory.Text)) return;

            var enteredExpenseTypeName = TextBoxEnterExpenseCategory.Text.Trim();

            if (_storageInstance.ExpenseTypes.Any(item => item.Name == enteredExpenseTypeName))
            {
                MessageBox.Show("Такая категория уже существует!");
                return;
            }

            var expenseType = new ExpenseType() { Name = enteredExpenseTypeName };
            _storageInstance.ExpenseTypes.Add(expenseType);
            _expenseTypes.Add(expenseType);

            TextBoxEnterExpenseCategory.Text = string.Empty;

            Storage.Save();
        }
        #endregion
    }
}

