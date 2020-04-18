using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ZeKi.Frame.UI.Handler
{
    public class GlobalHttpHandler : DelegatingHandler
    {
        public ILogger<GlobalHttpHandler> Logger { set; get; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //过滤处理请求
            if (request.RequestUri.AbsoluteUri.Contains("4399.com"))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("can't visite 4399.com")
                };
            }
            HttpResponseMessage httpResponseMessage = null;
            try
            {
                var postData = request.Content != null ? (await request.Content.ReadAsStringAsync()) : "";
                httpResponseMessage = await base.SendAsync(request, cancellationToken);
                Logger.LogTrace($"【{nameof(GlobalHttpHandler)}】\r\n请求地址:{request.RequestUri.AbsoluteUri}\r\n" +
                    $"Post参数:{postData}\r\n" +
                    $"返回内容:\r\n{await httpResponseMessage.Content.ReadAsStringAsync()}");
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"【{nameof(GlobalHttpHandler)}】请求出现错误\r\n" +
                    $"请求地址:{request.RequestUri.AbsoluteUri}\r\n");
                //报错返回空串
                return new HttpResponseMessage(httpResponseMessage.StatusCode)
                {
                    Content = new StringContent(string.Empty)
                };
            }

        }
    }
}
