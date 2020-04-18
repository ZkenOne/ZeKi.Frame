using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeKi.Frame.Model
{
    /// <summary>
    /// 表特性(子类模型 继承 父类模型 会继承此 特性,可以不再贴此特性)
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
