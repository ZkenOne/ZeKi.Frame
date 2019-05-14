using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace ZQ.SQL.Frame.Common
{
    /// <summary>
    /// 搜索条件 拼接辅助类
    /// </summary>
    public class SqlBuild
    {
        private StringBuilder sbSqlWhere = new StringBuilder(100);

        /// <summary>
        /// 防止参数名一样,值不一样 eg: action!='login' and action='getuser'
        /// </summary>
        private int parmIndex = 0;

        public SqlBuild()
        {
            sbSqlWhere.Append(" where 1=1 ");
        }

        /// <summary>
        /// 拼接参数
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="value"></param>
        /// <param name="type">枚举</param>
        /// <returns> and 1=1 </returns>
        public void BuildParm(string field, string value, SqlOpt type = (byte)SqlOpt.Equal)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            sbSqlWhere.Append($" and {field} ");
            var fieldParm = $"{field.Replace(".", "")}{parmIndex}";
            var fieldVal = value;
            switch (type)
            {
                case SqlOpt.Equal:
                    sbSqlWhere.Append($" = @{fieldParm} ");
                    break;
                case SqlOpt.Gt:
                    sbSqlWhere.Append($" > @{fieldParm} ");
                    break;
                case SqlOpt.Less:
                    sbSqlWhere.Append($" < @{fieldParm} ");
                    break;
                case SqlOpt.GtEqual:
                    sbSqlWhere.Append($" >= @{fieldParm} ");
                    break;
                case SqlOpt.LessEqual:
                    sbSqlWhere.Append($" <= @{fieldParm} ");
                    break;
                case SqlOpt.NotEqual:
                    sbSqlWhere.Append($" <> @{fieldParm} ");
                    break;
                case SqlOpt.Like:
                    sbSqlWhere.Append($" like @{fieldParm} ");
                    fieldVal = $"%{value}%";
                    break;
            }
            DyParm.Add(fieldParm, fieldVal);
            parmIndex++;
        }

        /// <summary>
        /// 拼接参数
        /// </summary>
        /// <returns> and 1 between 1 and 10 </returns>
        public void BuildDateBetweenParm(string field, DateTime begin_date, DateTime end_date)
        {
            sbSqlWhere.Append($" and {field} between @{field}_begin_date and @{field}_end_date ");
            DyParm.Add($"{field}_begin_date", begin_date.ToString("yyyy-MM-dd HH:mm:ss"));
            DyParm.Add($"{field}_end_date", end_date.ToString("yyyy-MM-dd HH:mm:ss"));
        }


        public DynamicParameters DyParm { get; } = new DynamicParameters();

        /// <summary>
        /// 返回拼接的where语句 where 1=1 and 2=2
        /// </summary>
        /// <returns></returns>
        public string GetSqlWhereStr()
        {
            return sbSqlWhere.ToString();
        }

        /// <summary>
        /// 重写,返回拼接好的条件sql
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetSqlWhereStr();
        }
    }

    public enum SqlOpt
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal = 0,
        /// <summary>
        /// 大于
        /// </summary>
        Gt = 1,
        /// <summary>
        /// 小于
        /// </summary>
        Less = 2,
        /// <summary>
        /// 大于等于
        /// </summary>
        GtEqual = 3,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessEqual = 4,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual = 5,
        /// <summary>
        /// 模糊查询
        /// </summary>
        Like = 10
    }

    //使用:
    //SqlBuild build = new SqlBuild();
    //build.BuildParm("action", "login", SqlOpt.NotEqual);
    //build.BuildDateBetweenParm("rq_create", queryModel.begin_date, queryModel.end_date);
    //build.BuildParm("id_masteruser", queryModel.id_masteruser);
    //build.BuildParm("id_shop", queryModel.id_shop);
    //build.BuildParm("id_user", queryModel.id_user);
    //build.BuildParm("content", queryModel.content, SqlOpt.Like);
    //build.BuildParm("flag_rank", queryModel.flag_rank == 0 ? "" : queryModel.flag_rank.ToString());
    //build.BuildParm("flag_type", queryModel.flag_type == 0 ? "" : queryModel.flag_type.ToString());
    //build.BuildParm("flag_from", queryModel.flag_from == 0 ? "" : queryModel.flag_from.ToString());
    //build.BuildParm("controller", queryModel.controllerStr);
    //build.BuildParm("action", queryModel.actionStr);
    //build.BuildParm("ip", queryModel.ip);
    //build.BuildParm("request_full", queryModel.request_full, SqlOpt.Like);
    //build.BuildParm("spend_second", queryModel.spend_second == 0 ? "" : queryModel.spend_second.ToString(), (SqlOpt) queryModel.spend_second_opt);
    //var strWhere = build.GetSqlWhereStr();
    //var sqlStrCount = $"select count(1) from {tableName} {strWhere}";
    //var sqlStr = $@"select * from ( select *, ROW_NUMBER() OVER(Order by {queryModel.orderBy} ) AS RowId from {tableName} {strWhere}) as t1 
    //                where RowId between {(queryModel.pi - 1) * queryModel.ps + 1} and {queryModel.ps * queryModel.pi}";
    //dataCount = SqlHelper.ExecuteScalar<int>(sqlStrCount, build.DyParm);
    //if (dataCount > 0)
    //logList = SqlHelper.Query<Log_VM>(sqlStr, build.DyParm);

}
