namespace Model
{
    public class Admin : BaseEntity
    {
        private string name;
        private Group group;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Group Group
        {
            get { return group; }
            set { group = value; }
        }
    }
}