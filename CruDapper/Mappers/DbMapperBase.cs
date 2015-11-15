using System.Collections.Generic;
using System.Configuration;
using CruDapper.Infrastructure;
using Dapper;

namespace CruDapper.Mappers
{
    public abstract class DbMapperBase : IDapperConnectable
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

        public IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null)
        {
            return ConnectionBridge.QueryDynamic(sqlQuery, parameters);
        }

        public IEnumerable<T> Query<T>(string sqlQuery, object parameters = null)
        {
            return ConnectionBridge.Query<T>(sqlQuery, parameters);
        }

        public SqlMapper.GridReader QueryMultiple(string sqlQuery, object parameters = null)
        {
            return ConnectionBridge.QueryMultiple(sqlQuery, parameters);
        }

        public void Execute(string sqlQuery, object parameters = null            )
        {
            ConnectionBridge.Execute(sqlQuery, parameters);
        }
    }
}