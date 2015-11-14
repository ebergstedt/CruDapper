using System;
using System.Collections.Generic;

namespace CruDapper.Services
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IDbMapper dbHelper;
        private readonly IDictionary<object, object> services;

        public ServiceFactory(IDbMapper dbHelper)
        {
            this.dbHelper = dbHelper;
            services = new Dictionary<object, object>();
        }

        public T Get<T>()
        {
            return Get<T>(dbHelper);
        }

        public T Get<T>(IDbMapper dbMapper)
        {
            if (services.ContainsKey(typeof (T)) == false)
            {
                services.Add(typeof (T), (T) Activator.CreateInstance(typeof (T), dbMapper));
            }

            return (T) services[typeof (T)];
        }
    }
}