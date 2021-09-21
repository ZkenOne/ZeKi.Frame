using System;
using Microsoft.Extensions.Logging;
using ZeKi.Frame.Common;
using ZeKi.Frame.IBLL;
using ZeKi.Frame.IDAL;

namespace ZeKi.Frame.BLL
{
    public class SysUserInfoBLL : BaseBLL, ISysUserInfoBLL
    {
        //使用DAL和BLL
        private readonly ISysUserInfoDAL _sysUserInfoDAL;
        private readonly ISysRoleBLL _sysRoleBLL;
        private readonly ILogger<SysUserInfoBLL> _logger;
        private readonly ICurrencyClient _client;

        public SysUserInfoBLL(ISysUserInfoDAL sysUserInfoDAL, ISysRoleBLL sysRoleBLL, ILogger<SysUserInfoBLL> logger, ICurrencyClient client, IBaseDAL baseDAL) : base(baseDAL)
        {
            _sysUserInfoDAL = sysUserInfoDAL;
            _sysRoleBLL = sysRoleBLL;
            _logger = logger;
            _client = client;
        }


        public string GetUserNameById(int id)
        {
            _logger.LogTrace("BLL层使用日志");
            return _sysUserInfoDAL.GetUserNameById(id);
        }

        public string GetTName()
        {
            //BLL层发起Http请求
            var t2 = _client.PostAsync("http://www.cnblogs.com", "name=zzq").Result;
            return $"{t2}->{_sysRoleBLL.GetRoL()}";
        }

        public override int Insert<TModel>(TModel model, bool getId = false)
        {
            Console.WriteLine("Insert");
            return _sysUserInfoDAL.Insert(model, getId);
        }

        public override bool Update<TModel>(TModel model)
        {
            Console.WriteLine("Update");
            return base.Update(model);
        }

        public override bool Delete<TModel>(TModel model)
        {
            Console.WriteLine("Delete");
            return base.Delete(model);
        }

    }
}
