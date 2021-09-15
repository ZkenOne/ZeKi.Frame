using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.Common
{
    /// <summary>
    /// 存储过程或拼写参数使用
    /// </summary>
    public class DataParameters
    {
        private readonly List<ParamInfo> parameters = new List<ParamInfo>();

        #region 添加参数方法
        /// <summary>
        /// 添加参数,使用ConditionOperator枚举指定操作符(同个字段可以添加多次,多个条件)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="conditionOperator"></param>
        /// <param name="tablePrefix">表前缀(如 tb),不用写.</param>
        public void Add(string name, object value, ConditionOperator conditionOperator = ConditionOperator.Equal, string tablePrefix = null)
        {
            BulidParameter(name, null, new ConditionValuePacket(value), conditionOperator, tablePrefix);
        }

        /// <summary>
        /// 添加参数,使用ConditionOperator枚举指定操作符(同个字段可以添加多次,多个条件)[推荐使用]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="value"></param>
        /// <param name="conditionOperator"></param>
        /// <param name="tablePrefix">表前缀(如 tb),不用写.</param>
        public void Add<T>(Expression<Func<T, dynamic>> lambda, object value, ConditionOperator conditionOperator = ConditionOperator.Equal, string tablePrefix = null)
        {
            var name = ExpressionHelper.GetPropertyName(lambda);
            BulidParameter(name, typeof(T), new ConditionValuePacket(value), conditionOperator, tablePrefix);
        }

        /// <summary>
        /// 添加修改参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="value"></param>
        public void AddUpdate<T>(Expression<Func<T, dynamic>> lambda, object value)
        {
            var name = ExpressionHelper.GetPropertyName(lambda);
            BulidParameter(name, typeof(T), new ConditionValuePacket(value), ConditionOperator.Equal, null, true);
        }

        public void AddBetween<T>(Expression<Func<T, dynamic>> lambda, object value1, object value2, string tablePrefix = null)
        {
            var name = ExpressionHelper.GetPropertyName(lambda);
            BulidParameter(name, typeof(T), new ConditionValuePacket() { Obj1 = value1, Obj2 = value2 }, ConditionOperator.Between, tablePrefix);
        }

        public void AddNotBetween<T>(Expression<Func<T, dynamic>> lambda, object value1, object value2, string tablePrefix = null)
        {
            var name = ExpressionHelper.GetPropertyName(lambda);
            BulidParameter(name, typeof(T), new ConditionValuePacket() { Obj1 = value1, Obj2 = value2 }, ConditionOperator.NotBetween, tablePrefix);
        }

        /// <summary>
        /// 用于写类次sql: (name=@name or age=@age)
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <param name="param">匿名类/字典/hashtable</param>
        /// <returns></returns>
        public void AddText(string sqlWhere, object param)
        {
            //给空值
            BulidParameter(string.Empty, null, new ConditionValuePacket() { SqlWhere = sqlWhere, Obj1 = param }, ConditionOperator.Text);
        }

        private void BulidParameter(string name, Type modelType, ConditionValuePacket packet, ConditionOperator conditionOperator, string tablePrefix = null, bool isUpdate = false)
        {
            var operatorStr = conditionOperator.GetAttribute<DescriptionAttribute>().Description;
            var value = packet.Obj1;
            SqlBaseCondition sqlCondition = null;
            switch (conditionOperator)
            {
                case ConditionOperator.Equal:
                case ConditionOperator.NotEqual:
                case ConditionOperator.GreaterThan:
                case ConditionOperator.LessThan:
                case ConditionOperator.GreaterThanEqual:
                case ConditionOperator.LessThanEqual:
                    sqlCondition = new SqlBasicCondition(operatorStr, value);
                    break;
                case ConditionOperator.Like:
                case ConditionOperator.NotLike:
                    sqlCondition = new SqlLikeCondition(operatorStr, value);
                    break;
                case ConditionOperator.IN:
                case ConditionOperator.NotIN:
                    sqlCondition = new SqlInCondition(operatorStr, value);
                    break;
                case ConditionOperator.Between:
                case ConditionOperator.NotBetween:
                    sqlCondition = new SqlBtCondition(operatorStr, value, packet.Obj2);
                    break;
                case ConditionOperator.Text:
                    sqlCondition = new SqlTextCondition(operatorStr, packet.SqlWhere, value);
                    break;
            }
            parameters.Add(new ParamInfo
            {
                Name = name,
                Value = value,
                ModelType = modelType,
                SqlCondition = sqlCondition,
                IsUpdate = isUpdate,
                TablePrefix = tablePrefix
            });
        }

        /// <summary>
        /// 添加参数(存储过程参数/手写sql方式需要指定类型)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value">值</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="direction"></param>
        /// <param name="size">size如果比实际字段内容小,则会截取文本,设置和数据库字段值一致或者大于</param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        public void Add(string name, object value, DbType dbType, ParameterDirection? direction = null, int? size = null, byte? precision = null, byte? scale = null)
        {
            parameters.Add(new ParamInfo
            {
                Name = name,
                Value = value,
                DbType = dbType,
                ParameterDirection = direction ?? ParameterDirection.Input,
                Size = size,
                Precision = precision,
                Scale = scale
            });
        }

        #endregion

        public IEnumerable<string> ParameterNames => parameters.Select(p => p.Name);

        public IEnumerable<string> ParameterUpdateNames => parameters.Where(p => p.IsUpdate).Select(p => p.Name);

        public IEnumerable<string> ParameterWhereNames => parameters.Where(p => !p.IsUpdate).Select(p => p.Name);

        /// <summary>
        /// 返回需要返回值的参数列表
        /// </summary>
        public IEnumerable<string> OutPutParameterNames
        {
            get
            {
                return parameters.Where(p => p.ParameterDirection == ParameterDirection.InputOutput
                                        || p.ParameterDirection == ParameterDirection.Output
                                        || p.ParameterDirection == ParameterDirection.ReturnValue
                       ).Select(p => p.Name);
            }
        }

        /// <summary>
        /// 设置参数化中的value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public void SetParamVal<T>(string name, T objValue)
        {
            parameters.FirstOrDefault(p => p.Name == name && !p.IsUpdate).Value = objValue;
        }

        /// <summary>
        /// 获取参数化中的value(用于存储过程Output时使用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetParamVal<T>(string name)
        {
            return (T)parameters.FirstOrDefault(p => p.Name == name && !p.IsUpdate).Value;
        }

        public List<ParamInfo> GetParameters()
        {
            return parameters;
        }

        public void Clear()
        {
            parameters.Clear();
        }

        public sealed class ParamInfo
        {
            public string Name { get; set; }
            public object Value { get; set; }

            #region 辅助字段
            /// <summary>
            /// 模型Type
            /// </summary>
            public Type ModelType { get; set; }
            public SqlBaseCondition SqlCondition { get; set; }
            /// <summary>
            /// 当前字段是否作为修改
            /// </summary>
            public bool IsUpdate { get; set; }
            /// <summary>
            /// 表前缀(如 tb),不用写.
            /// </summary>
            public string TablePrefix { get; set; }
            #endregion

            #region 字段类型等配置
            public ParameterDirection ParameterDirection { get; set; }
            public DbType? DbType { get; set; }
            /// <summary>
            /// size如果比实际字段内容小,则会截取文本,设置和数据库字段值一致或者大于
            /// </summary>
            public int? Size { get; set; }
            public byte? Precision { get; set; }
            public byte? Scale { get; set; }
            #endregion
        }

        public sealed class ConditionValuePacket
        {
            public ConditionValuePacket() { }
            public ConditionValuePacket(object obj1)
            {
                Obj1 = obj1;
            }

            public object Obj1 { get; set; }
            public object Obj2 { get; set; }
            /// <summary>
            /// text 类型时用到
            /// </summary>
            public string SqlWhere { get; set; }
        }
    }

    /// <summary>
    /// 参数化参数以及sql条件
    /// </summary>
    public class ParametersWithWhereSql
    {
        public string SqlWhere { get; set; } = string.Empty;
        public DynamicParameters Parameters { get; set; } = new DynamicParameters();
    }

    ///// <summary>
    ///// 参数化参数以及完整sql
    ///// </summary>
    //public class ParametersWithFullSql
    //{
    //    public string Sql { get; set; } = string.Empty;
    //    public DynamicParameters Parameters { get; set; } = new DynamicParameters();
    //}
}
