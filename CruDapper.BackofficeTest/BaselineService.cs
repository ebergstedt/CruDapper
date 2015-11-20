using System.Collections.Generic;
using System.Data.Common;
using CruDapper.Infrastructure;
using CruDapper.Services;
using Dapper;

namespace CruDapper.BackofficeTest
{
    public class BaselineService : CrudService
    {
        public BaselineService(IDbMapper dbMapper)
            : base(dbMapper)
        {
        }

        public void DoBaseline()
        {
            var testTables = GetAll<TestTable>();
            DeletePermanently<TestTable>(testTables);

            var testIdentifiableTables = GetAll<TestIdentifiableTable>();
            DeletePermanently<TestIdentifiableTable>(testIdentifiableTables);
        }
    }
}