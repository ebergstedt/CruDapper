using CruDapper.BackofficeTest;
using CruDapper.Infrastructure;
using CruDapper.Mappers;
using CruDapper.Services;

namespace CruDapper.Test
{
    public class BaseService
    {
        protected readonly IServiceFactory ServiceFactory;
        protected ICrudService CrudService;
        protected IDbMapper DbMapper;
        protected Provider Provider;

        public BaseService()
        {
            Provider = Provider.MsSql; //for test methods
            DbMapper = new MsSqlServerMapper("DefaultConnection");
            ServiceFactory = new ServiceFactory(DbMapper);

            CrudService = ServiceFactory.Get<CrudService>();
        }

        public void DoBaseline()
        {
            ServiceFactory
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

            CrudService
                .Put(entry);

            return entry;
        }
    }
}