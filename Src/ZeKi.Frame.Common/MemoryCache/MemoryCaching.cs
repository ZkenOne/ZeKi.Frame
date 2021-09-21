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
        private readonly IMemoryCache _memoryCache;

        public MemoryCaching(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public object Get(string cacheKey)
        {
            return _memoryCache.Get(cacheKey);
        }

        public TItem Get<TItem>(string cacheKey)
        {
            return _memoryCache.Get<TItem>(cacheKey);
        }

        public void Set(string cacheKey, object cacheValue, TimeSpan timeSpan)
        {
            _memoryCache.Set(cacheKey, cacheValue, timeSpan);
        }
    }

}
