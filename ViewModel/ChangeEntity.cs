using Model;

namespace ViewModel
{
    
    public delegate string CreateSQL(BaseEntity entity);
    public class ChangedEntity
    {
        private BaseEntity entity;
        private CreateSQL createSql;

        public ChangedEntity(CreateSQL createSql, BaseEntity entity)
        {
            this.createSql = createSql;
            this.entity = entity;
        }
        
        public BaseEntity Entity { get => entity; set => entity = value; }
        public CreateSQL CreateSql { get => createSql; set => createSql = value; }
    }
}