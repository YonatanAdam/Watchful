using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Model
{
    public class User : BaseEntity
    {
        private string name;
        private bool admin;
        private string password;

        private double latitude;
        private double longitude;

        public override string ToString()
        {
            return $"(ID: {Id}) -> Name: {name}, Password: {password}";
        }

        //test
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public bool Admin
        {
            get { return admin; }
            set { admin = value; }
        }

    }
}