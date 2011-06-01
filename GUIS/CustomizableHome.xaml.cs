using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUIProj1
{
    /// <summary>
    /// Interaction logic for CustomizableHome.xaml
    /// </summary>
    public partial class CustomizableHome : Page
    {
        public CustomizableHome()
        {
            InitializeComponent();
        }

      
        private void textblock1_DragOver(object sender, DragEventArgs e)
        {
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            SettingPage sp = new SettingPage();
            this.NavigationService.Navigate(sp);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            LanguagePage lp = new LanguagePage();
            this.NavigationService.Navigate(lp);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ThemeAppearancePage tp = new ThemeAppearancePage();
            this.NavigationService.Navigate(tp);
        }
    }
}
