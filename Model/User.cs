using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Model
{
    public class User : BaseEntity
    {
        private string name;
        private int phoneNumber;
        private AgeGroup ageGroup;
        private UserType userType;
        private List<string> emergencyContacts;
        private UserPreferences preferences;
        private string password;

        private int? GroupID { get; set; }
        public virtual Group group { get; set; }
        public virtual ICollection<NavHistory> navigationHistory { get; set; }

        public override string ToString()
        {
            return $"(ID: {Id}) -> Name: {name}, Password: {password}";
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

        public AgeGroup AgeGroup
        {
            get { return ageGroup; }
            set { ageGroup = value; }
        }

        public UserType UserType
        {
            get { return userType; }
            set { userType = value; }
        }

        public List<string> EmergencyContacts
        {
            get { return emergencyContacts; }
            set { emergencyContacts = value; }
        }

        public UserPreferences Preferences
        {
            get { return preferences; }
            set { preferences = value; }
        }

        public Group Group
        {
            get { return group; }
            set { group = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

    }
}