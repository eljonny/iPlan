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
<<<<<<< .mine
        private ArrayList
        gridContents = new ArrayList();
        private bool ed = true, view = false;
=======
        public ArrayList team = new ArrayList();

        private ArrayList gridContents = new ArrayList();

        private bool ed = true;

>>>>>>> .r36
        private int teamSize = 0;
<<<<<<< .mine
=======
        /*private System.Drawing.Color[] arrColor =
            { System.Drawing.Color.Maroon, System.Drawing.Color.Red,
                System.Drawing.Color.OrangeRed, System.Drawing.Color.Green,
                System.Drawing.Color.DarkGreen };*/
>>>>>>> .r36

<<<<<<< .mine
        private System.Drawing.Color[] arrColor =
        { System.Drawing.Color.Maroon, System.Drawing.Color.Red,
            System.Drawing.Color.OrangeRed, System.Drawing.Color.Olive,
            System.Drawing.Color.DarkGreen};
=======

        private System.Drawing.Color[] arrColor = { System.Drawing.Color.Maroon, System.Drawing.Color.Red, System.Drawing.Color.OrangeRed, System.Drawing.Color.Olive, System.Drawing.Color.DarkGreen };


>>>>>>> .r36
        private int pos = 0;
        private LinkedList<TimeBlock> timeBlocks = new LinkedList<TimeBlock>();
        private Canvas temp;

        public Window1()
        {

            InitializeComponent();
            gridContents.Cast<gridObject>();
            team.Cast<Member>();
        }

        private void file_quit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void labelTxtBoxFill(object sender, ToolTipEventArgs e)
        {
            if (ed)
            {
                Label clicked = (System.Windows.Controls.Label)sender;
                if (startTime.IsFocused)
                    startTime.Text = clicked.Content.ToString();
                else if (EndTime.IsFocused)
                    EndTime.Text = clicked.Content.ToString();
            }
            else if (ed == false && (startTime.IsFocused || EndTime.IsFocused))
                MessageBox.Show("Please Enable Edit Mode to change the schedule.");
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
                view = true;
            }
            else if (this.moCalGridContainer.Visibility == Visibility.Visible)
            {
                this.wkCalGridContainer.Visibility = Visibility.Visible;
                this.moCalGridContainer.Visibility = Visibility.Hidden;
                view = false;
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
            foreach (gridObject obj in gridContents)
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

        private void addGObj()
        {
            string[] contents = new string[3];
<<<<<<< .mine
            double[,] probData = new double[100,2];
            probData[0,1] = .4;probData[1,1] = .95;
            contents[0] = "James";contents[1]="Garner";contents[2]="Software Engineer";
=======
            double[] probData = new double[2];
            probData[0] = .4; probData[1] = .95;
            contents[0] = "James"; contents[1] = "Garner"; contents[2] = "Software Engineer";
>>>>>>> .r36
            gridObject tMTimeBlock =
                new gridObject
                    (1, 2, "00:00", "07:00", true, contents, "emp",
                    DateTime.Today.ToString(), probData);
            temp = tMTimeBlock.getCanvas();
        }

        private void populateGrid()
        {
<<<<<<< .mine
            foreach(TimeBlock c in timeBlocks)
=======
            foreach (Canvas c in timeBlocks)
>>>>>>> .r36
            {
                c.BringIntoView();
                this.AddVisualChild(c);
            }
        }

<<<<<<< .mine
        /* Ryan*/
=======
        /* Ryan
         * PLEASE COMMENT YOUR CODE.*/
>>>>>>> .r36
        private void testSlider2_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if(ed)
            {
                BrushConverter bc = new BrushConverter();
                System.Windows.Media.SolidColorBrush brush = new SolidColorBrush();
                System.Windows.Media.Color clr2;

                if(e.OldValue < e.NewValue && e.NewValue < 5)
                    pos = (int)e.NewValue;
                else if(e.OldValue >= e.NewValue && e.NewValue >= 0)
                    pos = (int)e.NewValue;

                if(pos >= 0 && pos < 5)
                    clr2 =
                        System.Windows.Media.Color.FromArgb(arrColor[pos].A,
                        arrColor[pos].R,arrColor[pos].G,arrColor[pos].B);

                clr2 = System.Windows.Media.Color.FromArgb(arrColor[pos].A,
                    arrColor[pos].R,arrColor[pos].G,arrColor[pos].B);
                brush.Color = clr2;
                Slider s = (Slider)sender;
                //brush.Opacity = 0.3;
                s.Background = brush;
            }
            else
            {
                Slider sl = (Slider)sender;
                sl.Value = 0;
            }
<<<<<<< .mine
=======
//<<<<<<< .mine
//            clr2 = System.Windows.Media.Color.FromArgb(arrColor[pos].A,
//                arrColor[pos].R, arrColor[pos].G, arrColor[pos].B);
//            brush.Color = clr2;
//            Slider s = (Slider)sender;
//            brush.Opacity = 0.3;
//            s.Background = brush;
>>>>>>> .r36
        }
<<<<<<< .mine
=======
        
>>>>>>> .r36

<<<<<<< .mine
=======




        /* Ryan
         * PLEASE COMMENT YOUR CODE.*/
>>>>>>> .r36
        private void resetCalButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Slider tb in FindVisualChildren<Slider>(this))
            {
                tb.Value = 0;
            }
        }
<<<<<<< .mine
=======
         
>>>>>>> .r36

<<<<<<< .mine
=======
        /* Ryan
         * PLEASE COMMENT YOUR CODE. */
>>>>>>> .r36
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
<<<<<<< .mine
        /*
=======
        

        /* Ryan
         * PLEASE COMMENT YOUR CODE.*/
>>>>>>> .r36
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
<<<<<<< .mine
        */
=======
         

         
        /* * PLEASE COMMENT YOUR CODE */
>>>>>>> .r36
        private void testSlider245tester_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (ed)
            {
                Slider l = sender as Slider;
                int row = (int)l.GetValue(Grid.RowProperty);
                int x = 1;
                foreach (Slider tb in FindVisualChildren<Slider>(weekGrid))
                {

                    if (x == row || x == row + 24 || x == row + 48 || x == row + 72 ||
                        x == row + 96)
                        tb.Value = l.Value;
                      Console.Write(tb.Value);
                    x++;

                }
            }
            else
            {
                Slider sl = (Slider)sender;
                sl.Value = 0;
            }
            
        }
<<<<<<< .mine
=======
         
>>>>>>> .r36

<<<<<<< .mine
=======
       
        /* Ryan
         * PLEASE COMMENT YOUR CODE*/
>>>>>>> .r36
        private void testSlider245tester_MouseLeave(object sender, MouseEventArgs e)
        {
            Slider obj = (Slider)sender;
            obj.Visibility = Visibility.Hidden;
        }
<<<<<<< .mine

=======
         
>>>>>>> .r36
        //George
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Customizable cs = new Customizable();
            cs.ShowDialog();
        }

<<<<<<< .mine
        //Ryan
        private void membersCalButton_Click(object sender, RoutedEventArgs e)
=======
        private void membersCalButton_Click(object sender, RoutedEventArgs e)
>>>>>>> .r36
        {
<<<<<<< .mine
            Team t = new Team();
=======
           Team t = new Team();
>>>>>>> .r36
            t.Show();
        }

<<<<<<< .mine
        private void addTimeBlock_Click(object sender, RoutedEventArgs e)
=======
        private void openTeam_Click(object sender, RoutedEventArgs e)
>>>>>>> .r36
        {
<<<<<<< .mine
            TimeBlock memberTimeBlock = new TimeBlock();
            timeBlocks.AddFirst(memberTimeBlock);
            if(view == false)
            {
                memberTimeBlock.InitializeComponent();
                memberTimeBlock.UpdateLayout();
                memberTimeBlock.BringIntoView();
                memberTimeBlock.Focusable=true;
                memberTimeBlock.Show();
            }
=======
            Team team = new Team();
            try
            {
                team.Show();
            }
            catch (Exception ex) { }

>>>>>>> .r36
        }

<<<<<<< .mine
        private void iPlanMain_Closing(object sender,System.ComponentModel.CancelEventArgs e)
        {
            foreach(TimeBlock t in timeBlocks)
                t.Close();
        }
=======
        private void newTeamMember_Click(object sender, RoutedEventArgs e)
        {
            NewMember m = new NewMember();
            m.ShowDialog();
            
        }

     

       



>>>>>>> .r36
    }
}
