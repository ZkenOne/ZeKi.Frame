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
    public class SysRoleDAL : BaseDAL<SysRole>
    {
        public SysRoleDAL() : base(GlobaConfig.SqlConn, GlobaConfig.DBType, GlobaConfig.CommandTimeout)
        {

        }

    }
}
