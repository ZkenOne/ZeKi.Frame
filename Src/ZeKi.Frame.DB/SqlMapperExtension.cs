using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using StackExchange.Profiling.Data;
using ZeKi.Frame.Model;
using ZeKi.Frame.Common;

namespace ZeKi.Frame.DB
{
    /// <summary>
    /// 基于Dapper扩展到IDbConnection对象方法
    /// </summary>
    public static partial class SqlMapperExtension
    {
        /// <summary>
        /// 新增/修改字段缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<string, List<string>> _paramNameCache = new ConcurrentDictionary<string, List<string>>();
        /// <summary>
        /// 表名缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<Type, string> _tableNameMap = new ConcurrentDictionary<Type, string>();
        /// <summary>
        /// 主键缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<Type, string> _pKeyNameMap = new ConcurrentDictionary<Type, string>();
        /// <summary>
        /// 参数化前缀
        /// </summary>
        private static readonly string _sqlParamPrefix = "__Param__";
        /// <summary>
        /// 修改字段的前缀(代表此字段为修改)
        /// </summary>
        private static readonly string _upset_prefix = "_new_";

        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToInsert">模型对象</param>
        /// <param name="getIncId">是否获取当前插入的ID,不是自增则不需要关注此值</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>getId为true并且有自增列则返回插入的id值,否则为影响行数</returns>
        public static int Insert<T>(this IDbConnection connection, T entityToInsert, bool getIncId = false, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            List<string> propertyNames = GetPropertyNames(type);
            string cols = string.Join(",", propertyNames);
            string colsParams = string.Join(",", propertyNames.Select(p => "@" + p));
            var sql = "insert into " + GetTableName(type) + " (" + cols + ") values (" + colsParams + ");";
            if (getIncId)
            {
                sql += GetInsertIncSql(connection);
                return connection.ExecuteScalar<int>(sql, entityToInsert, transaction, commandTimeout);
            }
            else
            {
                return connection.Execute(sql, entityToInsert, transaction, commandTimeout);
            }
        }

        /// <summary>
        /// 批量新增(自定义批次新增数,需要保证一致性可以在外部加事务)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entitysToInsert">模型对象集合</param>
        /// <param name="ps">每批次新增数,默认500</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>影响行数</returns>
        public static int BatchInsert<T>(this IDbConnection connection, IEnumerable<T> entitysToInsert, int ps = 500, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            List<string> propertyNames = GetPropertyNames(type);
            string cols = string.Join(",", propertyNames);
            string colsParams = string.Join(",", propertyNames.Select(p => "@" + p));
            var sql = "insert into " + GetTableName(type) + " (" + cols + ") values (" + colsParams + ");";
            //分批次执行
            int pi = 1, totalPage = (int)Math.Ceiling(entitysToInsert.Count() * 1.0 / ps), effectRow = 0;
            for (int i = 0; i < totalPage; i++)
            {
                var tpEntitys = entitysToInsert.Skip((pi - 1) * ps).Take(ps);
                //dapper支持 : connection.Execute(@"insert into MyTable(colA, colB) values (@a, @b)",new[] { new { a=1, b=1 }, new { a=2, b=2 }, new { a=3, b=3 } });
                effectRow += connection.Execute(sql, tpEntitys, transaction, commandTimeout);
                pi++;
            }
            return effectRow;
        }

        /// <summary>
        /// bulkcopy,仅支持myssql数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entitysToInsert"></param>
        /// <param name="timeOut">超时时间,单位：秒</param>
        public static void BulkCopyToInsert<T>(this IDbConnection connection, IEnumerable<T> entitysToInsert, SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default, IDbTransaction transaction = null, int timeOut = 60 * 10) where T : class, new()
        {
            if (!IsMsSqlConnection(connection))
                throw new NotSupportedException("只有mssql数据库支持");
            if (entitysToInsert == null || !entitysToInsert.Any())
                return;
            var sqlCon = (SqlConnection)ToActualConnetction(connection);
            SqlTransaction sqlTran = null;
            if (transaction != null)
                sqlTran = (SqlTransaction)ToActualTransaction(transaction);
            using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(sqlCon, copyOptions, sqlTran))
            {
                try
                {
                    sqlbulkcopy.DestinationTableName = GetTableName(typeof(T));
                    var table = ConvertUtil.ToDataTable(entitysToInsert);
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        sqlbulkcopy.ColumnMappings.Add(table.Columns[i].ColumnName, table.Columns[i].ColumnName);
                    }
                    sqlbulkcopy.BulkCopyTimeout = timeOut;
                    sqlbulkcopy.WriteToServer(table);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// 修改(单个,根据主键)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate">字段能否被修改受 <see cref="PropertyAttribute"/> 特性限制</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static bool Update<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            List<string> paramNames = GetPropertyNames(type, 1);
            var builder = new StringBuilder(30);
            builder.Append("update ").Append(GetTableName(type)).Append(" set ");
            builder.Append(string.Join(",", paramNames.Select(p => p + "= @" + p)));
            var pk = GetPrimaryKey(type);
            builder.Append($" where {pk} = @{pk}");
            return connection.Execute(builder.ToString(), entityToUpdate, transaction, commandTimeout) > 0;
        }

        /// <summary>
        /// 修改(根据自定义条件修改自定义值)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="setAndWhere">set和where的键值对,使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类
        /// <para>格式:new {renew_name="u",id=1},解析:set字段为renew_name="u",where条件为id=1</para>
        /// <para>修改值必须以renew_开头,如数据库字段名有此开头需要叠加</para>
        /// <para>如 where值中有集合/数组,则生成 in @Key ,sql: in ('','')</para>
        /// <para>set字段能否被修改受 <see cref="PropertyAttribute"/> 特性限制</para>
        /// </param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>影响行数</returns>
        public static int Update<T>(this IDbConnection connection, object setAndWhere, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var model_type = typeof(T);
            var dataParams = PackingToDataParams(setAndWhere);
            var setFields = dataParams.ParameterNames.Where(p => p.StartsWith(_upset_prefix));
            if (setFields == null || !setFields.Any())
                throw new ArgumentException("未设置修改值");
            if (!dataParams.ParameterNames.Any(p => !p.StartsWith(_upset_prefix)))
                throw new ArgumentException("未设置条件值");
            var allowUpFieldList = GetPropertyNames(model_type, 1);
            var forbidUpFieldList = new List<string>();
            foreach (var itemField in setFields)
            {
                var tp_name = itemField.Remove(0, _upset_prefix.Length);
                if (!allowUpFieldList.Any(p => p.ToLower() == tp_name.ToLower()))
                {
                    forbidUpFieldList.Add(tp_name);
                }
            }
            if (forbidUpFieldList.Any())
            {
                throw new ArgumentException($"字段[{string.Join(",", forbidUpFieldList)}]已设置为不允许修改");
            }
            var builder = new StringBuilder();
            builder.Append("update ").Append(GetTableName(model_type)).Append(" set ");
            //name=@new_name,flag_style=@new_flag_style
            foreach (var itemField in setFields)
            {
                builder.Append($"{itemField.Remove(0, _upset_prefix.Length)} = @{itemField},");
            }
            builder = builder.Remove(builder.Length - 1, 1);  //去掉多余的,
            var paramPacket = BuildSqlWithParams(dataParams, false);
            builder.Append(paramPacket.SqlWhere);
            return connection.Execute(builder.ToString(), paramPacket.DynamicParameters, transaction, commandTimeout);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">传递主键字段值即可,如需统一清除缓存,可以传递所有字段值数据</typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToDelete"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static bool Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var pk = GetPrimaryKey(type);
            var sql = $"delete from {GetTableName(type)} where {pk} = @{pk}";
            return connection.Execute(sql, entityToDelete, transaction, commandTimeout) > 0;
        }
        #endregion

        #region Query
        /// <summary>
        /// 查询(分页使用PageList方法)
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="sql">可以书写where name=@name,会自动补全, 也可以书写 select * from tb where name=@name</param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static IEnumerable<T> QueryList<T>(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var type = typeof(T);
            sql = BuildSqlStart(type, sql, 0);
            return connection.Query<T>(sql, AdapterParams(param), transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(分页使用PageList方法)
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static IEnumerable<T> QueryList<T>(this IDbConnection connection, object whereObj = null, string orderStr = null, string selectFields = "*", IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var type = typeof(T);
            var builder = new StringBuilder(50);
            builder.Append($"select {selectFields} from {GetTableName(type)} ");
            var paramPacket = BuildSqlWithParams(whereObj);
            builder.Append(paramPacket.SqlWhere);
            if (!string.IsNullOrEmpty(orderStr))
                builder.Append($" order by {orderStr}");
            return connection.Query<T>(builder.ToString(), paramPacket.DynamicParameters, transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(分页使用PageList方法)[适用于连表查询]
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="sqlNoWhere">sql语句,不包括where,如:select * from sysUserInfo as u left join sysDepartment as d on u.uDepId=d.depId</param>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static IEnumerable<T> QueryList<T>(this IDbConnection connection, string sqlNoWhere, object whereObj = null, string orderStr = "", IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var builder = new StringBuilder(50);
            builder.Append(sqlNoWhere);
            var paramPacket = BuildSqlWithParams(whereObj);
            builder.Append(paramPacket.SqlWhere);
            if (!string.IsNullOrEmpty(orderStr))
                builder.Append($" order by {orderStr}");
            return connection.Query<T>(builder.ToString(), paramPacket.DynamicParameters, transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(查询语句注意只写查询一条)
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">可以书写where name=@name 也可以书写 select top 1 * from tb where name=@name</param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回单个</returns>
        public static T QueryModel<T>(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var type = typeof(T);
            sql = BuildSqlStart(type, sql, 1);
            //QueryFirstOrDefault,查询如果是很多条记录,只会取其中的一条,所以需要自己写top 1
            return connection.QueryFirstOrDefault<T>(sql, AdapterParams(param), transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(查询语句注意只写查询一条)
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static T QueryModel<T>(this IDbConnection connection, object whereObj, string orderStr = null, string selectFields = "*", IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var type = typeof(T);
            var builder = new StringBuilder(50);
            builder.Append($"select top 1 {selectFields} from {GetTableName(type)} ");
            var paramPacket = BuildSqlWithParams(whereObj);
            builder.Append(paramPacket.SqlWhere);
            if (!string.IsNullOrEmpty(orderStr))
                builder.Append($" order by {orderStr}");
            return connection.QueryFirstOrDefault<T>(builder.ToString(), paramPacket.DynamicParameters, transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(查询语句注意只写查询一条)[适用于连表查询]
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="sqlNoWhere">sql语句,不包括where,如:select top 1 * from sysUserInfo as u left join sysDepartment as d on u.uDepId=d.depId</param>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static T QueryModel<T>(this IDbConnection connection, string sqlNoWhere, object whereObj = null, string orderStr = "", IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var builder = new StringBuilder(50);
            builder.Append(sqlNoWhere);
            var paramPacket = BuildSqlWithParams(whereObj);
            builder.Append(paramPacket.SqlWhere);
            if (!string.IsNullOrEmpty(orderStr))
                builder.Append($" order by {orderStr}");
            return connection.QueryFirstOrDefault<T>(builder.ToString(), paramPacket.DynamicParameters, transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="pcp">分页模型(参数传递查看<see cref="PageParameters"/>类字段注解)</param>
        /// <param name="param">查看<see cref="PageParameters"/>.Where字段注解</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static PageData<T> PageList<T>(this IDbConnection connection, PageParameters pcp, object param = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var sql = CreatePageSql<T>(connection, pcp, ref param);
            var multi = connection.QueryMultiple(sql, param, transaction, commandTimeout);
            var pageData = new PageData<T>()
            {
                Data = multi.Read<T>(),
                DataCount = multi.ReadSingle<long>(),
                PageIndex = pcp.PageIndex,
                PageSize = pcp.PageSize
            };
            pageData.PageCount = (int)Math.Ceiling(pageData.DataCount * 1.0 / pageData.PageSize);
            return pageData;
        }

        ///// <summary>
        ///// 分页查询返回数据集(需要在pcp定义表名)
        ///// </summary>
        ///// <param name="pcp">分页必填参数</param>
        ///// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        ///// <returns></returns>
        //public static DataSet PageDataSet(this IDbConnection connection, PageParameters pcp, object param = null)
        //{
        //    var sql = CreatePageSql(connection, null, pcp);
        //    return ExecDataSet(connection, sql, param);
        //}

        ///// <summary>
        ///// 分页查询返回数据表(需要在pcp定义表名)
        ///// </summary>
        ///// <param name="pcp">分页必填参数</param>
        ///// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        ///// <returns></returns>
        //public static DataTable PageDataTable(this IDbConnection connection, PageParameters pcp, object param = null)
        //{
        //    var sql = CreatePageSql(connection, null, pcp);
        //    return ExecDataTable(connection, sql, param);
        //}

        /// <summary>
        /// 创建 分页SQL语句
        /// </summary>
        /// <param name="pcp"></param>
        /// <returns></returns>
        private static string CreatePageSql<T>(IDbConnection connection, PageParameters pcp, ref object param)
        {
            var type = typeof(T);
            if (IsMsSqlConnection(connection))
            {
                if (type == null && string.IsNullOrEmpty(pcp.TableName))
                    throw new NotImplementedException("pcp中需要传递TableName");
                return CreatePageByMSSQL<T>(pcp, ref param);
            }
            throw new NotImplementedException("未实现该数据库操作");
        }

        /// <summary>
        /// 生成mssql分页sql
        /// </summary>
        /// <returns></returns>
        private static string CreatePageByMSSQL<T>(PageParameters pcp, ref object param)
        {
            //处理
            string strWhere;
            if (pcp.IsWhereTwo)
            {
                var paramPacket = BuildSqlWithParams(pcp.Where);
                strWhere = paramPacket.SqlWhere;
                //pcp.Where赋值后外部就变化了,所以这里不能赋值
                param = paramPacket.DynamicParameters;
            }
            else
            {
                strWhere = pcp.Where as string;
                param = AdapterParams(param);
            }

            //开始
            int start = (pcp.PageIndex - 1) * pcp.PageSize + 1;
            //结束
            int end = pcp.PageIndex * pcp.PageSize;
            //select字段
            pcp.ShowField = string.IsNullOrWhiteSpace(pcp.ShowField) ? "*" : pcp.ShowField;

            if (!string.IsNullOrWhiteSpace(pcp.Sql))
            {
                return $"SELECT {pcp.ShowField} FROM ( SELECT ROW_NUMBER() OVER(ORDER BY {pcp.KeyFiled}) AS ROW_NUMBER, " +
                    $" {pcp.Sql}  {strWhere} ) AS Tab WHERE ROW_NUMBER BETWEEN {start} AND {end}; " +
                    $" SELECT COUNT(0) AS DataCount FROM (SELECT {pcp.Sql} {strWhere}) AS CountTb;";
            }

            //此值为空则使用BaseDAL传入的TModel映射的表名
            if (string.IsNullOrWhiteSpace(pcp.TableName))
                pcp.TableName = GetTableName(typeof(T));
            if (string.IsNullOrWhiteSpace(pcp.OrderBy))
                pcp.OrderBy = pcp.KeyFiled;

            return $"SELECT {pcp.ShowField} FROM (SELECT ROW_NUMBER() OVER(ORDER BY {pcp.KeyFiled} " +
                $" ) AS ROW_NUMBER,{pcp.ShowField} FROM {pcp.TableName} {strWhere} " +
                $" ) Tab WHERE ROW_NUMBER BETWEEN {start} AND {end} ORDER BY {pcp.OrderBy};" +
                $" SELECT COUNT(0) AS DataCount FROM {pcp.TableName} {strWhere};";
        }
        #endregion

        #region Procedure
        /// <summary>
        /// 执行查询存储过程(用于查询数据)
        /// <para>参数传递参考：https://github.com/StackExchange/Dapper#stored-procedures </para>
        /// </summary>
        /// <typeparam name="T">返回模型,如果没有对应模型类接收,可以传入dynamic,然后序列化再反序列化成List泛型参数:Hashtable</typeparam>
        /// <param name="proceName">存储过程名</param>
        /// <param name="param">特定键值字典/Hashtable/匿名类/自定义类,过程中有OutPut或者Return参数,使用<see cref="DataParameters"/></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static IEnumerable<T> QueryProcedure<T>(this IDbConnection connection, string proceName, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var packetParam = AdapterParams(param);
            var resList = connection.Query<T>(proceName, packetParam, transaction, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure);
            if (param is DataParameters dataParams && packetParam is DynamicParameters dynamicParam)
            {
                //将output参数赋值给param
                foreach (var item in dataParams.OutPutParameterNames)
                {
                    dataParams.SetParamVal(item, dynamicParam.Get<dynamic>(item));
                }
            }
            return resList;
        }

        /// <summary>
        /// 执行存储过程(查询数据使用QueryProcedure方法)
        /// <para>参数传递参考：https://github.com/StackExchange/Dapper#stored-procedures </para>
        /// </summary>
        /// <param name="proceName">存储过程名</param>
        /// <param name="param">特定键值字典/Hashtable/匿名类/自定义类,过程中有OutPut或者Return参数,使用<see cref="DataParameters"/></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static void ExecProcedure(this IDbConnection connection, string proceName, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var packetParam = AdapterParams(param);
            connection.Execute(proceName, packetParam, transaction, commandTimeout, commandType: CommandType.StoredProcedure);
            if (param is DataParameters dataParams && packetParam is DynamicParameters dynamicParam)
            {
                //将output参数赋值给param
                foreach (var item in dataParams.OutPutParameterNames)
                {
                    dataParams.SetParamVal(item, dynamicParam.Get<dynamic>(item));
                }
            }
        }
        #endregion

        #region Statistics
        /// <summary>
        /// Count统计
        /// </summary>
        /// <param name="sqlWhere">1=1,省略where</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int Count<T>(this IDbConnection connection, string sqlWhere, object param = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var builder = new StringBuilder($"SELECT COUNT(0) FROM [{GetTableName(type)}]");
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                builder.Append($" where {sqlWhere};");
            return connection.ExecuteScalar<int>(builder.ToString(), AdapterParams(param), transaction, commandTimeout);
        }

        /// <summary>
        /// Count统计
        /// </summary>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <returns></returns>
        public static int Count<T>(this IDbConnection connection, object whereObj = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var builder = new StringBuilder($"SELECT COUNT(0) FROM [{GetTableName(type)}]");
            var paramPacket = BuildSqlWithParams(whereObj);
            builder.Append(paramPacket.SqlWhere);
            return connection.ExecuteScalar<int>(builder.ToString(), paramPacket.DynamicParameters, transaction, commandTimeout);
        }

        /// <summary>
        /// Sum统计
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="field">需要sum的字段</param>
        /// <param name="sqlWhere">1=1,省略where</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static TResult Sum<T, TResult>(this IDbConnection connection, string field, string sqlWhere, object param = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var sql = new StringBuilder($"SELECT SUM({field}) FROM [{GetTableName(type)}]");
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sql.Append($" where {sqlWhere};");
            return connection.ExecuteScalar<TResult>(sql.ToString(), AdapterParams(param), transaction, commandTimeout);
        }
        #endregion

        #region Transaction
        /// <summary>
        /// 批量数据事务提交
        /// </summary>
        /// <param name="strSqls">T-SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static int ExecTransaction(this IDbConnection connection, string strSqls, object param = null)
        {
            using (var conn = connection)
            {
                var s = 0;
                var tran = conn.BeginTransaction();
                try
                {
                    s = conn.Execute(strSqls, param, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
                return s;
            }
        }
        #endregion

        #region DataTable 
        ///// <summary>
        ///// 获取DataTable(与dapper无关)
        ///// </summary>
        ///// <param name="commandText"></param>
        ///// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        ///// <param name="commandType">CommandType为StoredProcedure时,直接写存储过程名</param>
        ///// <returns></returns>
        //public static DataTable ExecDataTable(this IDbConnection connection, string commandText, object param = null, CommandType commandType = CommandType.Text)
        //{
        //    var dt = new DataTable();
        //    using (var conn = connection)
        //    {
        //        if (IsSqlConnection(connection))
        //        {
        //            var sqlPar = param as List<SqlParameter>;
        //            SqlCommand cmd = new SqlCommand(commandText, ToActualConnetction(conn) as SqlConnection)
        //            {
        //                CommandType = commandType
        //            };
        //            if (sqlPar != null && sqlPar.Count > 0)
        //            {
        //                cmd.Parameters.AddRange(sqlPar.ToArray());
        //            }
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.Fill(dt);
        //            cmd.Parameters.Clear();
        //            return dt;
        //        }
        //        throw new NotImplementedException("未实现该数据库操作");
        //    }
        //}
        #endregion

        #region DataSet
        ///// <summary>
        ///// 获取DataSet(与dapper无关)
        ///// </summary>
        ///// <param name="commandText"></param>
        ///// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        ///// <param name="commandType">CommandType为StoredProcedure时,直接写存储过程名</param>
        ///// <returns></returns>
        //public static DataSet ExecDataSet(this IDbConnection connection, string commandText, object param = null, CommandType commandType = CommandType.Text)
        //{
        //    var ds = new DataSet();
        //    using (var conn = connection)
        //    {
        //        if (IsSqlConnection(connection))
        //        {
        //            var sqlPar = param as List<SqlParameter>;
        //            SqlCommand cmd = new SqlCommand(commandText, ToActualConnetction(conn) as SqlConnection)
        //            {
        //                CommandType = commandType
        //            };
        //            if (sqlPar != null && sqlPar.Count > 0)
        //            {
        //                cmd.Parameters.AddRange(sqlPar.ToArray());
        //            }
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.Fill(ds);
        //            cmd.Parameters.Clear();
        //            return ds;
        //        }
        //        throw new NotImplementedException("未实现该数据库操作");
        //    }
        //}
        #endregion

        #region 内部帮助/受保护类型 方法
        /// <summary>
        /// 适配参数化：DataParameters、Hashtable
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static object AdapterParams(object obj)
        {
            object objParam = null;
            if (obj is DataParameters parameters)
                objParam = parameters.ToDynamicParameters();
            else if (obj is Hashtable hashtable)
                objParam = hashtable.ToDictionary();
            return objParam ?? obj;
        }

        /// <summary>
        /// 将参数param转换成DataParameters类型
        /// </summary>
        /// <param name="param">查询参数</param>
        /// <returns></returns>
        private static DataParameters PackingToDataParams(object param)
        {
            var dataParams = new DataParameters();
            if (param is IDictionary<string, object>)
            {
                foreach (var item in (IDictionary<string, object>)param)
                {
                    AddToDataParameters(dataParams, item.Key, item.Value);
                }
            }
            else if (param is Hashtable)
            {
                foreach (DictionaryEntry item in (Hashtable)(param))
                {
                    AddToDataParameters(dataParams, item.Key.ToString(), item.Value);
                }
            }
            else if (param is DataParameters)
            {
                foreach (var item in ((DataParameters)param).GetParameters())
                {
                    AddToDataParameters(dataParams, item.Key, item.Value);
                }
            }
            else
            {
                var props = param.GetType().GetProperties();
                foreach (var itemProp in props)
                {
                    AddToDataParameters(dataParams, itemProp.Name, itemProp.GetValue(param));
                }
            }
            return dataParams;

            static void AddToDataParameters(DataParameters dataParams, string key, object objVal)
            {
                DataParameters.ParamInfo parmInfo = null;
                object obj;
                if (objVal is DataParameters.ParamInfo)
                {
                    parmInfo = (DataParameters.ParamInfo)objVal;
                    if (parmInfo.ParameterDirection != ParameterDirection.Input && (parmInfo.Value is SqlBaseCondition || objVal.IsAssemble()))
                        throw new NotSupportedException("当设置为输出参数不允许使用SqlBaseCondition或者集合/数组形式");
                    objVal = parmInfo.Value;
                }
                if (key.StartsWith(_upset_prefix) || objVal is SqlBaseCondition)
                    obj = objVal;  //不进行包装
                else if (objVal.IsAssemble())
                    obj = SCBuild.In(objVal);
                else  //默认为=
                    obj = SCBuild.Equal(objVal);
                dataParams.Add(key, obj, parmInfo?.DbType, parmInfo?.Size, parmInfo?.ParameterDirection, parmInfo?.Precision, parmInfo?.Scale);
            }
        }

        /// <summary>
        /// 生成sql条件字符串以及参数化
        /// </summary>
        /// <param name="param">查询参数化</param>
        /// <param name="packingParams">是否要调用包装 PackingToDataParams</param>
        /// <returns></returns>
        private static DynamicParametersWithSql BuildSqlWithParams(object param, bool packingParams = true)
        {
            var dataParams = packingParams ? PackingToDataParams(param) : (DataParameters)param;
            var paramPacket = new DynamicParametersWithSql();
            var fields = dataParams.ParameterNames.ToList();
            if (fields == null || fields.Count <= 0)
                return paramPacket;

            int index = 0;
            var sbSql = new StringBuilder(" where ", 50);
            for (int i = 0; i < fields.Count; i++)
            {
                var paramInfo = dataParams.GetParamInfo(fields[i]);
                sbSql.Append(BuildPart(paramInfo, paramPacket.DynamicParameters, ref index));
            }
            sbSql.Remove(sbSql.Length - 3, 3);//去掉多余的and
            paramPacket.SqlWhere = sbSql.ToString();
            return paramPacket;

            static string BuildPart(DataParameters.ParamInfo paramInfo, DynamicParameters dynamicParams, ref int index)
            {
                string sqlParam = string.Empty, partStr = string.Empty;
                var field = paramInfo.Name;
                var objVal = paramInfo.Value;
                if (!(objVal is SqlTextCondition))
                {
                    sqlParam = $"{_sqlParamPrefix}{++index}";
                }
                if (objVal is SqlBasicCondition)
                {
                    var actualCond = (SqlBasicCondition)objVal;
                    partStr = $" {field} {actualCond.SqlOpt} @{sqlParam} and";
                    dynamicParams.Add(sqlParam, actualCond.Value, paramInfo.DbType, paramInfo.ParameterDirection, paramInfo.Size, paramInfo.Precision, paramInfo.Scale);
                }
                else if (objVal is SqlInCondition)
                {
                    var actualCond = (SqlInCondition)objVal;
                    //in的时候如果指定了DbType,则拆分成一个个的参数化
                    if (paramInfo.DbType != null)
                    {
                        var inSbStr = new StringBuilder(100);
                        inSbStr.Append($" {field} {actualCond.SqlOpt} (");
                        var inList = actualCond.Value as IList;
                        if (inList == null || inList.Count <= 0)
                            throw new ArgumentNullException("In参数为空或无数据");
                        for (int i = 0; i < inList.Count; i++)
                        {
                            var tp_key = $"{sqlParam}__{i + 1}";
                            inSbStr.Append($"@{tp_key},");
                            dynamicParams.Add(tp_key, inList[i], paramInfo.DbType, paramInfo.ParameterDirection, paramInfo.Size, paramInfo.Precision, paramInfo.Scale);
                        }
                        inSbStr.Remove(inSbStr.Length - 1, 1).Append(" ) and");
                        partStr = inSbStr.ToString();
                    }
                    else
                    {
                        partStr = $" {field} {actualCond.SqlOpt} @{sqlParam} and";
                        dynamicParams.Add(sqlParam, actualCond.Value, paramInfo.DbType, paramInfo.ParameterDirection, paramInfo.Size, paramInfo.Precision, paramInfo.Scale);
                    }
                }
                else if (objVal is SqlLikeCondition)
                {
                    var actualCond = (SqlLikeCondition)objVal;
                    partStr = $" {field} {actualCond.SqlOpt} @{sqlParam} and";
                    dynamicParams.Add(sqlParam, $"%{actualCond.Value}%", paramInfo.DbType, paramInfo.ParameterDirection, paramInfo.Size, paramInfo.Precision, paramInfo.Scale);
                }
                else if (objVal is SqlBtCondition)
                {
                    var actualCond = (SqlBtCondition)objVal;
                    partStr = $" {field} {actualCond.SqlOpt} @{sqlParam}__1 and @{sqlParam}__2 and";
                    dynamicParams.Add($"{sqlParam}__1", actualCond.Value1, paramInfo.DbType, paramInfo.ParameterDirection, paramInfo.Size, paramInfo.Precision, paramInfo.Scale);
                    dynamicParams.Add($"{sqlParam}__2", actualCond.Value2, paramInfo.DbType, paramInfo.ParameterDirection, paramInfo.Size, paramInfo.Precision, paramInfo.Scale);
                }
                else if (objVal is SqlTextCondition)
                {
                    var actualCond = (SqlTextCondition)objVal;
                    partStr = $" {actualCond.SqlWhere} and";
                    if (actualCond.Parameters != null)
                    {
                        dynamicParams.AddDynamicParams(AdapterParams(actualCond.Parameters));
                    }
                }
                else
                {
                    //是修改字段
                    dynamicParams.Add(field, objVal, paramInfo.DbType, paramInfo.ParameterDirection, paramInfo.Size, paramInfo.Precision, paramInfo.Scale);
                }
                return partStr;
            }
        }

        /// <summary>
        /// 获取参数名
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flag">0:新增 1:修改 (获取属性)</param>
        /// <returns></returns>
        private static List<string> GetPropertyNames(Type type, int flag = 0)
        {
            Type t_model_type = type;
            var key = flag == 0 ? t_model_type.FullName + "_Ins" : t_model_type.FullName + "_Ups";
            List<string> paramNames = null;
            if (!_paramNameCache.TryGetValue(key, out paramNames))
            {
                paramNames = new List<string>();
                foreach (var prop in t_model_type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.GetGetMethod(false) != null))
                {
                    var propAttr = prop.GetCustomAttribute<PropertyAttribute>();
                    if (flag == 0)
                    {
                        if (propAttr == null || (propAttr.Ignore != DbIgnore.Insert && propAttr.Ignore != DbIgnore.InsertAndUpdate && !propAttr.IsInc))
                            paramNames.Add(prop.Name);
                    }
                    else
                    {
                        if (propAttr == null || (propAttr.Ignore != DbIgnore.Update && propAttr.Ignore != DbIgnore.InsertAndUpdate && !propAttr.IsInc))
                            paramNames.Add(prop.Name);
                    }
                    if (propAttr != null && propAttr.IsPKey)
                    {
                        _pKeyNameMap[t_model_type] = prop.Name;
                    }
                }
                _paramNameCache[key] = paramNames;
            }
            return paramNames;
        }

        /// <summary>
        /// 生成sql前缀(select * from table)
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="sql"></param>
        /// <param name="flag">0:所有 1:单个模型</param>
        /// <returns></returns>
        private static string BuildSqlStart(Type type, string sql, int flag = 0)
        {
            //开头为where 则自动补全
            if (sql.Trim(' ').StartsWith("where".ToLower()))
            {
                var topStr = flag == 0 ? "" : " top 1 ";
                return $"select {topStr} * from {GetTableName(type)} {sql}";
            }
            return sql;
        }

        /// <summary>
        /// 获取参数化前缀字符串 (@ : ?)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetParmPrefix(IDbConnection connection)
        {
            var parmPrefix = string.Empty;
            if (IsMsSqlConnection(connection))
                parmPrefix = "@";
            else if (IsMySqlConnection(connection))
                parmPrefix = "?";
            else if (IsOracleConnection(connection))
                parmPrefix = ":";
            return parmPrefix;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetTableName(Type type)
        {
            string tableName;
            if (!_tableNameMap.TryGetValue(type, out tableName))
            {
                var att = type.GetCustomAttribute<TableAttribute>();
                if (att == null)
                    tableName = type.Name;
                else
                    tableName = att.TableName;
                _tableNameMap[type] = tableName;
            }
            return tableName;
        }

        /// <summary>
        /// 获取表主键名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetPrimaryKey(Type type)
        {
            if (!_pKeyNameMap.ContainsKey(type))
                //触发缓存
                GetPropertyNames(type);
            if (!_pKeyNameMap.ContainsKey(type))
                throw new Exception($"Model为[{type.Name}]未设置主键");
            return _pKeyNameMap[type];
        }

        /// <summary>
        /// 获取新增之后,不同数据库获取新增主键的sql语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetInsertIncSql(IDbConnection connection)
        {
            if (IsMsSqlConnection(connection))
            {
                return "SELECT SCOPE_IDENTITY();";
            }
            throw new NotImplementedException("未实现该数据库操作");
        }

        /// <summary>
        /// 转换成真实的Connetction对象(ProfiledDbConnection包装)
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static IDbConnection ToActualConnetction(IDbConnection connection)
        {
            if (connection is ProfiledDbConnection)
                return ((ProfiledDbConnection)connection).WrappedConnection;
            return connection;
        }

        /// <summary>
        /// 转换成真实的Transaction对象(ProfiledDbTransaction)
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        private static IDbTransaction ToActualTransaction(IDbTransaction tran)
        {
            if (tran is ProfiledDbTransaction)
                return ((ProfiledDbTransaction)tran).WrappedTransaction;
            return tran;
        }

        /// <summary>
        /// 是否为mssql
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static bool IsMsSqlConnection(IDbConnection connection)
        {
            var conn = ToActualConnetction(connection);
            return (conn is SqlConnection);
        }

        /// <summary>
        /// 是否为mysql
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static bool IsMySqlConnection(IDbConnection connection)
        {
            //var conn = ToActualConnetction(connection);
            //return (conn is SqlConnection);

            throw new Exception("not imp");
        }

        /// <summary>
        /// 是否为oracle
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static bool IsOracleConnection(IDbConnection connection)
        {
            //var conn = ToActualConnetction(connection);
            //return (conn is SqlConnection);

            throw new Exception("not imp");
        }

        #endregion
    }
}
