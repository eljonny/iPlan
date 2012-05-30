#region Using Assemblies

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;

#endregion

namespace GUIProj1
{
    /// <summary>
    /// Interaction logic for iPlan_Welcome.xaml
    /// </summary>
    public partial class iPlan_Welcome : Window
    {
        #region Welcome Window Variables

        private iPlan_Main calendar = null;

        #endregion

        #region Construction

        public iPlan_Welcome()
        {
            InitializeComponent();

            welcomeStatBarText.Content =
                "iPlan v" +
                Assembly.
                    GetAssembly(typeof(iPlan_Main)).
                        GetName().Version.ToString();

            NameScope.SetNameScope(this, new NameScope());
        }

        #endregion

        #region Event Handlers

        #region Click Events

        public void iPlan_LoadCal_click(object sender, RoutedEventArgs newCalEventArgs)
        {
            OpenFileDialog loadFile = new OpenFileDialog();

            setupFileDialog(loadFile);

            if (loadFile.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
                (loadFile.FileName.EndsWith(".ipla") || loadFile.FileName.EndsWith(".csv")))
            {
                Stream cal;

                try
                {
                    cal = loadFile.OpenFile();

                    tryLoadFile(cal, loadFile);

                    calendar = new iPlan_Main(loadFile.OpenFile(), this);
                    this.Hide();
                    calendar.Show();
                    iPlan_NewCal_click_Cancel(sender, newCalEventArgs);
                    txtNewCalName.Clear();
                }
                catch (ArgumentNullException argNullEx)
                {
                    Console.WriteLine("\nInvalid argument {0}; source: {1}", argNullEx.ParamName, argNullEx.Source);
                    Console.WriteLine("{0}\n{1}", argNullEx.Message, argNullEx.StackTrace);
                }
                catch (ArgumentException argEx)
                {
                    Console.WriteLine("\nInvalid argument {0}; source: {1}", argEx.ParamName, argEx.Source);
                    Console.WriteLine("\n{0}\n{1}", argEx.Message, argEx.StackTrace);
                }
                catch (OutOfMemoryException outOfMemEx)
                {
                    Console.WriteLine("Source of memory overflow: {0}", outOfMemEx.Source);
                    Console.WriteLine("\n{0}\n{1}", outOfMemEx.Message, outOfMemEx.StackTrace);
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine("Source of IO error: {0}", ioEx.Source);
                    Console.WriteLine("\n{0}\n{1}", ioEx.Message, ioEx.StackTrace);
                }
                catch (NullReferenceException nullRefEx)
                {
                    Console.WriteLine("Source of Null Reference Exception: {0}", nullRefEx.Source);
                    Console.WriteLine("\n{0}\n{1}", nullRefEx.Message, nullRefEx.StackTrace);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unknown exception; source: {0}", ex.Source);
                    Console.WriteLine("\n{0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
            else
            {
                loadFileFailed();
            }
        }

        public void iPlan_MakeNewCal_click(object sender, RoutedEventArgs newCalEventArgs)
        {
            if(!txtNewCalName.Text.Equals(""))
            {
                calendar = new iPlan_Main(txtNewCalName.Text, this);
                this.Hide();
                calendar.Show();
                iPlan_NewCal_click_Cancel(sender, newCalEventArgs);
                txtNewCalName.Clear();
            }
            else {
                txtNewCalName.Focus();
                welcomeStatBarText.Content = "You need to enter a calendar name!";
                welcomeStatBarText.FontSize = 14;
            }
        }

        private void iPlan_NewCal_click_Cancel(object sender, RoutedEventArgs e)
        {
            if (welcomeStatBarText.FontSize > 12)
            {
                welcomeStatBarText.Content = "iPlan v " +
                                             Assembly.
                                                GetAssembly(typeof(iPlan_Main)).
                                                    GetName().Version.ToString();
                welcomeStatBarText.FontSize = 12;
            }

            dockIPlanWelcome.Visibility = Visibility.Visible;
            newCalPanelFadeOutTransition();
            welcomePanelFadeInTransition();
            newCalStackPanel.Visibility = Visibility.Hidden;
        }

        private void iPlan_NewCal_click(object sender, RoutedEventArgs e)
        {
            newCalStackPanel.Visibility = Visibility.Visible;
            welcomePanelFadeOutTransition();
            newCalPanelFadeInTransition();
            dockIPlanWelcome.Visibility = Visibility.Hidden;

            txtNewCalName.Focus();
        }

        private void iPlan_Exit_click(object sender, RoutedEventArgs e)
        {
            if (calendar != null)
                calendar.Close();
            this.Close();
        }

        private void iPlan_TitleBar_click(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void iPlan_ContextMinimize_click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void iPlan_ContextAbout_click(object sender, RoutedEventArgs e)
        {
            new iPlAbout().Show();
        }

        #endregion

        #region Keyboard events

        public void iPlan_NewCal_TxtBox_EnterKeyHit(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key.Equals(Key.Enter))
                iPlan_MakeNewCal_click(sender, e);
        }

        #endregion

        #region Title Bar close and minimize handlers

        private void titleBarX_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((OuterGlowBitmapEffect)titleBarXBack.BitmapEffect).GlowSize = 3;
            ((OuterGlowBitmapEffect)titleBarXForward.BitmapEffect).GlowSize = 3;
        }

        private void titleBarX_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((OuterGlowBitmapEffect)titleBarXBack.BitmapEffect).GlowSize = 0;
            ((OuterGlowBitmapEffect)titleBarXForward.BitmapEffect).GlowSize = 0;
        }

        private void titleBarX_click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
                ((OuterGlowBitmapEffect)titleBarXBack.BitmapEffect).GlowSize = 5;
                ((OuterGlowBitmapEffect)titleBarXForward.BitmapEffect).GlowSize = 5;
        }

        private void titleBarX_clickRelease(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (titleBarXContainer.IsMouseOver)
            {
                ((OuterGlowBitmapEffect)titleBarXBack.BitmapEffect).GlowSize = 0;
                ((OuterGlowBitmapEffect)titleBarXForward.BitmapEffect).GlowSize = 0;
                
                if (calendar != null)
                    calendar.Close();

                this.Close();
            }
        }

        private void titleBarMin_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((OuterGlowBitmapEffect)titleBar_.BitmapEffect).GlowSize = 3;
        }

        private void titleBarMin_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((OuterGlowBitmapEffect)titleBar_.BitmapEffect).GlowSize = 0;
        }

        private void titleBarMin_click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((OuterGlowBitmapEffect)titleBar_.BitmapEffect).GlowSize = 5;
        }

        private void titleBarMin_clickRelease(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (titleBarMinimizeContainer.IsMouseOver)
            {
                ((OuterGlowBitmapEffect)titleBar_.BitmapEffect).GlowSize = 0;

                this.WindowState = WindowState.Minimized;
            }
        }

        #endregion

        #endregion

        #region File Operations

        private void tryLoadFile(Stream cal, OpenFileDialog getfile)
        {
            StreamReader possibleCalendar = new StreamReader(cal);

            string fileContent = possibleCalendar.ReadLine();
            string[] data = fileContent.Split(new char[] { ',', ' ' });

            // Validate data read...
            if (data.Length > 0)
            {
                if (data[0].Equals("##begin#iplan#calendar##"))
                {
                    Console.WriteLine("\nFile {0} ok, creating calendar...",
                                      getfile.FileName);
                    possibleCalendar.Close();
                }

                else
                {
                    fileNameError(getfile);
                    throw new ArgumentException("Invalid iPlan calendar file!",
                                                               getfile.FileName);
                }
            }
            else throw new ArgumentException("Invalid iPlan calendar file!",
                                                               getfile.FileName);
        }

        private void fileNameError(OpenFileDialog loadFile)
        {
            if (loadFile.FileName.EndsWith(".ipla"))
            {
                Console.WriteLine("\n\nERROR:\niPlan calendar file {0}" +
                                  " has been tampered with or is corrupted!",
                                  loadFile.FileName);
            }
            else
            {
                Console.WriteLine("\n\nERROR:\nCalendar file {0}" +
                                  " has been tampered with or is corrupted!",
                                  loadFile.FileName);
            }
        }

        private void setupFileDialog(OpenFileDialog loadFile)
        {
            loadFile.CheckFileExists = true;
            loadFile.CheckPathExists = true;
            loadFile.Multiselect = false;
            loadFile.Title = "Open iPlan Calendar file";
            loadFile.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            loadFile.DereferenceLinks = true;
            loadFile.DefaultExt = ".ipla";
            loadFile.SupportMultiDottedExtensions = true;
            loadFile.AddExtension = true;
            loadFile.Filter = "iPlan Calendar File (*.ipla) | *.ipla |Comma Separated Values File (*.csv) | *.csv |All Files (*.*) | *.*";
        }

        #endregion

        #region Animations

        private void loadFileFailed()
        {
            this.RegisterName(welcomeStatBarText.Name, welcomeStatBarText);

            Storyboard playStatBarStr = new Storyboard();
            StringAnimationUsingKeyFrames statBarAnimation = new StringAnimationUsingKeyFrames();

            KeyTime animTime;

            animTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0));
            DiscreteStringKeyFrame frame1 = new DiscreteStringKeyFrame("Cannot load invalid calendar file.", animTime);

            animTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 3));
            DiscreteStringKeyFrame frame2 =
                new DiscreteStringKeyFrame("iPlan v" +
                                            Assembly.
                                               GetAssembly(typeof(iPlan_Main)).
                                                    GetName().Version.ToString(), animTime);

            statBarAnimation.KeyFrames.Add(frame1);
            statBarAnimation.KeyFrames.Add(frame2);

            statBarAnimation.BeginTime = new TimeSpan(0, 0, 0);
            statBarAnimation.Duration = new Duration(new TimeSpan(0, 0, 3));
            statBarAnimation.FillBehavior = FillBehavior.Stop;

            playStatBarStr.Children.Add(statBarAnimation);
            Storyboard.SetTargetName(statBarAnimation, welcomeStatBarText.Name);
            Storyboard.SetTargetProperty(statBarAnimation, new PropertyPath(StatusBarItem.ContentProperty));
            playStatBarStr.Begin(this);

            this.UnregisterName(welcomeStatBarText.Name);
        }

        private void welcomePanelFadeOutTransition()
        {
            this.RegisterName(dockIPlanWelcome.Name, dockIPlanWelcome);

            Storyboard welcomeToNewStoryboard = new Storyboard();

            DoubleAnimation welcomePanelOpacityAnimation = new DoubleAnimation();
            welcomePanelOpacityAnimation.From = 1.0;
            welcomePanelOpacityAnimation.To = 0.0;
            welcomePanelOpacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            welcomePanelOpacityAnimation.AutoReverse = false;
            welcomePanelOpacityAnimation.FillBehavior = FillBehavior.HoldEnd;

            welcomeToNewStoryboard.Children.Add(welcomePanelOpacityAnimation);
            Storyboard.SetTargetName(welcomePanelOpacityAnimation, dockIPlanWelcome.Name);
            Storyboard.SetTargetProperty(welcomePanelOpacityAnimation, new PropertyPath(DockPanel.OpacityProperty));
            welcomeToNewStoryboard.Begin(this);

            this.UnregisterName(dockIPlanWelcome.Name);
        }

        private void welcomePanelFadeInTransition()
        {
            this.RegisterName(dockIPlanWelcome.Name, dockIPlanWelcome);

            Storyboard welcomeToNewStoryboard = new Storyboard();

            DoubleAnimation welcomePanelOpacityAnimation = new DoubleAnimation();
            welcomePanelOpacityAnimation.From = 0.0;
            welcomePanelOpacityAnimation.To = 1.0;
            welcomePanelOpacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            welcomePanelOpacityAnimation.AutoReverse = false;
            welcomePanelOpacityAnimation.FillBehavior = FillBehavior.HoldEnd;

            welcomeToNewStoryboard.Children.Add(welcomePanelOpacityAnimation);
            Storyboard.SetTargetName(welcomePanelOpacityAnimation, dockIPlanWelcome.Name);
            Storyboard.SetTargetProperty(welcomePanelOpacityAnimation, new PropertyPath(DockPanel.OpacityProperty));
            welcomeToNewStoryboard.Begin(this);

            this.UnregisterName(dockIPlanWelcome.Name);
        }

        private void newCalPanelFadeOutTransition()
        {
            this.RegisterName(newCalStackPanel.Name, newCalStackPanel);

            Storyboard newToWelcomeStoryboard = new Storyboard();

            DoubleAnimation newCalPanelOpacityAnimation = new DoubleAnimation();
            newCalPanelOpacityAnimation.From = 1.0;
            newCalPanelOpacityAnimation.To = 0.0;
            newCalPanelOpacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            newCalPanelOpacityAnimation.AutoReverse = false;
            newCalPanelOpacityAnimation.FillBehavior = FillBehavior.HoldEnd;

            newToWelcomeStoryboard.Children.Add(newCalPanelOpacityAnimation);
            Storyboard.SetTargetName(newCalPanelOpacityAnimation, newCalStackPanel.Name);
            Storyboard.SetTargetProperty(newCalPanelOpacityAnimation, new PropertyPath(StackPanel.OpacityProperty));
            newToWelcomeStoryboard.Begin(this);

            this.UnregisterName(newCalStackPanel.Name);
        }

        private void newCalPanelFadeInTransition()
        {
            this.RegisterName(newCalStackPanel.Name, newCalStackPanel);

            Storyboard newToWelcomeStoryboard = new Storyboard();

            DoubleAnimation newCalPanelOpacityAnimation = new DoubleAnimation();
            newCalPanelOpacityAnimation.From = 0.0;
            newCalPanelOpacityAnimation.To = 1.0;
            newCalPanelOpacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(.5));
            newCalPanelOpacityAnimation.AutoReverse = false;
            newCalPanelOpacityAnimation.FillBehavior = FillBehavior.HoldEnd;

            newToWelcomeStoryboard.Children.Add(newCalPanelOpacityAnimation);
            Storyboard.SetTargetName(newCalPanelOpacityAnimation, newCalStackPanel.Name);
            Storyboard.SetTargetProperty(newCalPanelOpacityAnimation, new PropertyPath(StackPanel.OpacityProperty));
            newToWelcomeStoryboard.Begin(this);

            this.UnregisterName(newCalStackPanel.Name);
        }

        #endregion
    }
}
