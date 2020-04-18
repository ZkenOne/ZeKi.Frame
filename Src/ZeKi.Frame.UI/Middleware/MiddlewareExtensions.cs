using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace ZeKi.Frame.UI.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseIPIntercept(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IPInterceptMiddleware>();
        }
    }
}
