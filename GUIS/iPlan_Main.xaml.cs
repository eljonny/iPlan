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
    /// Interaction logic for iPlan_Main.xaml
    /// </summary>
    public partial class iPlan_Main : Window
    {
        #region iPlan Main Window Variables

        private ArrayList gridContents = new ArrayList();
        private bool ed = true, view = false, contextClckMkBlck=false, newcal = true, calEdited = false;
        private int teamSize = 0, mouseWheelDeltaTmp;
        private string calName;
        private double pxDiffBlockTop,pxDiffBlockLeft,wkCalGridContainerHeight,wkCalGridContainerWidth;
        public  Team team = new Team();
        private TimeBlock tmp;
        private System.Windows.Point mousePositionTmp;
        private LinkedList<TimeBlock> timeBlocks = new LinkedList<TimeBlock>();
        private LinkedList<TimeBlock>.Enumerator tBlocks;
        private ScrollViewer scroller;
        private OpenFileDialog openOrSaveCal;
        private iPlan_Welcome parent;

        #endregion

        #region Construction

        public iPlan_Main(string newCalName, iPlan_Welcome welcomeWin)
        {
            parent = welcomeWin;
            InitializeComponent();

            initCombos();

            this.calName = newCalName;

            this.Title = "iPlan - " + newCalName;
            
            this.Activate();
            
            gridContents.Cast<gridObject>();

            System.Drawing.Size displayRes = SystemInformation.PrimaryMonitorSize;
            
            wkCalGridContainerHeight = wkCalGridContainer.ActualHeight;
            wkCalGridContainerWidth = wkCalGridContainer.ActualWidth;
            
            System.Windows.Input.Mouse.AddMouseWheelHandler
                ( (DependencyObject)wkCalGridContainer,
                  new MouseWheelEventHandler(wkCalGridContainer_MouseWheel) );
        }

        public iPlan_Main(System.IO.Stream calendar, iPlan_Welcome welcomeWin)
        {
            parent = welcomeWin;
            InitializeComponent();

            initCombos();

            this.Activate();

            newcal = false;

            if (parseIPlanCal(calendar))
                rStatBarTxt.Text = calName + " successfully loaded!";

            gridContents.Cast<gridObject>();

            System.Drawing.Size displayRes = SystemInformation.PrimaryMonitorSize;
            
            wkCalGridContainerHeight = wkCalGridContainer.ActualHeight;
            wkCalGridContainerWidth = wkCalGridContainer.ActualWidth;
            
            System.Windows.Input.Mouse.AddMouseWheelHandler
                ((DependencyObject)wkCalGridContainer,
                 new MouseWheelEventHandler(wkCalGridContainer_MouseWheel));
        }

        private void initCombos()
        {
            ItemCollection weeks = wkComboBox.Items;
            ItemCollection years = yrComboBox.Items;

            for(int i = 0; ++i < 6; weeks.Add(i.ToString()));
            for (int i = (DateTime.Today.Year - 1); ++i < 2051; years.Add(i.ToString())) ;

            moComboBox.SelectedIndex = DateTime.Now.Month - 1;
            wkComboBox.SelectedIndex = getWeekOfMonth();
            yrComboBox.SelectedItem = DateTime.Now.Year.ToString();
        }

        private int getWeekOfMonth()
        {
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int dayOffset = 0;
            DateTime first = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            switch (first.DayOfWeek.ToString())
            {
                case "Monday":
                    dayOffset = 1;
                    break;
                case "Tuesday":
                    dayOffset = 2;
                    break;
                case "Wednesday":
                    dayOffset = 3;
                    break;
                case "Thursday":
                    dayOffset = 4;
                    break;
                case "Friday":
                    dayOffset = 5;
                    break;
                case "Saturday":
                    dayOffset = 6;
                    break;
            }

            return (DateTime.Now.Day + dayOffset) / 7;
        }

        #endregion

        #region Event Handlers
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
            else if (!ed && (startTime.IsFocused || EndTime.IsFocused))
                System.Windows.Forms.MessageBox.Show("Please Enable Edit Mode to change the schedule.");
        }

        //Jon
        //
        // Open the "About" information window.
        private void about_Click(object sender, RoutedEventArgs e)
        {
            iPlAbout aboutWin = new iPlAbout();
            aboutWin.ShowDialog();
        }

        // Jon
        //
        // Changes the view mode of the main iPlan window from week
        // view to month view.
        private void chgViewMode(object sender, RoutedEventArgs e)
        {
            this.rStatBarTxt.Text = "Changing views...";
            if (this.wkCalGridContainer.Visibility == Visibility.Visible)
            {
                // Make the week view hidden
                this.wkCalGridContainer.Visibility = Visibility.Hidden;
                // Make all the time blocks hidden if we
                // move to month view
                tBlocks = timeBlocks.GetEnumerator();
                if(tBlocks.Current == null)
                    tBlocks.MoveNext();
                if(timeBlocks.Count != 0 && tBlocks.Current != null && this.IsActive)
                {
                    do
                    {
                        tBlocks.Current.Topmost = false;
                        tBlocks.Current.Visibility = Visibility.Hidden;
                    }while(tBlocks.MoveNext());
                }
                // Clean up....
                tBlocks.Dispose();
                // Make the month view visible
                this.moCalGridContainer.Visibility = Visibility.Visible;
                
                view = true;
            }
            else if (this.moCalGridContainer.Visibility == Visibility.Visible)
            {
                this.wkCalGridContainer.Visibility = Visibility.Visible;

                tBlocks = timeBlocks.GetEnumerator();
                if(tBlocks.Current == null)
                    tBlocks.MoveNext();
                if(timeBlocks.Count != 0 && tBlocks.Current != null && this.IsActive)
                {
                    do
                    {
                        if(tBlocks.Current.IsEnabled)
                        {
                            tBlocks.Current.Topmost = true;
                            tBlocks.Current.Visibility = Visibility.Visible;
                        }
                        else if(!tBlocks.Current.IsEnabled)
                        {
                            tmp = tBlocks.Current;
                            break;
                        }
                    }while(tBlocks.MoveNext());
                }
                tBlocks.Dispose();
                if(tmp != null && !tmp.IsEnabled)
                {
                    timeBlocks.Remove(tmp);
                    tmp = null;
                }

                this.moCalGridContainer.Visibility = Visibility.Hidden;
                view = false;
            }

            this.rStatBarTxt.Text = "Checking tBlocks visibility...";
            
            if(timeBlocks.Count > 0 && !checkTBlockVisibleState())
            {
                this.rStatBarTxt.Text = "Normalizing visibility...";
                normalizeTBlockVisibility();
            }
            else
                this.rStatBarTxt.Text = "Ok.";

            // Update the status bar.
            if(view)
                this.rStatBarTxt.Text = "Month View."
                    + " View schedule details to help plan for "
                    + "long term goals. Easily organize your "
                    + "thoughts and notify your team of changes"
                    + " and new ideas.";
            else
                this.rStatBarTxt.Text = "Week View. Build your "
                    + "Schedule here and send notifications about"
                    + " schedule changes.";
        }

        //Jon
        private void enterOrLeaveEditMode(object sender, RoutedEventArgs e)
        {
            ed = !ed;
            /*if(passWd(Team))
            {
                if(ed)
                    System.Windows.MessageBox.Show("Edit mode enabled");
                else
                    System.Windows.MessageBox.Show("Edit mode disabled");
            }*/
            if(ed)
                System.Windows.MessageBox.Show("Edit mode enabled");
            else
                System.Windows.MessageBox.Show("Edit mode disabled");
        }

        //Jon
        private void startEmail(object sender, RoutedEventArgs e)
        {
            Process.Start("mail.exe");
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
            if (!view)
            {
                TimeBlock memberTimeBlock = new TimeBlock();
                memberTimeBlock.setParental(this);
                if (contextClckMkBlck)
                {
                    memberTimeBlock.setWindowStartup(mousePositionTmp);
                    contextClckMkBlck = false;
                }
                else
                {
                    memberTimeBlock.setWindowStartup();
                    memberTimeBlock.setPxDiff(pxDiffBlockLeft, pxDiffBlockTop);
                }
                timeBlocks.AddFirst(memberTimeBlock);
                memberTimeBlock.UpdateLayout();
                memberTimeBlock.Focusable = true;
                memberTimeBlock.Show();
                pxDiffBlockLeft = memberTimeBlock.Left - this.Left;
                pxDiffBlockTop = memberTimeBlock.Top - this.Top;
            }
            else
            {
                System.Windows.MessageBox.Show("You can not add a timeblock"
                    + " in Month View. Please change to Week View to add "
                    + "a time block.");
                this.rStatBarTxt.Text = "Can't add time block in Month View";
            }
        }

        //Jon
        private void iPlanMain_LocationChanged(object sender, EventArgs e)
        {
            wkCalGridContainerHeight = wkCalGridContainer.ActualHeight;
            wkCalGridContainerWidth = wkCalGridContainer.ActualWidth;
            tBlocks = timeBlocks.GetEnumerator();
            if (tBlocks.Current == null)
                tBlocks.MoveNext();
            if (timeBlocks.Count != 0 && tBlocks.Current != null)
            {
                do
                {
                    if (tBlocks.Current.IsEnabled)
                    {
                        tBlocks.Current.Top = tBlocks.Current.Top - ((tBlocks.Current.Top - this.Top)
                            - tBlocks.Current.getGO().getPxDiffs()[1]);
                        tBlocks.Current.Left = tBlocks.Current.Left - (tBlocks.Current.Left - this.Left)
                            - tBlocks.Current.getGO().getPxDiffs()[0];
                        tBlocks.Current.setPxDiff(tBlocks.Current.Left - this.Left, tBlocks.Current.Top - this.Top);
                    }
                    else if (!tBlocks.Current.IsEnabled)
                    {
                        tmp = tBlocks.Current;
                        break;
                    }
                } while (tBlocks.MoveNext());
            }
            tBlocks.Dispose();

            if (tmp != null && !tmp.IsEnabled)
            {
                timeBlocks.Remove(tmp);
                tmp = null;
            }
        }

        //Ryan
        private void openTeam_Click(object sender, RoutedEventArgs e)
        {
            Team team = new Team();
            try
            {
                team.Show();
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString()); }
        }

        //Jon
        private void iPlanMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void iPlanMain_windowStateChanged(object sender, EventArgs e)
        {
            tBlocks = timeBlocks.GetEnumerator();
            if (this.WindowState == WindowState.Minimized)
            {
                if (tBlocks.Current == null)
                    tBlocks.MoveNext();
                if (timeBlocks.Count != 0 && tBlocks.Current != null)
                {
                    do
                    {
                        if (tBlocks.Current.IsEnabled)
                        {
                            tBlocks.Current.Topmost = false;
                            tBlocks.Current.Visibility = Visibility.Hidden;
                        }
                        else if (!tBlocks.Current.IsEnabled)
                        {
                            tmp = tBlocks.Current;
                            break;
                        }
                    } while (tBlocks.MoveNext());
                }
            }
            else if (this.WindowState == WindowState.Maximized || this.WindowState == WindowState.Normal)
            {
                if (tBlocks.Current == null)
                    tBlocks.MoveNext();
                if (timeBlocks.Count != 0 && tBlocks.Current != null)
                {
                    do
                    {
                        if (tBlocks.Current.Visibility != Visibility.Visible
                            && tBlocks.Current.IsEnabled)
                        {
                            tBlocks.Current.Topmost = true;
                            tBlocks.Current.Visibility = Visibility.Visible;
                        }
                        else if (!tBlocks.Current.IsEnabled)
                        {
                            tmp = tBlocks.Current;
                            break;
                        }
                    } while (tBlocks.MoveNext());
                }
            }
            tBlocks.Dispose();
            if (tmp != null && !tmp.IsEnabled)
            {
                timeBlocks.Remove(tmp);
                tmp = null;
            }
        }

        private void iPlanMain_mouseOver(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tBlocks = timeBlocks.GetEnumerator();
            if (tBlocks.Current == null)
                tBlocks.MoveNext();
            if (timeBlocks.Count != 0 && tBlocks.Current != null)
            {
                do
                {
                    if (tBlocks.Current.IsEnabled)
                    {
                        tBlocks.Current.Topmost = true;
                        tBlocks.Current.Visibility = Visibility.Visible;
                    }
                    else if (!tBlocks.Current.IsEnabled)
                    {
                        tmp = tBlocks.Current;
                        break;
                    }
                } while (tBlocks.MoveNext());
            }
            tBlocks.Dispose();
            if (tmp != null && !tmp.IsEnabled)
            {
                timeBlocks.Remove(tmp);
                tmp = null;
            }
        }

        private void iPlanMain_mouseNotOver(object sender, System.Windows.Input.MouseEventArgs e)
        {
            bool overAny = false;
            int activeBlock;
            tBlocks = timeBlocks.GetEnumerator();
            if (timeBlocks.Count > 0)
            {
                if (tBlocks.Current == null)
                    tBlocks.MoveNext();

                do
                {
                    if (tBlocks.Current != null
                        && tBlocks.Current.IsMouseOver)
                    {
                        overAny = true;
                        break;
                    }
                } while (tBlocks.MoveNext());
                tBlocks.Dispose();

                if (!overAny)
                {
                    tBlocks = timeBlocks.GetEnumerator();
                    if (tBlocks.Current == null)
                        tBlocks.MoveNext();
                    do
                    {
                        tBlocks.Current.Topmost = false;
                        tBlocks.Current.Visibility = Visibility.Hidden;
                    } while (tBlocks.MoveNext());
                    tBlocks.Dispose();
                }
            }
        }

        private void iPlanMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            wkCalGridContainerHeight = wkCalGridContainer.ActualHeight;
        }

        private void propCalButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("CalGrdHght: " + wkCalGridContainer.ActualHeight
                + "\nCalGrdWdth: " + wkCalGridContainer.ActualWidth
                + "\nMainGrdHght: " + mainGrid.ActualHeight + "\nMainGrdWdth: "
                + mainGrid.ActualWidth);
        }

        /* Ryan */
        private void resetCalButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Slider tb in FindVisualChildren<Slider>(this))
            {
                tb.Value = 0;
            }
        }

        private void wkCalGridContainer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (timeBlocks.Count > 0)
            {
                tBlocks = timeBlocks.GetEnumerator();
                if (tBlocks.Current == null)
                    tBlocks.MoveNext();
                cleanTimeBlockList();
                do
                {
                    tBlocks.Current.setScrollContentOffset(wkCalGridContainer.VerticalOffset);
                } while (tBlocks.MoveNext());
                tBlocks.Dispose();
            }
        }

        private void weekGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            mousePositionTmp = System.Windows.Input.Mouse.GetPosition((IInputElement)this);
        }

        private void gridContextMenu_Click(object sender, RoutedEventArgs e)
        {
            contextClckMkBlck = true;
            addTimeBlock_Click(this, e);
        }

        private void wkCalGridContainer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mouseWheelDeltaTmp = e.Delta;
            System.Windows.MessageBox.Show("tester");
            scroller = (ScrollViewer)sender;
            if (mouseWheelDeltaTmp > 0)
            {
                scroller.LineUp();
            }
            else
            {
                scroller.LineDown();
            }
        }

        private void openCal_Click_1(object sender, RoutedEventArgs e)
        {
            openOrSaveCal = new OpenFileDialog();
            openOrSaveCal.ShowDialog();
        }

        //private 

        private void calSaveAs_Click(object sender, RoutedEventArgs e)
        {
            openOrSaveCal = new OpenFileDialog();
            openOrSaveCal.ShowDialog();
        }

        private void calSave_Button_Click(object sender, RoutedEventArgs e)
        {
            if (newcal)
                calSaveAs_Click(sender, e);
            else
                saveCurrentCal();
        }

        private void newCal_event_click(object sender, RoutedEventArgs e)
        {
            calSaveAs_Click(sender, e);
            parent.Show();
            this.Hide();
        }

        #endregion

        #region GridObject Operations

        //Jon
        private String[] getNamesOfCurrentTeam()
        {
            int i = 0;
            String[] participNames = new String[teamSize];

            foreach (gridObject teamMember in gridContents)
            {
                participNames[i] = teamMember.getName();
                i++;
            }

            return participNames;
        }

        //Jon
        private void setObjSlot(gridObject o, ArrayList gridObjects)
        {
            if (ed)
            {
                gridObjects.Insert(o.getDay(), o);
                gridObjects.RemoveAt(o.getDay() + 1);
            }
            else
                System.Windows.Forms.MessageBox.Show("Please Enable Edit Mode to change the schedule.");
        }

        //Jon
        public ArrayList getTeam()
        {
            return gridContents;
        }

        public string toString()
        {
            return gridContents.ToString();
        }

        //Jon
        private gridObject addGObj()
        {
            string[] contents = new string[3];
            double[,] probData = new double[100, 2];
            probData[0, 1] = .4; probData[1, 1] = .95;
            contents[0] = "James"; contents[1] = "Garner"; contents[2] = "Software Engineer";
            gridObject tMTimeBlock =
                new gridObject
                    (1, 2, 0, 0, "00:00", "07:00", true, contents, "emp",
                    DateTime.Today.ToString(), probData);
            return tMTimeBlock;
        }

        #endregion

        #region TimeBlock Operations

        // Detect visibility of time blocks (for potential cleaning)
        private bool checkTBlockVisibleState()
        {
            // If there is at least one time block...
            if (timeBlocks.Count > 0)
            {
                bool prev = false;
                tBlocks = timeBlocks.GetEnumerator();
                if (tBlocks.Current == null)
                    tBlocks.MoveNext();
                // Is the previous time block visible?
                prev = tBlocks.Current.IsVisible;
                do
                {
                    // If the current time block isn't enabled...
                    // make sure to remove it from the list.
                    if (!tBlocks.Current.IsEnabled)
                    {
                        // Assign the ref to a temp var
                        // for later disposal
                        tmp = tBlocks.Current;
                        break;
                    }
                    // If there's a time b
                    if (prev != tBlocks.Current.IsVisible)
                        return false;
                    prev = tBlocks.Current.IsVisible;

                } while (tBlocks.MoveNext());

                // Clean up...
                tBlocks.Dispose();

                if (tmp != null && !tmp.IsEnabled)
                {
                    // Remove the time block that is
                    // no longer active.
                    timeBlocks.Remove(tmp);
                    tmp = null;
                    checkTBlockVisibleState();
                }

                // Clean up...
                System.GC.Collect();
            }
            // Else all is well by default since
            // there are no time blocks.
            return true;
        }

        private void normalizeTBlockVisibility()
        {
            tBlocks = timeBlocks.GetEnumerator();
            if (tBlocks.Current == null)
                tBlocks.MoveNext();
            if (!view)
            {
                do
                {
                    if (!tBlocks.Current.IsEnabled)
                    {
                        tmp = tBlocks.Current;
                        break;
                    }
                    tBlocks.Current.Visibility = Visibility.Visible;
                } while (tBlocks.MoveNext());
            }
            else
            {
                do
                {
                    if (!tBlocks.Current.IsEnabled)
                    {
                        tmp = tBlocks.Current;
                        break;
                    }
                    tBlocks.Current.Visibility = Visibility.Hidden;
                } while (tBlocks.MoveNext());
            }

            tBlocks.Dispose();

            if (tmp != null && !tmp.IsEnabled)
            {
                timeBlocks.Remove(tmp);
                tmp = null;
                normalizeTBlockVisibility();
            }

            System.GC.Collect();
        }

        private void cleanTimeBlockList()
        {
            tBlocks = timeBlocks.GetEnumerator();
            if (tBlocks.Current == null)
                tBlocks.MoveNext();
            do
            {
                if (!tBlocks.Current.IsEnabled)
                {
                    tmp = tBlocks.Current;
                    break;
                }
            } while (tBlocks.MoveNext());

            tBlocks.Dispose();

            if (tmp != null && !tmp.IsEnabled)
            {
                timeBlocks.Remove(tmp);
                tmp = null;
                cleanTimeBlockList();
            }

            System.GC.Collect();
            this.rStatBarTxt.Text = "TimeBlock list cleaned.";
        }

        #endregion

        #region Miscellaneous Functions

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

        #endregion

        #region Saving and Loading

        private void saveCurrentCal()
        {
            System.Windows.MessageBox.Show("Calendar saved as " + calName,
                                           "Calendar Saved",
                                           MessageBoxButton.OK);
            //TODO write save calendar code
        }

        private bool parseIPlanCal(System.IO.Stream calendar)
        {
            System.IO.StreamReader calReader = new System.IO.StreamReader(calendar);
            string rawCalData = calReader.ReadLine();
            
            char[] delimiter = { ',' };
            string[] calData = rawCalData.Split(delimiter);
            
            foreach(string str in calData)
                str.Trim();

            calName = calData[0];

            string exStr = "";
            bool exCaught = false;

            try
            {
                moComboBox.SelectedIndex = Int32.Parse(calData[1]);
            }

            catch (ArgumentNullException argNullEx)
            {
                exStr += argNullEx.Message + "\n";
            }
            catch (FormatException formatEx)
            {
                exStr += formatEx.Message + "\n";
            }
            catch (OverflowException oFEx)
            {
                exStr += oFEx.Message + "\n";
            }
            catch (Exception ex)
            {
                exStr += ex.Message + "\n";
            }
            finally
            {
                rStatBarTxt.Text = "Loading calendar file failed!";

                DialogResult fileLoadingChoice =
                    System.Windows.Forms.MessageBox.Show("Incorrect CSV parameter! Try to reload the file?\n" +
                                                    "Value at parameter 2 (Month number) is invalid.\n\n" +
                                                    "Exception information:\n" + exStr,
                                                    "Calendar File Failed to Load",
                                                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (fileLoadingChoice == System.Windows.Forms.DialogResult.Yes)
                    parseIPlanCal(calendar);
                else
                    exCaught = true;
            }

            if (exCaught)
                return false;

            int wk = Int32.Parse(calData[2]);
            int yr = Int32.Parse(calData[3]);
            
            if (wk < 53 && wk > 0)
            {
                wkComboBox.SelectedValue = calData[2];
            }
            else
            {
                rStatBarTxt.Text = "Loading calendar file failed!";
                System.Windows.MessageBox.Show("Week parameter in file not formatted correctly!", "Calendar File Failed to Load");

                return false;
            }
            
            if (yr < 2051 && yr >= DateTime.Today.Year)
            {
                yrComboBox.SelectedValue = calData[3];
            }
            else
            {
                rStatBarTxt.Text = "Loading calendar file failed!";
                System.Windows.MessageBox.Show("Year parameter in file not formatted correctly!", "Calendar File Failed to Load");

                return false;
            }

            yrComboBox.SelectedValue = calData[3];

            view = Boolean.Parse(calData[4]);

            return true;
        }

        #endregion

    } // Partial class iPlan_Main end
} // namespace GUIProj1
