using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZQ.SQL.Frame.DAL;
using ZQ.SQL.Frame.Model;

namespace ZQ.SQL.Frame.BLL
{
    public class SysUserInfoBLL : BaseBLL<SysUserInfo>
    {
        public SysUserInfoBLL()
        {
            DAL = new SysUserInfoDAL();
        }

        public override long Insert(SysUserInfo model, bool getId = false)
        {
            //重写父类方法,可以在此进行清除缓存等
            Console.WriteLine("Insert");
            return base.Insert(model, getId);
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

        public override IEnumerable<SysUserInfo> QueryList(string sql, object param = null)
        {
            //重写父类方法,可以在此进行清除缓存等
            Console.WriteLine("QueryList");
            return base.QueryList(sql, param);
        }

        public override IEnumerable<T> QueryList<T>(string sql, object param = null)
        {
            return base.QueryList<T>(sql, param);
        }

        public override T QueryModel<T>(string sql, object param = null)
        {
            //重写父类方法,可以在此进行清除缓存等
            Console.WriteLine("QueryModel");
            return base.QueryModel<T>(sql, param);
        }
    }
}
