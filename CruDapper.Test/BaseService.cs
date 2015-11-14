using CruDapper.BackofficeTest;
using CruDapper.Helpers;
using CruDapper.Services;
using CruDapper.Mappers;

namespace CruDapper.Test
{
    public class BaseService
    {
        protected readonly IServiceFactory _serviceFactory;
        protected ICrudService _crudService;

        public BaseService()
        {
            _serviceFactory = new ServiceFactory(new PostgresMapper("Postgres"));

            _crudService = _serviceFactory.Get<CrudService>();
        }

        public void DoBaseline()
        {
            _serviceFactory
                .Get<BaselineService>()
                .DoBaseline();
        }

        protected TestTable BaseLineAndPutAndReturnEntry()
        {
            DoBaseline();

            var entry = new TestTable
            {
                SomeData = "data"
            };

            _crudService
                .Put(entry);

            return entry;
        }
    }
}