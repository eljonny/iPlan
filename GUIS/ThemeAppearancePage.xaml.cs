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
    /// Interaction logic for ThemeAppearancePage.xaml
    /// </summary>
    public partial class ThemeAppearancePage : Page
    {
        public ThemeAppearancePage()
        {
            InitializeComponent();
        }

      

        private void label1_MouseEnter(object sender, MouseEventArgs e)
        {
            label1.Foreground = Brushes.Blue;
        }

        private void label1_MouseLeave(object sender, MouseEventArgs e)
        {
            label1.Foreground = Brushes.Black;
        }
        private void label2_MouseEnter(object sender, MouseEventArgs e)
        {
            label2.Foreground = Brushes.Blue;
        }

        private void label2_MouseLeave(object sender, MouseEventArgs e)
        {
            label2.Foreground = Brushes.Black;
        }
        private void label3_MouseEnter(object sender, MouseEventArgs e)
        {
            label3.Foreground = Brushes.Blue;
        }

        private void label3_MouseLeave(object sender, MouseEventArgs e)
        {
            label3.Foreground = Brushes.Black;
        }

        

        
    }
}
