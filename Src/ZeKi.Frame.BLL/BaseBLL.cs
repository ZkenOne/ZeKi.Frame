using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ZeKi.Frame.Model;
using ZeKi.Frame.Common;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.IBLL;

namespace ZeKi.Frame.BLL
{
    /// <summary>
    /// 业务层父类
    /// </summary>
    /// <typeparam name="TModel">模型实体类</typeparam>
    public class BaseBLL<TModel> : IBaseBLL<TModel> where TModel : class, new()
    {
        public IBaseDAL<TModel> DAL { set; get; }

        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="getId">是否获取当前插入的ID,不是自增则不需要关注此值</param>
        /// <returns>getId为true并且有自增列则返回插入的id值,否则为影响行数</returns>
        public virtual int Insert(TModel model, bool getId = false)
        {
            return DAL.Insert(model, getId);
        }
        #endregion

        #region Update
        /// <summary>
        /// 修改(单个,根据主键)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual bool Update(TModel model)
        {
            return DAL.Update(model);
        }

        /// <summary>
        /// 修改(根据自定义条件修改自定义值)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="setAndWhere">set和where的键值对,使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类
        /// <para>格式:new {renew_name="u",id=1},解析:set字段为renew_name="u",where条件为id=1</para>
        /// <para>修改值必须以renew_开头,如数据库字段名有此开头需要叠加</para>
        /// <para>如 where值中有集合/数组,则生成 in @Key ,sql: in ('','')</para>
        /// <para>set字段能否被修改受 <see cref="PropertyAttribute"/> 特性限制</para>
        /// </param>
        /// <returns></returns>
        public virtual int Update(object setAndWhere)
        {
            return DAL.Update(setAndWhere);
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
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public virtual IEnumerable<T> QueryList<T>(object whereObj = null, string orderStr = null, string selectFields = "*")
        {
            return DAL.QueryList<T>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="orderStr">填写：id asc / id,name desc</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public virtual IEnumerable<TModel> QueryList(object whereObj = null, string orderStr = null, string selectFields = "*")
        {
            return QueryList<TModel>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public virtual T QueryModel<T>(object whereObj, string orderStr = null, string selectFields = "*")
        {
            return DAL.QueryModel<T>(whereObj, orderStr, selectFields);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
        /// <param name="selectFields">,分隔</param>
        /// <returns></returns>
        public virtual TModel QueryModel(object whereObj, string orderStr = null, string selectFields = "*")
        {
            return QueryModel<TModel>(whereObj, orderStr, selectFields);
        }
        #endregion

    }

}
