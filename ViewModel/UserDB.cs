using Model;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
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
                // inserted.Add(new ChangeEntity(base.CreateInsertSQL, entity));
            }
        }
        public override void Delete(BaseEntity entity)
        {
            if (entity is User user)
            {
                // deleted.Add(new ChangeEntity(base.CreateDeleteSQL, entity));
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }
        public override void Update(BaseEntity entity)
        {
            if (entity is User user)
            {
                // updated.Add(new ChangeEntity(base.CreateUpdateSQL, entity));
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            User user = entity as User;
            // Insert only the Name and Password fields, letting the ID auto-increment
            string sqlStr = string.Format("INSERT INTO UserTbl (UserName, [Password]) " +
                                          "VALUES('{0}', '{1}')", user.Name, user.Password);
            return sqlStr;
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            User user = entity as User;
            string sqlStr = $"UPDATE UserTbl SET UserName = '{user.Name}'," +
                            $"Password='{user.Password}'" +
                            $"WHERE UserID={user.Id}";
            return sqlStr;
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            User user = entity as User;
            StringBuilder sql_builder = new StringBuilder();
            sql_builder.AppendFormat("DELETE FROM UserTbl WHERE UserID = {0}", user.Id);
            return sql_builder.ToString();
        }

        protected override BaseEntity newEntity()
        {
            return new User();
        }

        protected override BaseEntity CreateModel(BaseEntity entity)
        {
            User userEntity = (User)entity;
            userEntity.Id = Convert.ToInt32(reader["UserId"]);
            userEntity.Name = Convert.ToString(reader["UserName"]);
            userEntity.Password = Convert.ToString(reader["Password"]);
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


        public bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            try
            {
                string query = "SELECT COUNT(*) FROM UserTbl WHERE UserName = ? AND Password = ?"; // Use parameters to prevent SQL injection

                OleDbCommand cmd = new OleDbCommand(query, connection);

                // Add parameters for username and password
                cmd.Parameters.AddWithValue("?", username);
                cmd.Parameters.AddWithValue("?", password);

                connection.Open();

                // Execute the query and check if any rows are returned
                int result = (int)cmd.ExecuteScalar();

                if (result > 0)
                {
                    isValid = true;  // User exists with correct username and password
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {e.Message}");
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
    }
}
