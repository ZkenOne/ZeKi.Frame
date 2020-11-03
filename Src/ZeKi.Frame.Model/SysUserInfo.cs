using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeKi.Frame.Model
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

        [Property(DbType.String, 100)]
        public string uRemark { get; set; }

        [Property(DbIgnore.Insert)]
        public bool uIsDel { get; set; }

        [Property(DbIgnore.Update)]
        public DateTime uAddTime { get; set; }

    }

    public class UserInfoWithDep : SysUserInfo
    {
        public string depName { get; set; }
    }
}
