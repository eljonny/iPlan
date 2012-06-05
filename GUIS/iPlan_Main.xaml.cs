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

//   Author Jonathan Hyry, Ryan Soushek, George Wanjiru
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
                          teams = new ArrayList(),
                          inactiveBlocks = new ArrayList();

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
            inactiveBlocks.Cast<TimeBlock>();
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
            #endregion

            // For rearranging the day labels later.
            lblsMonthNames = monthViewLabels.ToArray().Cast<object>().
                                    Cast<System.Windows.Controls.Label>();

            this.Activate();

            wkCalGridContainerScrollViewerHeight = wkCalGridContainerScrollViewer.ActualHeight;
            wkCalGridContainerScrollViewerWidth = wkCalGridContainerScrollViewer.ActualWidth;

            System.Windows.Input.Mouse.AddMouseWheelHandler
                ((DependencyObject)wkCalGridContainerScrollViewer,
                  new MouseWheelEventHandler(wkCalGridContainerScrollViewer_MouseWheel));
        }

        private void initCombos()
        {
            ItemCollection weeks = wkComboBox.Items;
            ItemCollection years = yrComboBox.Items;
            ItemCollection days = dayComboBox.Items;

            for(int i = 0; ++i < 7; weeks.Add(i.ToString()));
            for (int i = 0; i++ < DateTime.DaysInMonth(DateTime.Now.Year,DateTime.Now.Month); days.Add(i.ToString()));
            for (int i = (DateTime.Today.Year - 1); ++i < MAX_YEAR; years.Add(i.ToString()));

            dayComboBox.SelectedIndex = DateTime.Now.Day - 1;
            moComboBox.SelectedIndex = DateTime.Now.Month - 1;
            yrComboBox.SelectedItem = DateTime.Now.Year.ToString();
            wkComboBox.SelectedIndex = getWeekInMonth() - 1;
        }

        private void resetDayItemCollection()
        {
            int selectedDayIndex = dayComboBox.SelectedIndex;
#if DEBUG
            Console.WriteLine("Rebuilding day combo box... selected day: {0}", selectedDayIndex + 1);
#endif
            ItemCollection days = dayComboBox.Items;

            days.Clear();

            for (int i = 0;
                i++ < DateTime.DaysInMonth
                        (Int32.Parse
                            ((string)yrComboBox.SelectedValue),
                                   moComboBox.SelectedIndex + 1);
                days.Add(i.ToString()));
#if DEBUG
            Console.WriteLine("Selecting day with index {0}", selectedDayIndex);
#endif
            dayComboBox.SelectedIndex = selectedDayIndex;
        }

        #endregion

        #region Reflections helper functions that populate data structures.

        private void findMonthViewNumberTextBlocks()
        {
#if DEBUG
            Console.WriteLine("\n\nFinding day number text blocks....\n");
#endif
            FieldInfo[] iPlanFields = typeof(iPlan_Main).GetFields( BindingFlags.NonPublic |
                                                                    BindingFlags.Public    |
                                                                    BindingFlags.Instance    );
            foreach (FieldInfo field in iPlanFields)
                if (field.Name.EndsWith("Number"))
                {
#if DEBUG
                    Console.WriteLine("Adding {0}", field.Name);
#endif
                    monthViewDayNumbers.Add(field.GetValue(this));
                }
        }

        private void findMonthViewLabels()
        {
#if DEBUG
            Console.WriteLine("\n\nFinding month labels....\n");
#endif
            FieldInfo[] iPlanFields = typeof(iPlan_Main).GetFields( BindingFlags.NonPublic |
                                                                    BindingFlags.Public    |
                                                                    BindingFlags.Instance    );
            foreach (FieldInfo field in iPlanFields)
                if (field.Name.StartsWith("lblMonthView"))
                {
#if DEBUG
                    Console.WriteLine("Adding {0}", field.Name);
#endif           
                    monthViewLabels.Add(field.GetValue(this));
                }
        }

        private void findMonthViewWeekBorders()
        {
#if DEBUG
            Console.WriteLine("\n\nFinding month view week borders....\n");
#endif
            FieldInfo[] iPlanFields = typeof(iPlan_Main).GetFields(BindingFlags.NonPublic |
                                                                    BindingFlags.Public |
                                                                    BindingFlags.Instance);
            foreach (FieldInfo field in iPlanFields)
                if (field.Name.StartsWith("moCalGridWk") &&
                    field.Name.EndsWith("Border") )
                {
#if DEBUG
                    Console.WriteLine("Adding {0}", field.Name);
#endif           
                    monthViewWeekBorders.Add(((Border)field.GetValue(this)),false);
                }
        }

        #endregion

        #region Grid Population Methods

        private void populateMonthViewDayLabels()
        {
#if DEBUG
            Console.WriteLine("\n\nPopulating month grid for {0}...", moComboBox.SelectedValue.ToString());
#endif
            int startDay = getFirstDayOfMonthOffset(), numDaysPrevious, numDaysCurrent;
#if DEBUG
            Console.WriteLine("Start day for this month: {0}",startDay.ToString());
#endif
            if (mondayFirst && startDay > 0)
                startDay--;

            #region Get number of days in the previous month.
            if (moComboBox.SelectedIndex == 0)
                numDaysPrevious =
                    DateTime.DaysInMonth((Int32.Parse(yrComboBox.SelectedValue.ToString()) - 1),
                                                  /*If january, previous month is December.*/ 12);
            else
                numDaysPrevious =
                    DateTime.DaysInMonth(Int32.Parse(yrComboBox.SelectedValue.ToString()),
                                                                  (moComboBox.SelectedIndex));
#if DEBUG
            Console.WriteLine("Number of days in previous-to-the-current month, {0}: {1}",moComboBox.SelectedIndex,numDaysPrevious.ToString());
#endif
            #endregion

            #region Get number of days in current month.
            numDaysCurrent =
                DateTime.DaysInMonth(Int32.Parse(yrComboBox.SelectedValue.ToString()),
                                                          moComboBox.SelectedIndex + 1);
#if DEBUG
            Console.WriteLine("Number of days in current month, {0}: {1}", (moComboBox.SelectedIndex + 1), numDaysCurrent.ToString());
#endif
            #endregion

            IEnumerator labels = monthViewDayNumbers.GetEnumerator();

            TextBlock dayNumberTextBlock;

            #region Begin the iteration.
            labels.MoveNext();

            #region Populate day values pertaining to the previous month, if there are any.
            while (numDaysPrevious - startDay < numDaysPrevious)
            {
                dayNumberTextBlock = (TextBlock)labels.Current;
                dayNumberTextBlock.Opacity = .5;
                dayNumberTextBlock.Text = (numDaysPrevious - (startDay--)).ToString();
#if DEBUG
                Console.WriteLine("Setting text... {0}", dayNumberTextBlock.Text);
#endif
                labels.MoveNext();
            }
            #endregion

            /* Might need this later....
             * 
            // If there were days to populate from the previous month...
            if (startDay < 0)
                startDay *= -1;
            // Else continue normally...
            else
             * */
            startDay++;

            #region Populate the day values of the current month...
            while (startDay <= numDaysCurrent)
            {
                dayNumberTextBlock = (TextBlock)labels.Current;
                dayNumberTextBlock.Opacity = 1;
                dayNumberTextBlock.Text = (startDay++).ToString();
#if DEBUG
                Console.WriteLine("Setting text... {0}", dayNumberTextBlock.Text);
#endif
                labels.MoveNext();
            }
            #endregion

            startDay = 1;

            #region Populate the remaining days in the calendar grid.
            try
            {
                do
                {
                    dayNumberTextBlock = (TextBlock)labels.Current;
                    dayNumberTextBlock.Opacity = .5;
                    dayNumberTextBlock.Text = (startDay++).ToString();
#if DEBUG
                    Console.WriteLine("Setting text... {0}", dayNumberTextBlock.Text);
#endif
                } while (labels.MoveNext());
            }
            catch (InvalidOperationException invOpEx)
            {
                Console.WriteLine("\nCan not populate any days from the next month!\n");
                Console.WriteLine("Enumeration failed: already reached the end of enumeration." +
                                    "\n\nException detail:\n{0}\n\nStack trace:\n{1}\n", invOpEx.Message,
                                                                                         invOpEx.StackTrace);
            }
            #endregion

            #endregion
        }

        private void switchDayOrder()
        {
            if (lblsMonthNames == null)
                return;

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
#if DEBUG
            Console.WriteLine("Week index: {0}",index);
#endif
            foreach (Border value in monthViewWeekBorders.Keys)
            {
                if (value.Name.Equals( week.Name )) {
#if DEBUG
                    Console.WriteLine("\nWeek set to {0}", value.Name);
#endif
                    value.BorderBrush = System.Windows.Media.Brushes.Black;
                    value.BorderThickness = new Thickness( 2 );
                }
                else {
#if DEBUG
                    Console.WriteLine("Not selecting week {0}!", value.Name);
#endif           
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

        private void selectDay(int dayNumber)
        {
            foreach (TextBlock t in monthViewDayNumbers)
            {
                if (t.Text.Equals(dayNumber.ToString()) && (t.Opacity == 1))
                {

                    ((Border)((System.Windows.Controls.Label)t.Parent).Parent).BorderThickness = new Thickness(1.5);
                    ((Border)((System.Windows.Controls.Label)t.Parent).Parent).BorderBrush = System.Windows.Media.Brushes.Black;
                    ((System.Windows.Controls.Label)t.Parent).BorderThickness = new Thickness(1);
                    ((System.Windows.Controls.Label)t.Parent).BorderBrush = System.Windows.Media.Brushes.Black;
                    t.FontWeight = FontWeights.Bold;
                    t.FontSize++;
                }
                else
                {
                    if (t.FontWeight.Equals(FontWeights.Bold))
                    {
                        ((Border)((System.Windows.Controls.Label)t.Parent).Parent).BorderThickness = new Thickness(1);
                        ((Border)((System.Windows.Controls.Label)t.Parent).Parent).BorderBrush = System.Windows.Media.Brushes.DimGray;
                        ((System.Windows.Controls.Label)t.Parent).BorderThickness = new Thickness(.5);
                        ((System.Windows.Controls.Label)t.Parent).BorderBrush = System.Windows.Media.Brushes.DimGray;
                        t.FontWeight = FontWeights.Normal;
                        t.FontSize--;
                    }
                }
            }
        }

        private int getWeekInMonth() {

            int dayOffset = 0;

            if(mondayFirst)
                dayOffset = getFirstDayOfMonthOffset() - 1;
            else
                dayOffset = getFirstDayOfMonthOffset();

            int wk =
                (int)Math.Ceiling
                    ((double)(DateTime.Now.Day + dayOffset) / 7.0);
#if DEBUG
            Console.WriteLine("\nDetected week: {0}",wk);
#endif
            return wk;
        }

        private int getWeekOfSelectedDay()
        {
            int dayOffset = mondayFirst ? getFirstDayOfMonthOffset() - 1 :
                                            getFirstDayOfMonthOffset();
#if DEBUG
            Console.WriteLine("Current first-day offset: {0}", dayOffset);
#endif
            int wk = (int)Math.Ceiling
                        ((double)(dayComboBox.SelectedIndex + 1 + dayOffset) / 7.0);

            if (wk < 1) wk = 1;
#if DEBUG
            Console.WriteLine("\nDetected week from selected day: {0}", wk);
#endif
            return wk;
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

        private void iPlanMain_LocationChanged(object sender, EventArgs e)
        {
            wkCalGridContainerScrollViewerHeight = wkCalGridContainerScrollViewer.ActualHeight;
            wkCalGridContainerScrollViewerWidth = wkCalGridContainerScrollViewer.ActualWidth;

            if (timeBlocks.Count > 0)
                while (tBlocks.MoveNext())
                    if (tBlocks.Current.IsEnabled)
                        tBlocks.Current.setPxDiff(tBlocks.Current.Left - this.Left,
                                                        tBlocks.Current.Top - this.Top);
            tBlocks.Dispose();

            if(!checkTBlockVisibleState())
                normalizeTBlockVisibility();
        }

        private void iPlanMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void iPlanMain_windowStateChanged(object sender, EventArgs e)
        {
            tBlocks = timeBlocks.GetEnumerator();

            if (this.WindowState == WindowState.Minimized)
                while (tBlocks.MoveNext())
                    tBlocks.Current.Hide();

            else if (this.WindowState == WindowState.Maximized ||
                            this.WindowState == WindowState.Normal)
                while (tBlocks.MoveNext())
                    if (!tBlocks.Current.IsVisible)
                        tBlocks.Current.Show();

            tBlocks.Dispose();

            if (!checkTBlockVisibleState())
                normalizeTBlockVisibility();
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
#if DEBUG
            Console.WriteLine("\n\nButton name: {0}\n", buttonName);
#endif
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

        private void file_quit(object sender, EventArgs e)
        {
            this.Close();
        }

        // Open the "About" information window.
        private void about_Click(object sender, RoutedEventArgs e)
        {
            iPlAbout aboutWin = new iPlAbout();
            aboutWin.ShowDialog();
        }

        // Changes the view mode of the main iPlan window from week
        // view to month view.
        private void chgViewMode(object sender, RoutedEventArgs e)
        {
#if DEBUG
            this.rStatBarTxt.Text = "Changing views...";
#endif      
            #region If we are in week view...
            
            if (!view)
            {
                view = true;

                // Make the week view hidden
                this.wkCalGridContainerBorder.Visibility = Visibility.Hidden;
                
                // Make all the time blocks hidden
                tBlocks = timeBlocks.GetEnumerator();

                // Hide all the timeblocks if we have existing time blocks.
                if (timeBlocks.Count > 0 && this.IsActive)
                    while (tBlocks.MoveNext())
                        tBlocks.Current.Hide();

                tBlocks.Dispose();

                // Make the month view visible
                this.moCalGridContainerBorder.Visibility = Visibility.Visible;
#if DEBUG
                Console.WriteLine("Checking tBlocks visibility...");
#endif
                if (timeBlocks.Count > 0 && !checkTBlockVisibleState()) {
#if DEBUG
                    Console.WriteLine("Normalizing visibility...");
#endif
                    normalizeTBlockVisibility();
                }
                else
#if DEBUG
                    Console.WriteLine("Ok.");
#endif
                this.rStatBarTxt.Text = "Month View."
                    + " View schedule details to help plan for "
                    + "long term goals. Easily organize your "
                    + "thoughts and notify your team of changes"
                    + " and new ideas.";
            }

            #endregion

            #region If we are in month view...

            else
            {
                view = false;

                this.moCalGridContainerBorder.Visibility = Visibility.Hidden;
                
                this.wkCalGridContainerBorder.Visibility = Visibility.Visible;

                // Make all the time blocks visible.
                tBlocks = timeBlocks.GetEnumerator();

                // Show all the timeblocks if we have existing time blocks.
                if (timeBlocks.Count > 0 && this.IsActive)
                    while (tBlocks.MoveNext())
                        tBlocks.Current.Show();

                tBlocks.Dispose();
#if DEBUG
                Console.WriteLine("Checking tBlocks visibility...");
#endif
                if (timeBlocks.Count > 0 && !checkTBlockVisibleState()) {
#if DEBUG
                    Console.WriteLine("Normalizing visibility...");
#endif
                    normalizeTBlockVisibility();
                }
                else
#if DEBUG
                    Console.WriteLine("Ok.");
#endif
                this.rStatBarTxt.Text = "Week View. Build your "
                    + "Schedule here and send notifications about"
                    + " schedule changes.";
            }

            #endregion
        }

        private void enterOrLeaveEditMode(object sender, RoutedEventArgs e)
        {
            ed = !ed;
            if (ed)
                rStatBarTxt.Text = "Edit mode enabled";
            else
                rStatBarTxt.Text = "Edit mode disabled";

#if DEBUG
            Console.WriteLine("Edit mode status: {0}", ed);
#endif
        }

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

        private void addTimeBlock_Click(object sender, RoutedEventArgs e)
        {
            if (!view)
                addBlock();
            else
            {
                System.Windows.MessageBox.Show("You can not add a timeblock"
                    + " in Month View. Please change to Week View to add "
                    + "a time block.");
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

        private void selectToday_Click(object sender, RoutedEventArgs e)
        {
            moComboBox.SelectedIndex = DateTime.Today.Month - 1;
            dayComboBox.SelectedIndex = DateTime.Today.Day - 1;
            yrComboBox.SelectedValue = DateTime.Today.Year.ToString();
            wkComboBox.SelectedIndex = getWeekInMonth() - 1;
        }

        #endregion

        #region Week Grid Events

        private void wkCalGridContainerScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (timeBlocks.Count > 0)
            {
                tBlocks = timeBlocks.GetEnumerator();

                while (tBlocks.MoveNext())
                    tBlocks.Current.setScrollContentOffset(wkCalGridContainerScrollViewer.VerticalOffset);

                tBlocks.Dispose();
            }
        }

        private void weekGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            mousePositionTmp = System.Windows.Input.Mouse.GetPosition((IInputElement)this);
        }

        private void wkCalGridContainerScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mouseWheelDeltaTmp = e.Delta;

            Console.WriteLine("tester");
            scroller = (ScrollViewer)sender;

            if (mouseWheelDeltaTmp > 0)
                scroller.LineUp();
            else
                scroller.LineDown();
        }

        #endregion

        #region Main window Combo/Text Box events

        private void moComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (yrComboBox.SelectedValue != null &&
                ((getFirstDayOfMonthOffset() == 0 && mondayFirst) ||
                 (getFirstDayOfMonthOffset() > 4 && !mondayFirst)))
            {
                switchDayOrder();
            }

            if (yrComboBox.SelectedValue != null)
            {
                selectWeek(monthViewWeekBorders.ElementAt(getWeekOfSelectedDay() - 1).Key, getWeekOfSelectedDay() - 1);
                populateMonthViewDayLabels();
                resetDayItemCollection();
            }
        }

        private void wkComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("Week combo box modified....");
#endif
            selectWeek
               (monthViewWeekBorders.ElementAt(wkComboBox.SelectedIndex).Key,
                                                     wkComboBox.SelectedIndex);
            int daysInSelectedMonth = DateTime.DaysInMonth
                                        (Int32.Parse
                                            ((string)yrComboBox.SelectedValue),
                                                    moComboBox.SelectedIndex + 1);

            int wkDelta = wkComboBox.SelectedIndex - (getWeekOfSelectedDay() - 1);
            int daysToShift = wkDelta * 7;
            int newDay = daysToShift + dayComboBox.SelectedIndex;
#if DEBUG
            Console.WriteLine("Week delta: {0}\nDays to shift:{1}\nNew Day: {2}",
                                                        wkDelta, daysToShift, newDay);
#endif
            if (newDay >= 0 &&
                newDay < daysInSelectedMonth)
                dayComboBox.SelectedIndex += daysToShift;
            else
#if DEBUG
                Console.WriteLine("\nNew day out of range...");
#endif
        }

        private void dayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("Day combo box modified....");
            Console.WriteLine("Selected day index: {0}", dayComboBox.SelectedIndex);
#endif
            selectDay(dayComboBox.SelectedIndex + 1);

            if(moComboBox.SelectedValue != null  &&
                yrComboBox.SelectedValue != null   )
                wkComboBox.SelectedIndex = getWeekOfSelectedDay() - 1;
        }

        private void yrComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (moComboBox.SelectedValue != null &&
                ((getFirstDayOfMonthOffset() == 0 && mondayFirst) ||
                 (getFirstDayOfMonthOffset() > 4 && !mondayFirst)))
            {
                switchDayOrder();
            }

            if (moComboBox.SelectedValue != null)
            {
                selectWeek(monthViewWeekBorders.ElementAt(getWeekInMonth() - 1).Key, getWeekInMonth() - 1);
                populateMonthViewDayLabels();
            }

            selectDay(dayComboBox.SelectedIndex + 1);
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

        // Detect visibility of time blocks,
        // clean up if necessary.
        private bool checkTBlockVisibleState()
        {
#if DEBUG
            Console.WriteLine("Checking time block visibility normality...\n");
#endif
            //Just in case.
            inactiveBlocks.Clear();

            #region If there is at least one time block...
            if (timeBlocks.Count > 0)
            {
                bool prev = false;
                tBlocks = timeBlocks.GetEnumerator();
                tBlocks.MoveNext();

                prev = tBlocks.Current.IsVisible;

                do
                {
                    // If the current time block isn't enabled...
                    // make sure to remove it from the list.
                    if (!tBlocks.Current.IsEnabled) {
#if DEBUG
                        Console.WriteLine("Inactive block {0} will be removed.", tBlocks.Current);
#endif
                        inactiveBlocks.Add(tBlocks.Current);
                    }

                    // If there's a time b
                    else if (prev != tBlocks.Current.IsVisible)
                    {
                        inactiveBlocks.Clear();
#if DEBUG
                        Console.WriteLine("Visibility requires normalization!");
#endif
                        return false;
                    }

                    // Is the previous time block visible?
                    prev = tBlocks.Current.IsVisible;

                } while (tBlocks.MoveNext());

                // Clean up...
                tBlocks.Dispose();

                // Get rid of inactive timeblocks from the list.
                if (inactiveBlocks.Count > 0)
                    cleanTimeBlockList(inactiveBlocks);
            }
            #endregion

            // Else all is well by default since
            // there are no time blocks.
            return true;
        }

        // Normalize and clean the block list.
        private void normalizeTBlockVisibility()
        {
#if DEBUG
            Console.WriteLine("Normalizing blocks...");
#endif
            //Just in case.
            inactiveBlocks.Clear();

            tBlocks = timeBlocks.GetEnumerator();

            #region If we are in week view...
            // Normalize to Visible.
            // Check for inactive blocks.
            if (!view)
                while (tBlocks.MoveNext())
                {
                    if (!tBlocks.Current.IsEnabled)
                    {
#if DEBUG
                        Console.WriteLine("Inactive block {0} will be removed.", tBlocks.Current);
#endif
                        inactiveBlocks.Add(tBlocks.Current);
                    }
                    else
                        tBlocks.Current.Visibility = Visibility.Visible;
                }
            #endregion

            #region If we are in Month view...
            // Normalize to hidden.
            // Check for inactive blocks.
            else
                while (tBlocks.MoveNext())
                {
                    if (!tBlocks.Current.IsEnabled)
                    {
#if DEBUG
                        Console.WriteLine("Inactive block {0} will be removed.", tBlocks.Current);
#endif
                        inactiveBlocks.Add(tBlocks.Current);
                    }
                    else
                        tBlocks.Current.Visibility = Visibility.Hidden;
                }
            #endregion

            tBlocks.Dispose();

            if (inactiveBlocks.Count > 0)
                cleanTimeBlockList(inactiveBlocks);
        }

        private void cleanTimeBlockList(ArrayList blocks)
        {
            // Get rid of inactive timeblocks from the list.
            if (blocks.Count > 0)
            {
#if DEBUG
                Console.WriteLine("Removing inactive blocks...");
#endif
                foreach (TimeBlock block in inactiveBlocks)
                    timeBlocks.Remove(block);
            }

            blocks.Clear();
        }

        private void addBlock()
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

            // Add it to the block list.
            timeBlocks.AddFirst(memberTimeBlock);

            // Show the block.
            memberTimeBlock.UpdateLayout();
            memberTimeBlock.Focusable = true;
            memberTimeBlock.Show();

            // Set block position vars.
            pxDiffBlockLeft = memberTimeBlock.Left - this.Left;
            pxDiffBlockTop = memberTimeBlock.Top - this.Top;
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
