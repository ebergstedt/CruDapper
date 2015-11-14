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

            _crudService
                .Delete<TestTable>(entry);

            var getDeleteFlagged = _crudService
                .Get<TestTable>(entry.Id);

            Assert.IsNotNull(getDeleteFlagged);
            Assert.IsTrue(getDeleteFlagged.IsDeleted);

            DoBaseline();
        }

        [TestMethod]
        public void DeletePermanently()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            _crudService
                .DeletePermanently(entry);

            var getForceDeleted = _crudService
                .Get<TestTable>(entry.Id);

            Assert.IsNull(getForceDeleted);

            DoBaseline();
        }
    }
}