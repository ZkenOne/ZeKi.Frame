using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.Common
{
    /// <summary>
    /// 配置文件获取
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public static string SqlConnStr { get; } = ConfigurationManager.GetValue("ConnectionString");

        /// <summary>
        /// BLLFullName
        /// </summary>
        public static string BLLFullName { get; } = ConfigurationManager.GetValue("BLLFullName");

        /// <summary>
        /// DALFullName
        /// </summary>
        public static string DALFullName { get; } = ConfigurationManager.GetValue("DALFullName");

        /// <summary>
        /// DALFullName
        /// </summary>
        public static DBEnums.DBType DBType { get; } = (DBEnums.DBType)ConfigurationManager.GetValue<int>("DBType");
    }
}
