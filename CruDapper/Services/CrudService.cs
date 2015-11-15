using System.Collections.Generic;
using CruDapper.Infrastructure;
using Dapper;

namespace CruDapper.Services
{
    public class CrudService : ICrudService, IDapperConnectable
    {
        protected readonly IDbMapper DbHelper;

        public CrudService(IDbMapper dbHelper)
        {
            this.DbHelper = dbHelper;
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
                DbHelper.UpdateMultiple(enumerable);
            }
            else
            {
                DbHelper.UpdateMultiple(new List<object>
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
        public IEnumerable<T> GetAll<T>()
        {
            return DbHelper.GetAll<T>();
        }

        public T GetByPrimaryKey<T>(object id)
        {
            return DbHelper.GetByPrimaryKey<T>(id);
        }

        /// <summary>
        ///     Get a row by Dapper Id
        /// </summary>
        public T Get<T>(int id) where T : IDapperable
        {
            return DbHelper.Get<T>(id);
        }

        public IEnumerable<T> GetByColumn<T>(string column, object value)
        {
            return DbHelper.GetByColumn<T>(column, value);
        }

        public IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArgumentDtos)
        {
            return DbHelper.GetByColumns<T>(whereArgumentDtos);
        }

        /// <summary>
        ///     Gets an entry where IsDeleted is null or false
        /// </summary>
        public T GetNondeleted<T>(int id) where T : IDapperable, IDeletable
        {
            return DbHelper.GetNondeleted<T>(id);
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
                DbHelper.InsertMultiple(enumerable);
            }
            else
            {
                DbHelper.InsertMultiple(new List<object>
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
                return DbHelper.InsertMultipleIdentifiable<T>(enumerable);
            }
            return DbHelper.InsertMultipleIdentifiable<T>(new List<object>
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
                DbHelper.DeleteMultiple(enumerable);
            }
            else
            {
                DbHelper.DeleteMultiple(new List<object>
                {
                    obj
                });
            }
        }

        #endregion

        #region Dapper specific

        #endregion

        public IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null)
        {
            return DbHelper.QueryDynamic(sqlQuery, parameters);
        }

        public IEnumerable<T> Query<T>(string sqlQuery, object parameters = null)
        {
            return DbHelper.Query<T>(sqlQuery, parameters);
        }

        public SqlMapper.GridReader QueryMultiple(string sqlQuery, object parameters = null)
        {
            return DbHelper.QueryMultiple(sqlQuery, parameters);
        }

        public void Execute(string sqlQuery, object parameters)
        {
            DbHelper.Execute(sqlQuery, parameters);
        }
    }
}