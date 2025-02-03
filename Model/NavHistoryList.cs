using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class NavHistoryList : List<NavHistory>
    {
        public NavHistoryList()
        {
        }

        public NavHistoryList(IEnumerable<NavHistory> list) : base(list)
        {
        }

        public NavHistoryList(IEnumerable<BaseEntity> list) : base(list.Cast<NavHistory>().ToList())
        {
        }
    }
}