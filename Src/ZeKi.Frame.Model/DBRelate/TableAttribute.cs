using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeKi.Frame.Model
{
    /// <summary>
    /// 表特性(子类模型 继承 父类模型 会继承此 特性,可以不再贴此特性,如果贴此特性,则加的字段需要贴PropertyAttribute且DbIgnore为DbIgnore.Select)
    /// <para>非继承类,建议使用QueryList、QueryModel包含两个泛型参数TTable, TResult方法，而不是新建类标注此特性（字段属性的特性不能沿用）</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string _tableName)
        {
            TableName = _tableName;
        }

        /// <summary>
        /// 数据库表名
        /// </summary>
        public string TableName { get; set; }
    }
}
