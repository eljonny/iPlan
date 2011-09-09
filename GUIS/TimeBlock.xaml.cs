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

namespace GUIProj1
{
    /// <summary>
    /// Interaction logic for TimeBlock.xaml
    /// </summary>
    /// Author: Jonathan Hyry
    public partial class TimeBlock : Window
    {
        string[] content = new string[100];
        double[,] probData = new double[100,2];
        private double pxDiffL = 0, pxDiffT = 0,
            maxPxDiffLeft,maxPxDiffRight,maxPxDiffTop,
            maxPxDiffBottomMax,maxPxDiffBottomCurrent,
            maxPxDiffTopScrollCompensation;
        private gridObject gO;
        //private Point temp1, temp2;
        private int exCounter = 0;
        private Window1 iPlMain;
        public TimeBlock()
        {
            InitializeComponent();
            gO = new gridObject
                (0,0,pxDiffL,pxDiffT,"08:00","17:00",true,content,
                "emp",System.DateTime.Now.ToString(),probData);
        }

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
                if(exCounter == 0)
                {
                    MessageBox.Show(invOpEx.ToString());
                    exCounter++;
                }
                OnMouseLeftButtonUp(e);
            }
        }

        private void dragMoveEnd(object sender, MouseButtonEventArgs e)
        {
            setPxDiff(this.Left - iPlMain.Left,this.Top - iPlMain.Top);
            moveToClosest();
        }

        /*
        private Point coordsForSnap()
        {
            
        }*/

        private void removeBlock_Click(object sender,RoutedEventArgs e)
        {
            this.Close();
        }

        public void setPxDiff(double l, double t)
        {
            pxDiffL = l;
            pxDiffT = t;
            gO.setPxDiff(l,t);
        }

        private void tBlockLocationChanged(object sender, EventArgs e)
        {
            double currentBlockHeight = this.ActualHeight;
            maxPxDiffLeft = iPlMain.Left + 196;
            maxPxDiffTop = iPlMain.Top + 101;
            maxPxDiffRight = iPlMain.Left + 821;
            maxPxDiffBottomMax = iPlMain.Top + 771;
            maxPxDiffBottomCurrent = iPlMain.Top
                + iPlMain.gridHolder.ActualHeight + 42;
            if(this.Top < maxPxDiffTop)
            {
                if(iPlMain.wkCalGridContainer.VerticalOffset <= 30
                    && iPlMain.wkCalGridContainer.VerticalOffset != 0)
                    this.Top = maxPxDiffTop - maxPxDiffTopScrollCompensation;
                else if(iPlMain.wkCalGridContainer.VerticalOffset>30)
                    this.Top = maxPxDiffTop - maxPxDiffTopScrollCompensation;
                else
                    this.Top = maxPxDiffTop;
                if(iPlMain.wkCalGridContainer.VerticalScrollBarVisibility
                   == ScrollBarVisibility.Auto)
                    iPlMain.wkCalGridContainer.LineUp();
            }
            if(this.Left < maxPxDiffLeft)
                this.Left = maxPxDiffLeft;
            if(this.Left > maxPxDiffRight)
                this.Left = maxPxDiffRight;
            if(iPlMain.WindowState == WindowState.Normal)
            {
                if(this.Top + currentBlockHeight > maxPxDiffBottomCurrent)
                {
                    this.Top = maxPxDiffBottomCurrent - currentBlockHeight;
                    if(iPlMain.wkCalGridContainer.VerticalScrollBarVisibility
                        == ScrollBarVisibility.Auto)
                    {
                        iPlMain.wkCalGridContainer.LineDown();
                    }
                }
            }
            else if(iPlMain.WindowState == WindowState.Maximized)
            {
                if(this.Top + currentBlockHeight > maxPxDiffBottomMax)
                {
                    this.Top = maxPxDiffBottomMax - currentBlockHeight;
                    if(iPlMain.wkCalGridContainer.VerticalScrollBarVisibility
                       == ScrollBarVisibility.Auto)
                        iPlMain.wkCalGridContainer.LineDown();
                }
            }
        }

        private void moveToClosest()
        {
            if(pxDiffL <= 261)
                this.Left = iPlMain.Left + 196;
            else if(pxDiffL <= 363.5)
                this.Left = iPlMain.Left + 299;
            else if(pxDiffL <= 471)
                this.Left = iPlMain.Left + 407;
            else if(pxDiffL <= 579)
                this.Left = iPlMain.Left + 515;
            else if(pxDiffL <= 682)
                this.Left = iPlMain.Left + 618;
            else if(pxDiffL <= 784)
                this.Left = iPlMain.Left + 720;
            else if(pxDiffL > 784)
                this.Left = iPlMain.Left + 821;

            setPxDiff(this.Left - iPlMain.Left,this.Top - iPlMain.Top);
        }

        public void setParental(Window1 parentalUnit)
        {
             iPlMain = parentalUnit;
        }

        public void setScrollContentOffset(double offSet)
        {
            maxPxDiffTopScrollCompensation = offSet;
        }

        public gridObject getGO()
        {
            return gO;
        }

        private void currentTimeBlock_Closing(object sender,EventArgs e)
        {
            this.IsEnabled = false;
        }
    }
}
