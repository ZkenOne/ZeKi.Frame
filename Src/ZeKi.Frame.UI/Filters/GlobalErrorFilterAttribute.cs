using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ZeKi.Frame.UI.Filters
{
    /// <summary>
    /// 异常处理过滤器
    /// </summary>
    public class GlobalErrorFilterAttribute : ExceptionFilterAttribute
    {
        public ILogger<GlobalErrorFilterAttribute> Logger { set; get; }
        public IWebHostEnvironment WebHostEnvironment { set; get; }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            if (WebHostEnvironment.IsDevelopment())
                return base.OnExceptionAsync(context);

            context.ExceptionHandled = true;
            context.Result = new OkObjectResult(new { msg = "请求出现错误！！" });
            Logger.LogError(context.Exception, $"【{nameof(GlobalErrorFilterAttribute)}】未捕获异常:\r\n");

            return base.OnExceptionAsync(context);
        }

    }
}
