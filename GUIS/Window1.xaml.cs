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
using System.Drawing;

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
        private System.Drawing.Color[] arrColor = {System.Drawing.Color.Maroon,System.Drawing.Color.Red,System.Drawing.Color.OrangeRed,System.Drawing.Color.Green,System.Drawing.Color.DarkGreen};
        private int pos = 0;


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
            else if(this.moCalGridContainer.Visibility == Visibility.Visible)
            {
                this.wkCalGridContainer.Visibility = Visibility.Visible;
                this.moCalGridContainer.Visibility = Visibility.Hidden;
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


        private void testSlider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BrushConverter bc = new BrushConverter();
            System.Windows.Media.SolidColorBrush brush = new SolidColorBrush();
            System.Windows.Media.Color clr2;

            if (e.OldValue < e.NewValue && e.NewValue < 5)
                pos = (int)e.NewValue;
            else if (e.OldValue >= e.NewValue && e.NewValue >= 0)
                pos = (int)e.NewValue;

            if(pos>=0 && pos < 5)
                clr2 = System.Windows.Media.Color.FromArgb(arrColor[pos].A, arrColor[pos].R, arrColor[pos].G, arrColor[pos].B);
            
            clr2 = System.Windows.Media.Color.FromArgb(arrColor[pos].A, arrColor[pos].R, arrColor[pos].G, arrColor[pos].B);
            brush.Color = clr2;
            Slider s = (Slider)sender;
            s.Background = brush;
        }


    }
}
