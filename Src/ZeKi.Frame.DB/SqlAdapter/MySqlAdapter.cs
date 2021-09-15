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
    internal class MySqlAdapter : ISqlAdapter
    {
        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnsStr, string parametersStr, object entitysToInsert, bool getIncVal = false)
        {
            var cmd = $"insert into {tableName} ({columnsStr}) values ({parametersStr});";
            if (getIncVal)
            {
                cmd += "Select LAST_INSERT_ID() id";
                return connection.ExecuteScalar<int>(cmd, entitysToInsert, transaction, commandTimeout);
            }
            else
            {
                return connection.Execute(cmd, entitysToInsert, transaction, commandTimeout);
            }
        }

        public PageData<TResult> Pager<TResult>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, SqlAdapterPagerParameters adapterParam)
        {
            var sqlStr = $"select {adapterParam.Select} from {adapterParam.Table} {adapterParam.Where} {adapterParam.Order}" +
                         $"limit {(adapterParam.PageIndex - 1) * adapterParam.PageSize},{adapterParam.PageSize};" +
                         $"select count(0) from {adapterParam.Table} {adapterParam.Where};";

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
