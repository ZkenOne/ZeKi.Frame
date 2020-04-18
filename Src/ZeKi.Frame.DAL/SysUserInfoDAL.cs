using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.DAL
{
    public class SysUserInfoDAL : BaseDAL<SysUserInfo>, ISysUserInfoDAL
    {
        //使用DAL
        public ISysRoleDAL SysRoleDAL { set; get; }
        public ILogger<SysUserInfoDAL> Logger { set; get; }

        public string GetUserNameById(int id)
        {
            Logger.LogTrace("DAL层使用日志");
            var uModel = QueryModel(new { uId = id }, "uLoginName");
            var res_count = SysRoleDAL.Count(new { rId = 1 });
            return $"User_{uModel?.uLoginName}_{res_count}";
        }

        public override int Insert(SysUserInfo model, bool getId = false)
        {
            //做些其他操作
            return base.Insert(model, getId);
        }

    }
}
