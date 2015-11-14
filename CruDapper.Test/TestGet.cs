using System;
using System.Collections.Generic;
using System.Linq;
using CruDapper.BackofficeTest;
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

            _crudService
                .Put(entries);

            var testTables = _crudService
                .GetAll<TestTable>();

            Assert.IsTrue(testTables.Count() == entries.Count);
            Assert.IsTrue(testTables.All(t => t.CreatedAt != null && t.CreatedAt <= DateTime.UtcNow));

            DoBaseline();
        }

        [TestMethod]
        public void GetByPrimaryKey()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            var byPrimaryKey = _crudService
                .GetByPrimaryKey<TestTable>(entry.Id);

            Assert.IsNotNull(byPrimaryKey);

            DoBaseline();
        }

        [TestMethod]
        public void GetById()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            Assert.IsTrue(entry.Id > 0);

            var getEntry = _crudService
                .Get<TestTable>(entry.Id);

            Assert.IsNotNull(getEntry);

            DoBaseline();
        }

        [TestMethod]
        public void GetByColumn()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            var getByColumn = _crudService
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

            _crudService
                .Put(entries);

            var result = _crudService
                .GetByColumns<TestTable>(new List<WhereArgument>(){
                    new WhereArgument(){
                        Key = "SomeData",
                        Operator = Operator.Equals,
                        Value = "500"
                    }
                });

            Assert.IsTrue(result.Count() == 1);

            var result2 = _crudService
                .GetByColumns<TestTable>(new List<WhereArgument>(){
                    new WhereArgument(){
                        Key = "CreatedAt",
                        Operator = Operator.LessThan,
                        Value = DateTime.UtcNow
                    }
                });

            Assert.IsTrue(result2.Count() == entries.Count());
        }

        [TestMethod]
        public void GetNondeleted()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            var getNondeleted = _crudService
                .GetNondeleted<TestTable>(entry.Id);

            Assert.IsNotNull(getNondeleted);

            _crudService
                .Delete<TestTable>(getNondeleted);

            var deletedEntry = _crudService
                .GetNondeleted<TestTable>(entry.Id);

            Assert.IsNull(deletedEntry);

            DoBaseline();
        }
    }
}