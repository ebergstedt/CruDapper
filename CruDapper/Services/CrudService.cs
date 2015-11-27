using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using CruDapper.Code;
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

        public T GetByPrimaryKey<T>(object id, bool getDeleted = false)
        {
            return _dbMapper.GetByPrimaryKey<T>(id, getDeleted);
        }

        /// <summary>
        ///     Get a row by Dapper Id
        /// </summary>
        public T Get<T>(int id, bool getDeleted = false) where T : IDapperable
        {
            return _dbMapper.Get<T>(id, getDeleted);
        }

        public IEnumerable<T> GetByColumn<T>(string column, object value, bool getDeleted = false)
        {
            return _dbMapper.GetByColumn<T>(column, value, getDeleted);
        }

        public IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArgumentDtos, bool getDeleted = false)
        {
            return _dbMapper.GetByColumns<T>(whereArgumentDtos, getDeleted);
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

        /// <summary>
        ///    Sets IsDeleted to true
        /// </summary>
        public void Delete<T>(object obj) where T : IDeletable
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

        /// <summary>
        ///     Deletes row by Id
        /// </summary>
        public void DeletePermanently<T>(object obj)
        {
            var enumerable = obj as IEnumerable<T>;
            if (enumerable != null)
            {
                _dbMapper.DeleteMultiple(enumerable);
            }
            else
            {
                _dbMapper.DeleteMultiple(new List<T>()
                {
                    (T)obj
                });
            }
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

        public IEnumerable<T> Query<T>(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return _dbMapper.Query<T>(sqlQuery, parameters, commandTimeout);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return _dbMapper.QueryAsync<T>(sqlQuery, parameters, commandTimeout);
        }

        public IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return _dbMapper.QueryDynamic(sqlQuery, parameters, commandTimeout);
        }

        public Task<IEnumerable<dynamic>> QueryDynamicAsync(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return _dbMapper.QueryDynamicAsync(sqlQuery, parameters, commandTimeout);
        }

        public SqlMapper.GridReader QueryMultiple(DbConnection connection, string sqlQuery, object parameters = null, int? commandTimeout = null)
        {            
            return _dbMapper.QueryMultiple(connection, sqlQuery, parameters, commandTimeout);
        }

        public Task<SqlMapper.GridReader> QueryMultipleAsync(DbConnection connection, string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return _dbMapper.QueryMultipleAsync(connection, sqlQuery, parameters, commandTimeout);
        }

        public void Execute(string sqlQuery, object parameters, int? commandTimeout = null)
        {            
            _dbMapper.Execute(sqlQuery, parameters, commandTimeout);
        }

        public Task<int> ExecuteAsync(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return _dbMapper.ExecuteAsync(sqlQuery, parameters, commandTimeout);
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