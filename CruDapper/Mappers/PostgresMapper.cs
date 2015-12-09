using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CruDapper.Code;
using CruDapper.Helpers;
using CruDapper.Infrastructure;
using Dapper;
using MoreLinq;

namespace CruDapper.Mappers
{
    public class PostgresMapper : DbMapperBase, IDbMapper
    {
        private readonly Provider _provider = Provider.Postgres;

        public PostgresMapper(string connectionName, int? globalCommandTimeout = null) : base(connectionName, Provider.Postgres, globalCommandTimeout)
        {
        }

        public IEnumerable<T> GetAll<T>(bool getDeleted = false)
        {
            var tableName = ReflectionHelper.GetTableName(typeof (T));
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM {0}", tableName);
            if (!getDeleted && InterfaceHelper.VerifyIDeletable<T>())
            {
                query.AppendFormat(" WHERE {0} ", QueryHelper.GetIsDeletedSQL(_provider));
            }
            return ConnectionBridge.Query<T>(query.ToString());
        }

        public T GetByPrimaryKey<T>(object primaryKeyValue, bool getDeleted = false)
        {
            return GetByColumn<T>(ReflectionHelper.GetPrimaryKeyName(typeof(T)), primaryKeyValue, getDeleted)
                .FirstOrDefault();
        }

        public T Get<T>(int id, bool getDeleted = false) where T : IDapperable
        {
            var query = QueryHelper.GetQuery<T>(id, _provider, getDeleted);
            return ConnectionBridge.Query<T>(query.ToString()).SingleOrDefault();
        }

        public IEnumerable<T> GetByColumn<T>(string column, object value, bool getDeleted = false)
        {
            return GetByColumn<T>(new WhereArgument
            {
                Key = column,
                Value = value,
                Operator = Operator.Equals,
                Not = false
            }, getDeleted);
        }

        public IEnumerable<T> GetByColumn<T>(WhereArgument whereArgument, bool getDeleted = false)
        {
            return GetByColumns<T>(new List<WhereArgument>
            {
                whereArgument
            }, getDeleted);
        }

        public IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArguments, bool getDeleted = false)
        {
            var tableName = ReflectionHelper.GetTableName(typeof (T));
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

            if (!getDeleted)
            {
                query.AppendFormat(" AND {0} ", QueryHelper.GetIsDeletedSQL(_provider));
            }

            QueryHelper.AddWhereArgumentsToQuery(ref query, parameters, whereArguments, typeof (T));

            return ConnectionBridge.Query<T>(query.ToString(), parameters);
        }

        public IEnumerable<T> GetPaginated<T>(string sortColumn, int pageSize = 10, int currentPage = 1,
            OrderBy sortingDirection = OrderBy.Asc)
        {
            throw new NotImplementedException();
        }


        //http://stackoverflow.com/questions/29615445/dapper-bulk-insert-returning-serial-ids/29663184#29663184
        public IEnumerable<T> InsertMultipleIdentifiable<T>(IEnumerable<T> entities)
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

            var tableName = ReflectionHelper.GetTableName(typeof(T));
            var editableFields = ReflectionHelper.GetEditableFields(typeof(T));

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

            ConnectionBridge.Execute(query.ToString(), entities);

            var resultList = new List<T>();
            foreach (var batch in guidList.Batch(2000))
            {
                var tbatch = batch.ToList();
                resultList.AddRange(
                    ConnectionBridge.Query<T>(
                        string.Format("SELECT * FROM {0} WHERE RowGuid = ANY(@tbatch)", tableName), new
                        {
                            tbatch
                        }));
            }

            return resultList;
        }

        public void InsertMultiple<T>(IEnumerable<T> entities)
        {
            if (!entities.Any())
                return;

            InterfaceHelper.AssignInterfaceData(ref entities);
            InterfaceHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(typeof(T));
            var keys = ReflectionHelper.GetKeyFields(typeof(T));
            var editableFields = ReflectionHelper.GetEditableFields(typeof(T));

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

            if (entities.Count() == 1 && keys.Count() == 1 && keys.First().PropertyType != typeof (Guid))
            {
                var result = ConnectionBridge.Query<int?>(query.ToString(), entities.Single()).SingleOrDefault();

                if (result != null)
                    ReflectionHelper.SetFieldValue(entities.Single(), keys.First().Name, result);
            }
            else
            {
                ConnectionBridge.Execute(query.ToString(), entities);
            }
        }

        public void UpdateMultiple<T>(IEnumerable<T> entities)
        {
            if (!entities.Any())
                return;

            InterfaceHelper.AssignInterfaceData(ref entities);
            InterfaceHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(typeof(T));
            var keys = ReflectionHelper.GetKeyFields(typeof(T));
            var editableFields = ReflectionHelper.GetEditableFields(typeof(T));

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

            ConnectionBridge.Execute(query.ToString(), entities);
        }

        public void DeleteMultiple<T>(IEnumerable<T> entities)
        {
            if (!entities.Any())
                return;

            InterfaceHelper.AssignInterfaceData(ref entities);
            InterfaceHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(typeof(T));
            var keys = ReflectionHelper.GetKeyFields(typeof(T));

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

            ConnectionBridge.Execute(query.ToString(), entities);
        }

        public void MergeMultiple<T>(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}