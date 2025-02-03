namespace Model
{
    public class POI : BaseEntity
    {
        private string name;
        private Location location;
        private Category category;
        private string description;
        private string image_path;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Location Location
        {
            get { return location; }
            set { location = value; }
        }

        public Category Category
        {
            get { return category; }
            set { category = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string ImagePath
        {
            get { return image_path; }
            set { image_path = value; }
        }
    }
}