﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruDapper.BackofficeTest;
using CruDapper.Code;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CruDapper.Test
{
    [TestClass]
    public class TestSnapshotter : BaseService
    {
        [TestMethod]
        public void TestUpdate()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            var snapshot = Snapshotter.Start(entry);

            string dataChange = "changed my data";
            entry.SomeData = dataChange;

            //CrudService.UpdateBySnapshot<TestTable>(snapshot);

            var testTable = CrudService
                .Get<TestTable>(entry.Id);

            Assert.IsTrue(testTable.SomeData == dataChange);
        }
    }
}
