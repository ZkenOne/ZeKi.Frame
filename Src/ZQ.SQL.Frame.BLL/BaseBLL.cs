using Dapper.Extensions.ZQ;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZQ.SQL.Frame.Common;
using ZQ.SQL.Frame.DAL;
using static Dapper.SqlMapper;

namespace ZQ.SQL.Frame.BLL
{
    /// <summary>
    /// 业务层父类
    /// </summary>
    /// <typeparam name="TModel">模型实体类</typeparam>
    public abstract class BaseBLL<TModel> where TModel : class, new()
    {
        /// <summary>
        /// 由每个子BLL为其赋值
        /// </summary>
        protected BaseDAL<TModel> DAL { set; get; }

        #region 构造函数
        public BaseBLL()
        {
            //由每个子BLL为其赋值,此处不进行实例化
            //BaseDAL如果直接集成到项目,可以把这些参数直接在BaseDAL里赋值即可
            //BaseDAL = new BaseDAL<TModel>(GlobaConfig.SqlConn, GlobaConfig.DBType, GlobaConfig.CommandTimeout);
        }
        #endregion

        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="getId">是否获取当前插入的ID,不是自增则不需要关注此值</param>
        /// <returns>getId为true并且有自增列则返回插入的id值,否则为影响行数</returns>
        public virtual long Insert(TModel model, bool getId = false)
        {
            return DAL.Insert(model, getId);
        }
        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list"></param>
        /// <returns>返回总影响行数</returns>
        public virtual long Insert(IEnumerable<TModel> list)
        {
            return DAL.Insert(list);
        }
        #endregion

        #region Update
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Update(TModel model)
        {
            return DAL.Update(model);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">传递主键字段值即可,如需统一清除缓存,可以传递所有字段值数据</param>
        /// <returns></returns>
        public virtual bool Delete(TModel model)
        {
            return DAL.Delete(model);
        }
        #endregion

        #region Query
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型,如果与表不一致,需要在返回表添加表特性或者sql为全sql</typeparam>
        /// <param name="sql">可以书写where name=@name,会自动补全, 也可以书写 select * from tb where name=@name</param>
        /// <param name="param"></param>
        /// <returns>返回集合</returns>
        public virtual IEnumerable<T> QueryList<T>(string sql, object param = null)
        {
            return DAL.QueryList<T>(sql, param);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql">可以书写where name=@name,会自动补全, 也可以书写 select * from tb where name=@name</param>
        /// <param name="param"></param>
        /// <returns>返回集合</returns>
        public virtual IEnumerable<TModel> QueryList(string sql, object param = null)
        {
            return QueryList<TModel>(sql, param);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">返回模型,如果与表不一致,需要在返回表添加表特性或者sql为全sql</typeparam>
        /// <param name="sql">可以书写where name=@name 也可以书写 select top 1 * from tb where name=@name</param>
        /// <param name="param"></param>
        /// <returns>返回单个</returns>
        public virtual T QueryModel<T>(string sql, object param = null)
        {
            return DAL.QueryModel<T>(sql, param);
        }

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="sql">可以书写where name=@name,会自动补全, 也可以书写 select * from tb where name=@name</param>
        /// <param name="param"></param>
        /// <returns>返回集合</returns>
        public virtual TModel QueryModel(string sql, object param = null)
        {
            return DAL.QueryModel<TModel>(sql, param);
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
            return DAL.QueryList(sql, map, param);
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
            return DAL.QueryList(sql, map, param);
        }

        ///// <summary>
        ///// 查询返回多个结果集 (using之后在外面无法获取数据,可以在外部使用DbAction方法来完成)
        ///// </summary>
        ///// <param name="sql">需要输入全sql</param>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public GridReader QueryMultiple(string sql, object param = null)
        //{
        //    return DAL.QueryMultiple(sql, param);
        //}

        /// <summary>
        /// 分页查询返回集合
        /// </summary>
        /// <typeparam name="T">返回模型</typeparam>
        /// <param name="pcp">分页模型(参数传递查看PageParameters字段注解)</param>
        /// <param name="param">同dapper参数传值</param>
        /// <returns></returns>
        public virtual PageData<T> PageList<T>(PageParameters pcp, object param = null) where T : class, new()
        {
            return DAL.PageList<T>(pcp, param);
        }

        /// <summary>
        /// 分页查询返回集合
        /// </summary>
        /// <param name="sql">可以书写where name=@name,会自动补全, 也可以书写 select * from tb where name=@name</param>
        /// <param name="param"></param>
        /// <returns>返回集合</returns>
        public virtual PageData<TModel> PageList(PageParameters pcp, object param = null)
        {
            return DAL.PageList<TModel>(pcp, param);
        }

        /// <summary>
        /// 分页查询返回数据集
        /// </summary>
        /// <param name="pcp">分页必填参数</param>
        /// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        /// <returns></returns>
        public virtual DataSet PageDataSet(PageParameters pcp, object param = null)
        {
            return DAL.PageDataSet(pcp, param);
        }

        /// <summary>
        /// 分页查询返回数据表
        /// </summary>
        /// <param name="pcp">分页必填参数</param>
        /// <param name="param">该参数需传原生参数化对象.如mssql:SqlParameter,多个用List包含传递</param>
        /// <returns></returns>
        public virtual DataTable PageDataTable(PageParameters pcp, object param = null)
        {
            return DAL.PageDataTable(pcp, param);
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
        public virtual int Count(string sqlWhere, object param = null)
        {
            return DAL.Count(sqlWhere, param);
        }

        /// <summary>
        /// Sum统计
        /// </summary>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="field">需要sum的字段</param>
        /// <param name="sqlWhere">1=1,省略where</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual TResult Sum<TResult>(string field, string sqlWhere, object param = null)
        {
            return DAL.Sum<TResult>(field, sqlWhere, param);
        }
        #endregion

        #region Dapper原生
        /// <summary>
        /// 执行数据库操作,会自动释放,操作QueryMultiple可以用这个
        /// </summary>
        /// <param name="action"></param>
        public virtual void DbAction(Action<IDbConnection> action)
        {
            DAL.DbAction(action);
        }
        #endregion

        #region Transaction
        /// <summary>
        /// 执行事务(每次执行数据库操作需要将tran对象传入)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="isolation"></param>
        public virtual void ExecTransaction(Action<IDbConnection, IDbTransaction> action, IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            DAL.ExecTransaction(action, isolation);
        }

        /// <summary>
        /// 执行事务(每次执行数据库操作需要将tran对象传入)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="isolation">事务级别</param>
        /// <returns></returns>
        public virtual T ExecTransaction<T>(Func<IDbConnection, IDbTransaction, T> func, IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            return DAL.ExecTransaction(func, isolation);
        }

        /// <summary>
        /// 批量数据事务提交(与dapper无关)
        /// </summary>
        /// <param name="strSqls">T-SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public virtual int ExecTransaction(string strSqls, object param = null)
        {
            return DAL.ExecTransaction(strSqls, param);
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
        public virtual DataTable ExecDataTable(string commandText, object param = null, CommandType commandType = CommandType.Text)
        {
            return DAL.ExecDataTable(commandText, param, commandType);
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
        public virtual DataSet ExecDataSet(string commandText, object param = null, CommandType commandType = CommandType.Text)
        {
            return DAL.ExecDataSet(commandText, param, commandType);
        }
        #endregion

    }

}
