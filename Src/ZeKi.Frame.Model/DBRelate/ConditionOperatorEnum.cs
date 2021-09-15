using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ZeKi.Frame.Model
{
    /// <summary>
    /// 条件操作符  枚举
    /// </summary>
    public enum ConditionOperator
    {
        /// <summary>
        /// 等于
        /// </summary>
        [Description("=")]
        Equal,
        /// <summary>
        /// 不等于
        /// </summary>
        [Description("<>")]
        NotEqual,
        /// <summary>
        /// 大于
        /// </summary>
        [Description(">")]
        GreaterThan,
        /// <summary>
        /// 小于
        /// </summary>
        [Description("<")]
        LessThan,
        /// <summary>
        /// 大于等于
        /// </summary>
        [Description(">=")]
        GreaterThanEqual,
        /// <summary>
        /// 小于等于
        /// </summary>
        [Description("<=")]
        LessThanEqual,
        /// <summary>
        /// LIKE
        /// </summary>
        [Description("like")]
        Like,
        /// <summary>
        /// NOT LIKE
        /// </summary>
        [Description("not like")]
        NotLike,
        /// <summary>
        /// IN
        /// </summary>
        [Description("in")]
        IN,
        /// <summary>
        /// NOT IN
        /// </summary>
        [Description("not in")]
        NotIN,
        /// <summary>
        /// BETWEEN
        /// </summary>
        [Description("between")]
        Between,
        /// <summary>
        /// NOT BETWEEN
        /// </summary>
        [Description("not between")]
        NotBetween,
        /// <summary>
        /// 自定义sql条件
        /// </summary>
        [Description("")]
        Text
    }
}
