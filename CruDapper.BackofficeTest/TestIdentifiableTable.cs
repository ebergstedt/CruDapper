using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CruDapper.BackofficeTest
{
    [Table("CruDapperSchema.TestIdentifiableTable")]
    public class TestIdentifiableTable : IDapperable, IIdentifiable
    {
        public string SomeData { get; set; }

        [Key]
        [AutoIncrement]
        public int Id { get; set; }

        [Required]
        public Guid RowGuid { get; set; }
    }
}