using System.ComponentModel.DataAnnotations.Schema;
using CruDapper.Infrastructure;

namespace CruDapper.BackofficeTest
{
    [Table("CruDapperSchema.TestTable")]
    public class TestTable : StandardTable
    {
        public string SomeData { get; set; }
    }
}