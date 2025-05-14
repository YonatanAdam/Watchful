using System;

namespace Model
{
    public class Notification : BaseEntity
    {
        private int userId;
        private int ruleId;
        private DateTime CreateDate;

        public int GroupId { get => userId; set => userId = value; }
        public int RuleId { get => ruleId; set => ruleId = value; }
        public DateTime Date { get => CreateDate; set => CreateDate = value; }
    }
}
