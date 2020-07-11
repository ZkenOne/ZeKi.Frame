using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.IBLL
{
    public interface ISysPermissionBLL : IBaseBLL<SysPermission>
    {
        /// <summary>
        /// 展示调用
        /// </summary>
        void Example();
        
        /// <summary>
        /// 展示特性方式使用事务
        /// </summary>
        void TestTran();
    }
}
