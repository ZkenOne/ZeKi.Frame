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
    internal static partial class SqlMapperExtension
    {
        /// <summary>
        /// 新增/修改/查询字段 缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<string, List<string>> _fieldNameCache = new ConcurrentDictionary<string, List<string>>();
        /// <summary>
        /// 类Type对应属性特性缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<Type, List<PropertyAttribute>> _typePropertyMap = new ConcurrentDictionary<Type, List<PropertyAttribute>>();
        /// <summary>
        /// 类Type对应表名缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<Type, string> _typeTableNameMap = new ConcurrentDictionary<Type, string>();
        /// <summary>
        /// 类Type对应主键字段名缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<Type, string> _typePKeyNameMap = new ConcurrentDictionary<Type, string>();
        /// <summary>
        /// 类Type对应表Type缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type> _typeTableTypeMap = new ConcurrentDictionary<Type, Type>();
        /// <summary>
        /// 参数化前缀
        /// </summary>
        private static readonly string _sqlParamPrefix = "__Param__";

        private static readonly ISqlAdapter _defaultAdapter = new SqlServerAdapter();
        private static readonly Dictionary<string, ISqlAdapter> _adapterDictionary = new Dictionary<string, ISqlAdapter>
        {
            ["sqlconnection"] = new SqlServerAdapter(),
            ["mysqlconnection"] = new MySqlAdapter(),
            ["sqliteconnection"] = new SQLiteAdapter(),
        };

        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToInsert">模型对象</param>
        /// <param name="getIncVal">是否获取当前插入的自增值,不是自增则不需要关注此值</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>getIncVal为true并且有自增列则返回插入自增值,否则为影响行数</returns>
        public static int Insert<T>(this IDbConnection connection, T entityToInsert, bool getIncVal = false, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToInsert == null)
                return 0;
            var type = typeof(T);
            var adapter = GetSqlAdapter(connection);
            List<string> propertyNames = GetPropertyNames(type);
            string columnsStr = string.Join(",", propertyNames);
            string parametersStr = string.Join(",", propertyNames.Select(p => $"{adapter.ParametricSymbol}{p}"));
            return adapter.Insert(connection, transaction, commandTimeout, GetTableName(type), columnsStr, parametersStr, entityToInsert, getIncVal);
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
            if (entitysToInsert == null || !entitysToInsert.Any())
                return 0;
            var type = typeof(T);
            var adapter = GetSqlAdapter(connection);
            List<string> propertyNames = GetPropertyNames(type);
            string columnsStr = string.Join(",", propertyNames);
            string parametersStr = string.Join(",", propertyNames.Select(p => $"{adapter.ParametricSymbol}{p}"));
            var tableName = GetTableName(type);
            //分批次执行
            int pi = 1, totalPage = (int)Math.Ceiling(entitysToInsert.Count() * 1.0 / ps), effectRow = 0;
            for (int i = 0; i < totalPage; i++)
            {
                var tpEntitys = entitysToInsert.Skip((pi - 1) * ps).Take(ps);
                //dapper支持 : connection.Execute(@"insert into MyTable(colA, colB) values (@a, @b)",new[] { new { a=1, b=1 }, new { a=2, b=2 }, new { a=3, b=3 } });
                effectRow += adapter.Insert(connection, transaction, commandTimeout, tableName, columnsStr, parametersStr, tpEntitys);
                pi++;
            }
            return effectRow;
        }

        /// <summary>
        /// bulkcopy,仅支持mssql数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entitysToInsert"></param>
        /// <param name="timeOut">超时时间,单位：秒</param>
        public static void BulkCopyToInsert<T>(this IDbConnection connection, IEnumerable<T> entitysToInsert, SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default, IDbTransaction transaction = null, int timeOut = 60 * 10) where T : class, new()
        {
            if (!(ToActualConnetction(connection) is SqlConnection))
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
            List<string> propertyNames = GetPropertyNames(type, 1);
            var adapter = GetSqlAdapter(connection);
            var builder = new StringBuilder(30);
            builder.Append("update ").Append(GetTableName(type)).Append(" set ");
            for (var i = 0; i < propertyNames.Count; i++)
            {
                var propertyName = propertyNames[i];
                builder.Append($"{propertyName}={adapter.ParametricSymbol}{propertyName}");
                if (i < propertyNames.Count - 1)
                    builder.Append(", ");
            }
            var pk = GetPrimaryKey(type);
            builder.Append($" where {pk}={adapter.ParametricSymbol}{pk}");
            return connection.Execute(builder.ToString(), entityToUpdate, transaction, commandTimeout) > 0;
        }

        /// <summary>
        /// 修改(根据自定义条件修改自定义值,where条件字段会沿用标注的属性特性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="setAndWhere">使用指定数据类型类<see cref="DataParameters"/>.AddUpdate方法
        /// <para>set字段能否被修改受 <see cref="PropertyAttribute"/> 特性影响</para>
        /// </param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>影响行数</returns>
        public static int UpdatePart<T>(this IDbConnection connection, object setAndWhere, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var model_type = typeof(T);
            var table_name = GetTableName(model_type);
            var adapter = GetSqlAdapter(connection);
            var dataParams = PackingToDataParams(setAndWhere);
            var setFields = dataParams.ParameterUpdateNames;
            var whereFields = dataParams.ParameterWhereNames;
            if (setFields == null || !setFields.Any())
                throw new ArgumentException("Update未设置修改值");
            if (whereFields == null || !whereFields.Any())
                throw new ArgumentException("Update未设置条件值");
            var allowUpFieldList = GetPropertyNames(model_type, 1);
            var forbidUpFieldList = new List<string>();
            foreach (var itemField in setFields)
            {
                if (!allowUpFieldList.Any(p => p.ToLower() == itemField.ToLower()))
                {
                    forbidUpFieldList.Add(itemField);
                }
            }
            if (forbidUpFieldList.Any())
            {
                throw new ArgumentException($"字段[{string.Join(",", forbidUpFieldList)}]已设置为不允许修改或在表[{table_name}]中不存在");
            }
            var builder = new StringBuilder();
            builder.Append("update ").Append(table_name).Append(" set ");
            //name=@new_name,flag_style=@new_flag_style
            for (int i = 0; i < setFields.Count(); i++)
            {
                var itemField = setFields.ElementAt(i);
                builder.Append($"{itemField}={adapter.ParametricSymbol}{itemField}");
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1); //去掉多余的,
            var paramPacket = BuildWhereSqlWithParams(adapter, model_type, dataParams, false);
            builder.Append(paramPacket.SqlWhere);
            return connection.Execute(builder.ToString(), paramPacket.Parameters, transaction, commandTimeout);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">如需统一清除缓存,可以传递删除缓存相关字段</typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToDelete"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static bool Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var adapter = GetSqlAdapter(connection);
            var pk = GetPrimaryKey(type);
            var builder = new StringBuilder($"delete from {GetTableName(type)} where ");
            builder.Append($" {pk}={adapter.ParametricSymbol}{pk}");
            return connection.Execute(builder.ToString(), entityToDelete, transaction, commandTimeout) > 0;
        }
        #endregion

        #region Query
        /// <summary>
        /// 查询(分页使用PageList方法)
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="sql">全sql</param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static IEnumerable<TResult> QueryList<TResult>(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return connection.Query<TResult>(sql, AdapterParams(param), transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(分页使用PageList方法)
        /// </summary>
        /// <typeparam name="TTable">表对应模型</typeparam>
        /// <typeparam name="TResult">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔,默认全部</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static IEnumerable<TResult> QueryList<TTable, TResult>(this IDbConnection connection, object whereObj = null, string orderStr = null, string selectFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var builder = new StringBuilder(50);
            if (selectFields.IsNullOrWhiteSpace())
                selectFields = string.Join(",", GetPropertyNames(typeof(TTable), 2));
            builder.Append($"select {selectFields} from {GetTableName(typeof(TTable))} ");
            var paramPacket = BuildWhereSqlWithParams(GetSqlAdapter(connection), typeof(TTable), whereObj);
            builder.Append(paramPacket.SqlWhere);
            if (!string.IsNullOrEmpty(orderStr))
                builder.Append($" order by {orderStr}");
            return connection.Query<TResult>(builder.ToString(), paramPacket.Parameters, transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(分页使用PageList方法)[适用于连表查询]
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="sqlNoWhere">sql语句,不包括where,如:select * from sysUserInfo as u left join sysDepartment as d on u.uDepId=d.depId</param>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static IEnumerable<TResult> QueryJoinList<TResult>(this IDbConnection connection, string sqlNoWhere, object whereObj, string orderStr, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var builder = new StringBuilder(50);
            builder.Append(sqlNoWhere);
            var paramPacket = BuildWhereSqlWithParams(GetSqlAdapter(connection), null, whereObj);
            builder.Append(paramPacket.SqlWhere);
            if (!string.IsNullOrEmpty(orderStr))
                builder.Append($" order by {orderStr}");
            return connection.Query<TResult>(builder.ToString(), paramPacket.Parameters, transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(查询语句注意只写查询一条)
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="sql">全sql</param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回单个</returns>
        public static TResult QueryModel<TResult>(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            //QueryFirstOrDefault,查询如果是很多条记录,只会取其中的一条,所以需要自己写top 1
            return connection.QueryFirstOrDefault<TResult>(sql, AdapterParams(param), transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TTable">表对应模型</typeparam>
        /// <typeparam name="TResult">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static TResult QueryModel<TTable, TResult>(this IDbConnection connection, object whereObj, string orderStr = null, string selectFields = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var type = typeof(TTable);
            var builder = new StringBuilder(50);
            if (selectFields.IsNullOrWhiteSpace())
                selectFields = string.Join(",", GetPropertyNames(typeof(TTable), 2));
            builder.Append($"select top 1 {selectFields} from {GetTableName(type)} ");
            var paramPacket = BuildWhereSqlWithParams(GetSqlAdapter(connection), type, whereObj);
            builder.Append(paramPacket.SqlWhere);
            if (!string.IsNullOrEmpty(orderStr))
                builder.Append($" order by {orderStr}");
            return connection.QueryFirstOrDefault<TResult>(builder.ToString(), paramPacket.Parameters, transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(查询语句注意只写查询一条)[适用于连表查询]
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="sqlNoWhere">sql语句,不包括where,如:select top 1 * from sysUserInfo as u left join sysDepartment as d on u.uDepId=d.depId</param>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>返回集合</returns>
        public static TResult QueryJoinModel<TResult>(this IDbConnection connection, string sqlNoWhere, object whereObj, string orderStr, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var builder = new StringBuilder(50);
            builder.Append(sqlNoWhere);
            var paramPacket = BuildWhereSqlWithParams(GetSqlAdapter(connection), null, whereObj);
            builder.Append(paramPacket.SqlWhere);
            if (!string.IsNullOrEmpty(orderStr))
                builder.Append($" order by {orderStr}");
            return connection.QueryFirstOrDefault<TResult>(builder.ToString(), paramPacket.Parameters, transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="pcp">分页模型(参数传递查看<see cref="PageParameters"/>类字段注解)</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static PageData<TResult> PageList<TResult>(this IDbConnection connection, PageParameters pcp, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var adapter = GetSqlAdapter(connection);
            var paramPacket = BuildWhereSqlWithParams(adapter, null, pcp.WhereParam);
            return adapter.Pager<TResult>(connection, transaction, commandTimeout, new SqlAdapterPagerParameters()
            {
                PageIndex = pcp.PageIndex,
                PageSize = pcp.PageSize,
                Order = pcp.Order,
                Select = pcp.Select,
                Table = pcp.Table,
                Params = paramPacket.Parameters,
                Where = paramPacket.SqlWhere
            });
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
                    dataParams.SetParamVal<dynamic>(item, dynamicParam.Get<dynamic>(item));
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
                    dataParams.SetParamVal<dynamic>(item, dynamicParam.Get<dynamic>(item));
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
            var paramPacket = BuildWhereSqlWithParams(GetSqlAdapter(connection), type, whereObj);
            builder.Append(paramPacket.SqlWhere);
            return connection.ExecuteScalar<int>(builder.ToString(), paramPacket.Parameters, transaction, commandTimeout);
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
            var sql = new StringBuilder($"SELECT SUM({field}) FROM [{GetTableName(typeof(T))}]");
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sql.Append($" where {sqlWhere};");
            return connection.ExecuteScalar<TResult>(sql.ToString(), AdapterParams(param), transaction, commandTimeout);
        }
        #endregion

        #region Excute
        /// <summary>
        /// 执行sql(非查询)
        /// </summary>
        /// <returns></returns>
        public static int ExecuteSql(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return connection.Execute(sql, AdapterParams(param), transaction, commandTimeout: commandTimeout);
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

        #region 内部帮助 方法
        /// <summary>
        /// 适配参数化：DataParameters、Hashtable
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static object AdapterParams(object obj)
        {
            object objParam = null;
            if (obj is DataParameters dataParameters)
                objParam = dataParameters.ToDynamicParameters();
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
            var dataParms = new DataParameters();
            if (param == null)
                return dataParms;
            if (param is DataParameters)
            {
                //直接用传进来的
                dataParms = param as DataParameters;
            }
            else if (param is IDictionary<string, object>)
            {
                foreach (var item in (IDictionary<string, object>)param)
                {
                    dataParms.Add(item.Key, item.Value);
                }
            }
            else if (param is Hashtable)
            {
                foreach (DictionaryEntry item in (Hashtable)(param))
                {
                    dataParms.Add(item.Key.ToString(), item.Value);
                }
            }
            else
            {
                var props = param.GetType().GetProperties();
                foreach (var itemProp in props)
                {
                    dataParms.Add(itemProp.Name, itemProp.GetValue(param));
                }
            }
            return dataParms;
        }

        /// <summary>
        /// 生成sql条件字符串以及参数化
        /// </summary>
        /// <param name="modelType">对应表模型</param>
        /// <param name="param">查询参数化</param>
        /// <param name="packingParams">是否要调用包装 PackingToDataParams</param>
        /// <returns></returns>
        private static ParametersWithWhereSql BuildWhereSqlWithParams(ISqlAdapter adapter, Type modelType, object param, bool packingParams = true)
        {
            var dataParams = packingParams ? PackingToDataParams(param) : (DataParameters)param;
            var paramPacket = new ParametersWithWhereSql();
            var paramInfos = dataParams.GetParameters();
            if (paramInfos == null || paramInfos.Count <= 0)
                return paramPacket;

            int index = 0;
            var sbWhereSql = new StringBuilder(" where ", 50);
            foreach (var item in paramInfos)
            {
                var paramInfo = item;
                sbWhereSql.Append(BuildPartSqlCondition(paramPacket.Parameters, paramInfo, modelType, adapter.ParametricSymbol, ref index));
            }
            sbWhereSql.Remove(sbWhereSql.Length - 3, 3);//去掉多余的and
            paramPacket.SqlWhere = sbWhereSql.ToString();
            return paramPacket;
        }

        private static string BuildPartSqlCondition(DynamicParameters dynamicParams, DataParameters.ParamInfo paramInfo, Type modelType, string parametricSymbol, ref int index)
        {
            string sqlParam = string.Empty, partStr = string.Empty, field = paramInfo.Name, tablePrefix = paramInfo.TablePrefix.IsNullOrWhiteSpace() ? "" : paramInfo.TablePrefix + ".";
            var sqlCondition = paramInfo.SqlCondition;
            //优先级: 先取参数中设置的ParamInfo,没有设置则取Add泛型添加时字段对应的Property特性,前面没有则取modelType对应字段的Property特性
            var propertyAttr = GetPropertyAttr(paramInfo.ModelType ?? modelType, paramInfo.Name);
            var dbType = paramInfo.DbType != null ? paramInfo.DbType : propertyAttr?.DbType;
            var size = paramInfo.Size != null ? paramInfo.Size : propertyAttr?.Size;
            var precision = paramInfo.Precision != null ? paramInfo.Precision : propertyAttr?.Precision;
            var scale = paramInfo.Scale != null ? paramInfo.Scale : propertyAttr?.Scale;
            //修改放最前面做判断
            if (paramInfo.IsUpdate)
            {
                var actualCond = (SqlBasicCondition)sqlCondition;
                dynamicParams.Add(field, actualCond.Value, dbType, paramInfo.ParameterDirection, size, precision, scale);
                return partStr;  //直接返回
            }
            if (!(sqlCondition is SqlTextCondition))
            {
                sqlParam = $"{_sqlParamPrefix}{++index}";
            }
            if (sqlCondition is SqlBasicCondition)
            {
                var actualCond = (SqlBasicCondition)sqlCondition;
                partStr = $" {tablePrefix}{field} {actualCond.SqlOpt} {parametricSymbol}{sqlParam} and";
                dynamicParams.Add(sqlParam, actualCond.Value, dbType, paramInfo.ParameterDirection, size, precision, scale);
            }
            else if (sqlCondition is SqlInCondition)
            {
                var actualCond = (SqlInCondition)sqlCondition;
                if (!actualCond.Value.IsAssemble())
                    throw new ArgumentException("In参数值必须为数组", field);
                //in的时候如果字段指定了DbType,则拆分成一个个的参数化
                if (dbType != null)
                {
                    var inSbStr = new StringBuilder(100);
                    inSbStr.Append($" {tablePrefix}{field} {actualCond.SqlOpt} (");
                    var enumerator = (actualCond.Value as IEnumerable).GetEnumerator();
                    var i = 0;
                    while (enumerator.MoveNext())
                    {
                        i++;
                        var tp_key = $"{sqlParam}__{i}";
                        inSbStr.Append($"{parametricSymbol}{tp_key},");
                        dynamicParams.Add(tp_key, enumerator.Current, dbType, paramInfo.ParameterDirection, size, precision, scale);
                    }
                    if (i == 0)
                        throw new ArgumentException("In参数对应值必须有数据", field);
                    inSbStr.Remove(inSbStr.Length - 1, 1).Append(" ) and");
                    partStr = inSbStr.ToString();
                }
                else
                {
                    partStr = $" {tablePrefix}{field} {actualCond.SqlOpt} {parametricSymbol}{sqlParam} and";
                    dynamicParams.Add(sqlParam, actualCond.Value, dbType, paramInfo.ParameterDirection, size, precision, scale);
                }
            }
            else if (sqlCondition is SqlLikeCondition)
            {
                var actualCond = (SqlLikeCondition)sqlCondition;
                partStr = $" {tablePrefix}{field} {actualCond.SqlOpt} {parametricSymbol}{sqlParam} and";
                dynamicParams.Add(sqlParam, $"%{actualCond.Value}%", dbType, paramInfo.ParameterDirection, size, precision, scale);
            }
            else if (sqlCondition is SqlBtCondition)
            {
                var actualCond = (SqlBtCondition)sqlCondition;
                partStr = $" {tablePrefix}{field} {actualCond.SqlOpt} {parametricSymbol}{sqlParam}__1 and {parametricSymbol}{sqlParam}__2 and";
                dynamicParams.Add($"{sqlParam}__1", actualCond.Value1, dbType, paramInfo.ParameterDirection, size, precision, scale);
                dynamicParams.Add($"{sqlParam}__2", actualCond.Value2, dbType, paramInfo.ParameterDirection, size, precision, scale);
            }
            else if (sqlCondition is SqlTextCondition)
            {
                var actualCond = (SqlTextCondition)sqlCondition;
                partStr = $" {actualCond.SqlWhere} and";
                if (actualCond.Parameters != null)
                {
                    dynamicParams.AddDynamicParams(AdapterParams(actualCond.Parameters));
                }
            }
            else
            {
                throw new ArgumentException("值有误", "DataParameters.ParamInfo.SqlCondition");
            }
            return partStr;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="type">实体type</param>
        /// <returns></returns>
        private static string GetTableName(Type type)
        {
            var table_type = GetTableType(type);
            if (!_typeTableNameMap.ContainsKey(table_type))
                InitToCacheByType(table_type);
            return _typeTableNameMap[table_type];
        }

        /// <summary>
        /// 获取表主键名
        /// </summary>
        /// <param name="type">实体type</param>
        /// <returns></returns>
        private static string GetPrimaryKey(Type type)
        {
            var table_type = GetTableType(type);
            if (!_typePKeyNameMap.ContainsKey(table_type))
                InitToCacheByType(table_type);
            if (!_typePKeyNameMap.ContainsKey(table_type))
                throw new Exception($"Model为[{type.FullName}]未设置主键");
            return _typePKeyNameMap[table_type];
        }

        /// <summary>
        /// 获取新增/修改/查询的参数名
        /// </summary>
        /// <param name="type">实体type</param>
        /// <param name="get_flag">0:新增 1:修改 2:查询</param>
        /// <returns></returns>
        private static List<string> GetPropertyNames(Type type, byte get_flag = 0)
        {
            var table_type = GetTableType(type);
            var key = table_type.FullName;
            if (get_flag == 0)
                key += $"_Insert";
            else if (get_flag == 1)
                key += $"_Update";
            else if (get_flag == 2)
                key += $"_Select";
            if (!_fieldNameCache.ContainsKey(key))
                InitToCacheByType(table_type);
            return _fieldNameCache[key];
        }

        /// <summary>
        /// 获取指定Type且指定PropertyName的PropertyAttribute
        /// </summary>
        /// <param name="type">实体type</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        private static PropertyAttribute GetPropertyAttr(Type type, string propertyName)
        {
            if (type == null)
                return null;
            var table_type = GetTableType(type);
            if (!_typePropertyMap.ContainsKey(table_type))
                InitToCacheByType(table_type);
            var propertyAttributes = _typePropertyMap[table_type];
            return propertyAttributes.FirstOrDefault(p => p.Name == propertyName);
        }

        /// <summary>
        /// 获取表模型Type(贴有Table特性则直接取出,没有则取其父Type,以此类推,直到取到则返回)
        /// </summary>
        /// <param name="type">实体type</param>
        /// <returns></returns>
        private static Type GetTableType(Type type)
        {
            if (!_typeTableTypeMap.ContainsKey(type))
            {
                var tableAttr = type.GetCustomAttribute<TableAttribute>();
                if (tableAttr == null)
                    throw new NullReferenceException($"{type.FullName}未标注TableAttribute");
                var table_type = _GetTableType(type);
                _typeTableTypeMap[type] = table_type;
            }
            return _typeTableTypeMap[type];

            Type _GetTableType(Type current_type)
            {
                //false:不获取继承的特性,只取自身贴的特性
                var tableAttr = current_type.GetCustomAttribute<TableAttribute>(false);
                if (tableAttr == null && current_type.BaseType != null)
                    return _GetTableType(current_type.BaseType);
                return current_type;
            }
        }

        /// <summary>
        /// 初始化数据进行缓存
        /// </summary>
        /// <param name="t_model_type">表模型type</param>
        /// <returns></returns>
        private static void InitToCacheByType(Type t_model_type)
        {
            //false: t_model_type为表模型Type,只取自身就行
            var tableAttr = t_model_type.GetCustomAttribute<TableAttribute>(false);
            _typeTableNameMap[t_model_type] = tableAttr.TableName;
            var propertyAttributes = new List<PropertyAttribute>();
            var fieldSelectList = new List<string>();
            var fieldInsertList = new List<string>();
            var fieldUpdateList = new List<string>();
            foreach (var prop in t_model_type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.GetGetMethod(false) != null))
            {
                var propAttr = prop.GetCustomAttribute<PropertyAttribute>();
                if (propAttr != null)
                {
                    propAttr.Name = prop.Name;
                    propertyAttributes.Add(propAttr);
                }

                if (propAttr == null)
                {
                    fieldSelectList.Add(prop.Name);
                    fieldInsertList.Add(prop.Name);
                    fieldUpdateList.Add(prop.Name);
                }
                else
                {
                    /*/枚举位运算(比如: propAttr.Ignore=DbIgnore.Update | DbIgnore.Insert,(propAttr.Ignore & DbIgnore.Insert)返回为DbIgnore.Insert)
                    * 也就是在 propAttr.Ignore 中找 是不是 有 DbIgnore.Insert
                    */
                    if ((propAttr.Ignore & DbIgnore.All) != DbIgnore.All)
                    {
                        if (!propAttr.IsInc && (propAttr.Ignore & DbIgnore.Insert) != DbIgnore.Insert)
                        {
                            fieldInsertList.Add(prop.Name);
                        }
                        if (!propAttr.IsInc && (propAttr.Ignore & DbIgnore.Update) != DbIgnore.Update)
                        {
                            fieldUpdateList.Add(prop.Name);
                        }
                        if ((propAttr.Ignore & DbIgnore.Select) != DbIgnore.Select)
                        {
                            fieldSelectList.Add(prop.Name);
                        }
                    }
                }

                if (propAttr != null && propAttr.IsPKey)
                {
                    _typePKeyNameMap[t_model_type] = prop.Name;
                }
            }
            _typePropertyMap[t_model_type] = propertyAttributes;
            _fieldNameCache[$"{t_model_type.FullName}_Select"] = fieldSelectList;
            _fieldNameCache[$"{t_model_type.FullName}_Insert"] = fieldInsertList;
            _fieldNameCache[$"{t_model_type.FullName}_Update"] = fieldUpdateList;
        }

        /// <summary>
        /// 返回对应数据库的适配器
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static ISqlAdapter GetSqlAdapter(IDbConnection connection)
        {
            var name = ToActualConnetction(connection).GetType().Name.ToLower();
            return _adapterDictionary.TryGetValue(name, out var adapter) ? adapter : _defaultAdapter;
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
            if (tran is ProfiledDbTransaction transaction)
                return transaction.WrappedTransaction;
            return tran;
        }
        #endregion
    }
}
