using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeKi.Frame.Model;
using Dapper;

namespace ZeKi.Frame.DB
{
    internal class SqlServerAdapter : ISqlAdapter
    {
        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnsStr, string parametersStr, object entitysToInsert, bool getIncVal = false)
        {
            var cmd = $"insert into {tableName} ({columnsStr}) values ({parametersStr});";
            if (getIncVal)
            {
                cmd += "select SCOPE_IDENTITY() id";
                return connection.ExecuteScalar<int>(cmd, entitysToInsert, transaction, commandTimeout);
            }
            else
            {
                return connection.Execute(cmd, entitysToInsert, transaction, commandTimeout);
            }
        }

        public PageData<TResult> Pager<TResult>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, SqlAdapterPagerParameters adapterParam)
        {
            //开始
            int start = (adapterParam.PageIndex - 1) * adapterParam.PageSize + 1;
            //结束
            int end = adapterParam.PageIndex * adapterParam.PageSize;

            var sqlStr = $"SELECT * FROM ( SELECT ROW_NUMBER() OVER(ORDER BY {adapterParam.Order}) AS ROW_NUMBER, " +
                $" {adapterParam.Select} from {adapterParam.Table} {adapterParam.Where} ) AS Tab WHERE ROW_NUMBER BETWEEN {start} AND {end}; " +
                $" SELECT COUNT(0) AS DataCount FROM (SELECT 1 as [count] from {adapterParam.Table} {adapterParam.Where}) AS CountTb;";
                //$" SELECT COUNT(0) AS DataCount FROM (SELECT {adapterParam.Select} from {adapterParam.Table} {adapterParam.Where}) AS CountTb;";

            var multi = connection.QueryMultiple(sqlStr, adapterParam.Params, transaction, commandTimeout);
            var pageData = new PageData<TResult>()
            {
                Data = multi.Read<TResult>(),
                TotalCount = multi.ReadSingle<long>(),
                PageIndex = adapterParam.PageIndex,
                PageSize = adapterParam.PageSize
            };
            pageData.PageCount = (int)Math.Ceiling(pageData.TotalCount * 1.0 / pageData.PageSize);
            return pageData;
        }

        public string ParametricSymbol { get; } = "@";
    }
}
