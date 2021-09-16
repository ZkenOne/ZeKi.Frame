using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ZeKi.Frame.DB;
using System.Data;

namespace ZeKi.Frame.BLL.Interceptor
{
    /// <summary>
    /// 事务拦截器特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class TransactionAttribute : AbstractInterceptorAttribute
    {
        private readonly IsolationLevel isolationLevel;
        public TransactionAttribute(IsolationLevel _isolationLevel = IsolationLevel.ReadCommitted)
        {
            isolationLevel = _isolationLevel;
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            //获取数据库上下文(同一请求共用一个实例)
            var dbContext = context.ServiceProvider.GetService<DbContext>();
            //有正在运行的事务则取最开始的,不再开启
            if (dbContext.IsRunningTran)
            {
                await next(context);  //执行原始方法
                return;
            }
            try
            {
                dbContext.BeginTransaction(isolationLevel);
                await next(context);  //执行原始方法
                dbContext.CommitTransaction();
            }
            catch (Exception)
            {
                dbContext.RollbackTransaction();
                throw;
            }
        }
    }
}
