using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using ZeKi.Frame.DB;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.Model;
using ZeKi.Frame.Common;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace ZeKi.Frame.DAL
{
    /// <summary>
    /// 数据访问父类
    /// </summary>
    public class BaseDAL : IBaseDAL
    {
        private readonly DbContext _dbContext;

        #region 构造函数

        public BaseDAL(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="getIncVal">是否获取当前插入的自增值,不是自增则不需要关注此值</param>
        /// <returns>getIncVal为true并且有自增列则返回插入自增值,否则为影响行数</returns>
        public virtual int Insert<TModel>(TModel model, bool getIncVal = false) where TModel : class, new()
        {
            return _dbContext.Insert(model, getIncVal);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list"></param>
        /// <param name="ps">每批次数量,默认500</param>
        /// <returns>返回总影响行数</returns>
        public virtual int BatchInsert<TModel>(IEnumerable<TModel> list, int ps = 500) where TModel : class, new()
        {
            return _dbContext.BatchInsert(list, ps);
        }

        /// <summary>
        /// bulkcopy,仅支持mssql数据库
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entitysToInsert"></param>
        /// <param name="timeOut">超时时间,单位：秒</param>
        public virtual void BulkCopyToInsert<TModel>(IEnumerable<TModel> entitysToInsert, SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default, int timeOut = 60 * 10) where TModel : class, new()
        {
            _dbContext.BulkCopyToInsert(entitysToInsert, copyOptions, timeOut);
        }
        #endregion

        #region Update
        /// <summary>
        /// 修改(单个,根据主键)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Update<TModel>(TModel model) where TModel : class, new()
        {
            return _dbContext.Update(model);
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
        public virtual int UpdatePart<TModel>(object setAndWhere) where TModel : class, new()
        {
            return _dbContext.UpdatePart<TModel>(setAndWhere);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">如需统一清除缓存,可以传递删除缓存相关字段</param>
        /// <returns></returns>
        public virtual bool Delete<TModel>(TModel model) where TModel : class, new()
        {
            return _dbContext.Delete(model);
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
        public virtual IEnumerable<TResult> QueryList<TResult>(string sql, object param = null)
        {
            return _dbContext.QueryList<TResult>(sql, param);
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
        public virtual IEnumerable<TResult> QueryList<TTable, TResult>(object whereObj = null, string orderStr = null, string selectFields = null)
        {
            return _dbContext.QueryList<TTable, TResult>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询[适用于连表查询]
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        public virtual IEnumerable<TResult> QueryJoinList<TResult>(string sqlNoWhere, object whereObj, string orderStr)
        {
            return _dbContext.QueryJoinList<TResult>(sqlNoWhere, whereObj, orderStr);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryList<T>(object whereObj = null, string orderStr = null, string selectFields = null)
        {
            return _dbContext.QueryList<T, T>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="sql">全sql</param>
        /// <param name="param"></param>
        /// <returns>返回单个</returns>
        public virtual TResult QueryModel<TResult>(string sql, object param = null)
        {
            return _dbContext.QueryModel<TResult>(sql, param);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TTable">表对应模型</typeparam>
        /// <typeparam name="TResult">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        public virtual TResult QueryModel<TTable, TResult>(object whereObj, string orderStr = null, string selectFields = null)
        {
            return _dbContext.QueryModel<TTable, TResult>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        public virtual T QueryModel<T>(object whereObj, string orderStr = null, string selectFields = null)
        {
            return _dbContext.QueryModel<T, T>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询[适用于连表查询]
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        public virtual TResult QueryJoinModel<TResult>(string sqlNoWhere, object whereObj, string orderStr)
        {
            return _dbContext.QueryJoinModel<TResult>(sqlNoWhere, whereObj, orderStr);
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
        public virtual IEnumerable<TReturn> QueryList<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null)
        {
            return _dbContext.QueryList(sql, map, param);
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
        public virtual IEnumerable<TReturn> QueryList<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null)
        {
            return _dbContext.QueryList(sql, map, param);
        }

        /// <summary>
        /// 分页查询返回集合
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="pcp">分页模型(参数传递查看PageParameters字段注解)</param>
        /// <returns></returns>
        public virtual PageData<TResult> PageList<TResult>(PageParameters pcp)
        {
            return _dbContext.PageList<TResult>(pcp);
        }
        #endregion

        #region Procedure
        /// <summary>
        /// 执行查询存储过程(用于查询数据)
        /// <para>参数传递参考：https://github.com/StackExchange/Dapper#stored-procedures </para>
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="proceName">存储过程名</param>
        /// <param name="param">特定键值字典/Hashtable/匿名类/自定义类,过程中有OutPut或者Return参数,使用<see cref="DataParameters"/></param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryProcedure<T>(string proceName, object param = null)
        {
            if (typeof(T) == typeof(Hashtable) || typeof(T) == typeof(Dictionary<string, object>))
            {
                var resultList = _dbContext.QueryProcedure<dynamic>(proceName, param);
                return JsonConvert.DeserializeObject<IEnumerable<T>>(JsonConvert.SerializeObject(resultList));
            }
            return _dbContext.QueryProcedure<T>(proceName, param);
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
        public virtual void ExecProcedure(string proceName, object param = null)
        {
            _dbContext.ExecProcedure(proceName, param);
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
        public virtual int Count<TModel>(string sqlWhere, object param = null) where TModel : class, new()
        {
            return _dbContext.Count<TModel>(sqlWhere, param);
        }

        /// <summary>
        /// Count统计
        /// </summary>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <returns></returns>
        public virtual int Count<TModel>(object whereObj = null) where TModel : class, new()
        {
            return _dbContext.Count<TModel>(whereObj);
        }

        /// <summary>
        /// Sum统计
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="field">需要sum的字段</param>
        /// <param name="sqlWhere">1=1,省略where</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual TResult Sum<TModel, TResult>(string field, string sqlWhere, object param = null) where TModel : class, new()
        {
            return _dbContext.Sum<TModel, TResult>(field, sqlWhere, param);
        }
        #endregion

        #region Excute
        /// <summary>
        /// 执行sql(非查询)
        /// </summary>
        /// <returns></returns>
        public virtual int Execute(string sql, object param = null)
        {
            return _dbContext.Execute(sql, param);
        }
        #endregion

        #region Dapper原生
        ///// <summary>
        ///// 执行数据库操作,会自动释放,操作QueryMultiple可以用这个
        ///// </summary>
        ///// <param name="action"></param>
        //public virtual void DbAction(Action<IDbConnection> action)
        //{
        //    action(DBConn);
        //}
        #endregion

        #region Transaction
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="isolation"></param>
        public virtual void ExecTransaction(Action action, IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            try
            {
                _dbContext.BeginTransaction(isolation);
                action();
                _dbContext.CommitTransaction();
            }
            catch (Exception)
            {
                _dbContext.RollbackTransaction();
                throw;
            }
        }

        /// <summary>
        /// 批量数据事务提交
        /// </summary>
        /// <param name="strSqls">T-SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public virtual int ExecTransaction(string strSqls, object param = null)
        {
            return _dbContext.ExecTransaction(strSqls, param);
        }

        #endregion

        #region 内部帮助/受保护类型 方法

        #endregion
    }
}
