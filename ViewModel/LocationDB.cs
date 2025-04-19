using System;
using System.Text;
using Model;

namespace ViewModel
{
    public class LocationDB : BaseDB
    {
        public override void Insert(BaseEntity entity)
        {
            if (entity is Location location)
            {
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }
        public override void Delete(BaseEntity entity)
        {
            if (entity is Location location)
            {
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }
        public override void Update(BaseEntity entity)
        {
            if (entity is Location student)
            {
                updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Location location = entity as Location;
            string sqlStr = string.Format("INSERT INTO LocationsTbl (UserID, Latitude, Longitude) " +
                                          "VALUES('{0}','{1}','{2}')",
                location.UserId, location.Latitude, location.Longitude);
            return sqlStr;
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            Location location = entity as Location;
            string sqlStr = $"UPDATE LocationsTbl SET Latitude = '{location.Latitude}'," +
                            $"Longitude='{location.Longitude}'" +
                            $"UserID='{location.UserId}'" +
                            $"WHERE ID={location.Id}";
            return sqlStr;
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            Location location = entity as Location;
            StringBuilder sql_builder = new StringBuilder();
            sql_builder.AppendFormat("DELETE FROM LocationsTbl WHERE ID = {0}", location.Id);
            return sql_builder.ToString();
        }

        protected override BaseEntity newEntity()
        {
            return new Location();
        }
        
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