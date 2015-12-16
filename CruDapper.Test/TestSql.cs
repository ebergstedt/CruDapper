using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruDapper.BackofficeTest;
using CruDapper.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CSharp;

namespace CruDapper.Test
{
    [TestClass]
    public class TestSql : BaseService
    {
        [TestMethod]
        public void SelectByQuery()
        {
            var entry = BaseLineAndPutAndReturnTestTable();

            Assert.IsNotNull(entry);

            var testTablesByQuery = CrudService
                .Query<TestTable>("SELECT * FROM CruDapperSchema.TestTable");

            Assert.IsTrue(testTablesByQuery.Any());

            DoBaseline();
        }

        [TestMethod]
        public async Task SelectByQueryAsync()
        {
            var entry = BaseLineAndPutAndReturnTestTable();

            Assert.IsNotNull(entry);

            var testTablesByQuery = await CrudService
                .QueryAsync<TestTable>("SELECT * FROM CruDapperSchema.TestTable");

            Assert.IsTrue(testTablesByQuery.Any());

            DoBaseline();
        }

        [TestMethod]
        public void SelectByQueryDynamic()
        {
            var entry = BaseLineAndPutAndReturnTestTable();

            Assert.IsNotNull(entry);

            if (DbMapper.GetType() == typeof(PostgresMapper))
            {
                var dynamicEntry = CrudService
                    .QueryDynamic("SELECT * FROM CruDapperSchema.TestTable LIMIT 1")
                    .Single();

                dynamic data = dynamicEntry.somedata; 

                Assert.IsNotNull(data);
            }
            else if(DbMapper.GetType() == typeof(MsSqlServerMapper))
            {
                var dynamicEntry = CrudService
                    .QueryDynamic("SELECT TOP 1 * FROM CruDapperSchema.TestTable")
                    .Single();

                dynamic data = dynamicEntry.SomeData; 

                Assert.IsNotNull(data);
            }

            DoBaseline();
        }

        [TestMethod]
        public async Task SelectByQueryDynamicAsync()
        {
            var entry = BaseLineAndPutAndReturnTestTable();

            Assert.IsNotNull(entry);

            if (DbMapper.GetType() == typeof(PostgresMapper))
            {
                IEnumerable<dynamic> dynamicEntries = await CrudService
                    .QueryDynamicAsync("SELECT * FROM CruDapperSchema.TestTable LIMIT 1");

                dynamic data = dynamicEntries.Single().somedata;

                Assert.IsNotNull(data);
            }
            else if (DbMapper.GetType() == typeof(MsSqlServerMapper))
            {
                IEnumerable<dynamic> dynamicEntries = await CrudService
                    .QueryDynamicAsync("SELECT TOP 1 * FROM CruDapperSchema.TestTable");

                dynamic data = dynamicEntries.Single().SomeData;

                Assert.IsNotNull(data);
            }

            DoBaseline();
        }

        [TestMethod]
        public void Execute()
        {
            BaseLineAndPutAndReturnTestTable();

            Assert.IsTrue(CrudService.GetAll<TestTable>().Any());

            CrudService
                .Execute("DELETE FROM CruDapperSchema.TestTable");

            Assert.IsFalse(CrudService.GetAll<TestTable>().Any());
        }

        [TestMethod]
        public async Task ExecuteAsync()
        {
            BaseLineAndPutAndReturnTestTable();

            Assert.IsTrue(CrudService.GetAll<TestTable>().Any());

            await CrudService.ExecuteAsync("DELETE FROM CruDapperSchema.TestTable");

            Assert.IsFalse(CrudService.GetAll<TestTable>().Any());
        }
    }
}
