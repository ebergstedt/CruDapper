using System.Collections.Generic;
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
        public void Update(object obj)
        {
            var enumerable = obj as IEnumerable<object>;
            if (enumerable != null)
            {
                _dbMapper.UpdateMultiple(enumerable);
            }
            else
            {
                _dbMapper.UpdateMultiple(new List<object>
                {
                    obj
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
        public void Put(object obj)
        {
            var enumerable = obj as IEnumerable<object>;
            if (enumerable != null)
            {
                _dbMapper.InsertMultiple(enumerable);
            }
            else
            {
                _dbMapper.InsertMultiple(new List<object>
                {
                    obj
                });
            }
        }

        /// <summary>
        ///     Will assign Dapper Id for all returning objects
        /// </summary>
        public IEnumerable<T> PutIdentifiable<T>(object obj)
        {
            var enumerable = obj as IEnumerable<object>;
            if (enumerable != null)
            {
                return _dbMapper.InsertMultipleIdentifiable<T>(enumerable);
            }
            return _dbMapper.InsertMultipleIdentifiable<T>(new List<object>
            {
                obj
            });
        }

        #endregion

        #region DELETE

        /// <summary>
        ///     Sets IsDeleted to true
        /// </summary>
        public void Delete<T>(object obj) where T : IDeletable
        {
            var enumerable = obj as IEnumerable<object>;

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

            Update(obj);
        }

        /// <summary>
        ///     Deletes row by Id
        /// </summary>
        public void DeletePermanently(object obj)
        {
            var enumerable = obj as IEnumerable<object>;
            if (enumerable != null)
            {
                _dbMapper.DeleteMultiple(enumerable);
            }
            else
            {
                _dbMapper.DeleteMultiple(new List<object>
                {
                    obj
                });
            }
        }

        #endregion

        #region Dapper specific
        public IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return _dbMapper.QueryDynamic(sqlQuery, parameters, commandTimeout);
        }

        public IEnumerable<T> Query<T>(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return _dbMapper.Query<T>(sqlQuery, parameters, commandTimeout);
        }

        public SqlMapper.GridReader QueryMultiple(string sqlQuery, object parameters = null, int? commandTimeout = null)
        {
            return _dbMapper.QueryMultiple(sqlQuery, parameters, commandTimeout);
        }

        public void Execute(string sqlQuery, object parameters, int? commandTimeout = null)
        {            
            _dbMapper.Execute(sqlQuery, parameters, commandTimeout);
        }
        #endregion
    }
}