using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUIProj1
{
    class gridObject
    {
        private int row, col;
        private string timeBeg, timeEnd, gObjCont;
        private bool isPopulated;

        public gridObject()
        {
            row = 0;
            col = 0;
            timeBeg = "";
            timeEnd = "";
            isPopulated = false;
        }
        
        public gridObject(int r,int c,string tb,string te,bool pop)
        {
            row = r;
            col = c;
            timeBeg = tb;
            timeEnd = te;
            isPopulated = pop;
        }

        protected void setCoords(int r,int c)
        {
            row = r;
            col = c;
        }

        protected void setTimes(string b, string e)
        {
            timeBeg = b;
            timeEnd = e;
        }
        
        private void checkPop()
        {
            if(timeBeg=="" || timeEnd=="")
                isPopulated = false;
            else
                isPopulated = true;
        }

        protected int[] getRC()
        {
            int[] rc = { row,col };
            return rc;
        }

        protected string[] getTimeVals()
        {
            string[] begEndTimes = { timeBeg,timeEnd };
            return begEndTimes;
        }

        public bool isPop()
        {
            this.checkPop();
            return isPopulated;
        }
    }
}
