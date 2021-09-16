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
        [Column(true, true, DbIgnore.Insert | DbIgnore.Update)]
        public int uId { get; set; }

        public int uDepId { get; set; }

        [Column(DbType.String, 50)]
        public string uLoginName { get; set; }

        public string uPwd { get; set; }

        public bool uGender { get; set; }

        public string uEmail { get; set; }

        [Column(DbType.String, 200)]
        public string uRemark { get; set; }

        [Column(DbIgnore.Insert)]
        public bool uIsDel { get; set; }

        [Column(DbIgnore.Update)]
        public DateTime uAddTime { get; set; }

    }

    public class UserInfoWithDep : SysUserInfo
    {
        public string depName { get; set; }
    }
}
