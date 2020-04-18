using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ZeKi.Frame.Common
{
    /// <summary>
    /// 通用Http发送请求,此为示例
    /// </summary>
    public class CurrencyClient : ICurrencyClient
    {
        private readonly HttpClient _client;

        //HttpClient 这里只能使用构造函数注入
        public CurrencyClient(HttpClient httpClient)
        {
            //httpClient.BaseAddress = new Uri("https://www.baidu.com/");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-zzq");
            _client = httpClient;
        }

        /// <summary>
        /// get请求
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAsync(string url)
        {
            var response = await _client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// post请求,根据修改参数返回值
        /// </summary>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, string postData = "")
        {
            using HttpContent httpContent = new StringContent(postData, Encoding.UTF8);
            var response = await _client.PostAsync(url, httpContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
