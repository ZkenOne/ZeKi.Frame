using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZeKi.Frame.Model
{
    /// <summary>
    /// 分页参数模型
    /// </summary>
    public class PageParameters
    {
        /// <summary>
        /// 查询字段(没有则为*)
        /// <para>a.id,a.Name,b.sex,c.Flag</para>
        /// </summary>
        public string Select { get; set; } = "*";

        /// <summary>
        /// 表的sql
        /// <para>tb_single as s</para>
        /// <para>tb_a as a left join tb_b as b on b.id=a.id</para>
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// 排序字段(省略 order by),如: id desc
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// where条件参数
        /// <para>使用 匿名类、指定数据类型类<see cref="DataParameters"/>、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</para>
        /// </summary>
        public object WhereParam { get; set; }

        /// <summary>
        /// 当前第几页
        /// <para>默认1</para>
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页分多少条数据
        /// <para>默认10条</para>
        /// </summary>
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// 分页数据模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageData<T>
    {
        /// <summary>
        /// 分页总数据
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// 用于统计
        /// </summary>
        public object Total { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 数据总数
        /// </summary>
        public long TotalCount { get; set; }
    }
}
