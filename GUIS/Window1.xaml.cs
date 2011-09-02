using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
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
        private ArrayList gridContents = new ArrayList();
        private bool ed = true, view = false;
        private int teamSize = 0;
        private double pxDiffBlockTop,pxDiffBlockLeft;
        public Team team = new Team();
        //private int pos = 0;
        private LinkedList<TimeBlock> timeBlocks = new LinkedList<TimeBlock>();
        private LinkedList<TimeBlock>.Enumerator tBlocks;
        private RowDefinitionCollection init1, init2, init3;
        private ColumnDefinitionCollection init01,init02,init03;

        public Window1()
        {
            InitializeComponent();
            gridContents.Cast<gridObject>();
            init1 = iPlanMain.mainGrid.RowDefinitions;
            init2 = iPlanMain.weekGrid.RowDefinitions;
            init3 = iPlanMain.monthGrid.RowDefinitions;
            init01 = iPlanMain.mainGrid.ColumnDefinitions;
            init02 = iPlanMain.weekGrid.ColumnDefinitions;
            init03 = iPlanMain.monthGrid.ColumnDefinitions;
            System.Drawing.Size displayRes = SystemInformation.PrimaryMonitorSize;
        }

        //Jon
        private void file_quit(object sender, EventArgs e)
        {
            this.Close();
        }

        //Jon
        private void labelTxtBoxFill(object sender, ToolTipEventArgs e)
        {
            if (ed)
            {
                System.Windows.Controls.Label clicked = (System.Windows.Controls.Label)sender;
                if (startTime.IsFocused)
                    startTime.Text = clicked.Content.ToString();
                else if (EndTime.IsFocused)
                    EndTime.Text = clicked.Content.ToString();
            }
            else if (ed == false && (startTime.IsFocused || EndTime.IsFocused))
                System.Windows.Forms.MessageBox.Show("Please Enable Edit Mode to change the schedule.");
        }

        //Jon
        private void setObjSlot(gridObject o, ArrayList g)
        {
            if (ed)
            {
                g.Insert(o.getDay(), o);
                g.RemoveAt(o.getDay() + 1);
            }
            else
                System.Windows.Forms.MessageBox.Show("Please Enable Edit Mode to change the schedule.");
        }

        //Jon
        private void about_Click(object sender, RoutedEventArgs e)
        {
            iPlAbout aboutWin = new iPlAbout();
            aboutWin.ShowDialog();
        }

        //Jon
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

        //Jon
        private void enterOrLeaveEditMode(object sender, RoutedEventArgs e)
        {
            ed = !ed;
        }

        //Jon
        private void startEmail(object sender, RoutedEventArgs e)
        {
            Process.Start("mail.exe");
        }

        //Jon
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

        //Jon
        public ArrayList getTeam()
        {
            return gridContents;
        }

        private void notifyListCall()
        {
            //checkedList navWinPg = new checkedList();
        }

        public string toString()
        {
            return gridContents.ToString();
        }

        //Jon
        private gridObject addGObj()
        {
            string[] contents = new string[3];
            double[,] probData = new double[100,2];
            probData[0,1] = .4;probData[1,1] = .95;
            contents[0] = "James";contents[1]="Garner";contents[2]="Software Engineer";
            gridObject tMTimeBlock =
                new gridObject
                    (1, 2, 0,0,"00:00", "07:00", true, contents, "emp",
                    DateTime.Today.ToString(), probData);
            return tMTimeBlock;
        }

        /* Ryan
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
            clr2 = System.Windows.Media.Color.FromArgb(arrColor[pos].A,
                arrColor[pos].R, arrColor[pos].G, arrColor[pos].B);
            brush.Color = clr2;
            Slider s = (Slider)sender;
            brush.Opacity = 0.3;
            s.Background = brush;
        }*/

        /* Ryan */
        private void resetCalButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Slider tb in FindVisualChildren<Slider>(this))
            {
                tb.Value = 0;
            }
        }

        /* Ryan */
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

        /* Ryan
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
                
        }*/

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
       
        /* Ryan*/
        private void testSlider245tester_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Slider obj = (Slider)sender;
            obj.Visibility = Visibility.Hidden;
        }

        //George
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Customizable cs = new Customizable();
            cs.ShowDialog();
        }

        //Ryan
        private void membersCalButton_Click(object sender, RoutedEventArgs e)
        {
            Team t = new Team();
            t.Show();
        }

        //Jon
        private void addTimeBlock_Click(object sender, RoutedEventArgs e)
        {
            RowDefinition init =
                iPlanMain.weekGrid.RowDefinitions.ElementAt<RowDefinition>(2);
            TimeBlock memberTimeBlock = new TimeBlock();
            timeBlocks.AddFirst(memberTimeBlock);
            if(view == false)
            {
                memberTimeBlock.InitializeComponent();
                memberTimeBlock.UpdateLayout();
                memberTimeBlock.Focusable=true;
                memberTimeBlock.Show();
                pxDiffBlockLeft = memberTimeBlock.Left - this.Left;
                pxDiffBlockTop = memberTimeBlock.Top - this.Top;
                memberTimeBlock.setPxDiff(pxDiffBlockLeft, pxDiffBlockTop);
            }
        }

        //Jon
        private void iPlanMain_LocationChanged(object sender,EventArgs e)
        {
            tBlocks = timeBlocks.GetEnumerator();
            if(tBlocks.Current == null)
                tBlocks.MoveNext();
            if(timeBlocks.Count != 0 && tBlocks.Current != null)
            {
                do
                {
                    tBlocks.Current.Top = tBlocks.Current.Top - ((tBlocks.Current.Top - this.Top)
                        - tBlocks.Current.getGO().getPxDiffs()[1]);
                    tBlocks.Current.Left = tBlocks.Current.Left - (tBlocks.Current.Left - this.Left)
                        - tBlocks.Current.getGO().getPxDiffs()[0];
                    tBlocks.Current.setPxDiff(tBlocks.Current.Left - this.Left,tBlocks.Current.Top - this.Top);
                }while(tBlocks.MoveNext());
            }
            tBlocks.Dispose();
        }

        //Ryan
        private void openTeam_Click(object sender, RoutedEventArgs e)
        {
            Team team = new Team();
            try
            {
                team.Show();
            }
            catch (Exception ex) {System.Windows.Forms.MessageBox.Show(ex.ToString());}
        }

        //Jon
        private void iPlanMain_Closing(object sender,System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

        //George
        private void newTeamMember_Click(object sender, RoutedEventArgs e)
        {
            /*
            NewMember m = new NewMember();
            m.ShowDialog();*/
            
        }

        private void iPlanMain_windowStateChanged(object sender,EventArgs e)
        {
            tBlocks = timeBlocks.GetEnumerator();
            if(this.WindowState == WindowState.Minimized)
            {
                if(tBlocks.Current == null)
                    tBlocks.MoveNext();
                if(timeBlocks.Count != 0 && tBlocks.Current != null)
                {
                    do
                    {
                        tBlocks.Current.Topmost = false;
                        tBlocks.Current.Visibility = Visibility.Hidden;
                    }while(tBlocks.MoveNext());
                }
            }
            else
            {
                if(tBlocks.Current == null)
                    tBlocks.MoveNext();
                if(timeBlocks.Count != 0 && tBlocks.Current != null)
                {
                    do
                    {
                        tBlocks.Current.Topmost = true;
                        tBlocks.Current.Visibility = Visibility.Visible;
                    }while(tBlocks.MoveNext());
                }
            }
            tBlocks.Dispose();
            this.UpdateLayout();
        }

        private void iPlanMain_mouseNotOver(object sender, DependencyPropertyChangedEventArgs e)
        {
            tBlocks = timeBlocks.GetEnumerator();
            if(tBlocks.Current == null)
                tBlocks.MoveNext();
            if(timeBlocks.Count != 0 && tBlocks.Current != null)
            {
                do
                {
                    tBlocks.Current.Topmost = false;
                    tBlocks.Current.Visibility = Visibility.Hidden;
                }while(tBlocks.MoveNext());
            }
            tBlocks.Dispose();
            this.UpdateLayout();
        }
    }
}
