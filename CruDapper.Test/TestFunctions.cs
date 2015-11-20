using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CruDapper.Test
{
    [TestClass]
    public class TestFunctions : BaseService
    {
        [TestMethod]
        public void TestParseIdArray()
        {
            var ids = CrudService
                .Query<int>(@"
SELECT * 
FROM dbo.ParseIdArray('1,2,3,4,5')
");

            Assert.IsTrue(ids.Count() == 5);
        }
    }
}
