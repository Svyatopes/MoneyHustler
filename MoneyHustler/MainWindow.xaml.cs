using MoneyHustler.AuxiliaryWindows;
using MoneyHustler.Models;
using MoneyHustler.Tabs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoneyHustler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Storage.Load();

        }

        protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
        {
            double mainTabItemWidth;
            double financeTabControlWidth;

            int deltaForMainTab = 3;
            int deltaForFinanceTab = 13;
            foreach (TabItem item in MainTabControl.Items)
            {
                mainTabItemWidth = (this.ActualWidth / MainTabControl.Items.Count) - deltaForMainTab;
                if (mainTabItemWidth < 0) mainTabItemWidth = 0;

                item.Width = mainTabItemWidth;
            }

            foreach (TabItem item in FinanceTabControl.Items)
            {
                financeTabControlWidth = (this.ActualWidth / FinanceTabControl.Items.Count) - deltaForFinanceTab;
                if (financeTabControlWidth < 0) financeTabControlWidth = 0;

                item.Width = financeTabControlWidth;
            }
        }   
    }
}
