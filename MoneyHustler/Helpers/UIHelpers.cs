using System;
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

        public static void ChangeVisibilityColumns(bool visible, ObservableCollection<ColumnDefinition> columns)
        {
            if (visible)
            {
                foreach (var item in columns)
                {
                    item.Width = new GridLength(20, GridUnitType.Star);
                }

            }
            else
            {
                foreach (var item in columns)
                {
                    item.Width = new GridLength(0, GridUnitType.Star);
                }

            }
        }
    }
}
