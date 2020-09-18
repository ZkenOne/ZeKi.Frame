using Microsoft.Extensions.Caching.Memory;
using System;

namespace ZeKi.Frame.Common
{
    /// <summary>
    /// 实例化缓存接口ICaching
    /// </summary>
    public class MemoryCaching : ICaching
    {
        //引用Microsoft.Extensions.Caching.Memory;这个和.net 还是不一样，没有了Httpruntime了
        public IMemoryCache MemoryCache { set; get; }

        public object Get(string cacheKey)
        {
            return MemoryCache.Get(cacheKey);
        }

        public TItem Get<TItem>(string cacheKey)
        {
            return MemoryCache.Get<TItem>(cacheKey);
        }

        public void Set(string cacheKey, object cacheValue, TimeSpan timeSpan)
        {
            MemoryCache.Set(cacheKey, cacheValue, timeSpan);
        }
    }

}
