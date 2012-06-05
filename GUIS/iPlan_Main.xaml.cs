#region Using Assemblies

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
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
using System.IO;

#endregion

//   Author Jonathan Hyry, Ryan Soushek
namespace GUIProj1
{
    /// <summary>
    /// Interaction logic for iPlan_Main.xaml
    /// </summary>
    public sealed partial class iPlan_Main : Window
    {
        #region iPlan Main Window Variables

        private const int MAX_YEAR = 2051;

        // Hold GUI elements that are edited at run time.
        private ArrayList gridContents = new ArrayList(),
                          monthViewDayNumbers = new ArrayList(),
                          monthViewLabels = new ArrayList(),
                          teams = new ArrayList();

        // <Border object, is week selected?>
        private Dictionary<Border, Boolean> monthViewWeekBorders = new Dictionary<Border, Boolean>();

        // For accessing Label objects from monthViewLabels.
        private IEnumerable<System.Windows.Controls.Label> lblsMonthNames;

        private bool ed = true, view = false, contextClckMkBlck = false,
                     newcal = true, calEdited = false, mondayFirst = true;
        private int mouseWheelDeltaTmp;
        private string calName = null;
        private double pxDiffBlockTop, pxDiffBlockLeft,
                       wkCalGridContainerScrollViewerHeight, wkCalGridContainerScrollViewerWidth;
        private TimeBlock tmp;
        private System.Windows.Point mousePositionTmp;
        private LinkedList<TimeBlock> timeBlocks = new LinkedList<TimeBlock>();
        private LinkedList<TimeBlock>.Enumerator tBlocks;
        private ScrollViewer scroller;
        private OpenFileDialog openOrSaveCal;
        private iPlan_Welcome parent;
        private DateTime today = DateTime.Now;

        #endregion

        #region Construction

        #region Constructors

        public iPlan_Main(string newCalName, iPlan_Welcome welcomeWin)
        {
            iPlanInit(welcomeWin);

            newcal = true;
            this.calName = newCalName;
            this.Title = "iPlan - " + newCalName;
        }

        public iPlan_Main(System.IO.Stream calendar, iPlan_Welcome welcomeWin)
        {
            iPlanInit(welcomeWin);

            newcal = false;

            if (parseIPlanCal(calendar))
                rStatBarTxt.Text = calName + " successfully loaded!";
        }

        #endregion

        #region Init

        private void iPlanInit(iPlan_Welcome welcomeWin)
        {
            parent = welcomeWin;
            InitializeComponent();

            #region ArrayList Casting for later object storage
            gridContents.Cast<GridObject>();
            monthViewDayNumbers.Cast<TextBlock>();
            monthViewLabels.Cast<System.Windows.Controls.Label>();
            monthViewWeekBorders.Cast<Border>();
            teams.Cast<Team>();
            #endregion

            #region Set initial status bars texts
            lStatBarTxt.Text =
                "iPlan v " + Assembly.GetAssembly(typeof(iPlan_Main)).
                                            GetName().Version.ToString();

            rStatBarTxt.Text = "Welcome to iPlan! To get started in building a schedule, " +
                               "click the '+ Time Block' button; or, open an existing" +
                               " calendar through the file menu.";
            #endregion

            #region Month view construction
            findMonthViewNumberTextBlocks();
            findMonthViewLabels();
            findMonthViewWeekBorders();
            initCombos();
            populateMonthViewDayLabels();
            selectWeek(monthViewWeekBorders.ElementAt(moComboBox.SelectedIndex).Key, wkComboBox.SelectedIndex);
            #endregion

            // For rearranging the day labels later.
            lblsMonthNames = monthViewLabels.ToArray().Cast<object>().
                                    Cast<System.Windows.Controls.Label>();

            this.Activate();

            wkCalGridContainerScrollViewerHeight = wkCalGridContainerScrollViewer.ActualHeight;
            wkCalGridContainerScrollViewerWidth = wkCalGridContainerScrollViewer.ActualWidth;

            System.Windows.Input.Mouse.AddMouseWheelHandler
                ((DependencyObject)wkCalGridContainerScrollViewer,
                  new MouseWheelEventHandler(wkCalGridContainer_MouseWheel));
        }

        private void initCombos()
        {
            ItemCollection weeks = wkComboBox.Items;
            ItemCollection years = yrComboBox.Items;
            ItemCollection days = dayComboBox.Items;

            for(int i = 0; ++i < 6; weeks.Add(i.ToString()));
            for (int i = 0; ++i < DateTime.DaysInMonth(DateTime.Now.Year,DateTime.Now.Month); days.Add(i.ToString())) ;
            for (int i = (DateTime.Today.Year - 1); ++i < MAX_YEAR; years.Add(i.ToString()));

            moComboBox.SelectedIndex = DateTime.Now.Month - 1;
            yrComboBox.SelectedItem = DateTime.Now.Year.ToString();
            dayComboBox.SelectedIndex = DateTime.Now.Day - 1;
            wkComboBox.SelectedIndex = getWeekInMonth();
        }

        #endregion

        #region Reflections helper functions that populate data structures.

        private void findMonthViewNumberTextBlocks()
        {
            Console.WriteLine("\n\nFinding day number text blocks....\n");

            FieldInfo[] iPlanFields = typeof(iPlan_Main).GetFields( BindingFlags.NonPublic |
                                                                    BindingFlags.Public    |
                                                                    BindingFlags.Instance    );
            foreach (FieldInfo field in iPlanFields)
                if (field.Name.EndsWith("Number"))
                {
                    Console.WriteLine("Adding {0}", field.Name);
                    monthViewDayNumbers.Add(field.GetValue(this));
                }
        }

        private void findMonthViewLabels()
        {
            Console.WriteLine("\n\nFinding month labels....\n");

            FieldInfo[] iPlanFields = typeof(iPlan_Main).GetFields( BindingFlags.NonPublic |
                                                                    BindingFlags.Public    |
                                                                    BindingFlags.Instance    );
            foreach (FieldInfo field in iPlanFields)
                if (field.Name.StartsWith("lblMonthView"))
                {
                    Console.WriteLine("Adding {0}", field.Name);
                    monthViewLabels.Add(field.GetValue(this));
                }
        }

        private void findMonthViewWeekBorders()
        {
            Console.WriteLine("\n\nFinding month view week borders....\n");

            FieldInfo[] iPlanFields = typeof(iPlan_Main).GetFields(BindingFlags.NonPublic |
                                                                    BindingFlags.Public |
                                                                    BindingFlags.Instance);
            foreach (FieldInfo field in iPlanFields)
                if (field.Name.StartsWith("moCalGridWk") &&
                    field.Name.EndsWith("Border") )
                {
                    Console.WriteLine("Adding {0}", field.Name);
                    monthViewWeekBorders.Add(((Border)field.GetValue(this)),false);
                }
        }

        #endregion

        #region Grid Population Methods

        private void populateMonthViewDayLabels()
        {
            Console.WriteLine("\n\nPopulating month grid for {0}...", moComboBox.SelectedValue.ToString());

            int startDay = getFirstDayOfMonthOffset(), numDaysPrevious, numDaysCurrent;

            Console.WriteLine("Start day for this month: {0}",startDay.ToString());

            if (mondayFirst && startDay > 0)
                startDay--;

            // Get number of days in the previous month.
            if (moComboBox.SelectedIndex == 0)
                numDaysPrevious =
                    DateTime.DaysInMonth((Int32.Parse(yrComboBox.SelectedValue.ToString()) - 1),
                                                  /*If january, previous month is December.*/ 12);
            else
                numDaysPrevious =
                    DateTime.DaysInMonth(Int32.Parse(yrComboBox.SelectedValue.ToString()),
                                                                  (moComboBox.SelectedIndex + 1));

            Console.WriteLine("Number of days in previous-to-the-current month, {0}: {1}",moComboBox.SelectedValue.ToString(),numDaysPrevious.ToString());

            // Get number of days in current month.
            numDaysCurrent =
                DateTime.DaysInMonth(Int32.Parse(yrComboBox.SelectedValue.ToString()),
                                                          moComboBox.SelectedIndex + 1);

            Console.WriteLine("Number of days in current month, {0}: {1}", moComboBox.SelectedValue.ToString(), numDaysCurrent.ToString());

            IEnumerator labels = monthViewDayNumbers.GetEnumerator();

            TextBlock dayNumberTextBlock;

            // Begin the iteration.
            labels.MoveNext();

            // Populate day values pertaining to the previous month,
            // if there are any.
            while (numDaysPrevious - startDay < numDaysPrevious)
            {
                dayNumberTextBlock = (TextBlock)labels.Current;
                dayNumberTextBlock.Opacity = .5;
                dayNumberTextBlock.Text = (numDaysPrevious - (startDay--)).ToString();
                Console.WriteLine("Setting text... {0}", dayNumberTextBlock.Text);
                labels.MoveNext();
            }

            /* Might need this later....
             * 
            // If there were days to populate from the previous month...
            if (startDay < 0)
                startDay *= -1;
            // Else continue normally...
            else
             * */
            startDay++;
            
            // Populate the day values of the current month...
            while (startDay <= numDaysCurrent)
            {
                dayNumberTextBlock = (TextBlock)labels.Current;
                dayNumberTextBlock.Opacity = 1;
                dayNumberTextBlock.Text = (startDay++).ToString();
                Console.WriteLine("Setting text... {0}", dayNumberTextBlock.Text);
                labels.MoveNext();
            }

            startDay = 1;

            // Populate the remaining days in the calendar grid.
            try
            {
                do
                {
                    dayNumberTextBlock = (TextBlock)labels.Current;
                    dayNumberTextBlock.Opacity = .5;
                    dayNumberTextBlock.Text = (startDay++).ToString();
                    Console.WriteLine("Setting text... {0}", dayNumberTextBlock.Text);
                } while (labels.MoveNext());
            }
            catch (InvalidOperationException invOpEx)
            {
                Console.WriteLine("\nCan not populate any days from the next month!\n");
                Console.WriteLine("Enumeration failed: already reached the end of enumeration." +
                                    "\n\nException detail:\n{0}\n\nStack trace:\n{1}\n", invOpEx.Message,
                                                                                         invOpEx.StackTrace);
            }
        }

        private void switchDayOrder()
        {
            string first = lblsMonthNames.First().Content.ToString(), previous, next;

            lblsMonthNames.First().Content = lblsMonthNames.Last().Content.ToString();
            lblsMonthNames.Last().Content = first;

            if(mondayFirst)
                for (int i = lblsMonthNames.Count() - 1; i > 1; i--)
                {
                    previous = lblsMonthNames.ElementAt(i).Content.ToString();
                    lblsMonthNames.ElementAt(i).Content = lblsMonthNames.ElementAt(i - 1).Content.ToString();
                    lblsMonthNames.ElementAt(i - 1).Content = previous;
                }
            else
                for (int i = 1; i < lblsMonthNames.Count() - 1; i++)
                {
                    next = lblsMonthNames.ElementAt(i).Content.ToString();
                    lblsMonthNames.ElementAt(i).Content = lblsMonthNames.ElementAt(i - 1).Content.ToString();
                    lblsMonthNames.ElementAt(i - 1).Content = next;
                }

            mondayFirst = !mondayFirst;
        }

        private void selectWeek(Border week, int index)
        {
            foreach (Border value in monthViewWeekBorders.Keys)
            {
                if (value.Equals( week )) {
                    value.BorderBrush = System.Windows.Media.Brushes.Black;
                    value.BorderThickness = new Thickness( 2 );
                }
                else {
                    value.BorderBrush = System.Windows.Media.Brushes.Transparent;
                    value.BorderThickness = new Thickness( 1 );
                }
            }

            monthViewWeekBorders[week] = true;

            ArrayList unselectedWeeks = new ArrayList();
            unselectedWeeks.Cast<Border>();

            foreach (Border value in monthViewWeekBorders.Keys)
                if (!value.Equals(week))
                    unselectedWeeks.Add(value);

            foreach(Border value in unselectedWeeks)
                monthViewWeekBorders[value] = false;

            wkComboBox.SelectedIndex = index;
        }

        private int getWeekInMonth() {
            int days =
                DateTime.DaysInMonth( Int32.Parse( yrComboBox.SelectedValue.ToString() ),
                                                        ( moComboBox.SelectedIndex + 1 ) );
            int dayOffset = getFirstDayOfMonthOffset();

            return ( DateTime.Now.Day + dayOffset ) / 7;
        }

        private int getFirstDayOfMonthOffset() {
            DateTime first =
                new DateTime( Int32.Parse( yrComboBox.SelectedValue.ToString() ),
                                              ( moComboBox.SelectedIndex + 1 ), 1 );

            switch (first.DayOfWeek.ToString()) {
            case "Monday":
                return 1;
            case "Tuesday":
                return 2;
            case "Wednesday":
                return 3;
            case "Thursday":
                return 4;
            case "Friday":
                return 5;
            case "Saturday":
                return 6;
            default:
                return 0;
            }
        }

        #endregion

        #endregion

        #region Event Handlers

        #region iPlan Main Window events

        //Jon
        private void iPlanMain_LocationChanged(object sender, EventArgs e)
        {
            wkCalGridContainerScrollViewerHeight = wkCalGridContainerScrollViewer.ActualHeight;
            wkCalGridContainerScrollViewerWidth = wkCalGridContainerScrollViewer.ActualWidth;

            tBlocks = timeBlocks.GetEnumerator();
            if (tBlocks.Current == null)
                tBlocks.MoveNext();
            if (timeBlocks.Count != 0 && tBlocks.Current != null)
            {
                do
                {
                    if (tBlocks.Current.IsEnabled)
                    {
                        //tBlocks.Current.Top = tBlocks.Current.Top - ((tBlocks.Current.Top - this.Top)
                        //    - tBlocks.Current.getGO().getPxDiffs()[1]);
                        //tBlocks.Current.Left = tBlocks.Current.Left - (tBlocks.Current.Left - this.Left)
                        //    - tBlocks.Current.getGO().getPxDiffs()[0];
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

        //Jon
        private void iPlanMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
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

        private void iPlanMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            wkCalGridContainerScrollViewerHeight = wkCalGridContainerScrollViewer.ActualHeight;
        }

        #endregion

        #region Click Events

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

        private void openCal_Click_1(object sender, RoutedEventArgs e)
        {
            openOrSaveCal = new OpenFileDialog();
            openOrSaveCal.ShowDialog();
        }

        private void selectWeek_click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((System.Windows.Controls.Button)sender).Name;

            Console.WriteLine("\n\nButton name: {0}\n", buttonName);

            int index = -1;

            try
            {
                index = Int32.Parse(buttonName.Substring(buttonName.Length - 1));
            }

            #region Handle exceptions...

            catch (FormatException formEx)
            {
                Console.WriteLine("\nIncorrect input format.\n\n" +
                                  "Exception:\n{0}\n\nStack trace:\n{1}\n",
                                            formEx.Message, formEx.StackTrace);
            }
            catch (ArgumentNullException argNullEx)
            {
                Console.WriteLine("\nNull input not accepted.\n\n" +
                                  "Exception:\n{0}\n\nStack trace:\n{1}\n",
                                            argNullEx.Message, argNullEx.StackTrace);
            }
            catch (OverflowException overflowEx)
            {
                Console.WriteLine("\nInput cause an overflow.\n\n" +
                                  "Exception:\n{0}\n\nStack trace:\n{1}\n",
                                            overflowEx.Message, overflowEx.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUnknown error.\n\n" +
                                  "Exception:\n{0}\n\nStack trace:\n{1}\n",
                                            ex.Message, ex.StackTrace);
            }

            #endregion

            #region Then decide what to do.

            switch (index)
            {
                case 0:
                    if (monthViewWeekBorders[moCalGridWkOneBorder])
                    {

                        wkComboBox.SelectedIndex = index;
                        chgViewMode(sender, e);
                    }
                    else
                        selectWeek(moCalGridWkOneBorder, index);
                    break;
                case 1:
                    if (monthViewWeekBorders[moCalGridWkTwoBorder])
                    {

                        wkComboBox.SelectedIndex = index;
                        chgViewMode(sender, e);
                    }
                    else
                        selectWeek(moCalGridWkTwoBorder, index);
                    break;
                case 2:
                    if (monthViewWeekBorders[moCalGridWkThreeBorder])
                    {

                        wkComboBox.SelectedIndex = index;
                        chgViewMode(sender, e);
                    }
                    else
                        selectWeek(moCalGridWkThreeBorder, index);
                    break;
                case 3:
                    if (monthViewWeekBorders[moCalGridWkFourBorder])
                    {

                        wkComboBox.SelectedIndex = index;
                        chgViewMode(sender, e);
                    }
                    else
                        selectWeek(moCalGridWkFourBorder, index);
                    break;
                case 4:
                    if (monthViewWeekBorders[moCalGridWkFiveBorder])
                    {

                        wkComboBox.SelectedIndex = index;
                        chgViewMode(sender, e);
                    }
                    else
                        selectWeek(moCalGridWkFiveBorder, index);
                    break;
                case 5:
                    if (monthViewWeekBorders[ moCalGridWkSixBorder ]) {

                        wkComboBox.SelectedIndex = index;
                        chgViewMode( sender, e );
                    }
                    else
                        selectWeek( moCalGridWkSixBorder, index );
                    break;
            }

            #endregion
        }

        //Jon
        private void file_quit(object sender, EventArgs e)
        {
            this.Close();
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

            if (this.wkCalGridContainerBorder.Visibility == Visibility.Visible)
            {
                // Make the week view hidden
                this.wkCalGridContainerBorder.Visibility = Visibility.Hidden;
                // Make all the time blocks hidden if we
                // move to month view
                tBlocks = timeBlocks.GetEnumerator();
                if (tBlocks.Current == null)
                    tBlocks.MoveNext();
                if (timeBlocks.Count != 0 && tBlocks.Current != null && this.IsActive)
                {
                    do
                    {
                        tBlocks.Current.Topmost = false;
                        tBlocks.Current.Visibility = Visibility.Hidden;
                    } while (tBlocks.MoveNext());
                }
                // Clean up....
                tBlocks.Dispose();

                // Make the month view visible
                this.moCalGridContainerBorder.Visibility = Visibility.Visible;

                this.rStatBarTxt.Text = "Checking tBlocks visibility...";

                if (timeBlocks.Count > 0 && !checkTBlockVisibleState())
                {
                    this.rStatBarTxt.Text = "Normalizing visibility...";
                    normalizeTBlockVisibility();
                }
                else
                    this.rStatBarTxt.Text = "Ok.";

                this.rStatBarTxt.Text = "Month View."
                    + " View schedule details to help plan for "
                    + "long term goals. Easily organize your "
                    + "thoughts and notify your team of changes"
                    + " and new ideas.";

                view = true;
            }
            else if (this.moCalGridContainerBorder.Visibility == Visibility.Visible)
            {
                this.wkCalGridContainerBorder.Visibility = Visibility.Visible;

                tBlocks = timeBlocks.GetEnumerator();
                if (tBlocks.Current == null)
                    tBlocks.MoveNext();
                if (timeBlocks.Count != 0 && tBlocks.Current != null && this.IsActive)
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

                this.moCalGridContainerBorder.Visibility = Visibility.Hidden;

                this.rStatBarTxt.Text = "Checking tBlocks visibility...";

                if (timeBlocks.Count > 0 && !checkTBlockVisibleState())
                {
                    this.rStatBarTxt.Text = "Normalizing visibility...";
                    normalizeTBlockVisibility();
                }
                else
                    this.rStatBarTxt.Text = "Ok.";

                this.rStatBarTxt.Text = "Week View. Build your "
                    + "Schedule here and send notifications about"
                    + " schedule changes.";

                view = false;
            }
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
            if (ed)
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
                TimeBlock memberTimeBlock = new TimeBlock(this);

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
                memberTimeBlock.Show();
            }
            else
            {
                this.rStatBarTxt.Text = "Can't add time block in Month View";
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

        //George
        private void newTeamMember_Click(object sender, RoutedEventArgs e)
        {
            /*
            NewMember m = new NewMember();
            m.ShowDialog();*/

        }

        private void gridContextMenu_Click(object sender, RoutedEventArgs e)
        {
            contextClckMkBlck = true;
            addTimeBlock_Click(this, e);
        }

        private void propCalButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("CalGrdHght: " + wkCalGridContainerScrollViewer.ActualHeight
                + "\nCalGrdWdth: " + wkCalGridContainerScrollViewer.ActualWidth
                + "\nMonthGrdHght: " + monthGrid.ActualHeight + "\nMonthGrdWdth: "
                + monthGrid.ActualWidth + "\n" + mainGrid.ActualHeight + "\n" + mainGrid.ActualWidth);
        }

        private void detailsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO write this.
        }

        /* Ryan */
        private void resetCalButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Slider tb in FindVisualChildren<Slider>(this))
            {
                tb.Value = 0;
            }
        }

        #endregion

        #region Week Grid Events

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
                    tBlocks.Current.setScrollContentOffset(wkCalGridContainerScrollViewer.VerticalOffset);
                } while (tBlocks.MoveNext());
                tBlocks.Dispose();
            }
        }

        private void weekGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            mousePositionTmp = System.Windows.Input.Mouse.GetPosition((IInputElement)this);
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

        #endregion

        #region Main window Combo/Text Box events

        private void moComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (yrComboBox.SelectedValue != null &&
                ((getFirstDayOfMonthOffset() == 0 && mondayFirst) ||
                 (getFirstDayOfMonthOffset() > 4 && !mondayFirst)))

                switchDayOrder();

            if (yrComboBox.SelectedValue != null)
                populateMonthViewDayLabels();
        }

        private void wkComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = wkComboBox.SelectedIndex;

            switch (index)
            {
                case 0:
                    selectWeek(moCalGridWkOneBorder, index);
                    break;
                case 1:
                    selectWeek(moCalGridWkTwoBorder, index);
                    break;
                case 2:
                    selectWeek(moCalGridWkThreeBorder, index);
                    break;
                case 3:
                    selectWeek(moCalGridWkFourBorder, index);
                    break;
                case 4:
                    selectWeek(moCalGridWkFiveBorder, index);
                    break;
            }
        }

        private void yrComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (moComboBox.SelectedValue != null &&
                ((getFirstDayOfMonthOffset() == 0 && mondayFirst) ||
                 (getFirstDayOfMonthOffset() > 4 && !mondayFirst)    ))

                switchDayOrder();

            if(moComboBox.SelectedValue != null)
                populateMonthViewDayLabels();
        }

        //Jon
        private void labelTxtBoxFill( object sender, ToolTipEventArgs e ) {
            if (ed) {
                System.Windows.Controls.Label clicked = (System.Windows.Controls.Label)sender;
                if (txtboxStartTime.IsFocused)
                    txtboxStartTime.Text = clicked.Content.ToString();
                else if (txtboxEndTime.IsFocused)
                    txtboxEndTime.Text = clicked.Content.ToString();
            }
            else if (!ed && ( txtboxStartTime.IsFocused || txtboxEndTime.IsFocused ))
                System.Windows.Forms.MessageBox.Show( "Please Enable Edit Mode to change the schedule." );
        }

        #endregion

        #endregion

        #region GridObject Operations

        //Jon
        private void setObjSlot(GridObject o, ArrayList gridObjects)
        {
            if (ed)
            {
                //gridObjects.Insert(o.getDay(), o);
                //gridObjects.RemoveAt(o.getDay() + 1);
            }
            else
                System.Windows.Forms.MessageBox.Show("Please Enable Edit Mode to change the schedule.");
        }

        //Jon
        private GridObject addGObj()
        {
            // TODO Rewrite GridObject construction code.
            /*string[] contents = new string[3];
            double[,] probData = new double[100, 2];
            probData[0, 1] = .4; probData[1, 1] = .95;
            contents[0] = "James"; contents[1] = "Garner"; contents[2] = "Software Engineer";
            GridObject tMTimeBlock =
                new GridObject
                    (1, 2, 0, 0, "00:00", "07:00", true, contents, "emp",
                    DateTime.Today.ToString(), probData);
            return tMTimeBlock;
            */

            return null;
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

        #region Team Operations

        //Jon
        private ArrayList getNamesOfTeamMembers()
        {
            String[] participNames;
            ArrayList teamMemberNames = new ArrayList();

            teamMemberNames.Cast<String[]>();

            foreach (Team team in teams)
            {
                participNames = new String[team.getMemberList().Count()];

                TeamMember[] members = team.getMemberList();

                for (int i = 0; i < participNames.Length; )
                    participNames[i] = members[i].getName();

                teamMemberNames.Add(participNames);
            }

            return teamMemberNames;
        }

        //Jon
        public Team getTeam(string name)
        {
            foreach(Team t in teams)
                if (t.teamName.Equals(name))
                    return t;

            return null;
        }

        public ArrayList getTeams()
        {
            return teams;
        }

        #endregion

        #region Miscellaneous Functions

        public override string ToString()
        {
            // TODO Write some string stuff.
            return this.Name;
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

        #endregion

        #region Saving and Loading

        private void saveCurrentCal()
        {
            System.Windows.MessageBox.Show("Calendar saved as " + calName,
                                           "Calendar Saved",
                                           MessageBoxButton.OK);
            //TODO write save calendar code
        }

        /// <summary>
        /// Parses a saved iPlan CSV file.
        /// </summary>
        /// <param name="calendar">Stream that contains the open CSV calendar file</param>
        /// <returns>True if the calendar successfully loaded, otherwise false.</returns>
        private bool parseIPlanCal(System.IO.Stream calendar)
        {
            StreamReader calReader = new StreamReader(calendar);
            string rawCalData = calReader.ReadLine();

            string[] calData = rawCalData.Split(new char[] {','});
            
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
