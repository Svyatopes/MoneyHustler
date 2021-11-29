using MoneyHustler.Helpers;
using MoneyHustler.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MoneyHustler.Tabs
{
    /// <summary>
    /// Interaction logic for TabItemMoneyVaults.xaml
    /// </summary>
    public partial class TabItemMoneyVaults : TabItem
    {
        public class MoneyVaultForView
        {
            public string Name { get; set; }
            public decimal Balance { get; set; }
            public string TypeName { get; set; }
            public decimal? CashBack { get; set; }
            public decimal? Percent { get; set; }
            public MoneyVault Vault { get; set; }

            public MoneyVaultForView(MoneyVault vault)
            {
                Vault = vault;
                Name = vault.Name;
                Balance = vault.GetBalance();
                switch (vault)
                {
                    case Card:
                        CashBack = ((Card)vault).CashBack;
                        break;
                    case Deposit:
                        Percent = ((Deposit)vault).Percent;
                        break;
                }
            }
        }

        private enum MoneyVaultType
        {
            Card,
            Deposit,
            OnlyTopDeposit
        }

        private Storage _storageInstance;
        private ObservableCollection<MoneyVaultForView> _listMoneyVaults;
        private ObservableCollection<string> _moneyVaulTypes;
        private MoneyVault _vaultToEdit;

        public TabItemMoneyVaults()
        {
            InitializeComponent();
            _storageInstance = Storage.GetInstance();

            _listMoneyVaults = new ObservableCollection<MoneyVaultForView>();
            listViewForVaults.ItemsSource = _listMoneyVaults;
            UpdateListOfVaults();


            _moneyVaulTypes = new ObservableCollection<string>();
            ComboBoxTypeOfMoneyVault.ItemsSource = _moneyVaulTypes;

            _moneyVaulTypes.Add("Счет");
            _moneyVaulTypes.Add("Вклад");
            _moneyVaulTypes.Add("Вклад без снятия");

            ComboBoxTypeOfMoneyVault.SelectedIndex = (int)MoneyVaultType.Card;
            Slider_ValueChanged(null, null);

        }


        #region WorkWithView
        private void UpdateListOfVaults()
        {
            _listMoneyVaults.Clear();
            foreach (var vault in _storageInstance.Vaults)
            {
                MoneyVaultForView vaultView = new MoneyVaultForView(vault);

                vaultView.TypeName = vault switch
                {
                    Card => "Счет",
                    OnlyTopDeposit => "Вклад без снятия",
                    Deposit => "Вклад",
                    _ => string.Empty,
                };

                _listMoneyVaults.Add(vaultView);
            }
        }

        private void ChangeVisibilityOfGridAddEditVault(bool visible)
        {
            if (visible)
            {
                UIHelpers.ChangeWidthGridColumns(GridColumnAddEditMoneyVault, 1);
                GridListOfVaults.IsEnabled = false;
            }
            else
            {
                UIHelpers.ChangeWidthGridColumns(GridColumnAddEditMoneyVault, 0);
                GridListOfVaults.IsEnabled = true;
            }
        }

        private void ChangeAddEditVaultGridStateIsAdd(bool IsAdd)
        {
            if (IsAdd)
            {
                ComboBoxTypeOfMoneyVault.IsEnabled = true;
                StackPanelHowLong.Visibility = Visibility.Visible;
                DatePickerDayOfOpenDeposit.IsEnabled = true;
                TextBoxInitialAmount.IsEnabled = true;
            }
            else
            {
                ComboBoxTypeOfMoneyVault.IsEnabled = false;
                StackPanelHowLong.Visibility = Visibility.Hidden;
                DatePickerDayOfOpenDeposit.IsEnabled = false;
                TextBoxInitialAmount.IsEnabled = false;
            }
        }

        private void ClearAddEditVaultData()
        {
            _vaultToEdit = null;
            ComboBoxTypeOfMoneyVault.SelectedIndex = 0;
            TextBoxPercentCashback.Text = string.Empty;
            DatePickerDayOfOpenDeposit.SelectedDate = DateTime.Now;
            TextBoxVaultName.Text = string.Empty;
            TextBoxPercent.Text = string.Empty;
            TextBoxInitialAmount.Text = string.Empty;
            SliderHowLong.Value = 3;
        }

        #endregion

        #region Buttons

        private void ButtonAddMoneyVault_Click(object sender, RoutedEventArgs e)
        {
            ClearAddEditVaultData();
            ChangeAddEditVaultGridStateIsAdd(true);
            ChangeVisibilityOfGridAddEditVault(true);
        }



        private void ButtonEditVault_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var moneyVaultForView = (MoneyVaultForView)button.DataContext;

            _vaultToEdit = moneyVaultForView.Vault;
            TextBoxVaultName.Text = _vaultToEdit.Name;

            switch (_vaultToEdit)
            {
                case Card:
                    ComboBoxTypeOfMoneyVault.SelectedIndex = (int)MoneyVaultType.Card;
                    TextBoxPercentCashback.Text = ((Card)_vaultToEdit).CashBack.ToString();
                    break;
                case OnlyTopDeposit:
                    ComboBoxTypeOfMoneyVault.SelectedIndex = (int)MoneyVaultType.OnlyTopDeposit;
                    TextBoxPercent.Text = ((OnlyTopDeposit)_vaultToEdit).Percent.ToString();
                    break;
                case Deposit:
                    ComboBoxTypeOfMoneyVault.SelectedIndex = (int)MoneyVaultType.Deposit;
                    TextBoxPercent.Text = ((Deposit)_vaultToEdit).Percent.ToString();
                    break;
            }

            ChangeAddEditVaultGridStateIsAdd(false);
            ChangeVisibilityOfGridAddEditVault(true);
        }

        private void ButtonRemoveVault_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var moneyVaultForView = (MoneyVaultForView)button.DataContext;

            if (moneyVaultForView.Vault.IsHaveIncomesOrExpenses())
            {
                var userAnswer = MessageBox.Show("В этом кошельке есть какие-то доходы или расходы, вы ТОЧНО хотите его УДАЛИТЬ?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (userAnswer == MessageBoxResult.No)
                {
                    return;
                }
            }

            _storageInstance.Vaults.Remove(moneyVaultForView.Vault);
            _listMoneyVaults.Remove(moneyVaultForView);

            Storage.Save();

        }

        private void ButtonSaveVault_Click(object sender, RoutedEventArgs e)
        {

            if (_vaultToEdit == null)
            {
                if (!AddingNewVault())
                    return;
            }
            else
            {
                if (!EditingVault())
                    return;
            }

            Storage.Save();
            ChangeVisibilityOfGridAddEditVault(false);
            UpdateListOfVaults();
        }

        private bool AddingNewVault()
        {
            var enteredName = TextBoxVaultName.Text.Trim();
            if (_storageInstance.Vaults.Any(item => item.Name == enteredName))
            {
                MessageBox.Show("Такое имя уже используется.");
                return false;
            }

            MoneyVault vaultToAdd = ComboBoxTypeOfMoneyVault.SelectedIndex switch
            {
                (int)MoneyVaultType.Card => new Card(),
                (int)MoneyVaultType.Deposit => new Deposit(),
                (int)MoneyVaultType.OnlyTopDeposit => new OnlyTopDeposit(),
                _ => throw new NotSupportedException()
            };

            vaultToAdd.Name = enteredName;
            var proccessedInitialAmount = ProcessingInitialAmountToAdd(vaultToAdd);
            if (!proccessedInitialAmount)
            {
                return false;
            }

            switch (vaultToAdd)
            {
                case Card:

                    decimal? cashback = VariablesHelper.TryParseIfNotParsedShowMessageBox(TextBoxPercentCashback.Text, "Вы должны вести число в кэшбек");
                    if (cashback == null) return false;

                    ((Card)vaultToAdd).CashBack = cashback.Value;

                    break;
                case Deposit:

                    decimal? vaultPercentDeposit = VariablesHelper.TryParseIfNotParsedShowMessageBox(TextBoxPercent.Text, "Вы должны вести число в процент");
                    if (vaultPercentDeposit == null) return false;

                    ((Deposit)vaultToAdd).Percent = vaultPercentDeposit.Value;

                    var DateOfOpenDeposit = (DateTime)DatePickerDayOfOpenDeposit.DisplayDate;
                    ((Deposit)vaultToAdd).OpenDate = DateOfOpenDeposit;
                    ((Deposit)vaultToAdd).PaymentDay = DateOfOpenDeposit.Day;

                    break;

                default:
                    throw new NotSupportedException();
            }

            if (vaultToAdd.GetType() == typeof(OnlyTopDeposit))
            {
                var dayOfClose = ((OnlyTopDeposit)vaultToAdd).OpenDate.AddMonths((int)SliderHowLong.Value);
                ((OnlyTopDeposit)vaultToAdd).DayOfCloseDeposit = dayOfClose;
            }

            _storageInstance.Vaults.Add(vaultToAdd);
            return true;
        }

        private bool ProcessingInitialAmountToAdd(MoneyVault vaultToAdd)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxInitialAmount.Text))
            {
                var initalAmount = VariablesHelper.TryParseIfNotParsedShowMessageBox(TextBoxInitialAmount.Text, "Вы должны ввести число в начальный баланс");
                if (initalAmount == null) return false;

                if (initalAmount.Value < 0)
                {
                    MessageBox.Show("Начальный баланс должен быть больше либо равен нулю");
                    return false;
                }

                //TODO: default IncomeType
                var incomeType = _storageInstance.IncomeTypes.FirstOrDefault(item => item.Name == "Прочее");
                if (incomeType == null)
                {
                    incomeType = new IncomeType() { Name = "Прочеe" };
                    Storage.Save();
                }

                //TODO: default person
                vaultToAdd.IncreaseBalance(new Income(initalAmount.Value, (DateTime)DatePickerDayOfOpenDeposit.SelectedDate, null, "Начальный ввод баланса", incomeType));

            }
            return true;
        }

        private bool EditingVault()
        {
            var enteredName = TextBoxVaultName.Text.Trim();

            if (_vaultToEdit.Name != enteredName && _storageInstance.Vaults.Any(item => item.Name == enteredName))
            {
                MessageBox.Show("Такое имя уже существует!");
                return false;
            }

            switch (_vaultToEdit)
            {
                case Card:

                    var percentCashback = VariablesHelper.TryParseIfNotParsedShowMessageBox(TextBoxPercentCashback.Text, "Вы должны вести число в кэшбек");
                    if (percentCashback == null) return false;

                    ((Card)_vaultToEdit).CashBack = percentCashback.Value;
                    break;

                case Deposit:

                    var percent = VariablesHelper.TryParseIfNotParsedShowMessageBox(TextBoxPercent.Text, "Вы должны вести число в процент");
                    if (percent == null) return false;

                    ((Deposit)_vaultToEdit).Percent = percent.Value;

                    break;

                default:
                    throw new NotSupportedException();
            }

            _vaultToEdit.Name = enteredName;
            return true;
        }

        private void ButtonBackToListVaults_Click(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityOfGridAddEditVault(false);
        }

        #endregion

        #region Events
        private void ComboBoxTypeOfMoneyVault_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBoxTypeOfMoneyVault.SelectedIndex)
            {
                case (int)MoneyVaultType.Card:
                    GridCard.Visibility = Visibility.Visible;
                    GridDeposit.Visibility = Visibility.Hidden;
                    break;
                case (int)MoneyVaultType.Deposit:
                    GridCard.Visibility = Visibility.Hidden;
                    GridDeposit.Visibility = Visibility.Visible;
                    StackPanelHowLong.Visibility = Visibility.Visible;
                    break;
                case (int)MoneyVaultType.OnlyTopDeposit:
                    GridCard.Visibility = Visibility.Hidden;
                    GridDeposit.Visibility = Visibility.Visible;
                    StackPanelHowLong.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LabelHowLong == null)
            {
                return;
            }

            var totalMonths = (int)SliderHowLong.Value;

            var yearsAndMontshString = VariablesHelper.GetStringYearsAndMonths(totalMonths);

            decimal initialAmount;
            var parsedInitialAmount = decimal.TryParse(TextBoxInitialAmount.Text, out initialAmount);

            decimal percent;
            var parsedPercent = decimal.TryParse(TextBoxPercent.Text, out percent);

            if (parsedInitialAmount && parsedPercent)
            {
                var balanceInEnd = initialAmount;
                for (int i = 1; i <= totalMonths; i++)
                {
                    balanceInEnd += balanceInEnd * ((percent / 100m) / 12m);
                }
                balanceInEnd = Math.Round(balanceInEnd, 2);
                var howMuchEarnString = $" и у вас будет {balanceInEnd}";
                yearsAndMontshString += howMuchEarnString;
            }

            yearsAndMontshString = yearsAndMontshString.Trim();
            LabelHowLong.Content = yearsAndMontshString;

        }


        private void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            UpdateListOfVaults();
        }

        #endregion

    }
}
