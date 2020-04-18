using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeKi.Frame.Model
{
    /// <summary>
    /// 属性特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PropertyAttribute : Attribute
    {
        public PropertyAttribute(bool _isInc = false, bool _isPKey = false)
        {
            IsPKey = _isPKey;
            IsInc = _isInc;
        }

        public PropertyAttribute(DbIgnore _ignore = DbIgnore.No)
        {
            Ignore = _ignore;
        }

        public PropertyAttribute(bool _isPKey = false, bool _isInc = false, DbIgnore _ignore = DbIgnore.No)
        {
            IsPKey = _isPKey;
            IsInc = _isInc;
            Ignore = _ignore;
        }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPKey { get; set; }

        /// <summary>
        /// 是否自增(为true时不拼接此属性字段)
        /// </summary>
        public bool IsInc { get; set; }

        /// <summary>
        /// 新增/修改忽略标识
        /// </summary>
        public DbIgnore Ignore { get; set; }
    }

    /// <summary>
    /// 忽略新增/修改字段
    /// </summary>
    public enum DbIgnore
    {
        /// <summary>
        /// 不忽略
        /// </summary>
        No = 0,
        /// <summary>
        /// 新增数据时不添加此标注字段
        /// </summary>
        Insert = 1,
        /// <summary>
        /// 修改数据时不添加此标注字段
        /// </summary>
        Update = 2,
        /// <summary>
        /// 新增或修改数据时不添加此标注字段
        /// </summary>
        InsertAndUpdate = 3
    }
}
