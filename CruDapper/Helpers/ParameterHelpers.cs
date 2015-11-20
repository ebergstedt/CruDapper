using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace CruDapper.Helpers
{
    public static class ParameterHelpers
    {
        /// <summary>
        /// Reference type IntegerTable 
        /// </summary>
        public static DataTable ToIdIntegerTable(this IEnumerable<int> ids)
        {
            var idsTable = new DataTable();
            idsTable.Columns.Add("Id", typeof(int));
            idsTable.SetTypeName("dbo.IdIntegerTable");

            if (ids != null)
            {
                ids.ToList().ForEach(x =>
                {
                    idsTable.Rows.Add(x);
                });
            }

            return idsTable;
        }
    }
}
