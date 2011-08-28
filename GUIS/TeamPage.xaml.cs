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
    public partial class TeamPage : Window
    {
<<<<<<< .mine
        Uri p1, p2, p3;
=======
       
>>>>>>> .r36
        public TeamPage()
        {

            InitializeComponent();
            p1 = new Uri("person1.jpg", UriKind.RelativeOrAbsolute);
            p2 = new Uri("person2.png", UriKind.RelativeOrAbsolute);
            p3 = new Uri("person3.jpg", UriKind.RelativeOrAbsolute);

        }



        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
<<<<<<< .mine
            Image img = new Image();
            img.Width = 200;
            img.Height = 150;
            img.Source = new BitmapImage(p2);
=======
            //Image img = new Image();
            //img.Width = 200;
            //img.Height = 150;
            //img.Source = new BitmapImage(new Uri("person1.jpg"));
>>>>>>> .r36

            //Grid g = (Grid)sender;
            //MessageBox.Show("ss");
            //g.Children.Add(img);
            Window1 w = new Window1();
           // teamMembersInfo.ItemsSource = LoadListBoxData();
        }

        private ArrayList LoadListBoxData()
        {

            Window1 w = new Window1();

            return w.team;

        }
    }
}
