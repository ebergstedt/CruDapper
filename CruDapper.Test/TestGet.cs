using System;
using System.Collections.Generic;
using System.Linq;
using CruDapper.BackofficeTest;
using CruDapper.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CruDapper.Test
{
    [TestClass]
    public class TestGet : BaseService
    {
        [TestMethod]
        public void GetAll()
        {
            var entries = BaseLineAndPutAndReturnTestTables();

            var testTables = CrudService
                .GetAll<TestTable>();

            Assert.IsTrue(testTables.Count() == entries.Count());
            Assert.IsTrue(testTables.All(t => t.CreatedAt != null && t.CreatedAt <= DateTime.UtcNow));

            DoBaseline();
        }

        [TestMethod]
        public void GetSingle()
        {
            var entry = BaseLineAndPutAndReturnTestTable();

            var byPrimaryKey = CrudService
                .GetSingle<TestTable>(entry.Id);

            Assert.IsNotNull(byPrimaryKey);

            DoBaseline();
        }

        [TestMethod]
        public void GetMany()
        {
            var entries = BaseLineAndPutAndReturnTestTables();

            var testTables = CrudService
                .GetAll<TestTable>();

            var getMany = CrudService
                .GetMany<TestTable>(testTables.Select(s => s.Id));

            Assert.IsTrue(getMany.Count() == testTables.Count());
        }

        [TestMethod]
        public void GetByColumn()
        {
            var entry = BaseLineAndPutAndReturnTestTable();
            
            var getByColumn = CrudService
                .GetByColumn<TestTable>("SomeData", entry.SomeData)
                .Single();

            Assert.IsTrue(getByColumn.SomeData == entry.SomeData);

            DoBaseline();
        }

        [TestMethod]
        public void GetByColumns()
        {
            var entries = BaseLineAndPutAndReturnTestTables();

            var result = CrudService
                .GetByColumns<TestTable>(new List<WhereArgument>
                {
                    new WhereArgument
                    {
                        Key = "SomeData",
                        Operator = Operator.Equals,
                        Value = "500"
                    }
                });

            Assert.IsTrue(result.Count() == 1);

            var result2 = CrudService
                .GetByColumns<TestTable>(new List<WhereArgument>
                {
                    new WhereArgument
                    {
                        Key = "CreatedAt",
                        Operator = Operator.LessThan,
                        Value = DateTime.UtcNow
                    }
                });

            Assert.IsTrue(result2.Count() == entries.Count());

            DoBaseline();
        }

        [TestMethod]
        public void GetPaginated()
        {
            var entries = BaseLineAndPutAndReturnTestTables();

            var pages = CrudService
                .GetPaginated<TestTable>("Id", pageSize: 100);

            Assert.IsTrue(pages.Count() == 100);
        }
    }
}