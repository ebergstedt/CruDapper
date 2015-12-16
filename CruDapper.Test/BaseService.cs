using System;
using System.Collections.Generic;
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

            switch (Provider)
            {
                case Provider.MsSql:
                    DbMapper = new MsSqlServerMapper("DefaultConnection");
                    break;
                case Provider.Postgres:
                    DbMapper = new PostgresMapper("Postgres");
                    break;
                default:
                    throw new NotImplementedException();
            }            
            ServiceFactory = new ServiceFactory(DbMapper);
            CrudService = ServiceFactory.Get<CrudService>();
        }

        public void DoBaseline()
        {
            ServiceFactory
                .Get<BaselineService>()
                .DoBaseline();
        }

        protected TestTable BaseLineAndPutAndReturnTestTable()
        {
            DoBaseline();

            var entry = new TestTable
            {
                SomeData = "data"
            };

            CrudService
                .Put<TestTable>(entry);

            return entry;
        }

        protected IEnumerable<TestTable> BaseLineAndPutAndReturnTestTables()
        {
            DoBaseline();

            var entries = new List<TestTable>();
            for (var i = 0; i < 1000; i++)
            {
                entries.Add(new TestTable
                {
                    SomeData = i.ToString()
                });
            }

            return entries;
        }
    }
}