using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace ZeKi.Frame.Common
{
    /*
        引用的dll版本和core项目版本一致(F12查看),则不会生成一堆相关dll
        <PackageReference Include="Microsoft.Extensions.Configuration" Version="3.1.0" />
        <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="3.1.0" />
        <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="3.1.0" />
        <PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" Version="3.1.0" />
    */
    /// <summary>
    /// ConfigurationManager
    /// </summary>
    internal class ConfigurationManager
    {
        private readonly static IConfiguration configuration;

        static ConfigurationManager()
        {
            //reloadOnChange:true 则文件改了之后重新获取是最新的
            configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               //可以添加多个json文件,也可以获取到数据
               .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();
        }

        /// <summary>
        /// 获取键转换为对象形式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Bind<T>(string key) where T : class, new()
        {
            var obj = new ServiceCollection()
                .AddOptions()
                .Configure<T>(configuration.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return obj;
        }

        /// <summary>
        /// 对象属性获取 User:Name,数组中某个属性获取 Users:0:Name
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValue<T>(string key)
        {
            return configuration.GetValue<T>(key);
        }

        /// <summary>
        /// 对象属性获取 User:Name,数组中某个属性获取 Users:0:Name
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            return GetValue<string>(key);
        }
    }
}
