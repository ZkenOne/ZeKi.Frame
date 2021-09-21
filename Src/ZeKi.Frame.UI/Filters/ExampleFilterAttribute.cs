using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using ZeKi.Frame.IBLL;

namespace ZeKi.Frame.UI.Filters
{
    //example DI
    public class ExampleFilterAttribute : ActionFilterAttribute
    {
        private readonly ISysUserInfoBLL _sysUserInfoBLL;

        public ExampleFilterAttribute(ISysUserInfoBLL sysUserInfoBLL)
        {
            _sysUserInfoBLL = sysUserInfoBLL;
        }

        //推荐这种写法
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //action 前执行,相当于OnActionExecuting

            var name = _sysUserInfoBLL.GetTName();
            Console.WriteLine(name);

            await next();

            //action 后执行,相当于OnActionExecuted

        }
    }
}
