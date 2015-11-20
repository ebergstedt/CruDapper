using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruDapper.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CruDapper.Test
{
    [TestClass]
    public class TestFunctions : BaseService
    {
        [TestMethod]
        public void TestParseIdArray()
        {
            if(Provider != Provider.MsSql)
                return;

            var ids = CrudService
                .Query<int>(@"
SELECT * 
FROM dbo.ParseIdArray('1,2,3,4,5')
");

            Assert.IsTrue(ids.Count() == 5);
        }

        [TestMethod]
        public void Tokenize()
        {
            if (Provider != Provider.MsSql)
                return;

            IEnumerable<string> result = CrudService
                .Query<string>(@"
DECLARE @Tokens VARCHAR(50)

SET @Tokens = 'a, ''b'', ''''c'', ''d'', ''e'''', f, ''1,2,3,4'''

SELECT Token
FROM dbo.Tokenize(@Tokens)
");

            Assert.IsTrue(result.Count() == 5);
        }

        [TestMethod]
        public void PatternReplace()
        {
            if(Provider != Provider.MsSql)
                return;

            var result = CrudService
                .Query<string>(@"
SELECT dbo.PatternReplace('baaa', 'ba%', 'c')
");

            Assert.IsTrue(result.Single() == "c");
        }

        [TestMethod]
        public void SplitString()
        {
            if (Provider != Provider.MsSql)
                return;

            var result = CrudService
                .Query<string>(@"
SELECT * FROM dbo.SplitString('a, b, c, d, e', ',')
");

            Assert.IsTrue(result.Count() == 5);

            var result2 = CrudService
                .Query<string>(@"
SELECT * FROM dbo.SplitString('0100101', '1')
");

            Assert.IsTrue(result2.Count() == 3);

        }
    }
}
