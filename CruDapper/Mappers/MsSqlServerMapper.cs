﻿using System;
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
    public class MsSqlServerMapper : DbMapperBase, IDbMapper
    {
        private readonly Provider _provider = Provider.MsSql;
        private readonly IValueMapper _valueMapper;

        public MsSqlServerMapper(string connectionName, IValueMapper valueMapper, int? globalCommandTimeout = null) : base(connectionName, Provider.MsSql, globalCommandTimeout)
        {
            _valueMapper = valueMapper;
        }

        public IEnumerable<T> GetAll<T>(bool getDeleted = false)
        {
            var tableName = ReflectionHelper.GetTableName(typeof(T));
            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT * FROM {0}", tableName);
            if (!getDeleted && ValidationHelper.VerifyIDeletable<T>())
            {
                query.AppendFormat(" WHERE {0} ", QueryHelper.GetIsDeletedSQL(_provider));
            }
            return ConnectionBridge.Query<T>(query.ToString());
        }

        public IEnumerable<T> GetMany<T>(object primaryKeyValue, bool getDeleted = false)
        {
            return GetByColumn<T>(new WhereArgument()
            {
                Key = ReflectionHelper.GetPrimaryKeyName(typeof (T)),
                Value = primaryKeyValue,
                Operator = Operator.In
            });
        }

        public T GetSingle<T>(object primaryKeyValue, bool getDeleted = false)
        {            
            return GetByColumn<T>(ReflectionHelper.GetPrimaryKeyName(typeof(T)), primaryKeyValue, getDeleted)
                .FirstOrDefault();
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
            var tableName = ReflectionHelper.GetTableName(typeof(T));
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

            if (!getDeleted && ValidationHelper.VerifyIDeletable<T>())
            {
                query.AppendFormat(" AND {0} ", QueryHelper.GetIsDeletedSQL(_provider));
            }

            QueryHelper.AddWhereArgumentsToQuery(ref query, parameters, whereArguments, typeof(T));

            return ConnectionBridge.Query<T>(query.ToString(), parameters);
        }

        public IEnumerable<T> GetPaginated<T>(string sortColumn, int pageSize = 10, int currentPage = 1, OrderBy sortingDirection = OrderBy.Asc)
        {
            var query = new StringBuilder();
            query.AppendFormat(@"
WITH [Ordered] AS
(
    SELECT
        a.*,
        ROW_NUMBER() OVER (ORDER BY {0} {1}) AS RowNumber
    FROM
        {2} AS a
)
SELECT
    *
FROM
    [Ordered]
WHERE
    RowNumber BETWEEN {3} AND {4}"
                , sortColumn,
                QueryHelper.GetSortDirectionSQL(_provider, sortingDirection),
                ReflectionHelper.GetTableName(typeof(T)),
                (pageSize * (currentPage - 1)) + 1, 
                pageSize * currentPage);

            return ConnectionBridge.Query<T>(query.ToString());
        }

        //todo http://stackoverflow.com/questions/95988/how-to-insert-multiple-records-and-get-the-identity-value
        public IEnumerable<T> InsertMultipleIdentifiable<T>(IEnumerable<T> entities)
        {
            if (!entities.Any())
                return null;

            _valueMapper.AssignValues(ref entities);
            ValidationHelper.ValidateList(ref entities);

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

        public void InsertMultiple<T>(IEnumerable<T> entities)
        {
            if (!entities.Any())
                return;

            _valueMapper.AssignValues(ref entities);
            ValidationHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(typeof(T));
            var keys = ReflectionHelper.GetKeyFields(typeof(T));
            var editableFields = ReflectionHelper.GetEditableFields(typeof(T));

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

        public void UpdateMultiple<T>(IEnumerable<T> entities)
        {
            if (!entities.Any())
                return;

            _valueMapper.AssignValues(ref entities);
            ValidationHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(typeof(T));
            var keys = ReflectionHelper.GetKeyFields(typeof(T));
            var editableFields = ReflectionHelper.GetEditableFields(typeof(T));

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

        public void DeleteMultiple<T>(IEnumerable<T> entities)
        {
            if (!entities.Any())
                return;

            _valueMapper.AssignValues(ref entities);
            ValidationHelper.ValidateList(ref entities);

            var tableName = ReflectionHelper.GetTableName(typeof(T));
            var keys = ReflectionHelper.GetKeyFields(typeof(T));

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

        public void MergeMultiple<T>(IEnumerable<T> entities)
        {
            _valueMapper.AssignValues(ref entities);
            ValidationHelper.ValidateList(ref entities);

            var targetTableName = ReflectionHelper.GetTableName(typeof(T));
            var tempTableName  = ReflectionHelper.GetConcurrentConnectionSafeTempTableName<T>();
            var sourceTableName = string.Format("#{0}", tempTableName);
            var keys = ReflectionHelper.GetKeyFields(typeof(T));
            var editableFields = ReflectionHelper.GetEditableFields(typeof(T));            

            var query = new StringBuilder();
            //temp table will be dropped within transaction scope in Execute
            query.AppendFormat(@" SELECT TOP 0 * INTO {0} FROM {1} ", sourceTableName, targetTableName); //generate empty clone
            query.AppendFormat(" SET IDENTITY_INSERT {0} ON; ", sourceTableName); 
            query.AppendFormat(" INSERT INTO {0} (", sourceTableName); //seed temp table
            foreach (var key in keys)
            {
                query.AppendFormat("{0}, ", key.Name);
            }
            foreach (var editableField in editableFields)
            {
                query.AppendFormat("{0}, ", editableField.Name);
            }
            query.Length -= 2;
            query.Append(") VALUES (");
            foreach (var key in keys)
            {
                query.AppendFormat("@{0}, ", key.Name);
            }
            foreach (var editableField in editableFields)
            {
                query.AppendFormat("@{0}, ", editableField.Name);
            }
            query.Length -= 2;
            query.Append(");");
            query.AppendFormat(" MERGE {0} AS TargetTable ", targetTableName); //merge
            query.AppendFormat(" USING {0} AS SourceTable ", sourceTableName);
            query.AppendFormat(" ON 1=1 ");
            foreach (var key in keys)
            {
                query.AppendFormat(" AND TargetTable.{0} = SourceTable.{0} ", key.Name);
            }
            query.AppendFormat(" WHEN MATCHED THEN UPDATE SET ");
            foreach (var editableField in editableFields)
            {
                query.AppendFormat("TargetTable.{0} = SourceTable.{0}, ", editableField.Name);
            }
            query.Length -= 2;
            query.AppendFormat(" WHEN NOT MATCHED THEN INSERT ");
            query.Append("(");
            foreach (var editableField in editableFields)
            {
                query.AppendFormat("{0}, ", editableField.Name);
            }
            query.Length -= 2;
            query.Append(") VALUES (");
            foreach (var editableField in editableFields)
            {
                query.AppendFormat("SourceTable.{0}, ", editableField.Name);
            }
            query.Length -= 2;
            query.Append(");");            
            ConnectionBridge.Execute(query.ToString(), entities);
        }

        public void DeleteAll<T>()
        {
            var targetTableName = ReflectionHelper.GetTableName(typeof(T));
            var query = new StringBuilder();
            query.AppendFormat("DELETE FROM {0}", targetTableName);
            ConnectionBridge.Execute(query.ToString());
        }
    }
}