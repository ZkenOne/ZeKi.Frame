using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.DAL
{
    public class SysRoleDAL : BaseDAL<SysRole>, ISysRoleDAL
    {
        public string GetRoleNameById(int id)
        {
            return $"Role_{id}";
        }
    }
}
