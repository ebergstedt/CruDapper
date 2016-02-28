using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using CruDapper.Infrastructure;
using Dapper;
using Npgsql;
using Polly;

namespace CruDapper.Mappers
{
    public class ConnectionBridge : IDapperConnectable
    {
        private readonly string _connectionString;
        private readonly Provider _provider;

        public int? GlobalCommandTimeout;

        public ConnectionBridge(Provider provider, string connectionString)
        {
            this._provider = provider;
            this._connectionString = connectionString;
        }

        private void RetryExecute(Action action, int retryCount = 0)
        {
            Policy
             .Handle<Exception>()
             .Retry(retryCount)
             .Execute(action);
        }

        private T RetryExecuteAndCapture<T>(Func<T> action, int retryCount = 0)
        {
            PolicyResult<T> policyResult = Policy
             .Handle<Exception>()
             .Retry(retryCount)
             .ExecuteAndCapture<T>(action);

            if (policyResult.Outcome == OutcomeType.Failure)
            {
                throw policyResult.FinalException;
            }

            return policyResult.Result;
        }

        private async Task<T> RetryAsync<T>(Func<Task<T>> action, int retryCount = 0)
        {
            PolicyResult<T> policyResult = await Policy
             .Handle<Exception>()
             .Retry(retryCount)
             .ExecuteAndCaptureAsync<T>(action);

            if (policyResult.Outcome == OutcomeType.Failure)
            {
                throw policyResult.FinalException;
            }

            return policyResult.Result;
        }

        public IEnumerable<T> Query<T>(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return RetryExecuteAndCapture(() => _Query<T>(sqlQuery, parameters, commandTimeout), retryCount);
        }

        private IEnumerable<T> _Query<T>(string sqlQuery, object parameters, int? commandTimeout)
        {
            IEnumerable<T> result;
            using (var connection = GetDbConnection())
            using (var scope = new TransactionScope())
            {
                connection.Open();
                result = connection.Query<T>(sqlQuery, parameters, commandTimeout: GlobalCommandTimeout ?? commandTimeout);
                scope.Complete();
            }

            return result;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            IEnumerable<T> result;
            using (var connection = GetDbConnection())
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await connection.OpenAsync();
                result = await connection.QueryAsync<T>(sqlQuery, parameters, commandTimeout: GlobalCommandTimeout ?? commandTimeout);
                scope.Complete();
            }

            return result;
        }

        public IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return RetryExecuteAndCapture(() => _QueryDynamic(sqlQuery, parameters, commandTimeout), retryCount);
        }

        private IEnumerable<dynamic> _QueryDynamic(string sqlQuery, object parameters, int? commandTimeout)
        {
            IEnumerable<dynamic> result;
            using (var connection = GetDbConnection())
            using (var scope = new TransactionScope())
            {
                connection.Open();
                result = connection.Query(sqlQuery, parameters, commandTimeout: GlobalCommandTimeout ?? commandTimeout);
                scope.Complete();
            }

            return result;
        }

        public async Task<IEnumerable<dynamic>> QueryDynamicAsync(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            IEnumerable<dynamic> result;
            using (var connection = GetDbConnection())
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await connection.OpenAsync();
                result = await connection.QueryAsync<dynamic>(sqlQuery, parameters, commandTimeout: GlobalCommandTimeout ?? commandTimeout);
                scope.Complete();
            }
            return result;
        }

        public SqlMapper.GridReader QueryMultiple(DbConnection connection, string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return RetryExecuteAndCapture(() => _QueryMultiple(connection, sqlQuery, parameters, commandTimeout), retryCount);            
        }

        private SqlMapper.GridReader _QueryMultiple(DbConnection connection, string sqlQuery, object parameters, int? commandTimeout)
        {
            SqlMapper.GridReader result;
            using (var scope = new TransactionScope())
            {
                result = connection.QueryMultiple(sqlQuery, parameters, commandTimeout: GlobalCommandTimeout ?? commandTimeout);
                scope.Complete();
            }

            return result;
        }

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(DbConnection connection, string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            SqlMapper.GridReader result;
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await connection.QueryMultipleAsync(sqlQuery, parameters, commandTimeout: GlobalCommandTimeout ?? commandTimeout);
                scope.Complete();
            }
            return result;
        }

        public void Execute(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            RetryExecute(() => _Execute(sqlQuery, parameters, commandTimeout), retryCount);            
        }

        private void _Execute(string sqlQuery, object parameters, int? commandTimeout)
        {
            using (var connection = GetDbConnection())
            using (var scope = new TransactionScope())
            {
                connection.Open();
                connection.Execute(sqlQuery, parameters, commandTimeout: GlobalCommandTimeout ?? commandTimeout);
                scope.Complete();
            }
        }

        public async Task<int> ExecuteAsync(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            int result;
            using (var connection = GetDbConnection())
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await connection.OpenAsync();
                result = await connection.ExecuteAsync(sqlQuery, parameters, commandTimeout: GlobalCommandTimeout ?? commandTimeout);
                scope.Complete();
            }

            return result;
        }

        public DbConnection GetDbConnection()
        {
            switch (_provider)
            {
                case Provider.MsSql:
                    return new SqlConnection(_connectionString);
                case Provider.Postgres:
                    return new NpgsqlConnection(_connectionString);
            }

            throw new ArgumentException("Provider not implemented");
        }
    }
}