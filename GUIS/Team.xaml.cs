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
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Collections;

#endregion

namespace GUIProj1
{
    /// <summary>
    /// Interaction logic for Team.xaml
    /// </summary>
    public partial class Team : NavigationWindow
    {
        #region Team Class variables

        private LinkedList<TeamMember> members;
        internal string teamName { get; set; }

        #endregion

        public Team()
        {
            InitializeComponent();

            members = new LinkedList<TeamMember>();
        }

        public TeamMember[] getMemberList()
        {
            return members.ToArray();
        }

        public bool addMember(TeamMember newMember)
        {
            members.AddFirst(newMember);

            return members.Contains(newMember);
        }

        public bool Equals(Team other)
        {
            return this.teamName.Equals(other.teamName) ? true : false;
        }
    }
}
