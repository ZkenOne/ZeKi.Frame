//using Blog.Core.Common;
//using Microsoft.Extensions.DependencyInjection;
//using StackExchange.Redis;
//using System;

//namespace ZeKi.Frame.UI
//{
//    /// <summary>
//    /// Redis缓存 启动服务
//    /// </summary>
//    public static class RedisCacheSetup
//    {
//        public static void AddRedisCacheSetup(this IServiceCollection services)
//        {
//            if (services == null) throw new ArgumentNullException(nameof(services));

//            services.AddTransient<IRedisAssist, RedisAssist>();

//            services.AddSingleton<ConnectionMultiplexer>(sp =>
//            {
//                //获取连接字符串
//                string redisConfiguration = AppConfigs.GetValue("RedisConnectionString");
//                var configuration = ConfigurationOptions.Parse(redisConfiguration, true);

//                configuration.ResolveDns = true;

//                return ConnectionMultiplexer.Connect(configuration);
//            });

//        }
//    }
//}
