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
    /// Interaction logic for TimeBlock.xaml
    /// </summary>
    /// Author: Jonathan Hyry
    public partial class TimeBlock : Window
    {
        string[] content = new string[100];
        double[,] probData = new double[100,2];
        private gridObject gO;
        public TimeBlock(int pxDiffL,int pxDiffT)
        {
            InitializeComponent();
            gO = new gridObject
                (0,0,pxDiffL,pxDiffT,"08:00","17:00",true,content,
                "emp",System.DateTime.Now.ToString(),probData);
        }

        private void dragMove(object sender,MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch(InvalidOperationException invOpEx)
            {
                MessageBox.Show(invOpEx.ToString());
                OnMouseLeftButtonUp(e);
            }
        }

        /*
        private Point coordsForSnap()
        {
            
        }*/

        private void removeBlock_Click(object sender,RoutedEventArgs e)
        {
            this.Close();
        }

        public gridObject getGO()
        {
            return gO;
        }
}
