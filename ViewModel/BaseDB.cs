using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Model;


namespace ViewModel
{
    /// <summary>
    /// Abstract base class for database access objects, providing common CRUD operations and change tracking for entities.
    /// </summary>
    public abstract class BaseDB
    {
        private static string connectionString;
        protected OleDbConnection connection;
        protected OleDbCommand command;
        protected OleDbDataReader reader;

        /// <summary>
        /// List of entities to be inserted on SaveChanges.
        /// </summary>
        protected static List<ChangeEntity> inserted = new List<ChangeEntity>();
        /// <summary>
        /// List of entities to be deleted on SaveChanges.
        /// </summary>
        protected static List<ChangeEntity> deleted = new List<ChangeEntity>();
        /// <summary>
        /// List of entities to be updated on SaveChanges.
        /// </summary>
        protected static List<ChangeEntity> updated = new List<ChangeEntity>();

        /// <summary>
        /// Creates the SQL statement for inserting an entity.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>The SQL insert statement.</returns>
        protected abstract string CreateInsertSQL(BaseEntity entity);

        /// <summary>
        /// Creates the SQL statement for updating an entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The SQL update statement.</returns>
        protected abstract string CreateUpdateSQL(BaseEntity entity);

        /// <summary>
        /// Creates the SQL statement for deleting an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>The SQL delete statement.</returns>
        protected abstract string CreateDeleteSQL(BaseEntity entity);

        /// <summary>
        /// Creates a new instance of the entity type handled by this database class.
        /// </summary>
        /// <returns>A new entity instance.</returns>
        protected abstract BaseEntity newEntity();

        /// <summary>
        /// Populates an entity from the current data reader row.
        /// </summary>
        /// <param name="entity">The entity to populate.</param>
        /// <returns>The populated entity.</returns>
        protected abstract BaseEntity CreateModel(BaseEntity entity);

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDB"/> class and sets up the database connection.
        /// </summary>
        public BaseDB()
        {
            if (connectionString == null)
            {
                connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\\..\\..\\..\\ViewModel\\WatchfulDB.accdb;Persist Security Info=True";
            }

            connection = new OleDbConnection(connectionString);
            command = new OleDbCommand();
        }

        /// <summary>
        /// Executes the current command and returns a list of entities from the result set.
        /// </summary>
        /// <returns>A list of entities from the query result.</returns>
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

        /// <summary>
        /// Selects all entities from the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table to select from.</param>
        /// <returns>A list of entities from the specified table.</returns>
        public List<BaseEntity> Select(string tableName)
        {
            List<BaseEntity> list = new List<BaseEntity>();

            try
            {
                command.Connection = connection;
                connection.Open();

                // Set the SQL query to select all from the given table
                command.CommandText = $"SELECT * FROM {tableName}";

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

        /// <summary>
        /// Commits all pending insert, update, and delete operations to the database.
        /// </summary>
        /// <returns>The number of records affected.</returns>
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

        /// <summary>
        /// Adds an entity to the list of entities to be inserted.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        public virtual void Insert(BaseEntity entity)
        {
            BaseEntity reqEntity = this.newEntity();
            if (entity != null && entity.GetType() == reqEntity.GetType())
            {
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        /// <summary>
        /// Adds an entity to the list of entities to be updated.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public virtual void Update(BaseEntity entity)
        {
            BaseEntity reqEntity = this.newEntity();
            if (entity != null && entity.GetType() == reqEntity.GetType())
            {
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        /// <summary>
        /// Adds an entity to the list of entities to be deleted.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
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