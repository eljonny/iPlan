using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUIProj1
{
    public class GridObject
    {
        #region GridObject Variables

        // Randomly generated identifier
        private int id;

        //Overall probability for showing up on time
        private double probability;
        
        // Describes the block
        private DateTime blockBegin, blockEnd;
        
        // Should be either team or team member
        private string type;

        //Informations about the particular team or team member.
        /*
         * This string array is of the format:
         * {
         *  
         * */
        private string[] informations = null;

        //Basically, does this GridObject contain any meaningful information?
        private bool isPopulated;

        // Factors affecting the stats of a team or team member
        private LinkedList<ProbabilityFactor> factors;

        #endregion

        #region Construction

        public GridObject()
        {
            id = (new Random()).Next();
            probability = 1;
            blockBegin = new DateTime(DateTime.Now.Year,
                                        DateTime.Now.Month,
                                        DateTime.Now.Day,
                                        DateTime.Now.Hour,
                                        DateTime.Now.Minute,
                                        DateTime.Now.Second,
                                        DateTime.Now.Millisecond);
            blockEnd = new DateTime(DateTime.Now.Year,
                                    DateTime.Now.Month,
                                    DateTime.Now.Day,
                                    blockBegin.Hour + 8,
                                    blockBegin.Minute,
                                    blockBegin.Second,
                                    blockBegin.Millisecond);
            isPopulated = false;
            factors = new LinkedList<ProbabilityFactor>();
        }
        /*
        public GridObject()
        {
            
        }
        */
        public GridObject(GridObject makeCopyOf)
        {

        }

        #endregion

        #region GridObject operations

        public void setTimes(DateTime b, DateTime e)
        {
            blockBegin = b;
            blockEnd = e;
        }

        public void setType(string t)
        {
            type = t;
        }
        
        private bool checkPop()
        {
            return isPopulated;
        }

        public void setProb(double d)
        {
            probability = d;
        }

        public int getID()
        {
            return id;
        }

        public string toString()
        {
            //return gObjCont.ToString()+"\n"+col+"\n"
              //  +probability+"\n"+blockBegin+"\n"+blockEnd;
            return "";
        }

        #endregion
    }
}
