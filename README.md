# CruDapper
Dapper extension with CRUD functionality (and some more)

Based on **Dapper** https://github.com/StackExchange/dapper-dot-net

# Features

Currently supports **MS Sql Server** and **Postgres**. You can create extensions for other databases.

CruDapper uses SQL autogeneration and parameterized queries, together with reflection to provide CRUD functionality to the fast and excellent .NET ORM Dapper. **All basic CRUD methods support both single and multiple object execution and reading**. GET methods also support easy filtering on deleted rows (provided the table implements IDeletable).

Please refer to the provided Test project for detailed examples and syntax.

# Example usage
First initialize your database mapper with your connectionstring in App.config. Afterwards you can initialize the provided CrudService.
```c#
IServiceFactory _serviceFactory;
ICrudService _crudService;

_serviceFactory = new ServiceFactory(new MsSqlServerMapper("ConnectionStringName"));
_crudService = serviceFactory.Get<CrudService>();
```

With the crudService we can call some nifty methods.

#### Put a single object, and recieve the Id 
```c#
var entry = new TestTable
{
    SomeData = "data"
};

_crudService
    .Put(entry);

TestTable tableById = _crudService
    .Get<TestTable>(entry.Id);
```

#### Put multiple objects
```c#
var entries = new List<TestTable>();
for (var i = 0; i < 1000; i++)
{
    entries.Add(new TestTable
    {
        SomeData = i.ToString()
    });
}

_crudService
    .Put(entries);
```

#### Get all rows in a table
```c#
IEnumerable<TestTable> allRows = _crudService
    .GetAll<TestTable>();
```

#### Update
```c#
_crudService
    .Update(entry);
```

#### Delete
```c#
_crudService
    .Delete<TestTable>(entry);
```

CruDapper also provides some unusual methods too, which might prove useful.

#### PutIdentifiable
PutIdentifiable makes you able to insert any number of rows, and get their Ids assigned automatically on the return list. This is useful if it's difficult to sort your inserted rows in a table from other unwanted rows.
```c#
var entries = new List<TestIdentifiableTable>();
for (var i = 0; i < 1000; i++)
{
    entries.Add(new TestIdentifiableTable
    {
        SomeData = i.ToString() + 1
    });
}

identifiableTables = _crudService
    .PutIdentifiable<TestIdentifiableTable>(entries);

Assert.IsTrue(identifiableTables.All(t => t.Id > 0)); //evalutes to true
```

# Sample service usage pattern
CruDapper.Services.CrudService can be used as base for specific services, if you want to write SQL directly for more advanced queries. By using the CruDapper overrides (outlined below) **you do not have to worry about disposing your connection** - CruDapper will take care of it. This will hopefully reduce the amount of boilerplate code you need to write.

```c#
IEnumerable<T> Query<T>(string sqlQuery, object parameters = null, int? commandTimeout = null);
IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null, int? commandTimeout = null);
SqlMapper.GridReader QueryMultiple(string sqlQuery, object parameters = null, int? commandTimeout = null);
void Execute(string sqlQuery, object parameters = null, int? commandTimeout = null);
```

By using the ActiveDbConnection CruDapper will provide you with a usable DbConnection, if you want to use the original Dapper DbConnection extension methods.
```c#

public class MyService : CrudService
{
    public MyService(IDbMapper dbMapper)
        : base(dbMapper)
    {
    }
    
    public IEnumerable<MyTable> MyMethodUsingCruDapper()
    {
        //CruDapper automatically manages disposing the connection
        return connection.Query<MyTable>(@"
            SELECT * 
            FROM MyTable AS mt 
            INNER JOIN AnotherTable AS at 
                ON mt.Id = at.Id
        ");
    }
    
    public IEnumerable<MyTable> MyMethodUsingDapperExtensions()
    {
        using (DbConnection connection = ActiveDbConnection)
        {
            // calls the original Dapper DbConnection extension methods making you able to use Dappers full functionality 
            return connection.Query(@"
                DELETE
                FROM MyTable
            ");
        }
    }
}
```

# Complete method list
Will update this with more details. Until then, you may check the provided Test project in the repo.
```c#
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

IEnumerable<T> Query<T>(string sqlQuery, object parameters = null, int? commandTimeout = null);
IEnumerable<dynamic> QueryDynamic(string sqlQuery, object parameters = null, int? commandTimeout = null);
SqlMapper.GridReader QueryMultiple(string sqlQuery, object parameters = null, int? commandTimeout = null);
void Execute(string sqlQuery, object parameters = null, int? commandTimeout = null);
```

# Testing setup
Change your connectionstring in CruDapper.Test App.config and alter the BaseService accordingly.

Apply the PostgresBaseLine.sql or SqlServerBaseLine.sql to your testing database to setup the needed tables.

# TODO

* Cache reflection for performance
* User can specify more Dapper specific parameters such as transactions and timeouts
* New methods to map dynamic query result to Dictionary<string, object> and object[] 
* Pagination / Skip
* Lazy loading if desired
* Get single item by composite primary key
* Merge result into an existing object by comparing field name and type
* Templating
* More functionality?

# License

The MIT License (MIT)
