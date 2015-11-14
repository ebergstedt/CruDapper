using System.Collections.Generic;
using System.Linq;
using CruDapper.BackofficeTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CruDapper.Test
{
    [TestClass]
    public class TestPut : BaseService
    {
        [TestMethod]
        public void Put()
        {
            DoBaseline();

            var entry = new TestTable
            {
                SomeData = "data"
            };

            _crudService
                .Put(entry);

            var testTable = _crudService.Get<TestTable>(entry.Id);

            Assert.IsNotNull(testTable);

            DoBaseline();
        }

        [TestMethod]
        public void PutMany()
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

            _crudService
                .Put(entries);

            var testTables = _crudService
                .Get<TestTable>();

            Assert.IsTrue(testTables.Count() == entries.Count);

            DoBaseline();
        }

        [TestMethod]
        public void PutIdentifiable()
        {
            DoBaseline();

            var entries = new List<TestIdentifiableTable>();
            for (var i = 0; i < 1000; i++)
            {
                entries.Add(new TestIdentifiableTable
                {
                    SomeData = i.ToString() + 1
                });
            }

            var identifiableTables = _crudService
                .PutIdentifiable<TestIdentifiableTable>(entries);

            entries = new List<TestIdentifiableTable>();
            for (var i = 0; i < 1000; i++)
            {
                entries.Add(new TestIdentifiableTable
                {
                    SomeData = i.ToString() + 1
                });
            }

            identifiableTables = _crudService
                .PutIdentifiable<TestIdentifiableTable>(entries);

            Assert.IsTrue(identifiableTables.All(t => t.Id > 0));

            DoBaseline();
        }
    }
}