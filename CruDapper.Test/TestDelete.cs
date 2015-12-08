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
        public void DeleteVariant()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            CrudService
                .Delete<TestTable>(entry.Id);

            var testTable = CrudService
                .Get<TestTable>(entry.Id);

            Assert.IsNull(testTable);
        }

        [TestMethod]
        public void DeleteByPrimaryKey()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            CrudService
                .DeleteByPrimaryKey<TestTable>(entry.Id);

            var testTable = CrudService
                .Get<TestTable>(entry.Id);

            Assert.IsNull(testTable);
        }

        [TestMethod]
        public void DeleteByColumn()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            CrudService
                .DeleteByColumn<TestTable>("CreatedAt", entry.CreatedAt);

            var testTables = CrudService
                .GetByColumn<TestTable>("CreatedAt", entry.CreatedAt);

            Assert.IsFalse(testTables.Any());
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

        [TestMethod]
        public void DeletePermanentlyByColumn()
        {
            var entry = BaseLineAndPutAndReturnEntry();

            CrudService
                .DeletePermanentlyByColumn<TestTable>("CreatedAt", entry.CreatedAt);

            var testTables = CrudService
                .GetByColumn<TestTable>("CreatedAt", entry.CreatedAt, true);

            Assert.IsFalse(testTables.Any());
        }
    }
}