﻿using CruDapper.BackofficeTest;
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

        public BaseService()
        {
            DbMapper = new PostgresMapper("Postgres");
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