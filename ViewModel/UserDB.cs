using Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;

namespace ViewModel
{
    /// <summary>
    /// Provides database operations for the User entity, including CRUD operations, authentication, and group membership management.
    /// </summary>
    public class UserDB : BaseDB
    {
        /// <summary>
        /// Adds a User entity to the list of entities to be inserted into the database.
        /// </summary>
        /// <param name="entity">The User entity to insert.</param>
        public override void Insert(BaseEntity entity)
        {
            if (entity is User user)
            {
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        /// <summary>
        /// Adds a User entity to the list of entities to be deleted from the database.
        /// </summary>
        /// <param name="entity">The User entity to delete.</param>
        public override void Delete(BaseEntity entity)
        {
            if (entity is User user)
            {
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        /// <summary>
        /// Adds a User entity to the list of entities to be updated in the database.
        /// </summary>
        /// <param name="entity">The User entity to update.</param>
        public override void Update(BaseEntity entity)
        {
            if (entity is User user)
            {
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        /// <summary>
        /// Creates the SQL statement for inserting a User entity.
        /// </summary>
        /// <param name="entity">The User entity to insert.</param>
        /// <returns>The SQL insert statement.</returns>
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            User user = entity as User;
            string sqlStr = string.Format("INSERT INTO UserTbl (UserName, [Password], Longitude, Latitude) " +
                                          "VALUES('{0}', '{1}', '{2}', '{3}')", user.Name, user.Password, user.Longitude, user.Latitude);
            return sqlStr;
        }

        /// <summary>
        /// Creates the SQL statement for updating a User entity.
        /// </summary>
        /// <param name="entity">The User entity to update.</param>
        /// <returns>The SQL update statement.</returns>
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

        /// <summary>
        /// Creates the SQL statement for deleting a User entity.
        /// </summary>
        /// <param name="entity">The User entity to delete.</param>
        /// <returns>The SQL delete statement.</returns>
        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            User user = entity as User;
            StringBuilder sql_builder = new StringBuilder();
            sql_builder.AppendFormat("DELETE FROM UserTbl WHERE ID = {0}", user.Id);
            return sql_builder.ToString();
        }

        /// <summary>
        /// Creates a new instance of the User entity.
        /// </summary>
        /// <returns>A new User object.</returns>
        protected override BaseEntity newEntity()
        {
            return new User();
        }

        /// <summary>
        /// Creates a User model from the current data reader row.
        /// </summary>
        /// <param name="entity">The base entity to populate as a User.</param>
        /// <returns>A populated User object.</returns>
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

        /// <summary>
        /// Validates that a new user's name does not already exist in the database.
        /// </summary>
        /// <param name="name">The username to validate.</param>
        /// <returns>True if the username is available; otherwise, false.</returns>
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

        /// <summary>
        /// Attempts to log in a user with the specified username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The matching User object if credentials are valid; otherwise, null.</returns>
        public User Login(string username, string password)
        {
            string sqlstmt = $"SELECT ID, UserName, [Password], Latitude, Longitude FROM  UserTbl WHERE (UserName = '{username}') AND ([Password] = '{password}')";

            this.command.CommandText = sqlstmt;

            UserList list = new UserList(Select());

            if (list.Count > 0)
                return list[0];

            return null;
        }

        /// <summary>
        /// Updates the location of a user in the database.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="latitude">The new latitude.</param>
        /// <param name="longitude">The new longitude.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public bool UpdateUserLocation(int userId, double latitude, double longitude)
        {
            int rowsAffected = this.command.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        /// <summary>
        /// Selects the location of a user from the specified table.
        /// </summary>
        /// <param name="tableName">The table name to query.</param>
        /// <param name="userId">The user's ID.</param>
        /// <returns>The user's Location object if found; otherwise, null.</returns>
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

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <returns>The matching User object if found; otherwise, null.</returns>
        public User GetUserById(int userId)
        {
            string sqlstmt = $"SELECT ID, UserName , [Password], Latitude, Longitude FROM  UserTbl WHERE (ID = {userId})";

            this.command.CommandText = sqlstmt;

            UserList list = new UserList(Select());

            if (list.Count > 0)
                return list[0];

            return null;
        }

        /// <summary>
        /// Selects users who are members of a specific group.
        /// </summary>
        /// <param name="groupID">The group ID.</param>
        /// <returns>A list of users in the group.</returns>
        public UserList SelectUsersByGroupId(int groupID)
        {
            string sqlstmt = $"SELECT  UserTbl.ID, UserTbl.UserName, UserTbl.[Password], UserTbl.Latitude, UserTbl.Longitude\r\nFROM            (UserTbl INNER JOIN\r\n                         GroupMembersTbl ON UserTbl.ID = GroupMembersTbl.UserID)\r\nWHERE        (GroupMembersTbl.GroupID = {groupID})";

            this.command.CommandText = sqlstmt;

            UserList list = new UserList(Select());

            return list;
        }

        /// <summary>
        /// Gets all users who are members of a specific group.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <returns>A list of users in the group.</returns>
        public UserList GetAllUsersByGroupId(int groupId)
        {
            string sqlqry = $@"
            SELECT UserTbl.ID, UserTbl.UserName, UserTbl.[Password], UserTbl.Latitude, UserTbl.Longitude
            FROM GroupMembersTbl
            INNER JOIN UserTbl ON GroupMembersTbl.UserID = UserTbl.ID
            WHERE GroupMembersTbl.GroupID = {groupId}";

            this.command.CommandText = sqlqry;

            UserList users = new UserList(Select());

            return users;
        }

        /// <summary>
        /// Selects all users from the database.
        /// </summary>
        /// <returns>An enumerable collection of all users.</returns>
        public IEnumerable<User> SelectAllUsers()
        {
            string sqlstmt = $"SELECT ID, UserName, [Password], Latitude, Longitude FROM UserTbl";
            this.command.CommandText = sqlstmt;
            var users = new UserList(Select());

            return users;
        }

        /// <summary>
        /// Removes a user from a group by their IDs.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="groupId">The group ID.</param>
        public void RemoveUserFromGroup(int userId, int groupId)
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\\..\\..\\..\\ViewModel\\WatchfulDB.accdb;Persist Security Info=True";
            string sql = $"DELETE FROM GroupMembersTbl WHERE UserId = {userId} AND GroupId = {groupId}";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(sql, connection))
                {
                    connection.Open(); // Open the connection
                    command.ExecuteNonQuery(); // Execute the SQL command
                }
            }
        }
    }
}
