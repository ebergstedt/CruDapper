using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CruDapper.Infrastructure;
using Dapper;
using Polly;
using System.Data.SqlClient;

namespace CruDapper.Mappers
{
    public abstract class DbMapperBase : IDapperConnectable
    {        
        protected ConnectionBridge ConnectionBridge;
        private readonly Provider _provider;

        protected DbMapperBase(string connectionName, Provider provider, int? globalCommandTimeout)
        {
            this._provider = provider;
            this.ActiveConnectionName = connectionName;
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

        protected string _activeConnectionName;
        public string ActiveConnectionName
        {
            set
            {
                _activeConnectionName = value;
                ConnectionBridge = new ConnectionBridge(_provider, ConfigurationManager.ConnectionStrings[ActiveConnectionName].ConnectionString);
            }

            get
            {
                return _activeConnectionName;
            }
        }

        /// <summary>
        /// Use to call Dapper methods directly from your service
        /// </summary>
        public DbConnection ActiveDbConnection
        {
            get { return ConnectionBridge.GetDbConnection(); }            
        }

        public IEnumerable<T> Query<T>(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return ConnectionBridge.Query<T>(sqlQuery, parameters, commandTimeout, retryCount);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return ConnectionBridge.QueryAsync<T>(sqlQuery, parameters, commandTimeout, retryCount);
        }

        public IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return ConnectionBridge.QueryDynamic(sqlQuery, parameters, commandTimeout, retryCount);
        }

        public Task<IEnumerable<dynamic>> QueryDynamicAsync(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return ConnectionBridge.QueryDynamicAsync(sqlQuery, parameters, commandTimeout, retryCount);
        }

        public SqlMapper.GridReader QueryMultiple(DbConnection connection, string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return ConnectionBridge.QueryMultiple(connection, sqlQuery, parameters, commandTimeout, retryCount);
        }

        public Task<SqlMapper.GridReader> QueryMultipleAsync(DbConnection connection, string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return ConnectionBridge.QueryMultipleAsync(connection, sqlQuery, parameters, commandTimeout, retryCount);
        }

        public void Execute(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            ConnectionBridge.Execute(sqlQuery, parameters, commandTimeout, retryCount);
        }

        public Task<int> ExecuteAsync(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return ConnectionBridge.ExecuteAsync(sqlQuery, parameters, commandTimeout, retryCount);
        }
    }
}