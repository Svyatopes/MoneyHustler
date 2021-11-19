using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
    /// Interaction logic for WindowAddEditMoneyVault.xaml
    /// </summary>
    public partial class WindowAddEditMoneyVault : Window
    {



        private ObservableCollection<string> moneyVaulTypes;




        public WindowAddEditMoneyVault()
        {
            InitializeComponent();

            moneyVaulTypes = new ObservableCollection<string>();
            moneyVaulTypes.Add("Счет");
            moneyVaulTypes.Add("Вклад");
            moneyVaulTypes.Add("Вклад без снятия");
            ComboBoxTypeOfMoneyVault.ItemsSource = moneyVaulTypes;
            ComboBoxTypeOfMoneyVault.SelectedIndex = 0;
            Slider_ValueChanged(null, null);
        }

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
                for(int i = 1; i <= totalMonths; i++)
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
    }
}
