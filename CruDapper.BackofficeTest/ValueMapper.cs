using System;
using System.Collections.Generic;
using CruDapper.Infrastructure;

namespace CruDapper.BackofficeTest
{
    public class ValueMapper : IValueMapper
    {
        public void AssignValues<T>(ref IEnumerable<T> entities)
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
    }
}