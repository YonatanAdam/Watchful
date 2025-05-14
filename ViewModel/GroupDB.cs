using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace ViewModel
{
    /// <summary>
    /// Provides database operations for the Group entity, including CRUD operations and group membership management.
    /// </summary>
    public class GroupDB : BaseDB
    {
        /// <summary>
        /// Creates a Group model from the current data reader row.
        /// </summary>
        /// <param name="entity">The base entity to populate as a Group.</param>
        /// <returns>A populated Group object.</returns>
        protected override BaseEntity CreateModel(BaseEntity entity)
        {
            Group group = (Group)entity;
            group.Id = Convert.ToInt32(reader["ID"]);
            group.GroupName = Convert.ToString(reader["GroupName"]);
            group.PassCode = Convert.ToString(reader["Passcode"]);
            UserDB userDB = new UserDB();
            group.Admin = userDB.GetUserById(Convert.ToInt32(reader["AdminID"]));
            group.Members = userDB.SelectUsersByGroupId(group.Id);
            return group;
        }

        /// <summary>
        /// Creates a new instance of the Group entity.
        /// </summary>
        /// <returns>A new Group object.</returns>
        protected override BaseEntity newEntity()
        {
            return new Group();
        }

        /// <summary>
        /// Adds a Group entity to the list of entities to be inserted into the database.
        /// </summary>
        /// <param name="entity">The Group entity to insert.</param>
        public override void Insert(BaseEntity entity)
        {
            if (entity is Group group)
            {
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        /// <summary>
        /// Adds a Group entity to the list of entities to be deleted from the database.
        /// </summary>
        /// <param name="entity">The Group entity to delete.</param>
        public override void Delete(BaseEntity entity)
        {
            if (entity is Group group)
            {
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        /// <summary>
        /// Adds a Group entity to the list of entities to be updated in the database.
        /// </summary>
        /// <param name="entity">The Group entity to update.</param>
        public override void Update(BaseEntity entity)
        {
            if (entity is Group group)
            {
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        /// <summary>
        /// Creates the SQL statement for inserting a Group entity.
        /// </summary>
        /// <param name="entity">The Group entity to insert.</param>
        /// <returns>The SQL insert statement.</returns>
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Group group = entity as Group;
            string sqlStr = string.Format("INSERT INTO GroupTbl (GroupName, Passcode, AdminID) " +
                                          "VALUES('{0}', '{1}', '{2}')", group.GroupName, group.PassCode, group.Admin.Id);
            return sqlStr;
        }

        /// <summary>
        /// Creates the SQL statement for updating a Group entity.
        /// </summary>
        /// <param name="entity">The Group entity to update.</param>
        /// <returns>The SQL update statement.</returns>
        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            Group group = entity as Group;
            string sqlStr = $"UPDATE GroupTbl SET GroupName = '{group.GroupName}'," +
                            $"AdminID='{group.Admin.Id}'" +
                            $"Passcode='{group.PassCode}'" +
                            $"WHERE ID={group.Id}";
            return sqlStr;
        }

        /// <summary>
        /// Creates the SQL statement for deleting a Group entity.
        /// </summary>
        /// <param name="entity">The Group entity to delete.</param>
        /// <returns>The SQL delete statement.</returns>
        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            Group group = entity as Group;
            StringBuilder sql_builder = new StringBuilder();
            sql_builder.AppendFormat("DELETE FROM GroupTbl WHERE ID = {0}", group.Id);
            return sql_builder.ToString();
        }

        /// <summary>
        /// Checks if an admin has any groups.
        /// </summary>
        /// <param name="adminId">The admin's user ID.</param>
        /// <returns>True if the admin has at least one group; otherwise, false.</returns>
        public bool AdminHasGroup(int adminId)
        {
            string sqlStr = $"SELECT ID, AdminID, Passcode, GroupName FROM GroupTbl WHERE (AdminID = {adminId})";
            this.command.CommandText = sqlStr;
            GroupList groups = new GroupList(Select());
            return groups.Count > 0;
        }

        /// <summary>
        /// Creates a new group if the passcode is unique.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="passcode">The passcode for the group.</param>
        /// <param name="admin">The admin user for the group.</param>
        /// <returns>The created Group object, or null if the passcode already exists or creation fails.</returns>
        public Group CreateGroup(string groupName, string passcode, User admin)
        {
            string sqlstr = $"SELECT ID, GroupName, AdminID, Passcode FROM GroupTbl WHERE (Passcode = '{passcode}')";
            this.command.CommandText = sqlstr;
            GroupList groups = new GroupList(Select());
            if (groups.Count > 0)
            {
                return null;
            }
            Group g = new Group
            {
                GroupName = groupName,
                PassCode = passcode,
                Admin = admin
            };
            Insert(g);
            int changes = SaveChanges();
            if (changes > 0)
                return g;
            return null;
        }

        /// <summary>
        /// Gets the list of groups where the user is a member.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <returns>A list of groups where the user is a member.</returns>
        public GroupList GetGroupsForUserAsMember(int userId)
        {
            string sqlstr = $"SELECT GroupTbl.ID, GroupTbl.GroupName, GroupTbl.AdminID, GroupTbl.Passcode FROM GroupTbl INNER JOIN GroupMembersTbl ON GroupTbl.ID = GroupMembersTbl.GroupID WHERE GroupMembersTbl.UserID = {userId};";
            this.command.CommandText = sqlstr;
            GroupList groups = new GroupList(Select());
            return groups;
        }

        /// <summary>
        /// Gets the list of groups where the user is an admin.
        /// </summary>
        /// <param name="adminId">The admin's user ID.</param>
        /// <returns>A list of groups where the user is an admin.</returns>
        public GroupList GetGroupsForUserAsAdmin(int adminId)
        {
            string sqlstr = $"SELECT ID, GroupName, AdminID, Passcode FROM GroupTbl WHERE (AdminID = {adminId})";
            this.command.CommandText = sqlstr;
            GroupList groups = new GroupList(Select());
            return groups;
        }

        /// <summary>
        /// Gets all groups for a user, both as a member and as an admin, avoiding duplicates.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <returns>A list of all groups associated with the user.</returns>
        public GroupList GetAllGroupsForUser(int userId)
        {
            GroupList allGroups = new GroupList();
            try
            {
                GroupList memberGroups = GetGroupsForUserAsMember(userId);
                foreach (Group group in memberGroups)
                {
                    allGroups.Add(group);
                }
                GroupList adminGroups = GetGroupsForUserAsAdmin(userId);
                foreach (Group group in adminGroups)
                {
                    if (!allGroups.Contains(group))
                    {
                        allGroups.Add(group);
                    }
                }
                return allGroups;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all groups for user: {ex.Message}");
                return allGroups;
            }
        }

        /// <summary>
        /// Gets a group by its name and passcode.
        /// </summary>
        /// <param name="name">The group's name.</param>
        /// <param name="pass">The group's passcode.</param>
        /// <returns>The matching Group object, or null if not found.</returns>
        public Group GetGroupByNameAndPass(string name, string pass)
        {
            string sqlstr = $"SELECT ID, GroupName, AdminID, Passcode FROM GroupTbl WHERE (GroupName = '{name}') AND (Passcode = '{pass}')";
            this.command.CommandText = sqlstr;
            GroupList groups = new GroupList(Select());
            if (groups.Count > 0)
                return groups[0];
            return null;
        }

        /// <summary>
        /// Adds a user to a group by their IDs if not already a member.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <returns>True if the user was added or already exists; otherwise, false.</returns>
        public bool AddUserById(int groupId, int userId)
        {
            if (this.connection.State != ConnectionState.Open)
            {
                this.connection.Open();
            }
            string sqlstr = $"SELECT COUNT(*) FROM GroupMembersTbl WHERE (UserID = {userId}) AND (GroupID = {groupId})";
            this.command.CommandText = sqlstr;
            int count = Convert.ToInt32(this.command.ExecuteScalar());
            if (count > 0) return true;
            sqlstr = $"INSERT INTO GroupMembersTbl (GroupID, UserID) VALUES ({groupId}, {userId})";
            this.command.CommandText = sqlstr;
            int rowsAffected = this.command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        /// <summary>
        /// Gets a group by its ID.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <returns>The matching Group object, or null if not found.</returns>
        public Group GetGroupById(int groupId)
        {
            string sqlstr = $"SELECT ID, GroupName, AdminID, Passcode FROM GroupTbl WHERE (ID = {groupId})";
            this.command.CommandText = sqlstr;
            GroupList groups = new GroupList(Select());
            if (groups.Count > 0)
                return groups[0];
            return null;
        }

        /// <summary>
        /// Gets a group by its name.
        /// </summary>
        /// <param name="name">The group's name.</param>
        /// <returns>The matching Group object, or null if not found.</returns>
        public Group GetGroupByName(string name)
        {
            string sqlstr = $"SELECT ID, GroupName, AdminID, Passcode FROM GroupTbl WHERE (GroupName = {name})";
            this.command.CommandText = sqlstr;
            GroupList groups = new GroupList(Select());
            if (groups.Count > 0)
                return groups[0];
            return null;
        }

        /// <summary>
        /// Gets the list of users who are members of a specific group.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <returns>A list of users in the group.</returns>
        public UserList GetUsersByGroup(int groupId)
        {
            string sqlstr = $@"
                    SELECT UserTbl.ID, UserTbl.UserName, UserTbl.[Password], UserTbl.Latitude, UserTbl.Longitude
                    FROM GroupMembersTbl
                    INNER JOIN UserTbl ON GroupMembersTbl.UserID = UserTbl.ID
                    WHERE GroupMembersTbl.GroupID = {groupId}";
            this.command.CommandText = sqlstr;
            UserList users = new UserList(Select());
            return users;
        }
    }
}
