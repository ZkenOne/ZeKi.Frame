using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZQ.SQL.Frame.DAL;
using ZQ.SQL.Frame.Model;

namespace ZQ.SQL.Frame.BLL
{
    public class SysRoleBLL : BaseBLL<SysRole>
    {
        private SysUserInfoDAL SysUserInfoDAL = new SysUserInfoDAL();

        public SysRoleBLL()
        {
            DAL = new SysRoleDAL();
        }
        

    }
}
