using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZeKi.Frame.Common;
using ZeKi.Frame.IBLL;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.BLL
{
    public class SysUserInfoBLL : BaseBLL<SysUserInfo>, ISysUserInfoBLL
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

        public override int Insert(SysUserInfo model, bool getId = false)
        {
            //重写父类方法,可以在此进行清除缓存等
            Console.WriteLine("Insert");
            return SysUserInfoDAL.Insert(model, getId);
        }

        public override bool Update(SysUserInfo model)
        {
            //重写父类方法,可以在此进行清除缓存等
            Console.WriteLine("Update");
            return base.Update(model);
        }

        public override bool Delete(SysUserInfo model)
        {
            //重写父类方法,可以在此进行清除缓存等
            Console.WriteLine("Delete");
            return base.Delete(model);
        }

    }
}
