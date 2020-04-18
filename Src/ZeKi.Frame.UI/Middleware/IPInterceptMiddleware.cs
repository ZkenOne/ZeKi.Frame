using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ZeKi.Frame.UI.Middleware
{
    /// <summary>
    /// ip拦截中间件
    /// </summary>
    public class IPInterceptMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogger<IPInterceptMiddleware> _logger;
        //private readonly IWebHostEnvironment _env;

        //中间件 这里只能使用构造函数注入
        public IPInterceptMiddleware(RequestDelegate next/*, ILogger<IPInterceptMiddleware> logger, IWebHostEnvironment env*/)
        {
            _next = next;
            //_logger = logger;
            //_env = env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Connection.RemoteIpAddress.ToString() == "192.168.1.32")
                await httpContext.Response.WriteAsync("此IP不能访问");
            else
                await _next.Invoke(httpContext);
        }
    }
}
