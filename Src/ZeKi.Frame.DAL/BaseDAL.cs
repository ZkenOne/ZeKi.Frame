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

namespace ZeKi.Frame.DAL
{
    /// <summary>
    /// 数据访问父类
    /// </summary>
    public class BaseDAL : IBaseDAL
    {
        public DbContext DBContext { set; get; }

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        public BaseDAL()
        {

        }
        #endregion

        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="getId">是否获取当前插入的ID,不是自增则不需要关注此值</param>
        /// <returns>getId为true并且有自增列则返回插入的id值,否则为影响行数</returns>
        public virtual int Insert<TModel>(TModel model, bool getId = false) where TModel : class, new()
        {
            return DBContext.Insert(model, getId);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list"></param>
        /// <param name="ps">每批次数量,默认500</param>
        /// <returns>返回总影响行数</returns>
        public virtual int BatchInsert<TModel>(IEnumerable<TModel> list, int ps = 500) where TModel : class, new()
        {
            return DBContext.BatchInsert(list, ps);
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
            return DBContext.Update(model);
        }

        /// <summary>
        /// 修改(根据自定义条件修改自定义值)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="setAndWhere">set和where的键值对,使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类
        /// <para>格式:new {renew_name="u",id=1},解析:set字段为renew_name="u",where条件为id=1</para>
        /// <para>修改值必须以renew_开头,如数据库字段名有此开头需要叠加</para>
        /// <para>如 where值中有集合/数组,则生成 in @Key ,sql: in ('','')</para>
        /// <para>set字段能否被修改受 <see cref="PropertyAttribute"/> 特性限制</para>
        /// </param>
        /// <returns></returns>
        public virtual int Update<TModel>(object setAndWhere) where TModel : class, new()
        {
            return DBContext.Update<TModel>(setAndWhere);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">传递主键字段值即可,如需统一清除缓存,可以传递所有字段值数据</param>
        /// <returns></returns>
        public virtual bool Delete<TModel>(TModel model) where TModel : class, new()
        {
            return DBContext.Delete(model);
        }
        #endregion

        #region Query
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <typeparam name="T">返回模型,如果与表不一致,需要在返回表添加表特性或者sql为全sql</typeparam>
        /// <param name="sql">可以书写where name=@name,会自动补全, 也可以书写 select * from tb where name=@name</param>
        /// <param name="param"></param>
        /// <returns>返回集合</returns>
        public virtual IEnumerable<T> QueryList<T>(string sql, object param = null)
        {
            return DBContext.QueryList<T>(sql, param);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryList<T>(object whereObj = null, string orderStr = null, string selectFields = "*")
        {
            return DBContext.QueryList<T>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryList<T>(string sqlNoWhere, object whereObj = null, string orderStr = null)
        {
            return DBContext.QueryList<T>(sqlNoWhere, whereObj, orderStr);
        }

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <typeparam name="T">返回模型,如果与表不一致,需要在返回表添加表特性或者sql为全sql</typeparam>
        /// <param name="sql">可以书写where name=@name 也可以书写 select top 1 * from tb where name=@name</param>
        /// <param name="param"></param>
        /// <returns>返回单个</returns>
        public virtual T QueryModel<T>(string sql, object param = null)
        {
            return DBContext.QueryModel<T>(sql, param);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        public virtual T QueryModel<T>(object whereObj, string orderStr = null, string selectFields = "*")
        {
            return DBContext.QueryModel<T>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <returns></returns>
        public virtual T QueryModel<T>(string sqlNoWhere, object whereObj = null, string orderStr = null)
        {
            return DBContext.QueryModel<T>(sqlNoWhere, whereObj, orderStr);
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
            return DBContext.QueryList(sql, map, param);
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
            return DBContext.QueryList(sql, map, param);
        }

        /// <summary>
        /// 分页查询返回集合
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="pcp">分页模型(参数传递查看PageParameters字段注解)</param>
        /// <param name="param">同dapper参数传值</param>
        /// <returns></returns>
        public virtual PageData<T> PageList<T>(PageParameters pcp, object param = null) where T : class, new()
        {
            return DBContext.PageList<T>(pcp, param);
        }

        ///// <summary>
        ///// 分页查询返回数据集
        ///// </summary>
        ///// <param name="pcp">分页必填参数</param>
        ///// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        ///// <returns></returns>
        //public virtual DataSet PageDataSet(PageParameters pcp, object param = null)
        //{
        //    using (var _conn = GetConnection())
        //    {
        //        return DBConnection.PageDataSet(pcp, param);
        //    }
        //}

        ///// <summary>
        ///// 分页查询返回数据表
        ///// </summary>
        ///// <param name="pcp">分页必填参数</param>
        ///// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        ///// <returns></returns>
        //public virtual DataTable PageDataTable(PageParameters pcp, object param = null)
        //{
        //    using (var _conn = GetConnection())
        //    {
        //        return DBConnection.PageDataTable(pcp, param);
        //    }
        //}
        #endregion

        #region Procedure
        /// <summary>
        /// 执行查询存储过程(用于查询数据)
        /// <para>参数传递参考：https://github.com/StackExchange/Dapper#stored-procedures </para>
        /// </summary>
        /// <typeparam name="T">返回模型,如果没有对应模型类接收,可以传入dynamic,然后序列化再反序列化成List泛型参数:Hashtable</typeparam>
        /// <param name="proceName">存储过程名</param>
        /// <param name="param">特定键值字典/Hashtable/匿名类/自定义类,过程中有OutPut或者Return参数,使用<see cref="DataParameters"/></param>
        /// <returns>返回集合</returns>
        public virtual IEnumerable<T> QueryProcedure<T>(string proceName, object param = null)
        {
            return DBContext.QueryProcedure<T>(proceName, param);
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
            DBContext.ExecProcedure(proceName, param);
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
            return DBContext.Count<TModel>(sqlWhere, param);
        }

        /// <summary>
        /// Count统计
        /// </summary>
        /// <param name="whereObj">使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <returns></returns>
        public virtual int Count<TModel>(object whereObj = null) where TModel : class, new()
        {
            return DBContext.Count<TModel>(whereObj);
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
            return DBContext.Sum<TModel, TResult>(field, sqlWhere, param);
        }
        #endregion

        #region Excute
        /// <summary>
        /// 执行sql(非查询)
        /// </summary>
        /// <returns></returns>
        public int Execute(string sql, object param = null)
        {
            return DBContext.Execute(sql, param);
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
                DBContext.BeginTransaction(isolation);
                action();
                DBContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                DBContext.RollbackTransaction();
                throw ex;
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
            return DBContext.ExecTransaction(strSqls, param);
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
        //public virtual DataTable ExecDataTable(string commandText, object param = null, CommandType commandType = CommandType.Text)
        //{
        //   return DBConnection.ExecDataTable(commandText, param, commandType);
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
        //public virtual DataSet ExecDataSet(string commandText, object param = null, CommandType commandType = CommandType.Text)
        //{
        //   return DBConnection.ExecDataSet(commandText, param, commandType);
        //}
        #endregion

        #region 内部帮助/受保护类型 方法

        #endregion
    }
}
