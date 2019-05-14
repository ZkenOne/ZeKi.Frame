using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using Dapper.Extensions.ZQ;

namespace ZQ.SQL.Frame.Common
{
    public class GlobaConfig
    {
        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        public static string SqlConn
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public static Enums.DBType DBType
        {
            get
            {
                return Enums.DBType.MSSQL;
            }
        }

        /// <summary>
        /// 查询/新增/修改/删除超时时间
        /// </summary>
        public static int CommandTimeout
        {
            get
            {
                return 300;
            }
        }
    }
}
