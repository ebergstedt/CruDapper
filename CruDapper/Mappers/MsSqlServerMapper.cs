using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CruDapper.Helpers;
using CruDapper.Infrastructure;
using Dapper;
using MoreLinq;

namespace CruDapper.Mappers
{
    public class MsSqlServerMapper : DbMapperBase, IDbMapper
    {
        private readonly Provider _provider = Provider.MsSql;

        public MsSqlServerMapper(string connectionName, int? globalCommandTimeout = null) : base(connectionName, Provider.MsSql, globalCommandTimeout)
        {
        }

        public IEnumerable<T> GetAll<T>(bool getDeleted = false)
        {
            var tableName = ReflectionHelper.GetTableName(typeof(T), _provider);
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
            return GetByColumn<T>(ReflectionHelper.GetPrimaryKeyName<T>(), primaryKeyValue, getDeleted)
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

            if (!getDeleted && InterfaceHelper.VerifyIDeletable<T>())
            {
                query.AppendFormat(" AND {0} ", QueryHelper.GetIsDeletedSQL(_provider));
            }

            QueryHelper.AddWhereArgumentsToQuery(ref query, parameters, whereArguments, typeof(T));

            return ConnectionBridge.Query<T>(query.ToString(), parameters);
        }

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

            var tableName = ReflectionHelper.GetTableName(entities.First().GetType());
            var editableFields = ReflectionHelper.GetEditableFields(entities.First().GetType());

            var query = new StringBuilder();
            query.AppendFormat("INSERT INTO {0} (", tableName);

            foreach (var editableField in editableFields)
            {
                query.AppendFormat("[{0}], ", editableField.Name);
            }

            query.Length -= 2;
            query.Append(") VALUES (");

            foreach (var editableField in editableFields)
            {
                query.AppendFormat("@{0}, ", editableField.Name);
            }

            query.Length -= 2;
            query.Append(");");

            ConnectionBridge.Execute(query.ToString(), entities);

            var resultList = new List<T>();
            foreach (var batch in guidList.Batch(2000)) //SQL server maximum parameter for IN
            {
                resultList.AddRange(
                    ConnectionBridge.Query<T>(string.Format("SELECT * FROM {0} WHERE RowGuid IN @batch", tableName), new
                    {
                        batch
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

            var tableName = ReflectionHelper.GetTableName(entities.First().GetType());
            var keys = ReflectionHelper.GetKeyFields(entities.First().GetType());
            var editableFields = ReflectionHelper.GetEditableFields(entities.First().GetType());

            var query = new StringBuilder();
            query.AppendFormat("INSERT INTO {0} (", tableName);

            foreach (var editableField in editableFields)
            {
                query.AppendFormat("[{0}], ", editableField.Name);
            }

            query.Length -= 2;
            query.Append(") VALUES (");

            foreach (var editableField in editableFields)
            {
                query.AppendFormat("@{0}, ", editableField.Name);
            }

            query.Length -= 2;
            query.Append(");");
            if (entities.Count() == 1) //get ID
            {
                query.Append("SELECT CAST(SCOPE_IDENTITY() AS INT);");
            }

            if (entities.Count() == 1 && keys.Count() == 1 && keys.First().PropertyType != typeof (Guid))
            {
                var result = ConnectionBridge.Query<int?>(query.ToString(), entities.Single())
                    .SingleOrDefault();

                if (result != null)
                    ReflectionHelper.SetFieldValue(entities.Single(), keys.First().Name, result);
            }
            else
            {
                ConnectionBridge.Execute(query.ToString(), entities);
            }
        }

        public void UpdateMultiple(IEnumerable<object> entities)
        {
            if (!entities.Any())
                return;

            InterfaceHelper.AssignInterfaceData(ref entities);
            InterfaceHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(entities.First().GetType());
            var keys = ReflectionHelper.GetKeyFields(entities.First().GetType());
            var editableFields = ReflectionHelper.GetEditableFields(entities.First().GetType());

            var query = new StringBuilder();

            query.AppendFormat("UPDATE {0} SET ", tableName);

            foreach (var editableField in editableFields)
            {
                query.AppendFormat("[{0}] = @{0}, ", editableField.Name);
            }

            query.Length -= 2;
            query.Append(" WHERE ");

            foreach (var key in keys)
            {
                query.AppendFormat("[{0}] = @{0} AND ", key.Name);
            }

            query.Length -= 5;
            query.Append(";");

            ConnectionBridge.Execute(query.ToString(), entities);
        }

        public void DeleteMultiple(IEnumerable<object> entities)
        {
            if (!entities.Any())
                return;

            InterfaceHelper.AssignInterfaceData(ref entities);
            InterfaceHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(entities.First().GetType());
            var keys = ReflectionHelper.GetKeyFields(entities.First().GetType());

            if (!keys.Any())
                return;

            var query = new StringBuilder();

            query.AppendFormat("DELETE FROM {0} WHERE ", tableName);

            foreach (var key in keys)
            {
                query.AppendFormat("[{0}] = @{0} AND ", key.Name);
            }

            query.Length -= 5;
            query.Append(";");

            ConnectionBridge.Execute(query.ToString(), entities);
        }
    }
}