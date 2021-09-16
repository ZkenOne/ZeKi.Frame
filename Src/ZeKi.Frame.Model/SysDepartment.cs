using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeKi.Frame.Model
{
    [Table("sysDepartment")]
    public class SysDepartment
    {
        [Column(true, true, DbIgnore.Insert | DbIgnore.Update)]
        public int depId { get; set; }

        public int depPid { get; set; }

        public string depName { get; set; }

        public string depRemark { get; set; }

        public string depIsDel { get; set; }

        public string depAddTime { get; set; }
    }
}
