using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;
using ZeKi.Frame.BLL;
using ZeKi.Frame.Common;
using ZeKi.Frame.DAL;
using ZeKi.Frame.IBLL;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.Model;
using ZeKi.Frame.UI.Filters;
using ZeKi.Frame.UI.Handler;
using ZeKi.Frame.UI.Middleware;

namespace ZeKi.Frame.UI
{
    public class AutofacModuleRegister : Autofac.Module
    {
        //重写Autofac管道Load方法，在这里注册注入
        protected override void Load(ContainerBuilder builder)
        {
            //获取所有控制器类型并使用属性注入
            var controllerBaseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(typeof(Program).Assembly).Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType).InstancePerDependency().PropertiesAutowired();

            //过滤器注册并使用属性注入(普通类也如此)
            builder.RegisterType<ExampleFilterAttribute>().AsSelf().InstancePerDependency().PropertiesAutowired();
            builder.RegisterType<ProfilingFilterAttribute>().AsSelf().InstancePerDependency().PropertiesAutowired();
            builder.RegisterType<GlobalErrorFilterAttribute>().AsSelf().InstancePerDependency().PropertiesAutowired();

            //注册httpclient出站请求中间件
            builder.RegisterType<GlobalHttpHandler>().AsSelf().InstancePerDependency().PropertiesAutowired();

            //不注册也可以
            //builder.RegisterGeneric(typeof(BaseDAL<>)).As(typeof(IBaseDAL<>)).InstancePerDependency().PropertiesAutowired();
            //builder.RegisterGeneric(typeof(BaseBLL<>)).As(typeof(IBaseBLL<>)).InstancePerDependency().PropertiesAutowired();

            //ZeKi.Frame.BLL.dll/ZeKi.Frame.DAL.dll中的所有类注册给它的全部实现接口，并且把实现类中的属性也进行注册
            Assembly bllAsmService = Assembly.Load(AppConfig.BLLFullName);
            Assembly dalAsmService = Assembly.Load(AppConfig.DALFullName);
            //不是抽象类 并且 公开 并且 是class 并且 (后缀为BLL 或者 后缀为DAL)
            builder.RegisterAssemblyTypes(bllAsmService, dalAsmService).Where(t => !t.IsAbstract && t.IsPublic && t.IsClass && (t.Name.EndsWith("BLL") || t.Name.EndsWith("DAL")))
                .AsImplementedInterfaces().InstancePerDependency().PropertiesAutowired();  //允许属性注入

            //注册数据库连接对象(相同请求共用一个连接实例)
            builder.Register(componentContext =>
            {
                IDbConnection _conn = null;
                switch (AppConfig.DBType)
                {
                    case DBEnums.DBType.MSSQL:
                        _conn = new SqlConnection(AppConfig.SqlConnStr);
                        break;
                    case DBEnums.DBType.MYSQL:

                        break;
                }
                if (_conn == null)
                    throw new NotImplementedException($"未实现该数据库");
                if (MiniProfiler.Current != null) //MiniProfiler初始化
                    _conn = new StackExchange.Profiling.Data.ProfiledDbConnection((DbConnection)_conn, MiniProfiler.Current);
                if (_conn.State == ConnectionState.Closed)
                    _conn.Open();
                return _conn;
            }).PropertiesAutowired().InstancePerLifetimeScope();
            //.OnRelease(conn =>
            //{
            //    //释放时调用,有此方法时框架不会自动调用Dispose方法(默认会调用,没有其他特殊操作不需要写OnRelease方法)
            //})

        }
    }
}

////AutoFac的几种常见生命周期
////1. InstancePerDependency：每次请求 Resovle都返回一个新对象。InstancePerDependency()【这也是默认的创建实例的方式。】
////2. SingleInstance： 单例，只有在第一次请求的时候创建 。SingleInstance()
////3. InstancePerRequest：ASP.Net MVC 专用，每次http请求内一个对象（也可以理解为一个方法内）。InstancePerRequest() 和 CallContext神似(.net Core不一样)
////4. InstancePerLifetimeScope：在一个生命周期域中，每一个依赖或调用创建一个单一的共享的实例，且每一个不同的生命周期域，实例是唯一的，不共享的。(.net Core使用这种)