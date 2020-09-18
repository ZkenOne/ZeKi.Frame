using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;

namespace ZeKi.Frame.UI.Filters
{
    /// <summary>
    /// sql执行记录过滤器
    /// </summary>
    public class ProfilingFilterAttribute : ActionFilterAttribute
    {
        public ILogger<ProfilingFilterAttribute> Logger { set; get; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var mp = MiniProfiler.StartNew();
            if (mp != null)
            {
                var ad = context.ActionDescriptor as ControllerActionDescriptor;
                var area = context.RouteData.DataTokens.TryGetValue("area", out object areaToken)
                    ? areaToken as string + "."
                    : null;
                using (mp.Step($"Controller: {area}{ad.RouteValues["controller"]}.{ad.RouteValues["action"]}"))
                {
                    //执行Action
                    await next();
                }
                await Task.Factory.StartNew(() =>
                {
                    AnalysisProfiler(mp);
                });
            }
        }

        private void AnalysisProfiler(MiniProfiler profiler)
        {
            if (profiler?.Root == null)
                return;
            var root = profiler.Root;
            if (!root.HasChildren)
                return;
            foreach (var chil in root.Children)
            {
                if (chil.CustomTimings == null || chil.CustomTimings.Count <= 0)
                    continue;
                //var timingsJson = chil.CustomTimingsJson;  //执行的sql 序列化之后的文本
                foreach (var customTiming in chil.CustomTimings)
                {
                    if (customTiming.Value == null || customTiming.Value.Count <= 0)
                        continue;
                    var i = 1;
                    var sbStr = new StringBuilder();
                    sbStr.AppendLine($"{chil.Name}");
                    sbStr.AppendLine();
                    foreach (var value in customTiming.Value)
                    {
                        //if (value.ExecuteType == "OpenAsync" || value.ExecuteType == "Open")
                        //    continue;
                        //value.Errored:true 表示 sql执行出错
                        
                        sbStr.AppendLine($"NO -》 {i++}");
                        sbStr.AppendLine($"ExecuteType -》 {value.ExecuteType}");
                        sbStr.AppendLine($"CommandString -》 {value.CommandString}");
                        sbStr.AppendLine($"DurationMilliseconds-》 {value.DurationMilliseconds}");
                        sbStr.AppendLine($"StartMilliseconds -》 {value.StartMilliseconds}");
                        sbStr.AppendLine($"StackTraceSnippet -》 {value.StackTraceSnippet}");
                        sbStr.AppendLine($"Errored -》 {value.Errored}");
                    }

                    //也可以将这些存入数据库,不记录日志
                    Logger.LogTrace(sbStr.ToString());
                    //debug
                    Trace.WriteLine(sbStr.ToString());
                }
            }

        }
    }
}
