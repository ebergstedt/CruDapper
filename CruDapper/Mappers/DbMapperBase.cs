using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using CruDapper.Infrastructure;
using Dapper;

namespace CruDapper.Mappers
{
    public abstract class DbMapperBase : IDapperConnectable
    {
        protected string ActiveConnectionName;

        protected ConnectionBridge ConnectionBridge;
        private readonly Provider _provider;

        protected DbMapperBase(string connectionName, Provider provider, int? globalCommandTimeout)
        {
            this._provider = provider;
            this.ConnectionName = connectionName;
            this.GlobalCommandTimeout = globalCommandTimeout;
        }

        private int? _globalCommandTimeout;
        public int? GlobalCommandTimeout
        {
            get { return _globalCommandTimeout; }
            set
            {
                _globalCommandTimeout = value;
                ConnectionBridge.GlobalCommandTimeout = _globalCommandTimeout;
            }
        }

        public string ConnectionName
        {
            set
            {
                ActiveConnectionName = value;
                ConnectionBridge = new ConnectionBridge(_provider, ConfigurationManager.ConnectionStrings[ActiveConnectionName].ConnectionString);
            }

            get { return ActiveConnectionName; }
        }

        /// <summary>
        /// Use to call Dapper methods directly from your service
        /// </summary>
        public DbConnection DbConnection
        {
            get { return ConnectionBridge.GetDbConnection(); }            
        }

        public IEnumerable<T> Query<T>(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {            
            return ConnectionBridge.Query<T>(sqlQuery, parameters, commandTimeout);
        }

        public IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return ConnectionBridge.QueryDynamic(sqlQuery, parameters, commandTimeout);
        }

        public SqlMapper.GridReader QueryMultiple(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return ConnectionBridge.QueryMultiple(sqlQuery, parameters, commandTimeout);
        }

        public void Execute(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            ConnectionBridge.Execute(sqlQuery, parameters, commandTimeout);
        }
    }
}