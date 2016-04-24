using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CruDapper.Code;
using Dapper;

namespace CruDapper.Infrastructure
{
    public interface IValueMapper
    {
        void AssignValues<T>(ref IEnumerable<T> entities);
    }

    public interface IDbMapper : IDapperConnectable
    {
        string ActiveConnectionName { get; set; }
        DbConnection ActiveDbConnection { get; }

        IEnumerable<T> GetAll<T>(bool getDeleted = false);
        IEnumerable<T> GetMany<T>(
                                  object primaryKeyValue, 
                                  bool getDeleted = false);
        T GetSingle<T>(
                       object primaryKeyValue, 
                       bool getDeleted = false);

        IEnumerable<T> GetByColumn<T>(
                                      string column, 
                                      object value, 
                                      bool getDeleted = false);

        IEnumerable<T> GetByColumn<T>(
                                      WhereArgument whereArgument, 
                                      bool getDeleted = false);

        IEnumerable<T> GetByColumns<T>(
                                       List<WhereArgument> whereArguments, 
                                       bool getDeleted = false);

        IEnumerable<T> GetPaginated<T>(
                                       string sortColumn, 
                                       int pageSize = 10, 
                                       int currentPage = 1,
                                       OrderBy sortingDirection = OrderBy.Asc);
            
        IEnumerable<T> InsertMultipleIdentifiable<T>(IEnumerable<T> entities);

        void InsertMultiple<T>(IEnumerable<T> entities);

        void UpdateMultiple<T>(IEnumerable<T> entities);

        void DeleteMultiple<T>(IEnumerable<T> entities);

        void MergeMultiple<T>(IEnumerable<T> entities);

        void DeleteAll<T>();
    }

    public interface ICrudService : IDapperConnectable
    {
        string ActiveConnectionName { get; }

        DbConnection ActiveDbConnection { get; }

        IEnumerable<T> GetAll<T>(bool getDeleted = false);

        IEnumerable<T> GetMany<T>(
                                  object primaryKeyValues, 
                                  bool getDeleted = false);

        T GetSingle<T>(
                       object primaryKeyValue, 
                       bool getDeleted = false);

        IEnumerable<T> GetByColumn<T>(
                                      string column, 
                                      object value, 
                                      bool getDeleted = false);

        IEnumerable<T> GetByColumns<T>(
                                       List<WhereArgument> whereArgumentDtos, 
                                       bool getDeleted = false);

        IEnumerable<T> GetPaginated<T>(
                                       string sortColumn, 
                                       int pageSize = 10, 
                                       int currentPage = 1,
                                       OrderBy sortingDirection = OrderBy.Asc);

        void Put<T>(object obj);

        IEnumerable<T> PutIdentifiable<T>(object obj);

        void Update<T>(object obj);

        void Delete<T>(
                       object obj, 
                       bool permanently = true);

        void DeleteAll<T>(bool permanently = true);

        void DeleteMany<T>(
                           object primaryKeyValues, 
                           bool permanently = true);

        void DeleteSingle<T>(
                             object primaryKeyValue, 
                             bool permanently = true);

        void DeleteByColumn<T>(
                               string column, 
                               object value, 
                               bool permanently = true);

        void Merge<T>(object obj);
    }

    public interface IDapperConnectable
    {
        IEnumerable<T> Query<T>(
                                string sqlQuery, 
                                object parameters = null, 
                                int? commandTimeout = null, 
                                int retryCount = 0);

        Task<IEnumerable<T>> QueryAsync<T>(
                                           string sqlQuery, 
                                           object parameters = null, 
                                           int? commandTimeout = null,
                                           int retryCount = 0);

        IEnumerable<dynamic> QueryDynamic(
                                          string sqlQuery, 
                                          object parameters = null, 
                                          int? commandTimeout = null,
                                          int retryCount = 0);

        Task<IEnumerable<dynamic>> QueryDynamicAsync(
                                                     string sqlQuery, 
                                                     object parameters = null,
                                                     int? commandTimeout = null,
                                                     int retryCount = 0);

        SqlMapper.GridReader QueryMultiple(
                                           DbConnection connection, 
                                           string sqlQuery, 
                                           object parameters = null, 
                                           int? commandTimeout = null,
                                           int retryCount = 0);

        Task<SqlMapper.GridReader> QueryMultipleAsync(
                                                      DbConnection connection, 
                                                      string sqlQuery, 
                                                      object parameters = null, 
                                                      int? commandTimeout = null,
                                                      int retryCount = 0);

        void Execute(
                     string sqlQuery, 
                     object parameters = null, 
                     int? commandTimeout = null,
                     int retryCount = 0);

        Task<int> ExecuteAsync(
                               string sqlQuery, 
                               object parameters = null, 
                               int? commandTimeout = null,
                               int retryCount = 0);
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