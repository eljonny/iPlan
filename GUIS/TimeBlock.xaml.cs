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
    public partial class TimeBlock : Window
    {
        public TimeBlock()
        {
            InitializeComponent();
            gridObject gO = new gridObject();
            blockCanvas = gO.getCanvas();
        }

        private void dragMove(object sender,MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch(InvalidOperationException invOpEx)
            {
                OnMouseLeftButtonUp(e);
            }
        }

        private void removeBlock_Click(object sender,RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
