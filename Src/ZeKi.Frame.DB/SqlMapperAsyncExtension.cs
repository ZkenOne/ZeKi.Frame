// 注释先,不想两边都维护...
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Dapper;
//using ZeKi.Frame.Common;
//using ZeKi.Frame.Model;

//namespace ZeKi.Frame.DB
//{
//    /// <summary>
//    /// 基于Dapper扩展到IDbConnection对象 异步方法
//    /// </summary>
//    public static partial class SqlMapperExtension
//    {
//        #region Insert
//        /// <summary>
//        /// 新增
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="connection"></param>
//        /// <param name="entityToInsert">模型对象</param>
//        /// <param name="getIncId">是否获取当前插入的ID,不是自增则不需要关注此值</param>
//        /// <param name="transaction"></param>
//        /// <param name="commandTimeout"></param>
//        /// <returns>getId为true并且有自增列则返回插入的id值,否则为影响行数</returns>
//        public static async Task<int> InsertAsync<T>(this IDbConnection connection, T entityToInsert, bool getIncId = false, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
//        {
//            var type = typeof(T);
//            List<string> propertyNames = GetPropertyNames(type);
//            string cols = string.Join(",", propertyNames);
//            string colsParams = string.Join(",", propertyNames.Select(p => "@" + p));
//            var sql = "insert " + GetTableName(type) + " (" + cols + ") values (" + colsParams + ");";
//            if (getIncId)
//            {
//                sql += GetInsertIncSql(connection);
//                return await connection.ExecuteScalarAsync<int>(sql, entityToInsert, transaction, commandTimeout);
//            }
//            else
//            {
//                return await connection.ExecuteAsync(sql, entityToInsert, transaction, commandTimeout);
//            }
//        }

//        /// <summary>
//        /// 批量新增(自定义批次新增数,需要保证一致性可以在外部加事务)
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="connection"></param>
//        /// <param name="entitysToInsert">模型对象集合</param>
//        /// <param name="ps">每批次新增数,默认500</param>
//        /// <param name="transaction"></param>
//        /// <param name="commandTimeout"></param>
//        /// <returns>影响行数</returns>
//        public static async Task<int> BatchInsertAsync<T>(this IDbConnection connection, IEnumerable<T> entitysToInsert, int ps = 500, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
//        {
//            var type = typeof(T);
//            List<string> propertyNames = GetPropertyNames(type);
//            string cols = string.Join(",", propertyNames);
//            string colsParams = string.Join(",", propertyNames.Select(p => "@" + p));
//            var sql = "insert into " + GetTableName(type) + " (" + cols + ") values (" + colsParams + ");";
//            //分批次执行
//            int pi = 1, totalPage = (int)Math.Ceiling(entitysToInsert.Count() * 1.0 / ps), effectRow = 0;
//            for (int i = 0; i < totalPage; i++)
//            {
//                var tpEntitys = entitysToInsert.Skip((pi - 1) * ps).Take(ps);
//                //dapper支持 : connection.Execute(@"insert MyTable(colA, colB) values (@a, @b)",new[] { new { a=1, b=1 }, new { a=2, b=2 }, new { a=3, b=3 } });
//                effectRow += await connection.ExecuteAsync(sql, tpEntitys, transaction, commandTimeout);
//                pi++;
//            }
//            return effectRow;
//        }
//        #endregion

//        #region Update
//        /// <summary>
//        /// 修改(单个,根据主键)
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="connection"></param>
//        /// <param name="entityToUpdate">字段能否被修改受 <see cref="PropertyAttribute"/> 特性限制</param>
//        /// <param name="transaction"></param>
//        /// <param name="commandTimeout"></param>
//        /// <returns></returns>
//        public static async Task<bool> UpdateAsync<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
//        {
//            var type = typeof(T);
//            List<string> paramNames = GetPropertyNames(type, 1);
//            var builder = new StringBuilder();
//            builder.Append("update ").Append(GetTableName(type)).Append(" set ");
//            builder.Append(string.Join(",", paramNames.Select(p => p + "= @" + p)));
//            var pk = GetPrimaryKey(type);
//            builder.Append($" where {pk} = @{pk}");
//            return await connection.ExecuteAsync(builder.ToString(), entityToUpdate, transaction, commandTimeout) > 0;
//        }

//        /// <summary>
//        /// 修改(根据自定义条件修改自定义值)
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="connection"></param>
//        /// <param name="setAndWhere">set和where的键值对,使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类
//        /// <para>格式:new {renew_name="u",id=1},解析:set字段为renew_name="u",where条件为id=1</para>
//        /// <para>修改值必须以renew_开头,如数据库字段名有此开头需要叠加</para>
//        /// <para>如 where值中有集合/数组,则生成 in @Key ,sql: in ('','')</para>
//        /// <para>set字段能否被修改受 <see cref="PropertyAttribute"/> 特性限制</para>
//        /// </param>
//        /// <param name="transaction"></param>
//        /// <param name="commandTimeout"></param>
//        /// <returns>影响行数</returns>
//        public static async Task<int> UpdateAsync<T>(this IDbConnection connection, object setAndWhere, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
//        {
//            var upset_prefix = "renew_";
//            var model_type = typeof(T);
//            var dictPar = ToDictionary(setAndWhere);
//            var setFields = dictPar.Keys.Where(p => p.StartsWith(upset_prefix));
//            if (setFields == null || !setFields.Any())
//                throw new Exception("未设置修改值");
//            var whereFields = dictPar.Keys.Where(p => !p.StartsWith(upset_prefix));
//            if (whereFields == null || !whereFields.Any())
//                throw new Exception("未设置条件值");
//            var allowUpFieldList = GetPropertyNames(model_type, 1);
//            var forbidUpFieldList = new List<string>();
//            foreach (var itemField in setFields)
//            {
//                var tp_name = itemField.Remove(0, upset_prefix.Length);
//                if (!allowUpFieldList.Any(p => p.ToLower() == tp_name.ToLower()))
//                {
//                    forbidUpFieldList.Add(tp_name);
//                }
//            }
//            if (forbidUpFieldList.Any())
//            {
//                throw new Exception($"字段[{string.Join(",", forbidUpFieldList)}]已设置为不允许修改");
//            }
//            var builder = new StringBuilder();
//            builder.Append("update ").Append(GetTableName(model_type)).Append(" set ");
//            //name=@renew_name,flag_style=@renew_flag_style
//            foreach (var itemField in setFields)
//            {
//                builder.Append($"{itemField.Remove(0, upset_prefix.Length)} = @{itemField},");
//            }
//            builder = builder.Remove(builder.Length - 1, 1);  //去掉多余的,
//            builder.Append(BuildWhereSql(dictPar, whereFields));
//            return await connection.ExecuteAsync(builder.ToString(), dictPar, transaction, commandTimeout);
//        }
//        #endregion

//        #region Delete
//        /// <summary>
//        /// 删除
//        /// </summary>
//        /// <typeparam name="T">传递主键字段值即可,如需统一清除缓存,可以传递所有字段值数据</typeparam>
//        /// <param name="connection"></param>
//        /// <param name="entityToDelete"></param>
//        /// <param name="transaction"></param>
//        /// <param name="commandTimeout"></param>
//        /// <returns></returns>
//        public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
//        {
//            var type = typeof(T);
//            var pk = GetPrimaryKey(type);
//            var sql = $"delete from {GetTableName(type)} where {pk} = @{pk}";
//            return await connection.ExecuteAsync(sql, entityToDelete, transaction, commandTimeout) > 0;
//        }
//        #endregion

//        #region Query
//        /// <summary>
//        /// 查询
//        /// </summary>
//        /// <typeparam name="T">返回模型</typeparam>
//        /// <param name="sql">可以书写where name=@name,会自动补全, 也可以书写 select * from tb where name=@name</param>
//        /// <param name="param"></param>
//        /// <param name="transaction"></param>
//        /// <param name="commandTimeout"></param>
//        /// <returns>返回集合</returns>
//        public static async Task<IEnumerable<T>> QueryListAsync<T>(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
//        {
//            var type = typeof(T);
//            sql = BuildSqlStart(type, sql, 0);
//            return await connection.QueryAsync<T>(sql, param, transaction, commandTimeout: commandTimeout);
//        }

//        /// <summary>
//        /// 查询(分页使用PageList方法)
//        /// </summary>
//        /// <typeparam name="T">返回模型</typeparam>
//        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
//        /// <param name="orderStr">填写：id asc / id,name desc</param>
//        /// <param name="selectFields">,分隔</param>
//        /// <param name="transaction"></param>
//        /// <param name="commandTimeout"></param>
//        /// <returns>返回集合</returns>
//        public static async Task<IEnumerable<T>> QueryListAsync<T>(this IDbConnection connection, object whereObj, string orderStr = null, string selectFields = "*", IDbTransaction transaction = null, int? commandTimeout = null)
//        {
//            var type = typeof(T);
//            var builder = new StringBuilder();
//            var dictPar = ToDictionary(whereObj);
//            builder.Append($"select {selectFields} from {GetTableName(type)} ");
//            if (dictPar != null && dictPar.Any())
//                builder.Append(BuildWhereSql(dictPar, dictPar.Keys));
//            if (!string.IsNullOrEmpty(orderStr))
//                builder.Append($" order by {orderStr}");
//            return await connection.QueryAsync<T>(builder.ToString(), dictPar, transaction, commandTimeout: commandTimeout);
//        }

//        /// <summary>
//        /// 查询
//        /// </summary>
//        /// <typeparam name="T">返回类型</typeparam>
//        /// <param name="sql">可以书写where name=@name 也可以书写 select top 1 * from tb where name=@name</param>
//        /// <param name="param"></param>
//        /// <param name="transaction"></param>
//        /// <param name="commandTimeout"></param>
//        /// <returns>返回单个</returns>
//        public static async Task<T> QueryModelAsync<T>(this IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
//        {
//            var type = typeof(T);
//            sql = BuildSqlStart(type, sql, 1);
//            //QueryFirstOrDefault,查询如果是很多条记录,只会取其中的一条,所以需要自己写top 1
//            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout: commandTimeout);
//        }

//        /// <summary>
//        /// 查询
//        /// </summary>
//        /// <typeparam name="T">返回模型</typeparam>
//        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
//        /// <param name="selectFields">,分隔</param>
//        /// <param name="transaction"></param>
//        /// <param name="commandTimeout"></param>
//        /// <returns>返回集合</returns>
//        public static async Task<T> QueryModelAsync<T>(this IDbConnection connection, object whereObj, string selectFields = "*", IDbTransaction transaction = null, int? commandTimeout = null)
//        {
//            var type = typeof(T);
//            var builder = new StringBuilder();
//            var dictPar = ToDictionary(whereObj);
//            builder.Append($"select top 1 {selectFields} from {GetTableName(type)} ");
//            if (dictPar != null && dictPar.Any())
//                builder.Append(BuildWhereSql(dictPar, dictPar.Keys));
//            return await connection.QueryFirstOrDefaultAsync<T>(builder.ToString(), dictPar, transaction, commandTimeout: commandTimeout);
//        }

//        /// <summary>
//        /// 分页查询
//        /// </summary>
//        /// <typeparam name="T">返回模型</typeparam>
//        /// <param name="pcp">分页模型(参数传递查看PageParameters字段注解)</param>
//        /// <param name="param">同dapper参数传值</param>
//        /// <param name="transaction"></param>
//        /// <param name="commandTimeout"></param>
//        /// <returns></returns>
//        public static async Task<PageData<T>> PageListAsync<T>(this IDbConnection connection, PageParameters pcp, object param = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
//        {
//            var sql = CreatePageSql(connection, typeof(T), pcp);
//            var multi = await connection.QueryMultipleAsync(sql, param, transaction, commandTimeout);
//            var pageData = new PageData<T>()
//            {
//                Data = multi.Read<T>(),
//                DataCount = multi.ReadSingle<long>(),
//                PageIndex = pcp.PageIndex,
//                PageSize = pcp.PageSize
//            };
//            pageData.PageCount = (int)Math.Ceiling(pageData.DataCount * 1.0 / pageData.PageSize);
//            return pageData;
//        }
//        #endregion

//        #region Statistics
//        /// <summary>
//        /// Count统计
//        /// </summary>
//        /// <param name="sqlWhere">1=1,省略where</param>
//        /// <param name="param"></param>
//        /// <returns></returns>
//        public static async Task<int> CountAsync<T>(this IDbConnection connection, string sqlWhere, object param = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
//        {
//            var type = typeof(T);
//            var builder = new StringBuilder($"SELECT COUNT(0) FROM [{GetTableName(type)}]");
//            if (!string.IsNullOrWhiteSpace(sqlWhere))
//                builder.Append($" where {sqlWhere};");
//            return await connection.ExecuteScalarAsync<int>(builder.ToString(), param, transaction, commandTimeout);
//        }

//        /// <summary>
//        /// Count统计
//        /// </summary>
//        /// <param name="whereObj">使用 匿名类、字典(<see cref="Dictionary{TKey, TValue}"/>[键为string,值为object]、<see cref="Hashtable"/>)、自定义类</param>
//        /// <returns></returns>
//        public static async Task<int> CountAsync<T>(this IDbConnection connection, object whereObj = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
//        {
//            var type = typeof(T);
//            var builder = new StringBuilder($"SELECT COUNT(0) FROM [{GetTableName(type)}]");
//            var dictPar = ToDictionary(whereObj);
//            if (dictPar != null && dictPar.Any())
//                builder.Append(BuildWhereSql(dictPar, dictPar.Keys));
//            return await connection.ExecuteScalarAsync<int>(builder.ToString(), whereObj, transaction, commandTimeout);
//        }

//        /// <summary>
//        /// Sum统计
//        /// </summary>
//        /// <typeparam name="T">返回类型</typeparam>
//        /// <param name="field">需要sum的字段</param>
//        /// <param name="sqlWhere">1=1,省略where</param>
//        /// <param name="param"></param>
//        /// <returns></returns>
//        public static async Task<TResult> SumAsync<T, TResult>(this IDbConnection connection, string field, string sqlWhere, object param = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
//        {
//            var type = typeof(T);
//            var sql = new StringBuilder($"SELECT SUM({field}) FROM [{GetTableName(type)}]");
//            if (!string.IsNullOrWhiteSpace(sqlWhere))
//                sql.Append($" where {sqlWhere};");
//            return await connection.ExecuteScalarAsync<TResult>(sql.ToString(), param, transaction, commandTimeout);
//        }
//        #endregion

//        #region Transaction
//        /// <summary>
//        /// 批量数据事务提交
//        /// </summary>
//        /// <param name="strSqls">T-SQL语句</param>
//        /// <param name="param">参数</param>
//        /// <returns></returns>
//        public static async Task<int> ExecTransactionAsync(this IDbConnection connection, string strSqls, object param = null)
//        {
//            using (var conn = connection)
//            {
//                var s = 0;
//                var tran = conn.BeginTransaction();
//                try
//                {
//                    s = await conn.ExecuteAsync(strSqls, param, tran);
//                    tran.Commit();
//                }
//                catch (Exception ex)
//                {
//                    tran.Rollback();
//                    throw ex;
//                }
//                return s;
//            }
//        }
//        #endregion

//    }
//}
