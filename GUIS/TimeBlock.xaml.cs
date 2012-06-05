#region Using Assemblies

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

#endregion

namespace GUIProj1
{
    /// <summary>
    /// Interaction logic for TimeBlock.xaml
    /// </summary>
    /// Author: Jonathan Hyry
    public partial class TimeBlock : Window
    {
        #region Time Block Variables

        string[] content = new string[100];
        LinkedList<ProbabilityFactor> probData = new LinkedList<ProbabilityFactor>();
        private Random tmp;
        private double pxDiffL = 0, pxDiffT = 0,
            maxPxDiffLeft,maxPxDiffRight,maxPxDiffTop,
            maxPxDiffBottomMax,maxPxDiffBottomCurrent,
            maxPxDiffTopScrollCompensation,scrollVertOffsetTmp=0,
            offSetDelta;

        private GridObject gObj;
        private Point temp1;//, temp2;
        private iPlan_Main iPlMain;

        #endregion

        #region Construction

        public TimeBlock(iPlan_Main parent)
        {
            setParental( parent );

            InitializeComponent();

            gObj = new GridObject();

            maxPxDiffLeft = iPlMain.mainGridButtonMenuColumn.ActualWidth + iPlMain.timeLabels.ActualWidth;
            maxPxDiffTop = iPlMain.menuComboLabelRow.ActualHeight + iPlMain.moCalGridLabelRow.ActualHeight;
            maxPxDiffRight = maxPxDiffLeft + iPlMain.wkCalGridContainerScrollViewer.ActualWidth;
            maxPxDiffBottomMax = maxPxDiffTop + iPlMain.wkGrid.ActualHeight;
            setParental(parent);
        }

        #endregion

        #region Event Handlers

        protected override void  OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
 	        base.OnMouseLeftButtonDown(e);

            this.dragMove(this,e);
        }

        private void dragMove(object sender,MouseButtonEventArgs e)
        {
            double iPlTop = iPlMain.Top, iPlLeft = iPlMain.Left;
            try
            {
                this.DragMove();
            }
            catch(InvalidOperationException invOpEx)
            {
                Console.WriteLine(invOpEx);
                OnMouseLeftButtonUp(e);
            }
        }

        private void dragMoveEnd(object sender, MouseButtonEventArgs e)
        {
            setPxDiff(this.Left - iPlMain.Left,this.Top - iPlMain.Top);
            snapToClosest();
        }

        private void removeBlock_Click(object sender,RoutedEventArgs e)
        {
            this.Close();
        }

        private void tBlockLocationChanged(object sender, EventArgs e)
        {
            double currentBlockHeight = this.ActualHeight;

            maxPxDiffBottomCurrent = maxPxDiffTop + iPlMain.wkCalGridContainerScrollViewer.ActualHeight;

            if (iPlMain.wkCalGridContainerScrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                maxPxDiffRight -= 15; // Scrollbar width == 15px
        }

        private void snapToClosest()
        {
            if (pxDiffL <= 261)
                this.Left = iPlMain.Left + 193;
            else if (pxDiffL <= 363.5)
                this.Left = iPlMain.Left + 299;
            else if (pxDiffL <= 471)
                this.Left = iPlMain.Left + 407;
            else if (pxDiffL <= 579)
                this.Left = iPlMain.Left + 515;
            else if (pxDiffL <= 682)
                this.Left = iPlMain.Left + 618;
            else if (pxDiffL <= 784)
                this.Left = iPlMain.Left + 720;
            else if (pxDiffL > 784)
                this.Left = iPlMain.Left + 821;

            this.setPxDiff(this.Left - iPlMain.Left, this.Top - iPlMain.Top);
        }

        private void currentTimeBlock_Closing(object sender, EventArgs e)
        {
            this.IsEnabled = false;
        }

        #endregion

        #region Miscellaneous Functions

        public void setPxDiff(double l, double t)
        {
            pxDiffL = l;
            pxDiffT = t;
            //gO.setPxDiff(l,t);
        }

        public void setParental(iPlan_Main parentalUnit)
        {
             iPlMain = parentalUnit;
        }

        public void setWindowStartup()
        {
            tmp = new Random();
            this.Left = iPlMain.Left + 193 + (tmp.NextDouble()*1000);
            this.Top = iPlMain.Top + 97 + (tmp.NextDouble()*100);
            this.setPxDiff(this.Left - iPlMain.Left,this.Top - iPlMain.Top);
            this.snapToClosest();
            tmp = null;
        }

        public void setWindowStartup(System.Windows.Point mBClckMkBlck)
        {
            temp1 = mBClckMkBlck;
            this.Left = iPlMain.Left + temp1.X;
            this.Top = iPlMain.Top + temp1.Y;
            setPxDiff(this.Left - iPlMain.Left,this.Top - iPlMain.Top);
            snapToClosest();
        }

        public void setScrollContentOffset(double offSet)
        {
            offSetDelta = 0;
            //System.Windows.MessageBox.Show(offSet.ToString());
            //System.Windows.MessageBox.Show(this.Top.ToString());
            if(offSet != scrollVertOffsetTmp)
                offSetDelta = offSet - scrollVertOffsetTmp;
            else
                offSetDelta = 0;
            maxPxDiffTopScrollCompensation = offSet;
            if(offSetDelta>0)
            {
                if(this.Top > (iPlMain.Top + maxPxDiffTop - 32)
                    && this.Visibility.Equals(Visibility.Visible))
                    this.Top -= offSetDelta;
            }
            else if(offSetDelta<0)
            {
                if(this.Top > (iPlMain.Top + maxPxDiffTop)
                    && this.Visibility.Equals(Visibility.Visible))
                    this.Top -= offSetDelta;
            }
            if(this.Top <= (iPlMain.Top + maxPxDiffTop - 30)
                && this.Visibility.Equals(Visibility.Visible))
                    this.Visibility = Visibility.Hidden;
            else if(this.Top > (iPlMain.Top + maxPxDiffTop - 30)
                && this.Visibility.Equals(Visibility.Hidden))
                this.Visibility = Visibility.Visible;
            scrollVertOffsetTmp = offSet;
        }

        public GridObject getGO()
        {
            return gObj;
        }

        #endregion
    }
}
