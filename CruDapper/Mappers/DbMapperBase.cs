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
            this.provider = provider;
            this.ConnectionName = ConnectionName;            
        }

        protected ConnectionBridge _connectionBridge;
        protected string _activeConnectionName;
        public string ConnectionName
        {
            set
            {
                _activeConnectionName = value;
                _connectionBridge = new ConnectionBridge(provider, ConfigurationManager.ConnectionStrings[_activeConnectionName].ConnectionString);
            }

            get { return _activeConnectionName; }
        }
    }
}
