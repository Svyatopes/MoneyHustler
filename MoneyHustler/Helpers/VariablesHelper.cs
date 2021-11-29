using System.Windows;

namespace MoneyHustler.Helpers
{
    public static class VariablesHelper
    {
        public static decimal? TryParseIfNotParsedShowMessageBox(string textToParse, string textOfError)
        {
            decimal result;
            var parsedVaultPercentDeposit = decimal.TryParse(textToParse, out result);
            if (!parsedVaultPercentDeposit)
            {
                MessageBox.Show(textOfError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return result;
        }

        public static string GetStringYearsAndMonths(int totalMonths)
        {
            var years = totalMonths / 12;
            var months = totalMonths % 12;

            var yearsLastDigit = years % 10;
            var firstPartStrHowLong = yearsLastDigit switch
            {
                0 => string.Empty,
                1 => $"{years} год",
                <= 4 => $"{years} года",
                _ => $"{years} лет"
            };

            var secondPartStrHowLong = months switch
            {
                0 => string.Empty,
                1 => "1 месяц",
                <= 4 => $"{months} месяца",
                _ => $"{months} месяцев"
            };

            return firstPartStrHowLong + " " + secondPartStrHowLong;
        }
    }
}
