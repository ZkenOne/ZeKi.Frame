using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.IDAL
{
    public interface ISysRoleDAL : IBaseDAL
    {
        string GetRoleNameById(int id);
    }
}
