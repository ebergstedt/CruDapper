using CruDapper.Infrastructure;
using CruDapper.Services;

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
            DeletePermanently(testTables);

            var testIdentifiableTables = GetAll<TestIdentifiableTable>();
            DeletePermanently(testIdentifiableTables);
        }
    }
}