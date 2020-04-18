using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZeKi.Frame.Common
{
    /// <summary>
    /// 通用Http发送请求,此为示例
    /// </summary>
    public interface ICurrencyClient
    {
        /// <summary>
        /// get请求
        /// </summary>
        /// <returns></returns>
        Task<string> GetAsync(string url);

        /// <summary>
        /// post请求,根据修改参数返回值
        /// </summary>
        /// <returns></returns>
        Task<string> PostAsync(string url, string postData = "");
    }
}
