using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ZeKi.Frame.DB;
using System.Data;
using ZeKi.Frame.Common;
using System.Linq;

namespace ZeKi.Frame.BLL.Interceptor
{
    /// <summary>
    /// 缓存 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CacheAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 缓存绝对过期时间（分钟）
        /// </summary>
        public int AbsoluteExpiration { get; set; } = 30;

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var _cache = context.ServiceProvider.GetService<ICaching>();
            //获取自定义缓存键
            var cacheKey = GetCustomCacheKey(context);
            //根据key获取相应的缓存值
            var cacheValue = _cache.Get(cacheKey);
            if (cacheValue != null)
            {
                //将当前获取到的缓存值，赋值给当前执行方法
                context.ReturnValue = cacheValue;
                return;
            }
            //去执行当前的方法
            await next(context);
            //存入缓存
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                _cache.Set(cacheKey, context.ReturnValue, TimeSpan.FromMinutes(AbsoluteExpiration));
            }
        }

        #region 辅助方法
        /// <summary>
        /// 获取缓存键
        /// </summary>
        /// <returns></returns>
        private string GetCustomCacheKey(AspectContext context)
        {
            var typeName = context.Implementation.GetType().Name;
            var methodName = context.ServiceMethod.Name;
            //获取参数列表，最多三个
            var methodArguments = context.Parameters.Select(GetArgumentValue).Take(3);
            string key = $"{typeName}:{methodName}:";
            foreach (var param in methodArguments)
            {
                key = $"{key}{param}:";
            }
            return key.TrimEnd(':');
        }

        /// <summary>
        /// object 转 string
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private string GetArgumentValue(object arg)
        {
            if (arg is DateTime || arg is DateTime?)
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");
            if (arg is string || arg is ValueType || arg is Nullable)
                return arg.ToString();
            if (arg != null)
            {
                if (arg.GetType().IsClass)
                {
                    return MD5Helper.MD5Encrypt16(Newtonsoft.Json.JsonConvert.SerializeObject(arg));
                }
            }
            return string.Empty;
        } 
        #endregion
    }
}
