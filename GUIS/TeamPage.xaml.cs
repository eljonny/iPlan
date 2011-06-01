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
using System.Diagnostics;

namespace GUIProj1
{
    /// <summary>
    /// Interaction logic for TeamPage.xaml
    /// </summary>
    public partial class TeamPage : Page
    {
        public TeamPage()
        {

            InitializeComponent();

           

        }



        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {



        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This feature has not been implemented yet");

            ComboBox box = title;
            box.SelectedIndex = 0;
               

            
        
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mail.exe");
        }
        

    }
}
