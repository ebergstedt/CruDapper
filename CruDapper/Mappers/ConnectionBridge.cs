using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SqlClient;
using Npgsql;
using System.Transactions;
using Dapper;

namespace CruDapper.Mappers
{
    public class ConnectionBridge
    {
        private Provider provider;
        private string connectionString;

        public ConnectionBridge(Provider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }

        private DbConnection GetDbConnection()
        {
            switch (provider)
            {
                case Provider.MsSql:
                    return new SqlConnection(connectionString);
                case Provider.Postgres:
                    return new NpgsqlConnection(connectionString);
            }

            throw new ArgumentException("Provider not implemented");
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

        public void Execute(string sqlQuery, object parameters)
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
    }
}
