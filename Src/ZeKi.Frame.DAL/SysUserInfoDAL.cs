using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZeKi.Frame.DB;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.DAL
{
    public class SysUserInfoDAL : BaseDAL, ISysUserInfoDAL
    {
        //使用DAL
        private readonly ISysRoleDAL _sysRoleDAL;
        private readonly ILogger<SysUserInfoDAL> _logger;

        public SysUserInfoDAL(ISysRoleDAL sysRoleDAL, ILogger<SysUserInfoDAL> logger, DbContext dbContext) : base(dbContext)
        {
            _sysRoleDAL = sysRoleDAL;
            _logger = logger;
        }


        public string GetUserNameById(int id)
        {
            _logger.LogTrace("DAL层使用日志");
            var uModel = QueryModel<SysUserInfo>(new { uId = id }, "uLoginName");
            var res_count = _sysRoleDAL.Count<SysUserInfo>(new { uId = 1 });
            return $"User_{uModel?.uLoginName}_{res_count}";
        }

        public override int Insert<TModel>(TModel model, bool getId = false)
        {
            //清空缓存等
            return base.Insert(model, getId);
        }

    }
}
