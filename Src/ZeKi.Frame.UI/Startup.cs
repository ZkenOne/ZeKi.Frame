using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DependencyInjection;
using AspectCore.Extensions.Autofac;
using AspectCore.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using StackExchange.Profiling;
using ZeKi.Frame.Common;
using ZeKi.Frame.IBLL;
using ZeKi.Frame.Model.Config;
using ZeKi.Frame.UI.Filters;
using ZeKi.Frame.UI.Handler;
using ZeKi.Frame.UI.Middleware;

namespace ZeKi.Frame.UI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCacheSetup();
            //services.AddRedisCacheSetup();

            //加入控制器替换规则
            //services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            services.AddControllers((mvcOption) =>
            {
                //AddService需要在DI容器注册(Add则不需要,所以不能使用DI)
                mvcOption.Filters.AddService<GlobalErrorFilterAttribute>();
                mvcOption.Filters.AddService<ProfilingFilterAttribute>();
                //mvcOption.Filters.AddService<ExampleFilterAttribute>();
            });

            //注册CurrencyClient 且 添加一个出站请求中间件
            services.AddHttpClient<ICurrencyClient, CurrencyClient>().AddHttpMessageHandler<GlobalHttpHandler>();

            ////注册配置动态代理(AspectCore)[AutoFac和AspectCore共存时 在ConfigureContainer配置]
            //services.ConfigureDynamicProxy();

            //获取配置,方便其它地方注入使用
            services.Configure<DBConfig>(_configuration.GetSection("DBConfig"));
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            #region 配置AspectCore
            //var serviceContext = new ServiceContext();
            //builder.Populate(serviceContext);
            //var configuration = serviceContext.Configuration;
            ////调用RegisterDynamicProxy扩展方法在Autofac中注册动态代理服务和动态代理配置
            //builder.RegisterDynamicProxy(configuration, config =>
            //{
            //    //全局注册后不需要在方法上贴特性
            //    //config.Interceptors.AddTyped<MethodExecuteLoggerInterceptor>(Predicates.ForService("*Service"));
            //});

            //调用RegisterDynamicProxy扩展方法在Autofac中注册动态代理服务
            //不配置全局需要 手动在方法上打特性
            builder.RegisterDynamicProxy();
            #endregion

            // Register your own things directly with Autofac, like:
            builder.RegisterModule(new AutofacModuleRegister(_configuration));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //, IHost host
            //有时在系统初始化完成，接口和相应类注册完毕后想读取某个接口进行自定义初始化构
            //比如初始化自定义工厂，加载外部DLL,在不知外部类情况下进行初始化构建自己的服务
            //using (var container = host.Services.CreateScope())
            //{
            //    var sysUserInfoBLL = container.ServiceProvider.GetService<ISysUserInfoBLL>();
            //}

            //注册自定义中间件
            app.UseIPIntercept();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
