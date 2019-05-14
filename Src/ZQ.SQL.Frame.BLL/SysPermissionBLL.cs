using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extensions.ZQ;
using ZQ.SQL.Frame.DAL;
using ZQ.SQL.Frame.Model;

namespace ZQ.SQL.Frame.BLL
{
    public class SysPermissionBLL : BaseBLL<SysPermission>
    {
        public SysPermissionBLL()
        {
            DAL = new SysPermissionDAL();
        }
 
    }
}
