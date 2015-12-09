using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using CruDapper.Infrastructure;

namespace CruDapper.BackofficeTest
{
    [Table("CruDapperSchema.TestTable")]
    public class TestTable : StandardTable
    {
        public string SomeData { get; set; }

        //Additional properties can be added which are not in database. This is useful as you will not need to use another intermediary object and will reduce clutter.
        [NotMapped]        
        public string SomeDataNotMapped {
            get { return SomeData + " that is not mapped."; } 
        }
    }
}