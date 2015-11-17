using System;
using System.Collections.Generic;
using System.Data.Common;
using Dapper;

namespace CruDapper.Infrastructure
{
    public interface IDbMapper : IDapperConnectable
    {
        string ConnectionName { get; set; }
        DbConnection DbConnection { get; }

        IEnumerable<T> GetAll<T>(bool getDeleted = false);
        T GetByPrimaryKey<T>(object primaryKeyValue, bool getDeleted = false);
        T Get<T>(int id, bool getDeleted = false) where T : IDapperable;
        IEnumerable<T> GetByColumn<T>(string column, object value, bool getDeleted = false);
        IEnumerable<T> GetByColumn<T>(WhereArgument whereArgument, bool getDeleted = false);
        IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArguments, bool getDeleted = false);        

        IEnumerable<T> InsertMultipleIdentifiable<T>(IEnumerable<object> entities);
        void InsertMultiple(IEnumerable<object> entities);
        void UpdateMultiple(IEnumerable<object> entities);
        void DeleteMultiple(IEnumerable<object> entities);
    }

    public interface ICrudService : IDapperConnectable
    {
        IEnumerable<T> GetAll<T>(bool getDeleted = false);
        T GetByPrimaryKey<T>(object id, bool getDeleted = false);
        T Get<T>(int id, bool getDeleted = false) where T : IDapperable;
        IEnumerable<T> GetByColumn<T>(string column, object value, bool getDeleted = false);
        IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArgumentDtos, bool getDeleted = false);
        void Put(object obj);
        IEnumerable<T> PutIdentifiable<T>(object obj);
        void Update(object obj);
        void Delete<T>(object obj) where T : IDeletable;
        void DeletePermanently(object obj);
    }

    public interface IDapperConnectable
    {
        IEnumerable<T> Query<T>(string sqlQuery, object parameters = null, int? commandTimeout = null);
        IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null, int? commandTimeout = null);
        SqlMapper.GridReader QueryMultiple(string sqlQuery, object parameters = null, int? commandTimeout = null);
        void Execute(string sqlQuery, object parameters = null, int? commandTimeout = null);
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

    public interface IStandardLogTable : IDeletable, IDateLoggable, IDapperable
    {        
    }

    public interface IStandardTable : IDeletable, IDateLoggable, IUserLoggable, IDapperable
    {
    }
}