using System;
using System.Collections.Generic;
using System.Text;

namespace ZeKi.Frame.Model
{
    public class SqlBaseCondition
    {
        public SqlBaseCondition(string sqlOpt)
        {
            SqlOpt = sqlOpt;
        }
        public string SqlOpt { get; protected set; }
    }

    public class SqlBasicCondition : SqlBaseCondition
    {
        public SqlBasicCondition(string sqlOpt, object value) : base(sqlOpt)
        {
            SqlOpt = sqlOpt;
            Value = value;
        }
        public object Value { get; private set; }
    }

    public class SqlTextCondition : SqlBaseCondition
    {
        public SqlTextCondition(string sqlOpt, string sqlWhere, object parameters) : base(sqlOpt)
        {
            SqlOpt = sqlOpt;
            SqlWhere = sqlWhere;
            Parameters = parameters;
        }
        /// <summary>
        /// sql字符串 eg: name=@name and age=@age 或者 (name=@name or age=@age)
        /// </summary>
        public string SqlWhere { get; private set; }
        /// <summary>
        /// SQLWhere 中的 参数 键值对
        /// </summary>
        public object Parameters { get; private set; }
    }

    public class SqlLikeCondition : SqlBaseCondition
    {
        public SqlLikeCondition(string sqlOpt, object value) : base(sqlOpt)
        {
            SqlOpt = sqlOpt;
            Value = value;
        }
        public object Value { get; private set; }
    }

    public class SqlInCondition : SqlBaseCondition
    {
        public SqlInCondition(string sqlOpt, object value) : base(sqlOpt)
        {
            SqlOpt = sqlOpt;
            Value = value;
        }
        public object Value { get; private set; }
    }

    /// <summary>
    /// between 用于
    /// </summary>
    public class SqlBtCondition : SqlBaseCondition
    {
        public SqlBtCondition(string sqlOpt, object value1, object value2) : base(sqlOpt)
        {
            SqlOpt = sqlOpt;
            Value1 = value1;
            Value2 = value2;
        }
        public object Value1 { get; private set; }
        public object Value2 { get; private set; }
    }
}
