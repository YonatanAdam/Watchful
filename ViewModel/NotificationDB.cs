using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class NotificationDB : BaseDB
    {
        protected override string CreateDeleteSQL(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        protected override string CreateInsertSQL(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        protected override BaseEntity CreateModel(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        protected override string CreateUpdateSQL(BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        protected override BaseEntity newEntity()
        {
            throw new NotImplementedException();
        }
    }
}
