using System;
using System.Collections.Generic;
using System.Data;
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
        public PropertyAttribute(DbIgnore _ignore)
        {
            Ignore = _ignore;
        }

        /// <summary>
        /// 指定参数化字段类型
        /// </summary>
        /// <param name="_dbType"></param>
        public PropertyAttribute(DbType _dbType)
        {
            DbType = _dbType;
        }

        public PropertyAttribute(bool _isPKey = false, bool _isInc = false, DbIgnore _ignore = DbIgnore.No)
        {
            IsPKey = _isPKey;
            IsInc = _isInc;
            Ignore = _ignore;
        }

        /// <summary>
        /// 指定参数化字段类型
        /// </summary>
        /// <param name="_dbType"></param>
        /// <param name="_size">size如果比实际字段内容小,则会截取文本,设置和数据库字段值一致或者大于</param>
        public PropertyAttribute(DbType _dbType, int _size, DbIgnore _ignore = DbIgnore.No)
        {
            DbType = _dbType;
            Size = _size;
            Ignore = _ignore;
        }

        /// <summary>
        /// 指定参数化字段类型
        /// </summary>
        /// <param name="_dbType"></param>
        /// <param name="_size">size如果比实际字段内容小,则会截取文本,设置和数据库字段值一致或者大于</param>
        /// <param name="_precision"></param>
        /// <param name="_scale"></param>
        public PropertyAttribute(DbType _dbType, int _size, byte _precision, byte _scale)
        {
            DbType = _dbType;
            Size = _size;
            Precision = _precision;
            Scale = _scale;
        }

        public PropertyAttribute(bool _isPKey, bool _isInc, DbIgnore _ignore, DbType _dbType, int _size)
        {
            IsPKey = _isPKey;
            IsInc = _isInc;
            Ignore = _ignore;
            DbType = _dbType;
            Size = _size;
        }

        public PropertyAttribute(bool _isPKey, bool _isInc, DbIgnore _ignore, DbType _dbType, int _size, byte _precision, byte _scale)
        {
            IsPKey = _isPKey;
            IsInc = _isInc;
            Ignore = _ignore;
            DbType = _dbType;
            Size = _size;
            Precision = _precision;
            Scale = _scale;
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
        /// 查询/新增/修改忽略标识
        /// </summary>
        public DbIgnore Ignore { get; set; }

        /// <summary>
        /// 属性名,反射获取属性的特性用到,不用赋值
        /// </summary>
        public string Name { get; set; }

        #region 指定字段类型(只有不手写where条件并且为单表才起效)
        public DbType? DbType { get; set; }
        public int? Size { get; set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        #endregion
    }

    /// <summary>
    /// 忽略查询/新增/修改字段,多个用|分隔
    /// </summary>
    [Flags]
    public enum DbIgnore
    {
        /*/
        * 枚举的每一项值, 用2的n次方来赋值,参考系统自带的 AttributeTargets
        */

        /// <summary>
        /// 不忽略
        /// </summary>
        No = 1,
        /// <summary>
        /// 忽略新增
        /// </summary>
        Insert = 2,
        /// <summary>
        /// 忽略修改
        /// </summary>
        Update = 4,
        /// <summary>
        /// 忽略查询
        /// </summary>
        Select = 8,
        /// <summary>
        /// 忽略所有
        /// </summary>
        All = 16
    }
}
