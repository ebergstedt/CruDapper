﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CruDapper.Code;
using CruDapper.Helpers;
using CruDapper.Infrastructure;
using Dapper;

namespace CruDapper.Services
{
    public class CrudService : ICrudService
    {
        private readonly IDbMapper _dbMapper;

        public CrudService(IDbMapper dbMapper)
        {
            this._dbMapper = dbMapper;
        }

        #region UPDATE

        /// <summary>
        ///     Updates object with matching key fields
        /// </summary>
        public void Update<T>(object obj)
        {
            var enumerable = obj as IEnumerable<T>;
            if (enumerable != null)
            {
                _dbMapper.UpdateMultiple(enumerable);
            }
            else
            {                                
                _dbMapper.UpdateMultiple(new List<T>()
                {
                    (T)obj
                });
            }
        }

        #endregion

        #region GET
        /// <summary>
        ///     Gets all rows in a table
        /// </summary>
        public IEnumerable<T> GetAll<T>(bool getDeleted = false)
        {            
            return _dbMapper.GetAll<T>(getDeleted);
        }

        public IEnumerable<T> GetMany<T>(object primaryKeyValues, bool getDeleted = false)
        {
            return _dbMapper.GetMany<T>(primaryKeyValues, getDeleted);
        }

        public T GetSingle<T>(object primaryKeyValue, bool getDeleted = false)
        {
            return _dbMapper.GetSingle<T>(primaryKeyValue, getDeleted);
        }

        /// <param name="column">Recommended usage is nameof</param>
        public IEnumerable<T> GetByColumn<T>(string column, object value, bool getDeleted = false)
        {
            return _dbMapper.GetByColumn<T>(column, value, getDeleted);
        }

        public IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArgumentDtos, bool getDeleted = false)
        {
            return _dbMapper.GetByColumns<T>(whereArgumentDtos, getDeleted);
        }

        /// <param name="sortColumn">Recommended usage is nameof</param>
        public IEnumerable<T> GetPaginated<T>(string sortColumn, int pageSize = 10, int currentPage = 1,
            OrderBy sortingDirection = OrderBy.Asc)
        {
            return _dbMapper.GetPaginated<T>(sortColumn, pageSize, currentPage, sortingDirection);
        }

        #endregion

        #region PUT

        /// <summary>
        ///     Will assign IDapperable Id for returning object if it's a single object
        /// </summary>
        public void Put<T>(object obj)
        {
            var enumerable = obj as IEnumerable<T>;
            if (enumerable != null)
            {
                _dbMapper.InsertMultiple<T>(enumerable);
            }
            else
            {
                _dbMapper.InsertMultiple<T>(new List<T>()
                {
                    (T)obj
                });
            }
        }

        /// <summary>
        ///     Will assign Dapper Id for all returning objects
        /// </summary>
        public IEnumerable<T> PutIdentifiable<T>(object obj)
        {
            var enumerable = obj as IEnumerable<T>;
            if (enumerable != null)
            {
                return _dbMapper.InsertMultipleIdentifiable<T>(enumerable);
            }
            return _dbMapper.InsertMultipleIdentifiable<T>(new List<T>()
            {
                (T)obj
            });
        }

        #endregion

        #region DELETE
        
        public void Delete<T>(object obj, bool permanently = true)
        {
            if (permanently)
            {
                var enumerable = obj as IEnumerable<T>;
                    if (enumerable != null)
                    {
                        _dbMapper.DeleteMultiple<T>(enumerable);
                    }
                    else
                    {
                        _dbMapper.DeleteMultiple<T>(new List<T>()
                    {
                        (T)obj
                    });
                }
            }
            else if (ValidationHelper.VerifyIDeletable<T>())
            {
                var enumerable = obj as IEnumerable<T>;

                if (enumerable != null)
                {
                    foreach (var o in enumerable)
                    {
                        var deletableItem = o as IDeletable;
                        if (deletableItem != null)
                        {
                            deletableItem.IsDeleted = true;
                        }
                    }
                }
                else
                {
                    var deletable = obj as IDeletable;
                    if (deletable != null)
                    {
                        deletable.IsDeleted = true;
                    }
                }

                Update<T>(obj);
            }
        }

        public void DeleteAll<T>(bool permanently = true)
        {
            var items = GetAll<T>();
            Delete<T>(items, permanently);
        }

        public void DeleteMany<T>(object primaryKeyValues, bool permanently = true)
        {
            var items = GetMany<T>(primaryKeyValues);
            Delete<T>(items, permanently);
        }

        public void DeleteSingle<T>(object primaryKeyValue, bool permanently = true)
        {
            var item = GetSingle<T>(primaryKeyValue);
            Delete<T>(item, permanently);
        }

        /// <param name="column">Recommended usage is nameof</param>
        public void DeleteByColumn<T>(string column, object value, bool permanently = false)
        {
            var byColumn = GetByColumn<T>(column, value);
            Delete<T>(byColumn, permanently);
        }

        public void Merge<T>(object obj)
        {
            var enumerable = obj as IEnumerable<T>;
            if (enumerable != null)
            {
                _dbMapper.MergeMultiple(enumerable);
            }
            else
            {
                _dbMapper.MergeMultiple(new List<T>()
                {
                    (T)obj
                });
            }
        }

        #endregion

        #region Dapper specific

        public IEnumerable<T> Query<T>(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return _dbMapper.Query<T>(sqlQuery, parameters, commandTimeout, retryCount);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return _dbMapper.QueryAsync<T>(sqlQuery, parameters, commandTimeout, retryCount);
        }

        public IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return _dbMapper.QueryDynamic(sqlQuery, parameters, commandTimeout, retryCount);
        }

        public Task<IEnumerable<dynamic>> QueryDynamicAsync(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return _dbMapper.QueryDynamicAsync(sqlQuery, parameters, commandTimeout, retryCount);
        }

        public SqlMapper.GridReader QueryMultiple(DbConnection connection, string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {            
            return _dbMapper.QueryMultiple(connection, sqlQuery, parameters, commandTimeout, retryCount);
        }

        public Task<SqlMapper.GridReader> QueryMultipleAsync(DbConnection connection, string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return _dbMapper.QueryMultipleAsync(connection, sqlQuery, parameters, commandTimeout, retryCount);
        }

        public void Execute(string sqlQuery, object parameters, int? commandTimeout = null, int retryCount = 0)
        {            
            _dbMapper.Execute(sqlQuery, parameters, commandTimeout, retryCount);
        }

        public Task<int> ExecuteAsync(string sqlQuery, object parameters = null, int? commandTimeout = null, int retryCount = 0)
        {
            return _dbMapper.ExecuteAsync(sqlQuery, parameters, commandTimeout, retryCount);
        }

        #endregion

        #region Helpers

        public string ActiveConnectionName
        {
            get { return _dbMapper.ActiveConnectionName; }
        }

        public DbConnection ActiveDbConnection
        {
            get { return _dbMapper.ActiveDbConnection; }
        }
        #endregion
    }
}