using System;
using System.ComponentModel.DataAnnotations;

namespace CruDapper.Infrastructure
{
    public abstract class DapperableTable : IDapperable
    {
        [Key]
        [AutoIncrement]
        public int Id { get; set; }
    }

    public abstract class DateLoggableTable : IDateLoggable
    {
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }

    public abstract class DeletableTable : IDeletable
    {
        [Required]
        public bool IsDeleted { get; set; }
    }

    public abstract class IdentifiableTable : IIdentifiable
    {
        [Required]
        public Guid RowGuid { get; set; }
    }

    public abstract class UserLoggableTable : IUserLoggable
    {
        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int UpdatedBy { get; set; }
    }

    public abstract class StandardTable : IStandardTable
    {
        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int UpdatedBy { get; set; }

        [Key]
        [AutoIncrement]
        public int Id { get; set; }
    }

    public abstract class StandardLogTable : IStandardLogTable
    {
        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Key]
        [AutoIncrement]
        public int Id { get; set; }
    }
}