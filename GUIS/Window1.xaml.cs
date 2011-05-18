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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private ArrayList
            gridContents = new ArrayList();

        public Window1()
        {
            InitializeComponent();
            gridContents.Cast<gridObject>();
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

        private void setObjSlotWk(gridObject o, ArrayList g)
        {
            g.Insert(o.getDayWk(), o);
            g.RemoveAt(o.getDayWk() + 1);
        }

        private void setObjSlotMo(gridObject o, ArrayList g)
        {
            g.Insert(o.getDayMo(),o);
            g.RemoveAt(o.getDayMo()+1);
        }
        
        private void about_Click(object sender,RoutedEventArgs e)
        {
            iPlAbout aboutWin = new iPlAbout();
            aboutWin.ShowDialog();
        }

        public string toString()
        {
            return gridContents.ToString();
        }
    }
}
