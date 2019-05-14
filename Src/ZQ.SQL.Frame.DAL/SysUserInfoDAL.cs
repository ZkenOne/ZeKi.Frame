using Dapper.Extensions.ZQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZQ.SQL.Frame.Common;
using ZQ.SQL.Frame.Model;

namespace ZQ.SQL.Frame.DAL
{
    public class SysUserInfoDAL : BaseDAL<SysUserInfo>
    {
        public SysUserInfoDAL() : base(GlobaConfig.SqlConn, GlobaConfig.DBType, GlobaConfig.CommandTimeout)
        {

        }

        public override long Insert(SysUserInfo model, bool getId = false)
        {
            //做些其他操作
            return base.Insert(model, getId);
        }

    }
}
