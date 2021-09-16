using Dapper;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using ZeKi.Frame.Common;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.DB
{
    /// <summary>
    /// 数据库上下文,同一请求共用一个实例
    /// </summary>
    public class DbContext : IDisposable
    {
        private static readonly string connStr = AppSettings.GetValue("ConnectionString");
        private static readonly DBEnums.DBType dbType = (DBEnums.DBType)AppSettings.GetValue<int>("DBType");
        private static readonly int commandTimeout = AppSettings.GetValue<int>("CommandTimeout");   //单位：秒

        private IDbConnection conn = null;
        private IDbTransaction tran = null;

        private IDbConnection Connection
        {
            get
            {
                //DbContext对象同一请求共用一个实例,所以这里不会出现并发实例化多个数据库连接
                if (conn == null)
                {
                    conn = GetConnection();
                }
                return conn;
            }
        }

        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="getIncVal">是否获取当前插入的自增值,不是自增则不需要关注此值</param>
        /// <returns>getIncVal为true并且有自增列则返回插入自增值,否则为影响行数</returns>
        public int Insert<TModel>(TModel model, bool getIncVal = false) where TModel : class
        {
            return Connection.Insert(model, getIncVal, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list"></param>
        /// <param name="ps">每批次数量,默认500</param>
        /// <returns>返回总影响行数</returns>
        public int BatchInsert<TModel>(IEnumerable<TModel> list, int ps = 500) where TModel : class
        {
            return Connection.BatchInsert(list, ps, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// bulkcopy,仅支持mssql数据库
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entitysToInsert"></param>
        /// <param name="timeOut">超时时间,单位：秒</param>
        public void BulkCopyToInsert<TModel>(IEnumerable<TModel> entitysToInsert, SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default, int timeOut = 60 * 10) where TModel : class, new()
        {
            Connection.BulkCopyToInsert(entitysToInsert, copyOptions, tran, timeOut);
        }
        #endregion

        #region Update
        /// <summary>
        /// 修改(单个,根据主键)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update<TModel>(TModel model) where TModel : class
        {
            return Connection.Update(model, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 修改(根据自定义条件修改自定义值,where条件字段会沿用标注的属性特性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="setAndWhere">使用指定数据类型类<see cref="DataParameters"/>.AddUpdate方法
        /// <para>set字段能否被修改受 <see cref="ColumnAttribute"/> 特性影响</para>
        /// </param>
        /// <returns></returns>
        public int UpdatePart<TModel>(object setAndWhere) where TModel : class
        {
            return Connection.UpdatePart<TModel>(setAndWhere, tran, commandTimeout: commandTimeout);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">如需统一清除缓存,可以传递删除缓存相关字段</param>
        /// <returns></returns>
        public bool Delete<TModel>(TModel model) where TModel : class
        {
            return Connection.Delete(model, tran, commandTimeout: commandTimeout);
        }
        #endregion

        #region Query
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="sql">全sql</param>
        /// <param name="param"></param>
        /// <returns>返回集合</returns>
        public IEnumerable<TResult> QueryList<TResult>(string sql, object param = null)
        {
            return Connection.QueryList<TResult>(sql, param, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TTable">表对应模型</typeparam>
        /// <typeparam name="TResult">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public IEnumerable<TResult> QueryList<TTable, TResult>(object whereObj = null, string orderStr = null, string selectFields = null)
        {
            return Connection.QueryList<TTable, TResult>(whereObj, orderStr, selectFields, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询[适用于连表查询]
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        public IEnumerable<TResult> QueryJoinList<TResult>(string sqlNoWhere, object whereObj, string orderStr)
        {
            return Connection.QueryJoinList<TResult>(sqlNoWhere, whereObj, orderStr, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="sql">全sql</param>
        /// <param name="param"></param>
        /// <returns>返回单个</returns>
        public TResult QueryModel<TResult>(string sql, object param = null)
        {
            return Connection.QueryModel<TResult>(sql, param, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        public TResult QueryModel<TTable, TResult>(object whereObj, string orderStr = null, string selectFields = null)
        {
            return Connection.QueryModel<TTable, TResult>(whereObj, orderStr, selectFields, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询[适用于连表查询]
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        public TResult QueryJoinModel<TResult>(string sqlNoWhere, object whereObj, string orderStr)
        {
            return Connection.QueryJoinModel<TResult>(sqlNoWhere, whereObj, orderStr, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(可以对多个返回结果进行操作,最终返回一个)
        /// 使用参考: https://github.com/StackExchange/Dapper#multi-mapping
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="sql">需要输入全sql</param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<TReturn> QueryList<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null)
        {
            return Connection.Query(sql, map, param, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 查询(可以对多个返回结果进行操作,最终返回一个)
        /// 使用参考: https://github.com/StackExchange/Dapper#multi-mapping
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="sql">需要输入全sql</param>
        /// <param name="map"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<TReturn> QueryList<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null)
        {
            return Connection.Query(sql, map, param, tran, commandTimeout: commandTimeout);
        }

        ///// <summary>
        ///// 查询返回多个结果集 (using之后在外面无法获取数据,可以在外部使用DbAction方法来完成)
        ///// </summary>
        ///// <param name="sql">需要输入全sql</param>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public GridReader QueryMultiple(string sql, object param = null)
        //{
        //    using (var _conn = GetConnection())
        //    {
        //        return DBConnection.QueryMultiple(sql, param, commandTimeout: _commandTimeout);
        //    }
        //}

        /// <summary>
        /// 分页查询返回集合
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="pcp">分页模型(参数传递查看PageParameters字段注解)</param>
        /// <returns></returns>
        public PageData<TResult> PageList<TResult>(PageParameters pcp)
        {
            return Connection.PageList<TResult>(pcp, tran, commandTimeout: commandTimeout);
        }
        #endregion

        #region Procedure
        /// <summary>
        /// 执行查询存储过程(用于查询数据)
        /// <para>参数传递参考：https://github.com/StackExchange/Dapper#stored-procedures </para>
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="proceName">存储过程名</param>
        /// <param name="param">特定键值字典/Hashtable/匿名类/自定义类,过程中有OutPut或者Return参数,使用<see cref="DbParameters"/></param>
        /// <returns>返回集合</returns>
        public IEnumerable<T> QueryProcedure<T>(string proceName, object param = null)
        {
            return Connection.QueryProcedure<T>(proceName, param, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// 执行存储过程(查询数据使用QueryProcedure方法)
        /// <para>参数传递参考：https://github.com/StackExchange/Dapper#stored-procedures </para>
        /// </summary>
        /// <param name="proceName">存储过程名</param>
        /// <param name="param">特定键值字典/Hashtable/匿名类/自定义类,过程中有OutPut或者Return参数,使用<see cref="DbParameters"/></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public void ExecProcedure(string proceName, object param = null)
        {
            Connection.ExecProcedure(proceName, param, tran, commandTimeout: commandTimeout);
        }
        #endregion

        #region Statistics
        /// <summary>
        /// Count统计
        /// </summary>
        /// <param name="sqlWhere">1=1,省略where</param>
        /// <param name="param"></param>
        /// <typeparam name="T">模型对象(获取表名)</typeparam>
        /// <returns></returns>
        public int Count<TModel>(string sqlWhere, object param = null) where TModel : class
        {
            return Connection.Count<TModel>(sqlWhere, param, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// Count统计
        /// </summary>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <returns></returns>
        public int Count<TModel>(object whereObj = null) where TModel : class
        {
            return Connection.Count<TModel>(whereObj, tran, commandTimeout: commandTimeout);
        }

        /// <summary>
        /// Sum统计
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="field">需要sum的字段</param>
        /// <param name="sqlWhere">1=1,省略where</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public TResult Sum<TModel, TResult>(string field, string sqlWhere, object param = null) where TModel : class
        {
            return Connection.Sum<TModel, TResult>(field, sqlWhere, param, tran, commandTimeout: commandTimeout);
        }
        #endregion

        #region Excute
        /// <summary>
        /// 执行sql(非查询)
        /// </summary>
        /// <returns></returns>
        public int Execute(string sql, object param = null)
        {
            return Connection.ExecuteSql(sql, param, tran, commandTimeout: commandTimeout);
        }
        #endregion

        #region Dapper原生
        ///// <summary>
        ///// 执行数据库操作,会自动释放,操作QueryMultiple可以用这个
        ///// </summary>
        ///// <param name="action"></param>
        //public void DbAction(Action<IDbConnection> action)
        //{
        //    action(conn);
        //}
        #endregion

        #region Transaction
        /// <summary>
        /// 批量数据事务提交
        /// </summary>
        /// <param name="strSqls">T-SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public int ExecTransaction(string strSqls, object param = null)
        {
            return Connection.ExecTransaction(strSqls, param);
        }

        public void BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            if (tran != null)
                throw new InvalidOperationException("当前存在已经运行的事务,创建事务失败");
            tran = (Connection as DbConnection).BeginTransaction(isolation);
        }

        public void CommitTransaction()
        {
            if (tran != null)
            {
                tran.Commit();
                tran = null;
            }
        }

        public void RollbackTransaction()
        {
            if (tran != null)
            {
                if (tran is ProfiledDbTransaction transaction)
                {
                    if (transaction.WrappedTransaction.Connection != null)
                        tran.Rollback();
                }
                else if (tran.Connection != null)
                {
                    tran.Rollback();
                }
                tran = null;
            }
        }

        public bool IsRunningTran
        {
            get
            {
                return tran != null;
            }
        }
        #endregion

        #region 内部帮助/受保护类型 方法
        /// <summary>
        /// 获取连接对象
        /// </summary>
        /// <returns></returns>
        private IDbConnection GetConnection()
        {
            IDbConnection _conn = null;
            switch (dbType)
            {
                case DBEnums.DBType.MSSQL:
                    _conn = new SqlConnection(connStr);
                    break;
                case DBEnums.DBType.MYSQL:

                    break;
            }
            if (_conn == null)
                throw new NotImplementedException($"未实现该数据库");
            //core在此类的构造函数中获取不到MiniProfiler.Current其值
            //因为还没执行ProfilingFilterAttribute.OnActionExecutionAsync方法的StartNew,所以为null
            if (MiniProfiler.Current != null) //MiniProfiler初始化
                _conn = new StackExchange.Profiling.Data.ProfiledDbConnection((DbConnection)_conn, MiniProfiler.Current);
            if (_conn.State == ConnectionState.Closed)
                _conn.Open();
            return _conn;
        }

        #endregion

        #region 释放链接(请求完后容器会调用)
        public void Dispose()
        {
            if (conn != null)
                conn.Dispose();
            if (tran != null)
                tran.Dispose();
        }
        #endregion
    }
}
