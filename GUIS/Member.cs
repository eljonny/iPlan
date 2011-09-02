using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUIProj1
{
    class Member
    {
        private String firstName;
        private String lastName;
        private String role;
        private String ID;

        public Member(String f, String l, String r, String i)
        {
            firstName = f;
            lastName = l;
            role = r;
            ID = i;
        }

        public Member(string[] array)
        {
            firstName = array[0];
            lastName = array[1];
            role = array[2];
            ID = array[3];
        }

        public String toString()
        {
            String str = "";
            str += "First Name: " + firstName + "\n" + "Last Name: " + lastName + "\n" + "Role: " + role + "\n" + "ID: " + ID;

            return str;
        }
    }
}
