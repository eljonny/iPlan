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
    class gridObject
    {
        private int row,col,id;
        private int[] px = new int[2];
        private double prob;
        private string timeBeg, timeEnd, type, date;
        private string[] gObjCont = null;
        private bool isPopulated;
        private double[,] probDet = null;

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
        
        public gridObject(int r, int c, int pxDL, int pxDT, string tb,string te,
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

        protected void setCoords(int c, int r)
        {
            row = r;
            col = c;
        }

        protected void setR(int r)
        {
            row = r;
        }

        protected void setC(int c)
        {
            col = c;
        }

        protected void setTimes(string b, string e)
        {
            timeBeg = b;
            timeEnd = e;
        }

        public void setType(string t)
        {
            type = t;
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

        public int[] getPxDiffs()
        {
            return px;
        }

        public string toString()
        {
            return gObjCont.ToString()+"\n"+col+"\n"
                +prob+"\n"+timeBeg+"\n"+timeEnd;
        }
    }
}
