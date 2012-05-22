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
                "iPlan v." +
                Assembly.
                    GetAssembly(typeof(iPlan_Main)).
                        GetName().Version.ToString();
        }

        #endregion

        #region Event Handlers

        public void iPlan_LoadCal_click(object sender, RoutedEventArgs newCalEventArgs)
        {
            OpenFileDialog loadFile = new OpenFileDialog();

            loadFile.CheckFileExists = true;
            loadFile.CheckPathExists = true;
            loadFile.Multiselect = false;
            loadFile.Title = "Open iPlan Calendar file";
            loadFile.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            loadFile.DereferenceLinks = true;
            loadFile.DefaultExt = ".csv";
            loadFile.SupportMultiDottedExtensions = true;
            loadFile.AddExtension = true;
            loadFile.Filter = "Comma Seperated Values files (*.csv) | *.csv | All Files (*.*) | *.*";

            if (loadFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                calendar = new iPlan_Main(loadFile.OpenFile(), this);
                this.Hide();
                calendar.Show();
                iPlan_NewCal_click_Cancel(sender, newCalEventArgs);
                txtNewCalName.Clear();
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

        public void iPlan_NewCal_TxtBox_EnterKeyHit(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key.Equals(Key.Enter))
                iPlan_MakeNewCal_click(sender, e);
        }

        private void iPlan_NewCal_click_Cancel(object sender, RoutedEventArgs e)
        {
            if (welcomeStatBarText.FontSize > 12)
            {
                welcomeStatBarText.Content = "iPlan 1.1.513";
                welcomeStatBarText.FontSize = 12;
            }
            newCalStackPanel.Visibility = Visibility.Hidden;
            dockIPlanWelcome.Visibility = Visibility.Visible;
        }

        private void iPlan_NewCal_click(object sender, RoutedEventArgs e)
        {
            newCalStackPanel.Visibility = Visibility.Visible;
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
    }
}
