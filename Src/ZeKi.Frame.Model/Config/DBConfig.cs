using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeKi.Frame.Model.Config
{
    public class DBConfig
    {
        public string ConnectionString { get; set; }
        /// <summary>
        /// 对应<see cref="DBEnums.DBType"/>枚举值
        /// </summary>
        public DBEnums.DBType Type { get; set; }
        /// <summary>
        /// 执行sql超时时间(秒)
        /// </summary>
        public int CommandTimeout { get; set; }
    }
}
