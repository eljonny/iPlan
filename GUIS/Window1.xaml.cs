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
using System.Diagnostics;
//   Author Jonathan Hyry & George Wanjiru
namespace GUIProj1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private ArrayList
            gridContents = new ArrayList();
        private bool ed = true;

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
            if(ed)
            {
                Label clicked = (System.Windows.Controls.Label)sender;
                if(startTime.IsFocused)
                    startTime.Text = clicked.Content.ToString();
                else if(EndTime.IsFocused)
                    EndTime.Text = clicked.Content.ToString();
            }
            else if(ed == false &&(startTime.IsFocused || EndTime.IsFocused))
                MessageBox.Show("Please Enable Edit Mode to change the schedule.");
        }

        private void setObjSlot(gridObject o, ArrayList g)
        {
            if(ed)
            {
                g.Insert(o.getDay(), o);
                g.RemoveAt(o.getDay() + 1);
            }
            else
                MessageBox.Show("Please Enable Edit Mode to change the schedule.");
        }
        
        private void about_Click(object sender,RoutedEventArgs e)
        {
            iPlAbout aboutWin = new iPlAbout();
            aboutWin.ShowDialog();
        }

        private void chgViewMode(object sender,RoutedEventArgs e)
        {
            if(this.wkCalGridContainer.Visibility == Visibility.Visible)
            {
                this.wkCalGridContainer.Visibility = Visibility.Hidden;
                this.moCalGridContainer.Visibility = Visibility.Visible;
            }
            else
            {
                this.moCalGridContainer.Visibility = Visibility.Hidden;
                this.wkCalGridContainer.Visibility = Visibility.Visible;
            }
        }

        private void enterOrLeaveEditMode(object sender,RoutedEventArgs e)
        {
            ed = !ed;
        }

        private void startEmail(object sender,RoutedEventArgs e)
        {
            Process.Start("mail.exe");
        }

        public string toString()
        {
            return gridContents.ToString();
        }
    }
}
