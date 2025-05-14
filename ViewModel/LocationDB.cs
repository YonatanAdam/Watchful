using System;
using System.Text;
using Model;

namespace ViewModel
{
    /// <summary>
    /// Provides database operations for the Location entity.
    /// </summary>
    public class LocationDB : BaseDB
    {
        /// <summary>
        /// Creates the SQL statement for deleting a Location entity.
        /// </summary>
        /// <param name="entity"></param>
        public override void Insert(BaseEntity entity)
        {
            if (entity is Location location)
            {
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }

        /// <summary>
        /// Creates the SQL statement for deleting a Location entity.
        /// </summary>
        /// <param name="entity"></param>
        public override void Delete(BaseEntity entity)
        {
            if (entity is Location location)
            {
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }

        /// <summary>
        /// Creates the SQL statement for updating a Location entity.
        /// </summary>
        /// <param name="entity"></param>
        public override void Update(BaseEntity entity)
        {
            if (entity is Location student)
            {
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }

        /// <summary>
        /// Creates the SQL statement for inserting a Location entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Location location = entity as Location;
            string sqlStr = string.Format("INSERT INTO LocationsTbl (UserID, Latitude, Longitude) " +
                                          "VALUES('{0}','{1}','{2}')",
                location.UserId, location.Latitude, location.Longitude);
            return sqlStr;
        }

        /// <summary>
        /// Creates the SQL statement for updating a Location entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            Location location = entity as Location;
            string sqlStr = $"UPDATE LocationsTbl SET Latitude = '{location.Latitude}'," +
                            $"Longitude='{location.Longitude}'" +
                            $"UserID='{location.UserId}'" +
                            $"WHERE ID={location.Id}";
            return sqlStr;
        }

        /// <summary>
        /// Creates the SQL statement for deleting a Location entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            Location location = entity as Location;
            StringBuilder sql_builder = new StringBuilder();
            sql_builder.AppendFormat("DELETE FROM LocationsTbl WHERE ID = {0}", location.Id);
            return sql_builder.ToString();
        }

        /// <summary>
        /// Creates a new Location entity.
        /// </summary>
        /// <returns></returns>
        protected override BaseEntity newEntity()
        {
            return new Location();
        }

        /// <summary>
        /// Creates a Location model from the current data reader row.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override BaseEntity CreateModel(BaseEntity entity)
        {
            Location locationEntity = (Location)entity;
            locationEntity.Id = Convert.ToInt32(reader["ID"]);
            locationEntity.UserId = Convert.ToInt32(reader["UserID"]);
            locationEntity.Latitude = Convert.ToDouble(reader["Latitude"]);
            locationEntity.Longitude = Convert.ToDouble(reader["Longitude"]);
            return locationEntity;
        }
    }
}