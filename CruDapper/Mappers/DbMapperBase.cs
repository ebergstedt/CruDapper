using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruDapper.Mappers
{
    public abstract class DbMapperBase
    {
        Provider provider;
        public DbMapperBase(string ConnectionName, Provider provider)
        {
            this.ConnectionName = ConnectionName;
            this.provider = provider;
        }

        protected ConnectionBridge _connectionBridge;
        protected string _activeConnectionString;
        protected string _activeConnectionName;
        public string ConnectionName
        {
            set
            {
                _activeConnectionName = value;
                _activeConnectionString = ConfigurationManager.ConnectionStrings[_activeConnectionName].ConnectionString;

                _connectionBridge = new ConnectionBridge(provider, _activeConnectionString);
            }

            get { return _activeConnectionName; }
        }
    }
}
