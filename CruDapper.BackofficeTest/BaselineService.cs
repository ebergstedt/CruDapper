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
            var testTables = Get<TestTable>();
            DeletePermanently(testTables);

            var testIdentifiableTables = Get<TestIdentifiableTable>();
            DeletePermanently(testIdentifiableTables);
        }
    }
}