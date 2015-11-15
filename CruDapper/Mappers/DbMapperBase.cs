using System.Configuration;
using CruDapper.Infrastructure;

namespace CruDapper.Mappers
{
    public abstract class DbMapperBase
    {
        protected string ActiveConnectionName;

        protected ConnectionBridge ConnectionBridge;
        private readonly Provider _provider;

        public DbMapperBase(string connectionName, Provider provider)
        {
            this._provider = provider;
            ConnectionName = connectionName;
        }

        public string ConnectionName
        {
            set
            {
                ActiveConnectionName = value;
                ConnectionBridge = new ConnectionBridge(_provider,
                    ConfigurationManager.ConnectionStrings[ActiveConnectionName].ConnectionString);
            }

            get { return ActiveConnectionName; }
        }
    }
}