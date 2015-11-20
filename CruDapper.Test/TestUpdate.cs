using CruDapper.BackofficeTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CruDapper.Test
{
    [TestClass]
    public class TestUpdate : BaseService
    {
        [TestMethod]
        public void Update()
        {
            var originalEntry = BaseLineAndPutAndReturnEntry();

            var originalData = originalEntry.SomeData;
            originalEntry.SomeData = "alteredData";

            CrudService
                .Update<TestTable>(originalEntry);

            var updatedEntry = CrudService
                .Get<TestTable>(originalEntry.Id);

            Assert.AreNotEqual(originalData, updatedEntry.SomeData);

            DoBaseline();
        }
    }
}