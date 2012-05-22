using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GUIProj1
{
    public class TeamMember
    {
        #region Team Member Information (Variables)

        private string firstName { get; set; }
        private string middleName { get; set; }
        private string lastName { get; set; }
        private string streetAddr, unitNum, city, state, zip;
        private string age, sex, maritalStatus;
        private string notes, bio;

        GridObject stats;

        Image portrait = new Bitmap("defaultcontact.bmp");

        #endregion

        #region Construction

        public TeamMember()
        {

        }

        public TeamMember(GridObject info)
        {
            stats = new GridObject(info);
        }

        #endregion

        public string getName()
        {
            return lastName + ", " + firstName + " " + middleName;
        }
    }
}
