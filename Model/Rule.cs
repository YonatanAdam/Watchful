using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Rule : BaseEntity
    {
        private string ruleName;
        private string ruleType; // In or Out of area
        private int groupId;
        private double latitude;
        private double longitude;
        private double radius;

        public string RuleName
        {
            get { return ruleName; }
            set { ruleName = value; }
        }
        public string RuleType
        {
            get { return ruleType; }
            set { ruleType = value; }
        }
        public int GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }
        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
    }
}
