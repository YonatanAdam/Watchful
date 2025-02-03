using System;
using System.Text;
using Model;

namespace ViewModel
{
    public class LocationDB : BaseDB
    {
        
        
        public override void Insert(BaseEntity entity)
        {
            Location location = entity as Location;
            if (location != null)
            {
                inserted.Add(new ChangeEntity(base.CreateInsertSQL, entity));
                inserted.Add(new ChangeEntity(this.CreateInsertSQL, entity));
            }
        }
        public override void Delete(BaseEntity entity)
        {
            Location location = entity as Location;
            if (location != null)
            {
                deleted.Add(new ChangeEntity(base.CreateDeleteSQL, entity));
                deleted.Add(new ChangeEntity(this.CreateDeleteSQL, entity));
            }
        }
        public override void Update(BaseEntity entity)
        {
            Location student = entity as Location;
            if (student != null)
            {
                updated.Add(new ChangeEntity(base.CreateUpdateSQL, entity));
                // updated.Add(new ChangeEntity(this.CreateUpdateSQL, entity));
            }
        }
        protected override string CreateInsertSQL(BaseEntity entity)
        {
            Location location = entity as Location;
            string sqlStr = string.Format("INSERT INTO LocationTbl (Latitude, Longitude) " +
                                          "VALUES('{0}','{1}')",
                location.Latitude, location.Longitude);
            return sqlStr;
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            Location location = entity as Location;
            string sqlStr = $"UPDATE LocationTbl SET Latitude = '{location.Latitude}'," +
                            $"Longitude='{location.Longitude}'" +
                            $"WHERE ID={location.Id}";
            return sqlStr;
        }

        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            Location location = entity as Location;
            StringBuilder sql_builder = new StringBuilder();
            sql_builder.AppendFormat("DELETE FROM LocationTbl WHERE ID = {0}", location.Id);
            return sql_builder.ToString();
        }

        protected override BaseEntity newEntity()
        {
            return new Location();
        }
        
        protected override BaseEntity CreateModel(BaseEntity entity)
        {
            Location locationEntity = (Location)entity;
            locationEntity.Id = Convert.ToInt32(reader["LocationId"]);
            locationEntity.Latitude = Convert.ToDouble(reader["Latitude"]);
            locationEntity.Longitude = Convert.ToDouble(reader["Longitude"]);
            return locationEntity;
        }
    }
}