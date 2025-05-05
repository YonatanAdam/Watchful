using Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ViewModel
{
    public class UserDB : BaseDB
    {
        public override void Insert(BaseEntity entity)
        {
            if (entity is User user)
            {
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }
        public override void Delete(BaseEntity entity)
        {
            if (entity is User user)
            {
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }
        public override void Update(BaseEntity entity)
        {
            if (entity is User user)
            {
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            User user = entity as User;
            // Insert only the Name and Password fields, letting the ID auto-increment
            string sqlStr = string.Format("INSERT INTO UserTbl (UserName, [Password], Longitude, Latitude) " +
                                          "VALUES('{0}', '{1}', '{2}', '{3}')", user.Name, user.Password, user.Longitude, user.Latitude);
            return sqlStr;
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            User user = entity as User;
            string sqlStr = $"UPDATE UserTbl SET UserName = '{user.Name}'," +
                            $"[Password]='{user.Password}'," +
                            $"Latitude={user.Latitude}," +
                            $"Longitude={user.Longitude} " +
                            $" WHERE ID={user.Id}";
            return sqlStr;
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            User user = entity as User;
            StringBuilder sql_builder = new StringBuilder();
            sql_builder.AppendFormat("DELETE FROM UserTbl WHERE ID = {0}", user.Id);
            return sql_builder.ToString();
        }

        protected override BaseEntity newEntity()
        {
            return new User();
        }

        protected override BaseEntity CreateModel(BaseEntity entity)
        {
            User userEntity = (User)entity;
            userEntity.Id = Convert.ToInt32(reader["ID"]);
            userEntity.Name = Convert.ToString(reader["UserName"]);
            userEntity.Password = Convert.ToString(reader["Password"]);
            userEntity.Longitude = Convert.ToDouble(reader["Longitude"]);
            userEntity.Latitude = Convert.ToDouble(reader["Latitude"]);
            return userEntity;
        }

        public bool ValidateNewUser(string name)
        {
            bool isValid = false;

            try
            {
                // Check if the Name already exists in the UserTbl
                string query = "SELECT COUNT(*) FROM UserTbl WHERE UserName = ?";

                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    // Add the parameter to prevent SQL injection
                    cmd.Parameters.AddWithValue("?", name);

                    connection.Open();
                    int result = (int)cmd.ExecuteScalar();

                    // If result is 0, the name is available
                    isValid = result == 0;
                }
            }
            catch (Exception e)
            {
                // Log the error or display a user-friendly message
                Console.WriteLine($"Error validating new user: {e.Message}");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return isValid;
        }
        public User Login(string username, string password)
        {
            string sqlstmt = $"SELECT ID, UserName, [Password], Latitude, Longitude FROM  UserTbl WHERE (UserName = '{username}') AND ([Password] = '{password}')";

            this.command.CommandText = sqlstmt;

            UserList list = new UserList(Select());

            if (list.Count > 0)
                return list[0];

            return null;
        }

        public bool UpdateUserLocation(int userId, double latitude, double longitude)
        {
            //string sqlquery = $"UPDATE UserTbl SET Latitude = {latitude}, Longitude = {longitude} WHERE ID = {userId}";
            //this.command.CommandText = sqlquery;

            int rowsAffected = this.command.ExecuteNonQuery();

            return rowsAffected > 0;

        }

        public Location SelectUserLocation(string tableName, int userId)
        {
            try
            {
                string query = $"SELECT Latitude, Longitude FROM {tableName} WHERE ID = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("?", userId);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    _ = BaseDB.SaveChanges();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            double latitude = reader.GetDouble(0);
                            double longitude = reader.GetDouble(1);
                            return new Location
                            {
                                Latitude = latitude,
                                Longitude = longitude
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching location: {e.Message}");
            }
            return null;
        }
        public User GetUserById(int userId)
        {
            string sqlstmt = $"SELECT ID, UserName , [Password], Latitude, Longitude FROM  UserTbl WHERE (ID = {userId})";

            this.command.CommandText = sqlstmt;

            UserList list = new UserList(Select());

            if (list.Count > 0)
                return list[0];

            return null;
        }

        public UserList SelectUsersByGroupId(int groupID)
        {
            string sqlstmt = $"SELECT  UserTbl.ID, UserTbl.UserName, UserTbl.[Password], UserTbl.Latitude, UserTbl.Longitude\r\nFROM            (UserTbl INNER JOIN\r\n                         GroupMembersTbl ON UserTbl.ID = GroupMembersTbl.UserID)\r\nWHERE        (GroupMembersTbl.GroupID = {groupID})";

            this.command.CommandText = sqlstmt;

            UserList list = new UserList(Select());

            return list;
        }

        public UserList GetAllUsersByGroupId(int groupId)
        {
            // SQL query to join GroupMembersTbl and UserTbl to fetch user details for a specific group
            string sqlqry = $@"
        SELECT UserTbl.ID, UserTbl.UserName, UserTbl.[Password], UserTbl.Latitude, UserTbl.Longitude
        FROM GroupMembersTbl
        INNER JOIN UserTbl ON GroupMembersTbl.UserID = UserTbl.ID
        WHERE GroupMembersTbl.GroupID = {groupId}";

            // Set the command text to the SQL query
            this.command.CommandText = sqlqry;

            // Execute the query and convert the result to a UserList
            UserList users = new UserList(Select());

            return users;
        }

        public IEnumerable<User> Select()
        {
            throw new NotImplementedException();
        }
    }
}
