using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.Extensions.ZQ
{
    /// <summary>
    /// 表特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string _tableName)
        {
            TableName = _tableName;
        }

        /// <summary>
        /// 数据库表名(用于改写与model实体类不一致)
        /// </summary>
        public string TableName { get; set; }
    }
}
