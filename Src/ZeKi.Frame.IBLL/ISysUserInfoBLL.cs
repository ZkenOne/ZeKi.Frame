using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.IBLL
{
    public interface ISysUserInfoBLL : IBaseBLL<SysUserInfo>
    {
        string GetTName();
        string GetUserNameById(int id);
    }
}
