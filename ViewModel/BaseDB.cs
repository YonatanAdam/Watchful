using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Model;


namespace ViewModel
{
    public abstract class BaseDB
    {
        private static string connectionString;
        protected OleDbConnection connection;
        protected OleDbCommand command;
        protected OleDbDataReader reader;

        protected static List<ChangeEntity> inserted = new List<ChangeEntity>();
        protected static List<ChangeEntity> deleted = new List<ChangeEntity>();
        protected static List<ChangeEntity> updated = new List<ChangeEntity>();

        protected abstract string CreateInsertSQL(BaseEntity entity);
        protected abstract string CreateUpdateSQL(BaseEntity entity);
        protected abstract string CreateDeleteSQL(BaseEntity entity);


        protected abstract BaseEntity newEntity();

        protected abstract BaseEntity CreateModel(BaseEntity entity);

        /*        private static string Path()
                {
                    string[] args = Environment.GetCommandLineArgs();
                    string s;
                    if (args.Length == 1)
                    {
                        s = args[0];
                    }
                    else
                    {
                        s = args[1];
                        s = s.Replace("/Service:", "");
                    }

                    string[] ss = s.Split('\\');
                    int x = ss.Length - 4; // - ???
                    ss[x] = "ViewModel";
                    ss[x + 1] = "";
                    Array.Resize(ref ss, x + 2);

                    string str = string.Join("\\", ss);
                    return str;
                }*/

        private static string GetDatabasePath()
        {
            // Get the path of the executable
            string exePath = Environment.GetCommandLineArgs()[0];
            string baseDir = Path.GetDirectoryName(exePath);

            // Traverse up to find the "Watchful" root
            DirectoryInfo dir = new DirectoryInfo(baseDir);
            while (dir != null && !dir.Name.Equals("Watchful", StringComparison.OrdinalIgnoreCase))
            {
                dir = dir.Parent;
            }

            if (dir == null)
                throw new InvalidOperationException("Could not locate 'Watchful' directory.");

            // Build path: Watchful\ViewModel\testDesign1.accdb
            string dbPath = Path.Combine(dir.FullName, "ViewModel", "testDesign1.accdb");
            return dbPath;
        }


        public BaseDB()
        {
            if (connectionString == null)
            {

                /*                string dbPath = GetDatabasePath();
                                connectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=True";*/
                //string dbPath = GetDbPath();
                //connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbPath + ";Persist Security Info=True";
                connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\Watchful\ViewModel\testDesign1.accdb;Persist Security Info=True";
            }

            connection = new OleDbConnection(connectionString);
            command = new OleDbCommand();
        }

        protected List<BaseEntity> Select()
        {
            List<BaseEntity> list = new List<BaseEntity>();

            try
            {
                command.Connection = connection;
                connection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    BaseEntity entity = newEntity();
                    list.Add(CreateModel(entity));
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\nSQL" + command.CommandText);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return list;
        }

        public List<BaseEntity> Select(string tableName)
        {
            List<BaseEntity> list = new List<BaseEntity>();

            try
            {
                command.Connection = connection;
                connection.Open();

                // Set the SQL query to select all from the given table
                command.CommandText = $"SELECT * FROM {tableName}";  // Use tableName as input

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    BaseEntity entity = newEntity();
                    list.Add(CreateModel(entity));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}\nSQL: {command.CommandText}");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return list;
        }



        public static int SaveChanges()
        {
            int records_affected = 0;
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection connection = new OleDbConnection(connectionString);
            try
            {

                cmd.Connection = connection;
                connection.Open();

                foreach (var item in inserted)
                {
                    cmd.CommandText = item.CreateSQL(item.Entity);
                    records_affected += cmd.ExecuteNonQuery();

                    cmd.CommandText = "Select @@Identity"; // get last id   
                    item.Entity.Id = (int)cmd.ExecuteScalar();
                }
                foreach (var item in updated)
                {
                    cmd.CommandText = item.CreateSQL(item.Entity);
                    records_affected += cmd.ExecuteNonQuery();
                }
                foreach (var item in deleted)
                {
                    cmd.CommandText = item.CreateSQL(item.Entity);
                    records_affected += cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\nSQL: " + cmd.CommandText);
            }
            finally
            {
                inserted.Clear();
                updated.Clear();
                deleted.Clear();
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return records_affected;
        }

        public virtual void Insert(BaseEntity entity)
        {
            BaseEntity reqEntity = this.newEntity();
            if (entity != null && entity.GetType() == reqEntity.GetType())
            {
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        public virtual void Update(BaseEntity entity)
        {
            BaseEntity reqEntity = this.newEntity();
            if (entity != null && entity.GetType() == reqEntity.GetType())
            {
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        public virtual void Delete(BaseEntity entity)
        {
            BaseEntity reqEntity = this.newEntity();
            if (entity != null && entity.GetType() == reqEntity.GetType())
            {
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }
    }
}