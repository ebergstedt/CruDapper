using System.Collections.Generic;

namespace CruDapper.Services
{
    public class CrudService : ICrudService
    {
        protected readonly IDbMapper dbHelper;

        public CrudService(IDbMapper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        #region UPDATE
        
        /// <summary>
        /// Updates object with matching key fields
        /// </summary>        
        public void Update(object obj)
        {
            var enumerable = obj as IEnumerable<object>;
            if (enumerable != null)
            {
                dbHelper.UpdateMultiple(enumerable);
            }
            else
            {
                dbHelper.UpdateMultiple(new List<object>
                {
                    obj
                });
            }
        }

        #endregion

        #region GET

        /// <summary>
        /// Gets all rows in a table
        /// </summary>
        public IEnumerable<T> Get<T>()
        {
            return dbHelper.GetAll<T>();
        }

        public T GetByPrimaryKey<T>(object id)
        {
            return dbHelper.GetByPrimaryKey<T>(id);
        }

        /// <summary>
        /// Get a row by Dapper Id
        /// </summary>
        public T Get<T>(int id) where T : IDapperable
        {
            return dbHelper.Get<T>(id);
        }

        public IEnumerable<T> GetByColumn<T>(string column, int value)
        {
            return dbHelper.GetByColumn<T>(column, value);
        }

        public IEnumerable<T> GetByColumn<T>(string column, string value)
        {
            return dbHelper.GetByColumn<T>(column, value);
        }

        public IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArgumentDtos)
        {
            return dbHelper.GetByColumns<T>(whereArgumentDtos);
        }

        /// <summary>
        /// Gets an entry where IsDeleted is null or false
        /// </summary>
        public T GetNondeleted<T>(int id) where T : IDapperable, IDeletable
        {
            return dbHelper.GetNondeleted<T>(id);
        }

        #endregion

        #region PUT
        /// <summary>
        /// Will assign IDapperable Id for returning object if it's a single object
        /// </summary>        
        public void Put(object obj)
        {
            var enumerable = obj as IEnumerable<object>;
            if (enumerable != null)
            {
                dbHelper.InsertMultiple(enumerable);
            }
            else
            {
                dbHelper.InsertMultiple(new List<object>
                {
                    obj
                });
            }
        }

        /// <summary>
        /// Will assign Dapper Id for all returning objects
        /// </summary>
        public IEnumerable<T> PutIdentifiable<T>(object obj)
        {
            var enumerable = obj as IEnumerable<object>;
            if (enumerable != null)
            {
                return dbHelper.InsertMultipleIdentifiable<T>(enumerable);
            }
            return dbHelper.InsertMultipleIdentifiable<T>(new List<object>
            {
                obj
            });
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Sets IsDeleted to true
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
        /// Deletes row by Id
        /// </summary>        
        public void DeletePermanently(object obj)
        {
            var enumerable = obj as IEnumerable<object>;
            if (enumerable != null)
            {
                dbHelper.DeleteMultiple(enumerable);
            }
            else
            {
                dbHelper.DeleteMultiple(new List<object>
                {
                    obj
                });
            }
        }
        #endregion
    }
}