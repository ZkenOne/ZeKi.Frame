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
        private readonly ILogger<GlobalErrorFilterAttribute> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GlobalErrorFilterAttribute(ILogger<GlobalErrorFilterAttribute> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            if (_webHostEnvironment.IsDevelopment())
                return base.OnExceptionAsync(context);

            context.ExceptionHandled = true;
            context.Result = new OkObjectResult(new { msg = "请求出现错误！！" });
            _logger.LogError(context.Exception, $"【{nameof(GlobalErrorFilterAttribute)}】未捕获异常:\r\n");

            return base.OnExceptionAsync(context);
        }

    }
}
