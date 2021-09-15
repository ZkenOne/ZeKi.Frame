using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ZeKi.Frame.Common
{
    /// <summary>
    /// 表达式树帮助类
    /// </summary>
    public class ExpressionHelper
    {
        /// <summary>
        /// 获取属性的名称
        /// </summary>
        /// <returns></returns>
        public static string GetPropertyName<T, dynamic>(Expression<Func<T, dynamic>> lambda)
        {
            string propertyName = string.Empty;
            if (lambda != null)
            {
                Expression exp = lambda.Body;
                if (exp is UnaryExpression)
                {
                    propertyName = ((MemberExpression)((exp as UnaryExpression).Operand)).Member.Name;
                }
                else if (exp is MemberExpression)
                {
                    propertyName = ((MemberExpression)exp).Member.Name;
                }
                else if (exp is ParameterExpression)
                {
                    propertyName = (exp as ParameterExpression).Type.Name;
                }
            }
            return propertyName;
        }
    }
}
