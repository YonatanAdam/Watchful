using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class UserList : List<User>
    {
        public UserList()
        {
        }

        public UserList(IEnumerable<User> list) : base(list)
        {
        }

        public UserList(IEnumerable<BaseEntity> list) : base(list.Cast<User>().ToList())
        {
        }
    }
}