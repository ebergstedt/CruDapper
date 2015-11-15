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
                .Get<TestTable>(entry.Id);

            Assert.IsNotNull(getDeleteFlagged);
            Assert.IsTrue(getDeleteFlagged.IsDeleted);

            DoBaseline();
        }

        [TestMethod]
        public void DeletePermanently()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            CrudService
                .DeletePermanently(entry);

            var getForceDeleted = CrudService
                .Get<TestTable>(entry.Id);

            Assert.IsNull(getForceDeleted);

            DoBaseline();
        }
    }
}