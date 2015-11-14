using System;
using System.Collections.Generic;
using Dapper;

namespace CruDapper
{
    public interface IDbMapper
    {
        string ConnectionName {get; set; }
        IEnumerable<T> GetAll<T>();        
        T GetByPrimaryKey<T>(object primaryKeyValue);
        T Get<T>(int id) where T : IDapperable;
        IEnumerable<T> GetByColumn<T>(string column, object value);        
        IEnumerable<T> GetByColumn<T>(WhereArgument whereArgument);
        IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArguments);
        T GetNondeleted<T>(int id) where T : IDapperable, IDeletable;

        IEnumerable<T> InsertMultipleIdentifiable<T>(IEnumerable<object> entities);
        void InsertMultiple(IEnumerable<object> entities);
        void UpdateMultiple(IEnumerable<object> entities);
        void DeleteMultiple(IEnumerable<object> entities);
    }

    public interface ICrudService
    {
        IEnumerable<T> Get<T>();
        T GetByPrimaryKey<T>(object id);
        T Get<T>(int id) where T : IDapperable;
        IEnumerable<T> GetByColumn<T>(string column, int value);
        IEnumerable<T> GetByColumn<T>(string column, string value);
        IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArgumentDtos);
        T GetNondeleted<T>(int id) where T : IDapperable, IDeletable;
        void Put(object obj);
        IEnumerable<T> PutIdentifiable<T>(object obj);
        void Update(object obj);
        void Delete<T>(object obj) where T : IDeletable;
        void DeletePermanently(object obj);
    }

    public interface IServiceFactory
    {
        T Get<T>();
    }

    public interface IDapperable
    {
        int Id { get; set; }
    }

    public interface IDateLoggable
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }

    public interface IDeletable
    {
        bool IsDeleted { get; set; }
    }

    public interface IIdentifiable
    {
        Guid RowGuid { get; set; }
    }

    public interface IUserLoggable
    {
        int CreatedBy { get; set; }
        int UpdatedBy { get; set; }
    }

    public interface IStandardTable : IDeletable, IDateLoggable, IUserLoggable, IDapperable
    {
    }
}