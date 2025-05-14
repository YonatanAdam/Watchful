using Model;
using System;
using System.Text;

namespace ViewModel
{
    public class RuleDB : BaseDB
    {
        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            Rule rule = entity as Rule;
            StringBuilder sql_builder = new StringBuilder();
            sql_builder.AppendFormat("DELETE FROM RuleTbl WHERE ID = {0}", rule.Id);
            return sql_builder.ToString();
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Rule rule = entity as Rule;
            // Insert only the Name and Password fields, letting the ID auto-increment
            string sqlStr = string.Format("INSERT INTO RuleTbl (GroupID, RuleName, RuleType, Latitude, Longitude, Radius) " +
                              "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                              rule.GroupId, rule.RuleName, rule.RuleType, rule.Latitude, rule.Longitude, rule.Radius);

            return sqlStr;
        }

        protected override BaseEntity CreateModel(BaseEntity entity)
        {
            Rule rule = (Rule)entity;
            rule.Id = Convert.ToInt32(reader["ID"]);
            rule.RuleName = Convert.ToString(reader["RuleName"]);
            rule.RuleType = Convert.ToString(reader["RuleType"]);
            rule.Latitude = Convert.ToDouble(reader["Latitude"]);
            rule.Longitude = Convert.ToDouble(reader["Longitude"]);
            rule.Radius = Convert.ToDouble(reader["Radius"]);
            return rule;
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            Rule rule = entity as Rule;
            string sqlStr = $"UPDATE RuleTbl SET RuleName = '{rule.RuleName}'," +
                            $"RuleType='{rule.RuleType}'," +
                            $"GroupID='{rule.GroupId}'," +
                            $"Latitude='{rule.Latitude}'," +
                            $"Longitude='{rule.Longitude}'," +
                            $"Radius='{rule.Radius}'," +
                            $"WHERE ID={rule.Id}";
            return sqlStr;
        }

        protected override BaseEntity newEntity()
        {
            return new Rule();
        }

        public override void Insert(BaseEntity entity)
        {
            if (entity is Rule rule)
            {
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }
        public override void Delete(BaseEntity entity)
        {
            if (entity is Rule rule)
            {
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }
        public override void Update(BaseEntity entity)
        {
            if (entity is Rule rule)
            {
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        public RulesList GetAllRulesByGroupId(int groupId)
        {
            string sqlstmt = $"SELECT ID, RuleName, RuleType, Latitude, Longitude, Radius FROM RuleTbl WHERE (GroupID = {groupId})";
            this.command.CommandText = sqlstmt;
            RulesList list = new RulesList(Select());
            return list;
        }
    }
}
