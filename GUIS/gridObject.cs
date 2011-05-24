using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
//  Author Jonathan Hyry
namespace GUIProj1
{
    class gridObject
    {
        private int row, col;
        private double prob;
        private string timeBeg, timeEnd, type, date;
        private string[] gObjCont = null;
        private bool isPopulated;

        public gridObject()
        {
            prob = 0;
            row = 0;
            col = 0;
            timeBeg = "";
            timeEnd = "";
            isPopulated = false;
        }
        
        public gridObject(int r,int c,string tb,string te,
            bool pop, string[] content, string t, string d)
        {
            prob = 0;
            row = r;
            col = c;
            timeBeg = tb;
            timeEnd = te;
            isPopulated = pop;
            gObjCont = content;
            type = t;
            date = d;
        }

        protected void setCoords(int c, int r)
        {
            row = r;
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

        public string toString()
        {
            return gObjCont.ToString()+"\n"+col+"\n"
                +prob+"\n"+timeBeg+"\n"+timeEnd;
        }
    }
}
