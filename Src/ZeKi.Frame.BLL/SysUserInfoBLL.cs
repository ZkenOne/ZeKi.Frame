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
        public ISysUserInfoDAL SysUserInfoDAL { set; get; }
        public ISysRoleBLL SysRoleBLL { set; get; }
        public ILogger<SysUserInfoBLL> Logger { set; get; }
        public ICurrencyClient Client { set; get; }


        public string GetUserNameById(int id)
        {
            Logger.LogTrace("BLL层使用日志");
            return SysUserInfoDAL.GetUserNameById(id);
        }

        public string GetTName()
        {
            //BLL层发起Http请求
            var t2 = Client.PostAsync("http://www.cnblogs.com", "name=zzq").Result;
            return $"{t2}->{SysRoleBLL.GetRoL()}";
        }

        public override int Insert<TModel>(TModel model, bool getId = false)
        {
            Console.WriteLine("Insert");
            return SysUserInfoDAL.Insert(model, getId);
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
