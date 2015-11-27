﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruDapper.BackofficeTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CruDapper.Test
{
    [TestClass]
    public class TestMerge : BaseService
    {
        [TestMethod]
        public void MergeInsertAndUpdate()
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
                .Put<TestTable>(entries);

            var existingTestTables = CrudService
                .GetAll<TestTable>();

            foreach (var existingTestTable in existingTestTables)
            {
                existingTestTable.SomeData = "mergedAndUpdatedExisting";
            }

            var entriesToBeMerged = new List<TestTable>();
            for (var i = 0; i < 1000; i++)
            {
                entriesToBeMerged.Add(new TestTable
                {
                    SomeData = i.ToString() + "newlyInserted"
                });
            }

            entriesToBeMerged.AddRange(existingTestTables);

            CrudService
                .Merge<TestTable>(entriesToBeMerged);

            Assert.IsTrue(CrudService.GetAll<TestTable>().Count() == 2000);

            DoBaseline();
        }

        [TestMethod]
        public void MergeInsert()
        {
            DoBaseline();

            var entries = new List<TestTable>();
            for (var i = 0; i < 10; i++)
            {
                entries.Add(new TestTable
                {
                    SomeData = i.ToString()
                });
            }

            CrudService
                .Merge<TestTable>(entries);

            Assert.IsTrue(CrudService.GetAll<TestTable>().Count() == 10);

            DoBaseline();
        }

        [TestMethod]
        public void MergeUpdate()
        {
            DoBaseline();

            var entries = new List<TestTable>();
            for (var i = 0; i < 10; i++)
            {
                entries.Add(new TestTable
                {
                    SomeData = i.ToString()
                });
            }

            CrudService
                .Put<TestTable>(entries);

            var existingTestTables = CrudService
                .GetAll<TestTable>();

            foreach (var existingTestTable in existingTestTables)
            {
                existingTestTable.SomeData = "mergedAndUpdatedExisting";
            }

            CrudService
                .Merge<TestTable>(existingTestTables);

            Assert.IsTrue(CrudService.GetAll<TestTable>().Count() == 10);

            DoBaseline();
        }
    }
}
