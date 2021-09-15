using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeKi.Frame.DB
{
    /// <summary>
    /// sql适配器分页参数模型
    /// </summary>
    internal class SqlAdapterPagerParameters
    {
        /// <summary>
        /// 查询字段(没有则为*)
        /// <para>a.id,a.Name,b.sex,c.Flag</para>
        /// </summary>
        public string Select { get; set; }

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
        /// 参数化
        /// </summary>
        public object Params { get; set; }

        /// <summary>
        /// where name=@name
        /// </summary>
        public string Where { get; set; }

        /// <summary>
        /// 当前第几页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页分多少条数据
        /// </summary>
        public int PageSize { get; set; }
    }
}
