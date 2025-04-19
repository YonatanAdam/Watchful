using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModel
{
    public class GroupDB : BaseDB
    {
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

        protected override BaseEntity newEntity()
        {
            return new Group();
        }

        public override void Insert(BaseEntity entity)
        {
            if (entity is Group group)
            {
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }
        public override void Delete(BaseEntity entity)
        {
            if (entity is Group group)
            {
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }
        public override void Update(BaseEntity entity)
        {
            if (entity is Group group)
            {
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Group group = entity as Group;
            // Insert only the Name and Password fields, letting the ID auto-increment
            string sqlStr = string.Format("INSERT INTO GroupTbl (GroupName, Passcode, AdminID) " +
                                          "VALUES('{0}', '{1}', '{2}')", group.GroupName, group.PassCode, group.Admin.Id);
            return sqlStr;
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            Group group = entity as Group;
            string sqlStr = $"UPDATE GroupTbl SET GroupName = '{group.GroupName}'," +
                            $"AdminID='{group.Admin.Id}'" +
                            $"Passcode='{group.PassCode}'" +
                            $"WHERE ID={group.Id}";
            return sqlStr;
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            Group group = entity as Group;
            StringBuilder sql_builder = new StringBuilder();
            sql_builder.AppendFormat("DELETE FROM GroupTbl WHERE ID = {0}", group.Id);
            return sql_builder.ToString();
        }

        public bool AdminHasGroup(int adminId)
        {
            string sqlStr = $"SELECT ID, AdminID, Passcode, GroupName FROM GroupTbl WHERE (AdminID = {adminId})";

            this.command.CommandText = sqlStr;

            GroupList groups = new GroupList(Select());

            return groups.Count > 0;
        }

        public Group CreateGroup(string groupName, string passcode, User admin)
        {
            string sqlstr = $"SELECT ID, GroupName, AdminID, Passcode FROM GroupTbl WHERE (Passcode = '{passcode}')";

            this.command.CommandText = sqlstr;

            GroupList groups = new GroupList(Select());

            if (groups.Count > 0)
            {
                return null;
            }

            // 
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

        public GroupList GetGroupsForUser(int userId)
        {
            string sqlstr = $"SELECT GroupTbl.ID, GroupTbl.GroupName, GroupTbl.AdminID, GroupTbl.Passcode FROM GroupTbl INNER JOIN GroupMembersTbl ON GroupTbl.ID = GroupMembersTbl.GroupID WHERE GroupMembersTbl.UserID = {userId};";

            this.command.CommandText = sqlstr;

            GroupList groups = new GroupList(Select());

            return groups;
        }
    }
}
