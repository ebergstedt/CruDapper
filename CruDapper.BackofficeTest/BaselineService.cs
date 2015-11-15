using CruDapper.Infrastructure;
using CruDapper.Services;

namespace CruDapper.BackofficeTest
{
    public class BaselineService : CrudService
    {
        public BaselineService(IDbMapper dbHelper)
            : base(dbHelper)
        {
        }

        public void DoBaseline()
        {
            var testTables = GetAll<TestTable>();
            DeletePermanently(testTables);

            var testIdentifiableTables = GetAll<TestIdentifiableTable>();
            DeletePermanently(testIdentifiableTables);
        }
    }
}