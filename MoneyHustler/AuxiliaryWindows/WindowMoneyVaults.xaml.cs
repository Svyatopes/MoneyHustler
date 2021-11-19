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
using System.Windows.Shapes;

namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary>
    /// Interaction logic for WindowMoneyVaults.xaml
    /// </summary>
    public partial class WindowMoneyVaults : Window
    {
        public class MoneyVaultForView
        {
            public string Name { get; set; }
            public decimal Balance { get; set; }
            public string TypeName { get; set; }
            public MoneyVault Vault { get; set; }
        }

        private ObservableCollection<MoneyVaultForView> listMoneyVaults;

        public WindowMoneyVaults()
        {
            InitializeComponent();

            listMoneyVaults = new ObservableCollection<MoneyVaultForView>();
            foreach (var vault in Storage.Vaults)
            {
                MoneyVaultForView vaultView = new MoneyVaultForView() { Name = vault.Name, Balance = vault.GetBalance(), Vault = vault};

                vaultView.TypeName = vault switch
                {
                    Card => "Счет",
                    OnlyTopDeposit => "Вклад без снятия",
                    Deposit => "Вклад",
                    _ => string.Empty,
                };

                listMoneyVaults.Add(vaultView);
            }
            listViewForVaults.ItemsSource = listMoneyVaults;

        }

        private void ButtonAddMoneyVault_Click(object sender, RoutedEventArgs e)
        {
            WindowAddEditMoneyVault windowAddEditMoneyVault = new WindowAddEditMoneyVault();
            windowAddEditMoneyVault.ShowDialog();
        }

        private void ButtonEditVault_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonRemoveVault_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var moneyVaultForView = (MoneyVaultForView)button.DataContext;

            if(moneyVaultForView.Vault.Incomes.Any() || moneyVaultForView.Vault.Expenses.Any())
            {
                MessageBox.Show("This vault have some incomes or expenses, so you can't remove it.");
                return;
            }

            Storage.Vaults.Remove(moneyVaultForView.Vault);
            listMoneyVaults.Remove(moneyVaultForView);

            Storage.Save();

        }
    }
}
