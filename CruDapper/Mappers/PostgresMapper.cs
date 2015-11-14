using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using MoreLinq;
using Npgsql;
using CruDapper.Helpers;

namespace CruDapper.Mappers
{
    public class PostgresMapper : DbMapperBase, IDbMapper
    {
        Provider _provider = Provider.Postgres;
        public PostgresMapper(string ConnectionName) : base(ConnectionName, Provider.Postgres)
        {

        }

        public IEnumerable<T> GetAll<T>()
        {
            var tableName = ReflectionHelper.GetTableName(typeof(T), _provider);
            var query = string.Format("SELECT * FROM {0}", tableName);
            return _connectionBridge.Query<T>(query);
        }
        
        public T GetByPrimaryKey<T>(object primaryKeyValue)
        {
            return GetByColumn<T>(ReflectionHelper.GetPrimaryKeyName<T>(), primaryKeyValue)
                .FirstOrDefault();
        }

        public T Get<T>(int id) where T : IDapperable
        {
            var query = QueryHelper.GetQuery<T>(id, _provider);
            return _connectionBridge.Query<T>(query.ToString()).SingleOrDefault();
        }

        public IEnumerable<T> GetByColumn<T>(string column, object value)
        {
            return GetByColumn<T>(new WhereArgument
            {
                Key = column,
                Value = value,
                Operator = Operator.Equals,
                Not = false
            });
        }

        public IEnumerable<T> GetByColumn<T>(WhereArgument whereArgument)
        {
            return GetByColumns<T>(new List<WhereArgument>
            {
                whereArgument
            });
        }

        public IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArguments)
        {
            var tableName = ReflectionHelper.GetTableName(typeof(T), _provider);
            var parameters = new DynamicParameters();

            var query = new StringBuilder();
            query.AppendFormat(@"
                SELECT
                    a.*
                FROM
                    {0} AS a
                WHERE
                    1 = 1
            ", tableName);

            QueryHelper.AddWhereArgumentsToQuery(ref query, parameters, whereArguments, typeof(T));

            return _connectionBridge.Query<T>(query.ToString(), parameters);
        }

        public T GetNondeleted<T>(int id) where T : IDapperable, IDeletable
        {
            var query = QueryHelper.GetQuery<T>(id, _provider);
            query.Append(" AND IsDeleted = false ");
            return _connectionBridge.Query<T>(query.ToString()).SingleOrDefault();
        }

        //http://stackoverflow.com/questions/29615445/dapper-bulk-insert-returning-serial-ids/29663184#29663184
        public IEnumerable<T> InsertMultipleIdentifiable<T>(IEnumerable<object> entities)
        {
            if (!entities.Any())
                return null;

            InterfaceHelper.AssignInterfaceData(ref entities);
            InterfaceHelper.ValidateList(ref entities);

            var guidList = new List<Guid>();
            foreach (var entity in entities)
            {
                var identifiable = entity as IIdentifiable;
                if (identifiable == null || identifiable.RowGuid == default(Guid))
                    throw new ArgumentException("Does not implement IIDentifiable");
                guidList.Add(identifiable.RowGuid);
            }

            var tableName = ReflectionHelper.GetTableName(entities.First().GetType(), _provider);
            var editableFields = ReflectionHelper.GetEditableFields(entities.First().GetType());

            var query = new StringBuilder();
            query.AppendFormat("INSERT INTO {0} (", tableName);

            foreach (var editableField in editableFields)
            {
                query.AppendFormat("{0}, ", editableField.Name);
            }

            query.Length -= 2;
            query.Append(") VALUES (");

            foreach (var editableField in editableFields)
            {
                query.AppendFormat("@{0}, ", editableField.Name);
            }

            query.Length -= 2;
            query.Append(") ");

            _connectionBridge.Execute(query.ToString(), entities);

            var resultList = new List<T>();
            foreach (var batch in guidList.Batch(2000)) 
            {
                var tbatch = batch.ToList<Guid>();
                resultList.AddRange(_connectionBridge.Query<T>(string.Format("SELECT * FROM {0} WHERE RowGuid = ANY(@tbatch)", tableName), new
                {
                    tbatch
                }));
            }

            return resultList;
        }

        public void InsertMultiple(IEnumerable<object> entities)
        {
            if (!entities.Any())
                return;

            InterfaceHelper.AssignInterfaceData(ref entities);
            InterfaceHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(entities.First().GetType(), _provider);
            var keys = ReflectionHelper.GetKeyFields(entities.First().GetType());
            var editableFields = ReflectionHelper.GetEditableFields(entities.First().GetType());

            var query = new StringBuilder();
            query.AppendFormat("INSERT INTO {0} (", tableName);

            foreach (var editableField in editableFields)
            {
                query.AppendFormat("{0}, ", editableField.Name);
            }

            query.Length -= 2;
            query.Append(") VALUES (");

            foreach (var editableField in editableFields)
            {
                query.AppendFormat("@{0}, ", editableField.Name);
            }

            query.Length -= 2;
            query.Append(")");
            query.Append(" RETURNING Id; "); 

            if (entities.Count() == 1 && keys.Count() == 1 && keys.First().PropertyType != typeof(Guid))
            {
                var result = _connectionBridge.Query<int?>(query.ToString(), entities.Single()).SingleOrDefault();

                if (result != null)
                    ReflectionHelper.SetFieldValue(entities.Single(), keys.First().Name, result);
            }
            else
            {
                _connectionBridge.Execute(query.ToString(), entities);
            }
        }

        public void UpdateMultiple(IEnumerable<object> entities)
        {
            if (!entities.Any())
                return;

            InterfaceHelper.AssignInterfaceData(ref entities);
            InterfaceHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(entities.First().GetType(), _provider);
            var keys = ReflectionHelper.GetKeyFields(entities.First().GetType());
            var editableFields = ReflectionHelper.GetEditableFields(entities.First().GetType());

            var query = new StringBuilder();

            query.AppendFormat("UPDATE {0} SET ", tableName);

            foreach (var editableField in editableFields)
            {
                query.AppendFormat("{0} = @{0}, ", editableField.Name);
            }

            query.Length -= 2;
            query.Append(" WHERE ");

            foreach (var key in keys)
            {
                query.AppendFormat("{0} = @{0} AND ", key.Name);
            }

            query.Length -= 5;
            query.Append(";");

            _connectionBridge.Execute(query.ToString(), entities);
        }

        public void DeleteMultiple(IEnumerable<object> entities)
        {
            if (!entities.Any())
                return;

            InterfaceHelper.AssignInterfaceData(ref entities);
            InterfaceHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(entities.First().GetType(), _provider);
            var keys = ReflectionHelper.GetKeyFields(entities.First().GetType());

            if (!keys.Any())
                return;

            var query = new StringBuilder();

            query.AppendFormat("DELETE FROM {0} WHERE ", tableName);

            foreach (var key in keys)
            {
                query.AppendFormat("{0} = @{0} AND ", key.Name);
            }

            query.Length -= 5;
            query.Append(";");

            _connectionBridge.Execute(query.ToString(), entities);
        }
    }
}
