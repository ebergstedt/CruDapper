using System.Collections.Generic;
using System.Linq;
using CruDapper.BackofficeTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CruDapper.Test
{
    [TestClass]
    public class TestDelete : BaseService
    {
        [TestMethod]
        public void Delete()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            CrudService
                .Delete<TestTable>(entry);

            var getDeleteFlagged = CrudService
                .Get<TestTable>(entry.Id, true);

            Assert.IsNotNull(getDeleteFlagged);
            Assert.IsTrue(getDeleteFlagged.IsDeleted);

            var noDeletedTestTables = CrudService
                .GetAll<TestTable>();

            Assert.IsTrue(!noDeletedTestTables.Any());

            DoBaseline();
        }

        [TestMethod]
        public void DeletePermanently()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            CrudService
                .DeletePermanently<TestTable>(entry);

            var getForceDeleted = CrudService
                .Get<TestTable>(entry.Id);

            Assert.IsNull(getForceDeleted);

            DoBaseline();
        }
    }
}