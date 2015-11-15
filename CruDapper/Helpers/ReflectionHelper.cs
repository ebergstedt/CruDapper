using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using CruDapper.Infrastructure;

namespace CruDapper.Helpers
{
    public static class ReflectionHelper
    {
        public static bool HasColumn(Type type, string columnName)
        {
            var column = type
                .GetProperties()
                .FirstOrDefault(x => x.Name == columnName);

            return column != null && column.GetCustomAttributes(true)
                .All(x => x.GetType().Name != "ParameterAttribute");
        }

        public static void SetFieldValue(object entity, string field, dynamic value)
        {
            var property = entity.GetType().GetProperty(field);
            property.SetValue(entity, Convert.ChangeType(value, property.PropertyType), null);
        }

        public static List<PropertyInfo> GetEditableFields(Type type)
        {
            var properties = type.GetProperties();
            var result = new List<PropertyInfo>();

            foreach (var property in properties)
            {
                if (typeof (string) == property.PropertyType == false &&
                    typeof (IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    continue;
                }

                if (property.PropertyType != typeof (Guid) &&
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

                result.Add(property);
            }

            return result;
        }

        public static string GetTableName(Type type, Provider provider = Provider.Default)
        {
            var tableAttribute = type
                .GetCustomAttributes(true)
                .SingleOrDefault(x => x.GetType().Name == "TableAttribute") as TableAttribute;

            return tableAttribute != null ? tableAttribute.Name : null;
        }

        public static List<PropertyInfo> GetKeyFields(Type type)
        {
            return type.GetProperties()
                .Where(x => x.GetCustomAttributes(true).Any(y => y.GetType().Name == "KeyAttribute"))
                .ToList();
        }

        public static string GetPrimaryKeyName<T>()
        {
            var keys = GetKeyFields(typeof (T));

            if (keys.Any() == false || keys.Count() > 1)
            {
                throw new Exception("The given table has more or less than one primary key");
            }

            return keys.First().Name;
        }
    }
}