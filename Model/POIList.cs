using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class POIList : List<POI>
    {
        public POIList()
        {
        }

        public POIList(IEnumerable<POI> list) : base(list)
        {
        }

        public POIList(IEnumerable<BaseEntity> list) : base(list.Cast<POI>().ToList())
        {
        }
        
        
    }
}