using System;
using System.Collections.Generic;
using CruDapper.Infrastructure;

namespace CruDapper.Helpers
{
    public static class InterfaceHelper
    {
        public static void AssignInterfaceData(ref IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                var dateLoggable = entity as IDateLoggable;
                if (dateLoggable != null)
                {
                    if (dateLoggable.CreatedAt == default(DateTime))
                        dateLoggable.CreatedAt = DateTime.UtcNow;
                    dateLoggable.UpdatedAt = DateTime.UtcNow;
                }

                var identifiable = entity as IIdentifiable;
                if (identifiable != null && identifiable.RowGuid == default(Guid))
                {
                    identifiable.RowGuid = Guid.NewGuid();
                }
            }
        }

        public static bool VerifyIDeletable<T>()
        {
            if (!typeof (IDeletable).IsAssignableFrom(typeof(T)))
                return false;
            return true;
        }

        public static void ValidateList(ref IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                ValidationHelper.ValidateModel(entity);
            }
        }
    }
}