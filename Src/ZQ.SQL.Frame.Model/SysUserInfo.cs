using Dapper.Extensions.ZQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZQ.SQL.Frame.Model
{
    [Table("sysUserInfo")]
    public class SysUserInfo
    {
        [Property(true, true, DbIgnore.InsertAndUpdate)]
        public int uId { get; set; }

        public int uDepId { get; set; }

        public string uLoginName { get; set; }

        public string uPwd { get; set; }

        public bool uGender { get; set; }

        public string uEmail { get; set; }

        public string uRemark { get; set; }

        [Property(DbIgnore.Insert)]
        public bool uIsDel { get; set; }

        [Property(DbIgnore.Update)]
        public DateTime uAddTime { get; set; }

    }
}
