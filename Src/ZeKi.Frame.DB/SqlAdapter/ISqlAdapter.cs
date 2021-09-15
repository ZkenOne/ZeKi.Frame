using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.DB
{
    internal interface ISqlAdapter
    {
        //entitysToInsert : 可为数组或实体
        int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string tableName, string columnsStr, string parametersStr, object entitysToInsert, bool getIncVal = false);

        PageData<T> Pager<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, SqlAdapterPagerParameters adapterParam);

        string ParametricSymbol { get; }
    }
}
