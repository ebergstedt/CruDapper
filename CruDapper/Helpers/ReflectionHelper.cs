using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using CruDapper.Infrastructure;

namespace CruDapper.Helpers
{
    public static class ReflectionHelper
    {
        static ConcurrentDictionary<Type, bool> hasColumnMap = new ConcurrentDictionary<Type, bool>();
        public static bool HasColumn(Type type, string columnName)
        {
            bool result;

            if (!hasColumnMap.TryGetValue(type, out result))
            {
                var column = type
                    .GetProperties()
                    .FirstOrDefault(x => x.Name == columnName);

                result = column != null &&
                         column.GetCustomAttributes(true).All(x => x.GetType().Name != "ParameterAttribute");

                hasColumnMap[type] = result;
            }

            return result;
        }

        public static void SetFieldValue(object entity, string field, dynamic value)
        {
            var property = entity.GetType().GetProperty(field);
            property.SetValue(entity, Convert.ChangeType(value, property.PropertyType), null);
        }

        static ConcurrentDictionary<Type, List<PropertyInfo>> editableFieldsMap = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        public static List<PropertyInfo> GetEditableFields(Type type)
        {
            List<PropertyInfo> result;

            if (!editableFieldsMap.TryGetValue(type, out result))
            {
                var properties = type.GetProperties();
                var propertyList = new List<PropertyInfo>();

                foreach (var property in properties)
                {
                    if (typeof(string) == property.PropertyType == false &&
                        typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                    {
                        continue;
                    }

                    if (property.PropertyType != typeof(Guid) &&
                        property.GetCustomAttributes(true).Any(x => x.GetType().Name == "KeyAttribute") &&
                        property.GetCustomAttributes(true).Any(x => x.GetType().Name == "AutoIncrementAttribute"))
                    {
                        continue;
                    }

                    if (property.GetCustomAttributes(true).Any(x => x.GetType().Name == "NotMappedAttribute"))
                    {
                        continue;
                    }

                    if (property.GetCustomAttributes(true).Any(x => x.GetType().Name == "ParameterAttribute"))
                    {
                        continue;
                    }

                    propertyList.Add(property);
                }

                result = propertyList;
                editableFieldsMap[type] = result;
            }

            return result;
        }

        static ConcurrentDictionary<Type, string> tableNameMap = new ConcurrentDictionary<Type, string>();
        public static string GetTableName(Type type, bool withSchema = true)
        {
            string result;
            if (!tableNameMap.TryGetValue(type, out result))
            {
                var tableAttribute = type
                    .GetCustomAttributes(true)
                    .SingleOrDefault(x => x.GetType().Name == "TableAttribute") as TableAttribute;                                

                result = tableAttribute != null ? tableAttribute.Name : null;
                tableNameMap[type] = result;
            }

            if (!withSchema)
                return result.Split('.').Last();
            return result;
        }

        static ConcurrentDictionary<Type, List<PropertyInfo>> keyFieldsMap = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        public static List<PropertyInfo> GetKeyFields(Type type)
        {
            List<PropertyInfo> result;
            if (!keyFieldsMap.TryGetValue(type, out result))
            {
                result = type.GetProperties()
                    .Where(x => x.GetCustomAttributes(true).Any(y => y.GetType().Name == "KeyAttribute"))
                    .ToList();

                keyFieldsMap[type] = result;
            }

            return result;
        }

        static ConcurrentDictionary<Type, string> primaryKeyNameMap = new ConcurrentDictionary<Type, string>();
        public static string GetPrimaryKeyName(Type type)
        {
            string result;
            if (!primaryKeyNameMap.TryGetValue(type, out result))
            {
                var keys = GetKeyFields(type);

                if (keys.Any() == false || keys.Count() > 1)
                {
                    throw new Exception("The given table has more or less than one primary key");
                }

                result = keys.First().Name;
                primaryKeyNameMap[type] = result;
            }

            return result;
        }
    }
}