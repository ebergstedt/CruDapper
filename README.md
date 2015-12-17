# CruDapper
Dapper extension with CRUD functionality (and some more)

Based on **Dapper** https://github.com/StackExchange/dapper-dot-net

# Features

Currently supports **MS Sql Server** and **Postgres** (work in progress - some methods are missing). You can create extensions for other databases.

CruDapper uses SQL autogeneration and parameterized queries, together with reflection to provide CRUD functionality to the fast and excellent .NET ORM Dapper. **All basic CRUD methods support both single and multiple object execution and reading**. 

GET methods also support easy filtering rows that have been flagged as deleted (provided the table implements IDeletable). 
```c#
IEnumerable<T> GetAll<T>(bool getDeleted = false);
```

It is similarly easy to both flag rows as deleted, or just delete them outright 
```c#
void DeleteAll<T>(bool permanently = true);
```

CruDapper features **automatic value assignment of interfaces upon any CRUD execution**, such as setting UpdatedAt to the current date when using Update. Check InterfaceHelper.cs if you wish to implement or change the behavior.

CruDapper **caches reflection results for improved performance**, just like Dapper.

CruDapper **automatically enlists transactions** - you will not need to worry about partial data corruption if your queries throws an exception midway.

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
    .Put<TestTable>(entry);

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
    .Put<TestTable>(entries);
```

#### Get all rows in a table
```c#
IEnumerable<TestTable> allRows = _crudService
    .GetAll<TestTable>();
```

#### Update
```c#
_crudService
    .Update<TestTable>(entry);
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

#### Merge

Merge will perform a merge command, performing update on the entries that already exists based on keys, and inserts if the keys do not exist ( = they are default values).
```c#
_crudService
    .Merge<TestTable>(entries);
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
    
    public IEnumerable<MyTable> MyMethodUsingStandardDapperExtensions()
    {
        using (DbConnection connection = ActiveDbConnection)
        {
            // calls the original Dapper DbConnection extension methods making you able to use Dappers full functionality 
            connection.Open();
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
IEnumerable<T> GetMany<T>(object primaryKeyValues, bool getDeleted = false);
T GetSingle<T>(object primaryKeyValue, bool getDeleted = false);
IEnumerable<T> GetByColumn<T>(string column, object value, bool getDeleted = false);
IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArgumentDtos, bool getDeleted = false);
IEnumerable<T> GetPaginated<T>(string sortColumn, int pageSize = 10, int currentPage = 1,
OrderBy sortingDirection = OrderBy.Asc);

void Put<T>(object obj);
IEnumerable<T> PutIdentifiable<T>(object obj);

void Update<T>(object obj);

void Delete<T>(object obj, bool permanently = true);
void DeleteAll<T>(bool permanently = true);
void DeleteMany<T>(object primaryKeyValues, bool permanently = true);
void DeleteSingle<T>(object primaryKeyValue, bool permanently = true);
void DeleteByColumn<T>(string column, object value, bool permanently = true);

void Merge<T>(object obj);
```

# Testing setup
Change your connectionstring in CruDapper.Test App.config and alter the BaseService accordingly.

Apply the TestBaslines/TestPostgresBaseLine.sql or TestBaslines/TestSqlServerBaseLine.sql to your testing database to setup the needed tables.

# TODO

* Async
* User can specify more Dapper specific parameters such as transactions and timeouts
* New methods to map dynamic query result to Dictionary<string, object> and object[] 
* Hold off db execution for lazy loading if specified
* Get stuff by composite primary keys
* Templating
* More functionality?

# License

The MIT License (MIT)

Copyright (c) 2016 Erik Bergstedt

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
