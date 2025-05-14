using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class RulesList : List<Rule>
    {
        public RulesList()
        {
        }

        public RulesList(IEnumerable<User> list) : base()
        {
        }

        public RulesList(IEnumerable<BaseEntity> list) : base(list.Cast<Rule>().ToList())
        {
        }
    }
}
