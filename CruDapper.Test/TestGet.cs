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
                .Put(entries);

            var testTables = CrudService
                .GetAll<TestTable>();

            Assert.IsTrue(testTables.Count() == entries.Count);
            Assert.IsTrue(testTables.All(t => t.CreatedAt != null && t.CreatedAt <= DateTime.UtcNow));

            DoBaseline();
        }

        [TestMethod]
        public void GetByPrimaryKey()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            var byPrimaryKey = CrudService
                .GetByPrimaryKey<TestTable>(entry.Id);

            Assert.IsNotNull(byPrimaryKey);

            DoBaseline();
        }

        [TestMethod]
        public void GetById()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            Assert.IsTrue(entry.Id > 0);

            var getEntry = CrudService
                .Get<TestTable>(entry.Id);

            Assert.IsNotNull(getEntry);

            DoBaseline();
        }

        [TestMethod]
        public void GetByColumn()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            var getByColumn = CrudService
                .GetByColumn<TestTable>("SomeData", entry.SomeData)
                .Single();

            Assert.IsTrue(getByColumn.SomeData == entry.SomeData);

            DoBaseline();
        }

        [TestMethod]
        public void GetByColumns()
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

            CrudService
                .Put(entries);

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
        }
    }
}