using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Group : BaseEntity
    {
        private string groupName;
        private string passCode;
        private User admin;
        private UserList members;


        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }

        public User Admin
        { 
            get { return admin; }
            set {  admin = value; }
        }

        public UserList Members { get => members; set => members = value; }
        public string PassCode { get => passCode; set => passCode = value; }
    }
}
