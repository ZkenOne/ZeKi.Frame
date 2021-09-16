using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ZeKi.Frame.Model;
using ZeKi.Frame.Common;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.IBLL;
using System.Data;

namespace ZeKi.Frame.BLL
{
    /// <summary>
    /// 业务层父类
    /// </summary>
    public abstract class BaseBLL : IBaseBLL
    {
        public IBaseDAL DAL { set; get; }

        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="getId">是否获取当前插入的ID,不是自增则不需要关注此值</param>
        /// <returns>getId为true并且有自增列则返回插入的id值,否则为影响行数</returns>
        public virtual int Insert<TModel>(TModel model, bool getId = false) where TModel : class, new()
        {
            return DAL.Insert<TModel>(model, getId);
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
            return DAL.Update<TModel>(model);
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
            return DAL.UpdatePart<TModel>(setAndWhere);
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
            return DAL.Delete<TModel>(model);
        }
        #endregion

        #region Query
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public virtual IEnumerable<TResult> QueryList<TTable, TResult>(object whereObj = null, string orderStr = null, string selectFields = null)
        {
            return DAL.QueryList<TTable, TResult>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryList<T>(object whereObj = null, string orderStr = null, string selectFields = null)
        {
            return DAL.QueryList<T>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public virtual TResult QueryModel<TTable, TResult>(object whereObj, string orderStr = null, string selectFields = null)
        {
            return DAL.QueryModel<TTable, TResult>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public virtual T QueryModel<T>(object whereObj, string orderStr = null, string selectFields = null)
        {
            return DAL.QueryModel<T>(whereObj, orderStr, selectFields);
        }
        #endregion

    }

}
