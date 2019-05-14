using Dapper.Extensions.ZQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZQ.SQL.Frame.Model
{
    [Table("sysRole_1")]
    public class SysRole
    {
        public int rId { get; set; }

        public int rDepId { get; set; }

        public string rName { get; set; }

        public string rRemark { get; set; }

        public bool rIsDel { get; set; }

        public DateTime rAddTime { get; set; }

    }
}
