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
            var entry = BaseLineAndPutAndReturnTestTable();

            CrudService
                .Delete<TestTable>(entry);

            var getDeleteFlagged = CrudService
                .GetSingle<TestTable>(entry.Id, true);

            Assert.IsNotNull(getDeleteFlagged);
            Assert.IsTrue(getDeleteFlagged.IsDeleted);

            var noDeletedTestTables = CrudService
                .GetAll<TestTable>();

            Assert.IsTrue(!noDeletedTestTables.Any());

            DoBaseline();
        }

        [TestMethod]
        public void DeleteAll()
        {
            var entries = BaseLineAndPutAndReturnTestTables();

            CrudService
                .Put<TestTable>(entries);            

            CrudService
                .DeleteAll<TestTable>();

            var testTables = CrudService
                .GetAll<TestTable>();

            Assert.IsTrue(!testTables.Any());
        }

        [TestMethod]
        public void DeleteMany()
        {
            var entries = BaseLineAndPutAndReturnTestTables();

            CrudService
                .Put<TestTable>(entries);

            var testTables = entries.Take(10);

            CrudService
                .DeleteMany<TestTable>(testTables.Select(s => s.Id));

            var getMany = CrudService
                .GetMany<TestTable>(testTables.Select(s => s.Id));

            Assert.IsTrue(!getMany.Any());
        }

        [TestMethod]
        public void DeleteSingle()
        {
            var entry = BaseLineAndPutAndReturnTestTable();

            CrudService
                .DeleteSingle<TestTable>(entry.Id);

            var testTable = CrudService
                .GetSingle<TestTable>(entry.Id);

            Assert.IsNull(testTable);
        }

        [TestMethod]
        public void DeleteByColumn()
        {
            var entry = BaseLineAndPutAndReturnTestTable();

            CrudService
                .DeleteByColumn<TestTable>("CreatedAt", entry.CreatedAt);

            var testTables = CrudService
                .GetByColumn<TestTable>("CreatedAt", entry.CreatedAt);

            Assert.IsFalse(testTables.Any());
        }

        [TestMethod]
        public void DeletePermanently()
        {
            var entry = BaseLineAndPutAndReturnTestTable();

            CrudService
                .DeletePermanently<TestTable>(entry);

            var getForceDeleted = CrudService
                .GetSingle<TestTable>(entry.Id);

            Assert.IsNull(getForceDeleted);

            DoBaseline();
        }

        [TestMethod]
        public void DeletePermanentlyByColumn()
        {
            var entry = BaseLineAndPutAndReturnTestTable();

            CrudService
                .DeletePermanentlyByColumn<TestTable>("CreatedAt", entry.CreatedAt);

            var testTables = CrudService
                .GetByColumn<TestTable>("CreatedAt", entry.CreatedAt, true);

            Assert.IsFalse(testTables.Any());
        }

        [TestMethod]
        public void DeleteAllPermanently()
        {
            BaseLineAndPutAndReturnTestTable();

            CrudService
                .DeleteAllPermanently<TestTable>();

            var testTables = CrudService
                .GetAll<TestTable>(true);
            
            Assert.IsTrue(!testTables.Any());
        }

        [TestMethod]
        public void DeleteManyPermanently()
        {
            var entries = BaseLineAndPutAndReturnTestTables();

            var ids = entries.Take(10).Select(s => s.Id);

            CrudService
                .DeleteManyPermanently<TestTable>(ids);

            var testTables = CrudService.GetMany<TestTable>(ids, true);

            Assert.IsTrue(!testTables.Any());
        }

        [TestMethod]
        public void DeleteSinglePermanently()
        {
            var entry = BaseLineAndPutAndReturnTestTable();

            CrudService
                .DeleteSinglePermanently<TestTable>(entry.Id);

            var testTable = CrudService
                .GetSingle<TestTable>(entry.Id);

            Assert.IsNull(testTable);
        }
    }
}