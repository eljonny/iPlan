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
using System.Collections;

namespace GUIProj1
{
    /// <summary>
    /// Interaction logic for TeamPage.xaml
    /// </summary>
    public partial class TeamPage : Page
    {
        ArrayList teamMembersInfo = new ArrayList();

        public TeamPage()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Image img = new Image();
            img.Width = 200;
            img.Height = 150;
            img.Source = new BitmapImage(new Uri("defaultcontact.bmp", UriKind.Relative));
            Grid g = (Grid)sender;
            //MessageBox.Show("ss");
            g.Children.Add(img);
            //iPlan_Main w = new iPlan_Main();
            //teamMembersInfo = LoadListBoxData();
        }

        private ArrayList LoadListBoxData()
        {
            ArrayList returned = new ArrayList();
            return returned;

        }
    }
}
