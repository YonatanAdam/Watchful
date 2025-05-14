using Model;
using System;
using System.Text;

namespace ViewModel
{
    /// <summary>
    /// Provides database operations for the Rule entity, including CRUD operations and retrieval by group.
    /// </summary>
    public class RuleDB : BaseDB
    {
        /// <summary>
        /// Creates the SQL statement for deleting a Rule entity.
        /// </summary>
        /// <param name="entity">The Rule entity to delete.</param>
        /// <returns>The SQL delete statement.</returns>
        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            Rule rule = entity as Rule;
            StringBuilder sql_builder = new StringBuilder();
            sql_builder.AppendFormat("DELETE FROM RuleTbl WHERE ID = {0}", rule.Id);
            return sql_builder.ToString();
        }

        /// <summary>
        /// Creates the SQL statement for inserting a Rule entity.
        /// </summary>
        /// <param name="entity">The Rule entity to insert.</param>
        /// <returns>The SQL insert statement.</returns>
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Rule rule = entity as Rule;
            string sqlStr = string.Format("INSERT INTO RuleTbl (GroupID, RuleName, RuleType, Latitude, Longitude, Radius) " +
                              "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                              rule.GroupId, rule.RuleName, rule.RuleType, rule.Latitude, rule.Longitude, rule.Radius);

            return sqlStr;
        }

        /// <summary>
        /// Creates a Rule model from the current data reader row.
        /// </summary>
        /// <param name="entity">The base entity to populate as a Rule.</param>
        /// <returns>A populated Rule object.</returns>
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

        /// <summary>
        /// Creates the SQL statement for updating a Rule entity.
        /// </summary>
        /// <param name="entity">The Rule entity to update.</param>
        /// <returns>The SQL update statement.</returns>
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

        /// <summary>
        /// Creates a new instance of the Rule entity.
        /// </summary>
        /// <returns>A new Rule object.</returns>
        protected override BaseEntity newEntity()
        {
            return new Rule();
        }

        /// <summary>
        /// Adds a Rule entity to the list of entities to be inserted into the database.
        /// </summary>
        /// <param name="entity">The Rule entity to insert.</param>
        public override void Insert(BaseEntity entity)
        {
            if (entity is Rule rule)
            {
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        /// <summary>
        /// Adds a Rule entity to the list of entities to be deleted from the database.
        /// </summary>
        /// <param name="entity">The Rule entity to delete.</param>
        public override void Delete(BaseEntity entity)
        {
            if (entity is Rule rule)
            {
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        /// <summary>
        /// Adds a Rule entity to the list of entities to be updated in the database.
        /// </summary>
        /// <param name="entity">The Rule entity to update.</param>
        public override void Update(BaseEntity entity)
        {
            if (entity is Rule rule)
            {
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        /// <summary>
        /// Gets all rules associated with a specific group.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <returns>A list of rules for the specified group.</returns>
        public RulesList GetAllRulesByGroupId(int groupId)
        {
            string sqlstmt = $"SELECT ID, RuleName, RuleType, Latitude, Longitude, Radius FROM RuleTbl WHERE (GroupID = {groupId})";
            this.command.CommandText = sqlstmt;
            RulesList list = new RulesList(Select());
            return list;
        }
    }
}
