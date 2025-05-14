using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class LocationList : List<Location>
    {
        public LocationList()
        {
        }

        public LocationList(IEnumerable<BaseEntity> list) : base(list.Cast<Location>().ToList())
        {
        }
    }
}