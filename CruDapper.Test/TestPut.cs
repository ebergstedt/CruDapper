﻿using System.Collections.Generic;
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

            CrudService
                .Put(entry);

            var testTable = CrudService.Get<TestTable>(entry.Id);            

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

            CrudService
                .Put(entries);

            var testTables = CrudService
                .GetAll<TestTable>();

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

            var identifiableTables = CrudService
                .PutIdentifiable<TestIdentifiableTable>(entries);

            entries = new List<TestIdentifiableTable>();
            for (var i = 0; i < 1000; i++)
            {
                entries.Add(new TestIdentifiableTable
                {
                    SomeData = i.ToString() + 1
                });
            }

            identifiableTables = CrudService
                .PutIdentifiable<TestIdentifiableTable>(entries);

            Assert.IsTrue(identifiableTables.All(t => t.Id > 0));

            DoBaseline();
        }
    }
}