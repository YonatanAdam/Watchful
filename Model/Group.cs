namespace Model
{
    public class Group : BaseEntity
    {
        private Admin admin;
        private User user;

        public Admin Admin
        {
            get { return admin; }
            set { admin = value; }
        }

        public User User
        {
            get { return user; }
            set { user = value; }
        }
    }
}