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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private gridObject[,] dataGrid;
        public Window1()
        {
            InitializeComponent();
            dataGrid = new gridObject[48,7];
            for(int i = 0;i<dataGrid.GetLength(0);i++)
            {
                for(int j = 0;j<dataGrid.GetLength(1);j++)
                {

                }
            }

        }
        public gridObject[,] setArray(int x,int y)
        {
            dataGrid = new gridObject[x,y];
            return dataGrid;
        }

        private void UniformGrid_SourceUpdated(object sender,DataTransferEventArgs e)
        {
            
        }
    }
}
