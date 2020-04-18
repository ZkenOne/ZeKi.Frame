using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeKi.Frame.Model
{
    public class UserWithRole
    {
        public SysRole Role { get; set; }
        public SysUserInfo User { get; set; }
    }
}
