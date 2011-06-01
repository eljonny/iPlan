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
using System.Runtime.InteropServices;

//   Author Jonathan Hyry, George Wanjiru, Ryan Soushek
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
        private int teamSize = 0;
        private System.Drawing.Color[] arrColor = { System.Drawing.Color.Maroon, System.Drawing.Color.Red, System.Drawing.Color.OrangeRed, System.Drawing.Color.Green, System.Drawing.Color.DarkGreen };
        private int pos = 0;

        public Window1()
        {

            InitializeComponent();
            gridContents.Cast<gridObject>();


        }

        private void file_quit(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void labelTxtBoxFill(object sender, ToolTipEventArgs e)
        {
            /*if (ed)
            {
                Label clicked = (System.Windows.Controls.Label)sender;
                if (startTime.IsFocused)
                    startTime.Text = clicked.Content.ToString();
                else if (EndTime.IsFocused)
                    EndTime.Text = clicked.Content.ToString();
            }
            else if (ed == false && (startTime.IsFocused || EndTime.IsFocused))
                MessageBox.Show("Please Enable Edit Mode to change the schedule.");*/
        }
        
        private void setObjSlot(gridObject o, ArrayList g)
        {
            if (ed)
            {
                g.Insert(o.getDay(), o);
                g.RemoveAt(o.getDay() + 1);
            }
            else
                MessageBox.Show("Please Enable Edit Mode to change the schedule.");
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            iPlAbout aboutWin = new iPlAbout();
            aboutWin.ShowDialog();
        }

        private void chgViewMode(object sender, RoutedEventArgs e)
        {
            if (this.wkCalGridContainer.Visibility == Visibility.Visible)
            {
                this.wkCalGridContainer.Visibility = Visibility.Hidden;
                this.moCalGridContainer.Visibility = Visibility.Visible;
            }
            else if (this.moCalGridContainer.Visibility == Visibility.Visible)
            {
                this.wkCalGridContainer.Visibility = Visibility.Visible;
                this.moCalGridContainer.Visibility = Visibility.Hidden;
            }
        }

        private void enterOrLeaveEditMode(object sender, RoutedEventArgs e)
        {
            ed = !ed;
        }

        private void startEmail(object sender, RoutedEventArgs e)
        {
            Process.Start("mail.exe");
        }

        private String[] getNamesOfCurrentTeam()
        {
            int i = 0;
            String[] participNames = new String[teamSize];
            foreach(gridObject obj in gridContents)
            {
                participNames[i] = obj.getName();
                i++;
            }
            return participNames;
        }

        private void notifyListCall()
        {
            //checkedList navWinPg = new checkedList();

        }

        public string toString()
        {
            return gridContents.ToString();
        }

        private void testSlider2_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (ed)
            {
                BrushConverter bc = new BrushConverter();
                System.Windows.Media.SolidColorBrush brush = new SolidColorBrush();
                System.Windows.Media.Color clr2;

                if (e.OldValue < e.NewValue && e.NewValue < 5)
                    pos = (int)e.NewValue;
                else if (e.OldValue >= e.NewValue && e.NewValue >= 0)
                    pos = (int)e.NewValue;

                if (pos >= 0 && pos < 5)
                    clr2 = System.Windows.Media.Color.FromArgb(arrColor[pos].A, arrColor[pos].R, arrColor[pos].G, arrColor[pos].B);

                clr2 = System.Windows.Media.Color.FromArgb(arrColor[pos].A,
                    arrColor[pos].R, arrColor[pos].G, arrColor[pos].B);
                brush.Color = clr2;
                Slider s = (Slider)sender;
                brush.Opacity = 0.3;
                s.Background = brush;
            }
            else
            {
                Slider sl = (Slider)sender;
                sl.Value = 0;
            }
            /*
            clr2 = System.Windows.Media.Color.FromArgb(arrColor[pos].A,
                arrColor[pos].R, arrColor[pos].G, arrColor[pos].B);
            brush.Color = clr2;
            Slider s = (Slider)sender;
            brush.Opacity = 0.3;
            s.Background = brush;
             */
        }

        private void resetCalButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Slider tb in FindVisualChildren<Slider>(this))
            {
                tb.Value = 0;
            }
        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void EditRow(object sender, MouseEventArgs e)
        {
            Label l = (Label)sender;
            int row = (int)l.GetValue(Grid.RowProperty);
            Slider[] s = { testSlider245tester0, testSlider245tester1,
                             testSlider245tester2, testSlider245tester3,
                             testSlider245tester4 , testSlider245tester5,
                             testSlider245tester6, testSlider245tester7,
                             testSlider245tester8,testSlider245tester9,
                             testSlider245tester10,testSlider245tester11,
                             testSlider245tester12,testSlider245tester13,
                             testSlider245tester14,testSlider245tester15,
                             testSlider245tester16,testSlider245tester17,
                             testSlider245tester18,testSlider245tester19,
                             testSlider245tester20,testSlider245tester21,
                             testSlider245tester22,testSlider245tester23,
                             testSlider245tester};
            if (row >= 0 && row < s.Length)
                s[row].Visibility = Visibility.Visible;
                
        }

        private void testSlider245tester_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (ed)
            {
                Slider l = sender as Slider;
                int row = (int)l.GetValue(Grid.RowProperty);
                int x = 1;
                foreach (Slider tb in FindVisualChildren<Slider>(theGrid))
                {

                    if (x == row || x == row + 24 || x == row + 48 || x == row + 72 ||
                        x == row + 96)
                        tb.Value = l.Value;

                    x++;

                }
            }
            else
            {
                Slider sl = (Slider)sender;
                sl.Value = 0;
            }

        }


        private void testSlider245tester_MouseLeave(object sender, MouseEventArgs e)
        {
            Slider obj = (Slider)sender;
            obj.Visibility = Visibility.Hidden;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Customizable cs = new Customizable();
            cs.ShowDialog();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Team t = new Team();
            t.ShowDialog();
        }

        private void membersCalButton_Click(object sender, RoutedEventArgs e)
        {
            Team t = new Team();
            t.ShowDialog();
        }

    }
}
