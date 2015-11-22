using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruDapper.BackofficeTest;
using CruDapper.Helpers;
using CruDapper.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CruDapper.Test
{
    [TestClass]
    public class TestParameter : BaseService
    {
        [TestMethod]
        public void TestToIdIntegerTable()
        {
            if (Provider != Provider.MsSql)
                return;

            DoBaseline();

            var entries = new List<TestTable>();
            for (var i = 0; i < 1000; i++)
            {
                entries.Add(new TestTable
                {
                    SomeData = i.ToString()
                });
            }

            CrudService
                .Put<TestTable>(entries);

            var testTables = CrudService
                .GetAll<TestTable>();

            var IdIntegerTable = testTables.Select(s => s.Id).ToIdIntegerTable();
            //note that Dapper can take WHERE Id IN @Ids (which are set to Ids = testTables.Select(s => s.Id)) as a parameter below, but this just shows 
            //how to create an integer table if you use the user defined table type as a sql function parameter for example
            var testTablesByIdTable = CrudService.Query<TestTable>(@"
SELECT *
FROM CruDapperSchema.TestTable
WHERE Id IN (
    SELECT Id 
    FROM @IdIntegerTable
)
", new
            {
                IdIntegerTable
            });

            Assert.IsTrue(testTablesByIdTable.Count() == entries.Count);

            DoBaseline();
        }
    }
}
