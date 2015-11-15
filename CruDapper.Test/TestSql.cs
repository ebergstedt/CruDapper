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
            var entry = BaseLineAndPutAndReturnEntry();

            Assert.IsNotNull(entry);

            var testTablesByGetAll = CrudService
                .GetAll<TestTable>();

            var testTablesByQuery = CrudService
                .Query<TestTable>("SELECT * FROM CruDapperSchema.TestTable");

            Assert.IsTrue(testTablesByGetAll.Count() == testTablesByQuery.Count());

            DoBaseline();
        }

        [TestMethod]
        public void SelectByQueryDynamic()
        {
            var entry = BaseLineAndPutAndReturnEntry();

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
        public void Execute()
        {
            BaseLineAndPutAndReturnEntry();

            Assert.IsTrue(CrudService.GetAll<TestTable>().Any());

            CrudService
                .Execute("DELETE FROM CruDapperSchema.TestTable");

            Assert.IsFalse(CrudService.GetAll<TestTable>().Any());
        }
    }
}
