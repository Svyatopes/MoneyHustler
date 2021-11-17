using System;
using System.Collections.Generic;
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
using MoneyHustler.Models;

namespace MoneyHustler.AuxiliaryWindows
{
    /// <summary>
    /// Interaction logic for WindowExpenses.xaml
    /// </summary>
    public partial class WindowExpenses : Window
    {
        List<Button> editButtons = new List<Button>();

        private int indexName = 1;

        public WindowExpenses()
        {
            InitializeComponent();
            
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)e.OriginalSource).Content == "Edit")
            {
                ((Button)e.OriginalSource).Content = "Save";
                if(((Button)e.OriginalSource).Parent is StackPanel)
                {
                    StackPanel sp = (StackPanel)((Button)e.OriginalSource).Parent;

                    foreach (UIElement item in sp.Children)
                    {
                        item.IsEnabled = true;
                    }
                    
                }
            }
            else if (((Button)e.OriginalSource).Content == "Save")
            {
                ((Button)e.OriginalSource).Content = "Edit";
                if (((Button)e.OriginalSource).Parent is StackPanel)
                {
                    StackPanel sp = (StackPanel)((Button)e.OriginalSource).Parent;

                    foreach (UIElement item in sp.Children)
                    {
                        if (!(item is Button))
                        {
                            item.IsEnabled = false;
                        }
                        
                    }

                }
            }


        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            //string contentString = DatePick.SelectedDate + "                     ";
            //contentString += Amount.Text + "                     ";
            //contentString += Person.Text + "                     ";
            //contentString += Vault.Text + "                     ";

            TextBox amountBox = new TextBox { Text = Amount.Text, IsEnabled = false };
            TextBox personBox = new TextBox { Text = Person.Text, IsEnabled = false };
            TextBox vaultBox = new TextBox { Text = Vault.Text, IsEnabled = false };
            DatePicker datePicker = new DatePicker { SelectedDate = DatePick.SelectedDate, IsEnabled = false };

            StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            stackPanel.Children.Add(amountBox);
            stackPanel.Children.Add(personBox);
            stackPanel.Children.Add(vaultBox);
            stackPanel.Children.Add(datePicker);
            editButtons.Add(new Button { Content = "Edit" });
            editButtons[editButtons.Count - 1].Click += Edit_Click;
            stackPanel.Children.Add(editButtons[editButtons.Count - 1]);

            listBox1.Items.Add(stackPanel);
            //listBox1.Items.Add(delete);

        }

        
    }
}
