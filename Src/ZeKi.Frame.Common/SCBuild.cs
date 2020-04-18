using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.Common
{
    public static class SCBuild
    {
        public static SqlBaseCondition Equal(object obj)
        {
            return new SqlBasicCondition("=", obj);
        }
        public static SqlBaseCondition Gt(object obj)
        {
            return new SqlBasicCondition(">", obj);
        }
        public static SqlBaseCondition Less(object obj)
        {
            return new SqlBasicCondition("<", obj);
        }
        public static SqlBaseCondition GtEqual(object obj)
        {
            return new SqlBasicCondition(">=", obj);
        }
        public static SqlBaseCondition LessEqual(object obj)
        {
            return new SqlBasicCondition("<=", obj);
        }
        public static SqlBaseCondition NotEqual(object obj)
        {
            return new SqlBasicCondition("<>", obj);
        }
        public static SqlBaseCondition Like(object obj)
        {
            return new SqlLikeCondition("like", obj);
        }
        public static SqlBaseCondition NotLike(object obj)
        {
            return new SqlLikeCondition("not like", obj);
        }
        public static SqlBaseCondition In(object obj)
        {
            return new SqlInCondition("in", obj);
        }
        public static SqlBaseCondition NotIn(object obj)
        {
            return new SqlInCondition("not in", obj);
        }
        public static SqlBaseCondition Between(object obj1, object obj2)
        {
            return new SqlBtCondition("between", obj1, obj2);
        }
        public static SqlBaseCondition NotBetween(object obj1, object obj2)
        {
            return new SqlBtCondition("not between", obj1, obj2);
        }

        /// <summary>
        /// 用于写类次sql: (name=@name or age=@age)
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <param name="parameters">匿名类/字典/hashtable</param>
        /// <returns></returns>
        public static SqlBaseCondition Text(string sqlWhere, object parameters = null)
        {
            return new SqlTextCondition(string.Empty, sqlWhere, parameters);
        }

        /// <summary>
        /// 组合条件,用于一个字段需要多个条件的情况
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static object Combin(params SqlBaseCondition[] conditions)
        {
            if (!(conditions is SqlBaseCondition[]))
                throw new InvalidCastException("传入类型有误");
            return conditions;
        }
    }

}
