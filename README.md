# CruDapper
Dapper extension with CRUD functionality (and some more)

# Features

Based on **Dapper** https://github.com/StackExchange/dapper-dot-net

Currently supports **MS Sql Server** and **Postgres**. You can create extensions for other databases.

CruDapper uses SQL autogeneration and parameterized queries, together with reflection to provide CRUD functionality to the fast and excellent .NET ORM Dapper. All basic CRUD methods support both single and multiple object execution and reading.

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

# Complete method list
Will update this with more details. Until then, you may check the provided Test project in the repo.
```c#
IEnumerable<T> GetAll<T>();
T GetByPrimaryKey<T>(object id);
T Get<T>(int id) where T : IDapperable;
IEnumerable<T> GetByColumn<T>(string column, object value);
IEnumerable<T> GetByColumns<T>(List<WhereArgument> whereArgumentDtos);
T GetNondeleted<T>(int id) where T : IDapperable, IDeletable;
void Put(object obj);
IEnumerable<T> PutIdentifiable<T>(object obj);
void Update(object obj);
void Delete<T>(object obj) where T : IDeletable;
void DeletePermanently(object obj);
```

# Testing setup
Change your connectionstring in CruDapper.Test App.config and alter the BaseService accordingly.

Apply the PostgresBaseLine.sql or SqlServerBaseLine.sql to your testing database to setup the needed tables.

# License

MIT