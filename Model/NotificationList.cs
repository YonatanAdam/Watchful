using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class NotificationList : List<Notification>
    {
        public NotificationList()
        {
        }
        public NotificationList(IEnumerable<Notification> collection) : base(collection)
        {
        }
    }
}
