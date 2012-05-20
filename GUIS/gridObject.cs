using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
//  Author Jonathan Hyry
namespace GUIProj1
{
    public class gridObject
    {
        #region GridObject Variables

        private int row,col,id;
        private double[] px = new double[2];
        private double prob;
        private string timeBeg, timeEnd, type, date;
        private string[] gObjCont = null;
        private bool isPopulated;
        private double[,] probDet = null;

        #endregion

        #region Construction

        public gridObject()
        {
            Random rand = new Random();
            id = rand.Next();
            row=0;
            col=0;
            prob = 1;
            timeBeg = "00:00";
            timeEnd = "06:00";
            isPopulated = false;
            rand = null;
            probDet=new double[100,2];
            probDet[0,0]=1;
        }
        
        public gridObject(int r, int c, double pxDL, double pxDT, string tb,string te,
                                bool pop,string[] content,string t,string d,double[,] pD)
        {
            Random rand = new Random();
            id = rand.Next();
            prob = 0;
            row = r;
            col = c;
            px[0] = pxDL;
            px[1] = pxDT;
            timeBeg = tb;
            timeEnd = te;
            isPopulated = pop;
            gObjCont = content;
            probDet = pD;
            type = t;
            date = d;
            rand = null;
        }

        #endregion

        #region GridObject operations

        public void setCoords(int c, int r)
        {
            row = r;
            col = c;
        }

        public void setR(int r)
        {
            row = r;
        }

        public void setC(int c)
        {
            col = c;
        }

        public void setTimes(string b, string e)
        {
            timeBeg = b;
            timeEnd = e;
        }

        public void setType(string t)
        {
            type = t;
        }

        public void setPxDiff(double L, double T)
        {
            px[0] = L;
            px[1] = T;
        }

        public void setDate(string d)
        {
            date = d;
        }
        
        private bool checkPop()
        {
            if(timeBeg=="" || timeEnd=="")
                if(col == 0)
                    if(prob == 0)
                        if(gObjCont == null)
                            isPopulated = false;
            else
                isPopulated = true;
            return isPopulated;
        }

        public int getDay()
        {
            if(row==1)
                return col;
            else
                return (col + (7*row));
        }

        public string[] getTimeVals()
        {
            string[] begEndTimes = { timeBeg,timeEnd };
            return begEndTimes;
        }

        public void setProb(double d)
        {
            prob = d;
        }

        public string getName()
        {
            return (gObjCont[0]+" "+gObjCont[1]);
        }

        public int getID()
        {
            return id;
        }

        public double[] getPxDiffs()
        {
            return px;
        }

        public string toString()
        {
            return gObjCont.ToString()+"\n"+col+"\n"
                +prob+"\n"+timeBeg+"\n"+timeEnd;
        }

        #endregion
    }
}
