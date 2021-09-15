using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ZeKi.Frame.Model;
using ZeKi.Frame.Common;
using System.Data.SqlClient;

namespace ZeKi.Frame.IDAL
{
    /// <summary>
    /// 数据访问父类接口
    /// </summary>
    public interface IBaseDAL
    {
        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="getIncVal">是否获取当前插入的自增值,不是自增则不需要关注此值</param>
        /// <returns>getIncVal为true并且有自增列则返回插入自增值,否则为影响行数</returns>
        int Insert<TModel>(TModel model, bool getIncVal = false) where TModel : class, new();

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list"></param>
        /// <param name="ps">每批次数量,默认500</param>
        /// <returns>返回总影响行数</returns>
        int BatchInsert<TModel>(IEnumerable<TModel> list, int ps = 500) where TModel : class, new();

        /// <summary>
        /// bulkcopy,仅支持mssql数据库
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entitysToInsert"></param>
        /// <param name="timeOut">超时时间,单位：秒</param>
        void BulkCopyToInsert<TModel>(IEnumerable<TModel> entitysToInsert, SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default, int timeOut = 60 * 10) where TModel : class, new();
        #endregion

        #region Update
        /// <summary>
        /// 修改(单个,根据主键)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Update<TModel>(TModel model) where TModel : class, new();

        /// <summary>
        /// 修改(根据自定义条件修改自定义值,where条件字段会沿用标注的属性特性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="setAndWhere">使用指定数据类型类<see cref="DataParameters"/>.AddUpdate方法
        /// <para>set字段能否被修改受 <see cref="PropertyAttribute"/> 特性影响</para>
        /// </param>
        /// <returns></returns>
        int UpdatePart<TModel>(object setAndWhere) where TModel : class, new();
        #endregion

        #region Delete
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">如需统一清除缓存,可以传递删除缓存相关字段</param>
        /// <returns></returns>
        bool Delete<TModel>(TModel model) where TModel : class, new();
        #endregion

        #region Query
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="sql">全sql</param>
        /// <param name="param"></param>
        /// <returns>返回集合</returns>
        IEnumerable<TResult> QueryList<TResult>(string sql, object param = null);

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TTable">表对应模型</typeparam>
        /// <typeparam name="TResult">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        IEnumerable<TResult> QueryList<TTable, TResult>(object whereObj = null, string orderStr = null, string selectFields = null);

        /// <summary>
        /// 查询[适用于连表查询]
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        IEnumerable<TResult> QueryJoinList<TResult>(string sqlNoWhere, object whereObj, string orderStr);

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        IEnumerable<T> QueryList<T>(object whereObj = null, string orderStr = null, string selectFields = null);

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TTable">表对应模型</typeparam>
        /// <typeparam name="TResult">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        TResult QueryModel<TTable, TResult>(object whereObj, string orderStr = null, string selectFields = null);

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="sql">全sql</param>
        /// <param name="param"></param>
        /// <returns>返回单个</returns>
        TResult QueryModel<TResult>(string sql, object param = null);

        /// <summary>
        /// 查询[适用于连表查询]
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="sqlNoWhere">sql语句,不包括where,如:select * from sysUserInfo as u left join sysDepartment as d on u.uDepId=d.depId</param>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        TResult QueryJoinModel<TResult>(string sqlNoWhere, object whereObj, string orderStr);

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        T QueryModel<T>(object whereObj, string orderStr = null, string selectFields = null);

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
        IEnumerable<TReturn> QueryList<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null);

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
        IEnumerable<TReturn> QueryList<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null);

        /// <summary>
        /// 分页查询返回集合
        /// </summary>
        /// <typeparam name="TResult">返回类型,可以不是数据库对应模型</typeparam>
        /// <param name="pcp">分页模型(参数传递查看PageParameters字段注解)</param>
        /// <returns></returns>
        PageData<TResult> PageList<TResult>(PageParameters pcp);
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
        IEnumerable<T> QueryProcedure<T>(string proceName, object param = null);

        /// <summary>
        /// 执行存储过程(查询数据使用QueryProcedure方法)
        /// <para>参数传递参考：https://github.com/StackExchange/Dapper#stored-procedures </para>
        /// </summary>
        /// <param name="proceName">存储过程名</param>
        /// <param name="param">特定键值字典/Hashtable/匿名类/自定义类,过程中有OutPut或者Return参数,使用<see cref="DbParameters"/></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        void ExecProcedure(string proceName, object param = null);
        #endregion

        #region Statistics
        /// <summary>
        /// Count统计
        /// </summary>
        /// <param name="sqlWhere">1=1,省略where</param>
        /// <param name="param"></param>
        /// <typeparam name="T">模型对象(获取表名)</typeparam>
        /// <returns></returns>
        int Count<TModel>(string sqlWhere, object param = null) where TModel : class, new();

        /// <summary>
        /// Count统计
        /// </summary>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <returns></returns>
        int Count<TModel>(object whereObj = null) where TModel : class, new();

        /// <summary>
        /// Sum统计
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="field">需要sum的字段</param>
        /// <param name="sqlWhere">1=1,省略where</param>
        /// <param name="param"></param>
        /// <returns></returns>
        TResult Sum<TModel, TResult>(string field, string sqlWhere, object param = null) where TModel : class, new();
        #endregion

        #region Excute
        /// <summary>
        /// 执行sql(非查询)
        /// </summary>
        /// <returns></returns>
        int Execute(string sql, object param = null);
        #endregion

        #region Dapper原生
        ///// <summary>
        ///// 执行数据库操作,会自动释放,操作QueryMultiple可以用这个
        ///// </summary>
        ///// <param name="action"></param>
        //void DbAction(Action<IDbConnection> action);
        #endregion

        #region Transaction
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="isolation"></param>
        void ExecTransaction(Action action, IsolationLevel isolation = IsolationLevel.ReadCommitted);

        /// <summary>
        /// 批量数据事务提交
        /// </summary>
        /// <param name="strSqls">T-SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        int ExecTransaction(string strSqls, object param = null);
        #endregion
    }
}
