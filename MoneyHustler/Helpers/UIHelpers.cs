using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MoneyHustler.Helpers
{
    public static class UIHelpers
    {
        public static void SetButtonEnabledAndVisibility(Button button, bool enabled)
        {
            if (enabled)
            {
                button.Visibility = Visibility.Visible;
                button.IsEnabled = true;
            }
            else
            {
                button.Visibility = Visibility.Hidden;
                button.IsEnabled = false;
            }
        }

        public static void ChangeWidthGridColumns(ObservableCollection<ColumnDefinition> columns, double stars)
        {
            foreach (var item in columns)
            {
                item.Width = new GridLength(stars, GridUnitType.Star);
            }
        }

        public static void SetComboBoxItemsSourceAndSelectZeroIndex(ComboBox comboBox, IEnumerable source)
        {
            comboBox.ItemsSource = source;
            comboBox.SelectedIndex = 0;
        }

        public static void ChangeStackPanelVisibilityAndEnabled(bool isEnableAndVisible, StackPanel stackPanel)
        {
            if (isEnableAndVisible)
                stackPanel.Visibility = Visibility.Visible;
            else
                stackPanel.Visibility = Visibility.Hidden;

            stackPanel.IsEnabled = isEnableAndVisible;
        }
    }
}
