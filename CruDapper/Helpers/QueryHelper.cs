using System;
using System.Collections.Generic;
using System.Text;
using CruDapper.Infrastructure;
using Dapper;

namespace CruDapper.Helpers
{
    public static class QueryHelper
    {
        public static void AddWhereArgumentsToQuery(ref StringBuilder query, DynamicParameters parameters,
            List<WhereArgument> arguments, Type entity)
        {
            if (arguments != null)
            {
                foreach (var argument in arguments)
                {
                    if (ReflectionHelper.HasColumn(entity, argument.Key))
                    {
                        if (argument.Value == null)
                        {
                            query.AppendFormat(" AND {0} IS {1}", argument.Key,
                                (argument.Not.HasValue && argument.Not.Value ? "NOT NULL" : "NULL"));
                        }
                        else
                        {
                            query.AppendFormat(" AND {0} {1} @{0}", argument.Key,
                                GetOperator(argument.Operator, argument.Not));
                            parameters.Add(argument.Key, argument.Value);
                        }
                    }
                }
            }
        }        

        public static StringBuilder GetQuery<T>(int id, Provider provider, bool getDeleted = false)
        {
            var tableName = ReflectionHelper.GetTableName(typeof (T));
            var query = new StringBuilder();
            query.AppendFormat(@"
                SELECT
                    *
                FROM
                    {0} 
                WHERE Id = {1}
            ", tableName, id);

            if (!getDeleted)
            {
                query.AppendFormat(" AND {0} ", QueryHelper.GetIsDeletedSQL(provider));
            }

            return query;
        }

        public static string GetIsDeletedSQL(Provider provider)
        {
            switch (provider)
            {
                case Provider.MsSql:
                    return " IsDeleted = 0 ";
                case Provider.Postgres:
                    return " IsDeleted = false ";
                case Provider.Default:
                    throw new ArgumentException("Provider not implemented");
            }

            return string.Empty;
        }

        public static string GetConcurrentConnectionSafeTempTableName<T>()
        {
            return string.Format("#{0}{1}", ReflectionHelper.GetTableName(typeof(T), false), Guid.NewGuid().ToString().Replace("-", String.Empty));
        }

        public static string GetOperator(Operator? op = null, bool? not = null)
        {
            not = not ?? false;

            switch (op)
            {
                case Operator.GreaterThan:
                    return not == true ? "!>" : ">";
                case Operator.GreaterThanOrEqual:
                    return not == true ? "<" : ">=";
                case Operator.LessThan:
                    return not == true ? "!<" : "<";
                case Operator.LessThanOrEqual:
                    return not == true ? ">" : "<=";
                case Operator.In:
                    return not == true ? "NOT IN" : "IN";
                case Operator.Like:
                    return not == true ? "NOT LIKE" : "LIKE";
                default:
                    return not == true ? "!=" : "=";
            }
        }
    }
}