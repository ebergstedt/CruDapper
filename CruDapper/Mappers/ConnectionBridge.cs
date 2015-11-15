using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;
using CruDapper.Infrastructure;
using Dapper;
using Npgsql;

namespace CruDapper.Mappers
{
    public class ConnectionBridge : IDapperConnectable
    {
        private readonly string _connectionString;
        private readonly Provider _provider;

        public ConnectionBridge(Provider provider, string connectionString)
        {
            this._provider = provider;
            this._connectionString = connectionString;
        }

        public IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null)
        {
            IEnumerable<dynamic> result;
            using (var scope = new TransactionScope())
            {
                using (var connection = GetDbConnection())
                {
                    connection.Open();
                    result = connection.Query(sqlQuery, parameters);
                }
                scope.Complete();
            }
            return result;
        }

        public IEnumerable<T> Query<T>(string sqlQuery, object parameters = null)
        {
            IEnumerable<T> result;
            using (var scope = new TransactionScope())
            {
                using (var connection = GetDbConnection())
                {
                    connection.Open();
                    result = connection.Query<T>(sqlQuery, parameters);
                }
                scope.Complete();
            }
            return result;
        }

        public SqlMapper.GridReader QueryMultiple(string sqlQuery, object parameters = null)
        {
            SqlMapper.GridReader result;
            using (var scope = new TransactionScope())
            {
                using (var connection = GetDbConnection())
                {
                    result = connection.QueryMultiple(sqlQuery, parameters);
                }
                scope.Complete();
            }
            return result;
        }

        public void Execute(string sqlQuery, object parameters = null)
        {
            using (var scope = new TransactionScope())
            {
                using (var connection = GetDbConnection())
                {
                    connection.Open();
                    connection.Execute(sqlQuery, parameters);
                }
                scope.Complete();
            }
        }

        private DbConnection GetDbConnection()
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