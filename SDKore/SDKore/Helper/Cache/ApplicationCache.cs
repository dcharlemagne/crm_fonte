using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SDKore.Helper.Cache
{
    public class ApplicationCache : ICacheManager
    {
        private System.Web.Caching.Cache CurrentContext
        {
            get
            {
                if (HttpRuntime.Cache == null)
                    throw new ApplicationException("Não encontrado o contexto.");
                return HttpRuntime.Cache;
            }
        }

        public void Add(string key, object value, int hours)
        {
            if (CurrentContext[key] == null)
                CurrentContext.Add(
                    key, 
                    value, 
                    null, 
                    DateTime.Now.Add(TimeSpan.FromHours(hours)), 
                    System.Web.Caching.Cache.NoSlidingExpiration, 
                    System.Web.Caching.CacheItemPriority.Normal, 
                    null);
        }

        public void Add(string key, object value, TimeSpan timeSpan)
        {
            if (CurrentContext[key] == null)
                CurrentContext.Add(
                    key,
                    value,
                    null,
                    DateTime.Now.Add(timeSpan),
                    System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.Normal,
                    null);
        }

        public void Update(string key, object value)
        {
            if (CurrentContext[key] != null)
                CurrentContext[key] = value;
        }

        public void Delete(string key)
        {
            if (CurrentContext[key] != null)
                CurrentContext.Remove(key);
        }

        public object Get(string key)
        {
            return CurrentContext[key] ?? null;
        }
    }
}
