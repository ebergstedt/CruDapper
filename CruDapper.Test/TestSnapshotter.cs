using System;
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
            var entry = BaseLineAndPutAndReturnTestTable();

            var snapshot = Snapshotter.Start(entry);

            string dataChange = "changed my data";
            entry.SomeData = dataChange;

            Assert.IsTrue(snapshot.memberWiseClone.SomeData == "data");
        }
    }
}
