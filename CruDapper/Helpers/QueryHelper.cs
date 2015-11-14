using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                                QueryHelper.GetOperator(argument.Operator, argument.Not));
                            parameters.Add(argument.Key, argument.Value);
                        }
                    }
                }
            }
        }

        public static StringBuilder GetQuery<T>(int id, Provider provider)
        {
            var tableName = ReflectionHelper.GetTableName(typeof(T), provider);
            var query = new StringBuilder();
            query.AppendFormat(@"
                SELECT
                    *
                FROM
                    {0} 
                WHERE Id = {1}
            ", tableName, id);

            return query;
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
