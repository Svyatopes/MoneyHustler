using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MoneyHustler.Helpers
{
    public class UIHelpers
    {
        public void SetButtonEnabledAndVisibility(Button button, bool enabled) 
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
    }
}
