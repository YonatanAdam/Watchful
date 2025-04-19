using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class GroupList : List<Group>
    {
        public GroupList()
        {
        }

        // public GroupList(IEnumerable<NavHistory> list) : base(list)
        // {
        // }

        public GroupList(IEnumerable<BaseEntity> list) : base(list.Cast<Group>().ToList())
        {
        }
    }
}