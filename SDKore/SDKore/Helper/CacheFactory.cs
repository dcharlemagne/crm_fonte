using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace SDKore.Helper
{
    public class CacheFactory
    {
        public static readonly CacheFactory Instance = new CacheFactory();
        UnityContainer _container = null;
        public UnityContainer Container
        {
            get
            {
                if (_container == null)
                    _container = new UnityContainer();
                return _container;
            }
        }

        public CacheFactory()
        {
            Container.RegisterType<Cache.ICacheManager, Cache.ApplicationCache>(new ContainerControlledLifetimeManager());
        }
    }
}
