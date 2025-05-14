namespace Model
{
    /// <summary>
    /// Base class for all entities in the application.
    /// </summary>
    public class BaseEntity
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public override string ToString()
        {
            return $"ID: {this.id} ";
        }
    }
}