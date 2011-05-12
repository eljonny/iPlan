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
        private gridObject[,] 
            gridContents = new gridObject[48,7];

        public Window1()
        {
            InitializeComponent();
        }

        private void file_quit(object sender,EventArgs e)
        {
            this.Close();
        }
        private void labelTxtBoxFill(object sender,ToolTipEventArgs e)
        {
            Label clicked = (System.Windows.Controls.Label)sender;
            if(startTime.IsFocused)
                startTime.Text = clicked.Content.ToString();
            else if(EndTime.IsFocused)
                EndTime.Text = clicked.Content.ToString();
        }
    }
}
