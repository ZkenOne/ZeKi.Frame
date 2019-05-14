using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dapper.Extensions.ZQ
{
    /// <summary>
    /// 基于Dapper扩展到IDbConnection对象方法
    /// </summary>
    public static class SqlMapperExtensions
    {
        /// <summary>
        /// 新增/修改字段缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<string, List<string>> paramNameCache = new ConcurrentDictionary<string, List<string>>();
        /// <summary>
        /// 表名缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<Type, string> tableNameMap = new ConcurrentDictionary<Type, string>();
        /// <summary>
        /// 主键缓存变量
        /// </summary>
        private static readonly ConcurrentDictionary<Type, string> pKeyNameMap = new ConcurrentDictionary<Type, string>();

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
        public static long Insert<T>(this IDbConnection connection, T entityToInsert, bool getIncId = false, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            List<string> propertyNames = GetPropertyNames(type);
            string cols = string.Join(",", propertyNames);
            string colsParams = string.Join(",", propertyNames.Select(p => "@" + p));
            var sql = "insert " + GetTableName(type) + " (" + cols + ") values (" + colsParams + ");";
            if (getIncId)
            {
                sql += GetInsertIncSql(connection);
                return Convert.ToInt64(connection.ExecuteScalar(sql, entityToInsert, transaction, commandTimeout));
            }
            else
            {
                return connection.Execute(sql, entityToInsert, transaction, commandTimeout);
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// 修改(单个,根据主键)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static bool Update<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            List<string> paramNames = GetPropertyNames(type, 1);
            var builder = new StringBuilder();
            builder.Append("update ").Append(GetTableName(type)).Append(" set ");
            builder.AppendLine(string.Join(",", paramNames.Select(p => p + "= @" + p)));
            var pk = GetPrimaryKey(type);
            builder.Append($" where {pk} = @{pk}");
            return connection.Execute(builder.ToString(), entityToUpdate, transaction, commandTimeout) > 0;
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
        /// 查询
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
            return connection.Query<T>(sql, param, transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询
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
            return connection.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 分页查询返回集合
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="pcp">分页模型(参数传递查看PageParameters字段注解)</param>
        /// <param name="param">同dapper参数传值</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static PageData<T> PageList<T>(this IDbConnection connection, PageParameters pcp, object param = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            var sql = CreatePageSql(connection, typeof(T), pcp);
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

        /// <summary>
        /// 分页查询返回数据集(需要在pcp定义表名)
        /// </summary>
        /// <param name="pcp">分页必填参数</param>
        /// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        /// <returns></returns>
        public static DataSet PageDataSet(this IDbConnection connection, PageParameters pcp, object param = null)
        {
            var sql = CreatePageSql(connection, null, pcp);
            return ExecDataSet(connection, sql, param);
        }

        /// <summary>
        /// 分页查询返回数据表(需要在pcp定义表名)
        /// </summary>
        /// <param name="pcp">分页必填参数</param>
        /// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        /// <returns></returns>
        public static DataTable PageDataTable(this IDbConnection connection, PageParameters pcp, object param = null)
        {
            var sql = CreatePageSql(connection, null, pcp);
            return ExecDataTable(connection, sql, param);
        }

        /// <summary>
        /// 创建 分页SQL语句
        /// </summary>
        /// <param name="pcp"></param>
        /// <returns></returns>
        private static string CreatePageSql(IDbConnection connection, Type type, PageParameters pcp)
        {
            if (connection is SqlConnection)
            {
                if (type == null && string.IsNullOrEmpty(pcp.TableName))
                    throw new NotImplementedException("pcp中需要传递TableName");
                return CreatePageByMSSQL(type, pcp);
            }
            throw new NotImplementedException("未实现该数据库操作");
        }

        /// <summary>
        /// 生成mssql分页sql
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pcp"></param>
        /// <returns></returns>
        private static string CreatePageByMSSQL(Type type, PageParameters pcp)
        {
            //开始
            int start = (pcp.PageIndex - 1) * pcp.PageSize + 1;
            //结束
            int end = pcp.PageIndex * pcp.PageSize;
            //select字段
            pcp.ShowField = string.IsNullOrWhiteSpace(pcp.ShowField) ? "*" : pcp.ShowField;

            if (!string.IsNullOrWhiteSpace(pcp.Sql))
            {
                return $"SELECT {pcp.ShowField} FROM ( SELECT ROW_NUMBER() OVER(ORDER BY {pcp.KeyFiled}) AS ROW_NUMBER, " +
                    $" {pcp.Sql}  {pcp.Where} ) AS Tab WHERE ROW_NUMBER BETWEEN {start} AND {end}; " +
                    $" SELECT COUNT(0) AS DataCount FROM (SELECT {pcp.Sql} {pcp.Where}) AS CountTb;";
            }

            //此值为空则使用BaseDAL传入的TModel映射的表名
            if (string.IsNullOrWhiteSpace(pcp.TableName))
                pcp.TableName = GetTableName(type);
            if (string.IsNullOrWhiteSpace(pcp.OrderBy))
                pcp.OrderBy = pcp.KeyFiled;

            return $"SELECT {pcp.ShowField} FROM (SELECT ROW_NUMBER() OVER(ORDER BY {pcp.KeyFiled} " +
                $" ) AS ROW_NUMBER,{pcp.ShowField} FROM {pcp.TableName} {pcp.Where} " +
                $" ) Tab WHERE ROW_NUMBER BETWEEN {start} AND {end} ORDER BY {pcp.OrderBy};" +
                $" SELECT COUNT(0) AS DataCount FROM {pcp.TableName} {pcp.Where};";
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
            var sql = new StringBuilder($"SELECT COUNT(0) FROM [{GetTableName(type)}]");
            if (!string.IsNullOrWhiteSpace(sqlWhere))
                sql.Append($" where {sqlWhere};");
            return Convert.ToInt32(connection.ExecuteScalar(sql.ToString(), param, transaction, commandTimeout));
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
            return (TResult)connection.ExecuteScalar(sql.ToString(), param, transaction, commandTimeout);
        }
        #endregion

        #region Transaction
        /// <summary>
        /// 执行事务(每次执行数据库操作需要将tran对象传入)
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="action">参数:连接对象</param>
        /// <param name="isolation">事务级别</param>
        public static void ExecTransaction(this IDbConnection connection, Action<IDbConnection, IDbTransaction> action, IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            var _transaction = (connection as DbConnection).BeginTransaction(isolation);
            try
            {
                action(connection, _transaction);
                if (_transaction != null)
                {
                    _transaction.Commit();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction = null;
                }
                throw ex;
            }
        }

        /// <summary>
        /// 执行事务(每次执行数据库操作需要将tran对象传入)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">参数:连接对象</param>
        /// <param name="isolation">事务级别</param>
        /// <returns></returns>
        public static T ExecTransaction<T>(this IDbConnection connection, Func<IDbConnection, IDbTransaction, T> func, IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            var _transaction = (connection as DbConnection).BeginTransaction(isolation);
            try
            {
                T result = func(connection, _transaction);
                if (_transaction != null)
                {
                    _transaction.Commit();
                    _transaction = null;
                }
                return result;
            }
            catch (Exception ex)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction = null;
                }
                throw ex;
            }
        }

        /// <summary>
        /// 批量数据事务提交(与dapper无关)
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
        /// <summary>
        /// 获取DataTable(与dapper无关)
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        /// <param name="commandType">CommandType为StoredProcedure时,直接写存储过程名</param>
        /// <returns></returns>
        public static DataTable ExecDataTable(this IDbConnection connection, string commandText, object param = null, CommandType commandType = CommandType.Text)
        {
            var dt = new DataTable();
            using (var conn = connection)
            {
                if (connection is SqlConnection)
                {
                    var sqlPar = param as List<SqlParameter>;
                    SqlCommand cmd = new SqlCommand(commandText, conn as SqlConnection)
                    {
                        CommandType = commandType
                    };
                    if (sqlPar != null && sqlPar.Count > 0)
                    {
                        cmd.Parameters.AddRange(sqlPar.ToArray());
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    cmd.Parameters.Clear();
                    return dt;
                }
                throw new NotImplementedException("未实现该数据库操作");
            }
        }
        #endregion

        #region DataSet
        /// <summary>
        /// 获取DataSet(与dapper无关)
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        /// <param name="commandType">CommandType为StoredProcedure时,直接写存储过程名</param>
        /// <returns></returns>
        public static DataSet ExecDataSet(this IDbConnection connection, string commandText, object param = null, CommandType commandType = CommandType.Text)
        {
            var ds = new DataSet();
            using (var conn = connection)
            {
                if (connection is SqlConnection)
                {
                    var sqlPar = param as List<SqlParameter>;
                    SqlCommand cmd = new SqlCommand(commandText, conn as SqlConnection)
                    {
                        CommandType = commandType
                    };
                    if (sqlPar != null && sqlPar.Count > 0)
                    {
                        cmd.Parameters.AddRange(sqlPar.ToArray());
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    cmd.Parameters.Clear();
                    return ds;
                }
                throw new NotImplementedException("未实现该数据库操作");
            }
        }
        #endregion

        #region 内部帮助/受保护类型 方法
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
            if (!paramNameCache.TryGetValue(key, out paramNames))
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
                        pKeyNameMap[t_model_type] = prop.Name;
                    }
                }
                paramNameCache[key] = paramNames;
            }
            return paramNames;
        }

        /// <summary>
        /// 生成sql前缀(select * from table)
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="sql"></param>
        /// <param name="falg">0:所有 1:单个模型</param>
        /// <returns></returns>
        private static string BuildSqlStart(Type type, string sql, int falg = 0)
        {
            //开头为where 则自动补全
            if (sql.Trim(' ').StartsWith("where".ToLower()))
            {
                var topStr = falg == 0 ? "" : " top 1 ";
                return $"select {topStr} * from {GetTableName(type)} {sql}";
            }
            return sql;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetTableName(Type type)
        {
            var tableName = string.Empty;
            if (!tableNameMap.TryGetValue(type, out tableName))
            {
                var att = type.GetCustomAttribute<TableAttribute>();
                if (att == null)
                    tableName = type.Name;
                else
                    tableName = att.TableName;
                tableNameMap[type] = tableName;
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
            if (!pKeyNameMap.ContainsKey(type))
                //触发缓存
                GetPropertyNames(type);
            if (!pKeyNameMap.ContainsKey(type))
                throw new Exception($"Model为[{type.Name}]未设置主键");
            return pKeyNameMap[type];
        }

        /// <summary>
        /// 获取新增之后,不同数据库获取新增主键的sql语句
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetInsertIncSql(IDbConnection connection)
        {
            if (connection is SqlConnection)
            {
                return "SELECT SCOPE_IDENTITY();";
            }
            throw new NotImplementedException("未实现该数据库操作");
        }

        #endregion
    }
}
