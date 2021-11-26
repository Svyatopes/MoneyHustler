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
            public MoneyVault Vault { get; set; }
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

            ComboBoxTypeOfMoneyVault.SelectedIndex = 0;
            Slider_ValueChanged(null, null);

        }



        private void UpdateListOfVaults()
        {
            _listMoneyVaults.Clear();
            foreach (var vault in _storageInstance.Vaults)
            {
                MoneyVaultForView vaultView = new MoneyVaultForView() { Name = vault.Name, Balance = vault.GetBalance(), Vault = vault };

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
                GridColumnAddEditMoneyVault.Width = new GridLength(1, GridUnitType.Star);
                GridListOfVaults.IsEnabled = false;
            }
            else
            {
                GridColumnAddEditMoneyVault.Width = new GridLength(0, GridUnitType.Pixel);
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
                    ComboBoxTypeOfMoneyVault.SelectedIndex = 0;
                    TextBoxPercentCashback.Text = ((Card)_vaultToEdit).CashBack.ToString();
                    break;
                case OnlyTopDeposit:
                    ComboBoxTypeOfMoneyVault.SelectedIndex = 2;
                    TextBoxPercent.Text = ((OnlyTopDeposit)_vaultToEdit).Percent.ToString();
                    break;
                case Deposit:
                    ComboBoxTypeOfMoneyVault.SelectedIndex = 1;
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

            if (moneyVaultForView.Vault.Incomes.Any() || moneyVaultForView.Vault.Expenses.Any())
            {
                MessageBox.Show("This vault have some incomes or expenses, so you can't remove it.");
                return;
            }

            _storageInstance.Vaults.Remove(moneyVaultForView.Vault);
            _listMoneyVaults.Remove(moneyVaultForView);

            Storage.Save();

        }

        private void ButtonSaveVault_Click(object sender, RoutedEventArgs e)
        {


            if (_vaultToEdit == null)
            {
                var enteredName = TextBoxVaultName.Text.Trim();
                if (_storageInstance.Vaults.Any(item => item.Name == enteredName))
                {
                    MessageBox.Show("Такое имя уже используется.");
                    return;
                }

                MoneyVault vaultToAdd = ComboBoxTypeOfMoneyVault.SelectedItem switch
                {
                    "Счет" => new Card(),
                    "Вклад" => new Deposit(),
                    "Вклад без снятия" => new OnlyTopDeposit(),
                    _ => throw new NotSupportedException()
                };


                if (!string.IsNullOrWhiteSpace(TextBoxInitialAmount.Text))
                {
                    decimal initalAmount;
                    var parsedInitialAmount = decimal.TryParse(TextBoxInitialAmount.Text, out initalAmount);
                    if (!parsedInitialAmount)
                    {
                        MessageBox.Show("Вы должны вести число в начальный баланс");
                        return;
                    }

                    if (initalAmount > 0)
                    {
                        var incomeType = _storageInstance.IncomeTypes.FirstOrDefault(item => item.Name == "Прочее");
                        if (incomeType == null)
                        {
                            incomeType = new IncomeType() { Name = "Прочеe" };
                            Storage.Save();
                        }

                        //TODO: default person
                        vaultToAdd.IncreaseBalance(new Income(initalAmount, (DateTime)DatePickerDayOfOpenDeposit.SelectedDate, null, "Начальный ввод баланса", incomeType));
                    }

                }

                vaultToAdd.Name = enteredName;



                switch (vaultToAdd)
                {
                    case Card:

                        decimal cashback;
                        var parsedCashback = decimal.TryParse(TextBoxPercentCashback.Text, out cashback);

                        if (!parsedCashback)
                        {
                            MessageBox.Show("Вы должны вести число в кэшбек");
                            return;
                        }

                        ((Card)vaultToAdd).CashBack = cashback;

                        break;
                    case Deposit:

                        decimal vaultPercentDeposit;
                        var parsedVaultPercentDeposit = decimal.TryParse(TextBoxPercent.Text, out vaultPercentDeposit);
                        if (!parsedVaultPercentDeposit)
                        {
                            MessageBox.Show("Вы должны вести число в процент");
                            return;
                        }
                        ((Deposit)vaultToAdd).Percent = vaultPercentDeposit;


                        var DateOfOpenDeposit = (DateTime)DatePickerDayOfOpenDeposit.DisplayDate;
                        ((Deposit)vaultToAdd).OpenDate = DateOfOpenDeposit;
                        ((Deposit)vaultToAdd).PaymentDay = DateOfOpenDeposit.Day;

                        break;

                    default:
                        throw new NotSupportedException();
                        break;
                }

                if (vaultToAdd.GetType() == typeof(OnlyTopDeposit))
                {
                    var dayOfClose = ((OnlyTopDeposit)vaultToAdd).OpenDate.AddMonths((int)SliderHowLong.Value);
                    ((OnlyTopDeposit)vaultToAdd).DayOfCloseDeposit = dayOfClose;
                }

                _storageInstance.Vaults.Add(vaultToAdd);
            }
            else
            {
                var enteredName = TextBoxVaultName.Text.Trim();
                if (_vaultToEdit.Name != enteredName && _storageInstance.Vaults.Any(item => item.Name == enteredName))
                {
                    MessageBox.Show("Такое имя уже существует!");
                    return;
                }

                switch (_vaultToEdit)
                {
                    case Card:

                        decimal percentCashback;
                        var parsedPercentCashback = decimal.TryParse(TextBoxPercentCashback.Text, out percentCashback);
                        if (!parsedPercentCashback)
                        {
                            MessageBox.Show("Вы должны вести число в кэшбек");
                            return;
                        }
                        ((Card)_vaultToEdit).CashBack = percentCashback;

                        break;

                    case Deposit:

                        decimal percent;
                        var parsedPercent = decimal.TryParse(TextBoxPercent.Text, out percent);
                        if (!parsedPercent)
                        {
                            MessageBox.Show("Вы должны вести число в процент");
                            return;
                        }
                        ((Deposit)_vaultToEdit).Percent = percent;

                        break;

                    default:
                        throw new NotSupportedException();
                        break;
                }

                _vaultToEdit.Name = enteredName;

            }


            Storage.Save();
            ChangeVisibilityOfGridAddEditVault(false);
            UpdateListOfVaults();
        }

        private void ButtonBackToListVaults_Click(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityOfGridAddEditVault(false);
        }

        #endregion



        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LabelHowLong == null)
            {
                return;
            }

            var totalMonths = (int)SliderHowLong.Value;

            var years = totalMonths / 12;
            var months = totalMonths % 12;

            var firstPartStrHowLong = years switch
            {
                0 => string.Empty,
                1 => "1 год",
                2 => "2 года",
                3 => "3 года",
                4 => "4 года",
                5 => "5 лет",
                6 => "6 лет",
                7 => "7 лет",
                8 => "8 лет",
                9 => "9 лет",
                10 => "10 лет",
                _ => string.Empty
            };

            var secondPartStrHowLong = months switch
            {
                0 => string.Empty,
                1 => "1 месяц",
                2 => "2 месяца",
                3 => "3 месяца",
                4 => "4 месяца",
                5 => "5 месяцев",
                6 => "6 месяцев",
                7 => "7 месяцев",
                8 => "8 месяцев",
                9 => "9 месяцев",
                10 => "10 месяцев",
                11 => "11 месяцев",
                _ => string.Empty,
            };

            var resultString = firstPartStrHowLong + " " + secondPartStrHowLong;

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
                var howMuchEarnStr = $" и у вас будет {balanceInEnd}";
                resultString += howMuchEarnStr;
            }

            resultString = resultString.Trim();
            LabelHowLong.Content = resultString;

        }

        #region Events
        private void ComboBoxTypeOfMoneyVault_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBoxTypeOfMoneyVault.SelectedItem)
            {
                case "Счет":
                    GridCard.Visibility = Visibility.Visible;
                    GridDeposit.Visibility = Visibility.Hidden;
                    break;
                case "Вклад":
                    GridCard.Visibility = Visibility.Hidden;
                    GridDeposit.Visibility = Visibility.Visible;
                    StackPanelHowLong.Visibility = Visibility.Visible;
                    break;
                case "Вклад без снятия":
                    GridCard.Visibility = Visibility.Hidden;
                    GridDeposit.Visibility = Visibility.Visible;
                    StackPanelHowLong.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        #endregion

        private void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            UpdateListOfVaults();
        }
    }
}
