# CruDapper
Dapper extension with CRUD functionality (and some more).

Based on **[Dapper](https://github.com/StackExchange/dapper-dot-net)** 

Use **CruDapper** if you are tired of writing boilerplate for Dapper, and you are missing some key features from the other existing CRUD libraries for Dapper, such as async, retries, composite primary keys, multiple object inserts and updates, automatic field value assignment, merges, null validation, ignored fields, temporary and permanent row deletion. Read below to find out more.

# [Nuget](https://www.nuget.org/packages/CruDapper/)

    PM> Install-Package CruDapper

# Features

* Currently supports **MS Sql Server** and **Postgres** (work in progress - some methods are missing). You can create extensions for other databases.

* CruDapper uses SQL autogeneration and parameterized queries, together with reflection to provide CRUD functionality to the fast and excellent .NET ORM Dapper. **All basic CRUD methods support both single and multiple object execution and reading**. 

* Unlike most other Dapper CRUD extensions, **GET methods by primary key takes whatever object you have as primary key: you are not limited to integers.**.

GET methods also support easy filtering rows that have been flagged as deleted (provided the table implements IDeletable). 
```c#
IEnumerable<T> GetAll<T>(bool getDeleted = false);
```

It is similarly easy to both flag rows as deleted, or just delete them outright 
```c#
void DeleteAll<T>(bool permanently = true);
```

* CruDapper features **automatic value assignment of interfaces upon any CRUD execution**, such as setting UpdatedAt to the current date when using Update. Check **DefaultValueMapper.cs** if you wish to implement or change the behavior.

* CruDapper **caches reflection results for improved performance**, just like Dapper.

* CruDapper **provides an easy interface for data queries, without using statements for your database connection and transaction scope**. This will remove a lot of boilerplate code clutter from your database services. The below example automatically enlists both transactionscope and your connectionstring.

```c#
var myQueryResult = _crudService.Query<TestTable>("SELECT * FROM TestTable");
```

* CruDapper can **retry failed connections** as many times as you want, thanks to internal **[Polly](/https://github.com/App-vNext/Polly)** integration.

```c#
var myQueryResult = _crudService.Query<TestTable>("SELECT * FROM TestTable", retryCount: 3);
```

Please refer to the provided Test project for detailed examples and syntax.

# Example usage
First initialize your database mapper with your connectionstring in App.config. Afterwards you can initialize the provided CrudService.
This is preferably done with [dependency injection](http://autofac.org/).
```c#
ICrudService _crudService = new CrudService(new MsSqlServerMapper("DefaultConnection", new DefaultValueMapper()));
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
        return Query<MyTable>(@"
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
            // calls the original Dapper DbConnection extension methods making you able to use 
            //Dappers full original functionality, if you want to bypass CruDapper.
            connection.Open();
            return connection.Query(@"
                DELETE
                FROM MyTable
            ");
        }
    }
}
```

# Attributes

CruDapper needs attributes on your DTO's to function.

**[Table("Schema.TableName")]** is your schema and table name. This must be set at your class declaration. Your class name does not need to correspond to the attribute parameter, but the attribute parameter must be the same name as the schema+table in your database.

```C#
 [Table("MySchema.MyTable")]
 public class MyTable (...)
```

**[Key]** must be set if your table has a key. Currently composite keys are not supported.
```c#
[Key]
public int Id { get; set; }
```
    
**[AutoIncrement]** must be set on keys that are autoincrement.
```c#
[Key]
[AutoIncrement]
public int Id { get; set; }
```

**[Required]** will be evaluated by CruDapper to be not null, else an Exception is thrown.

```c#
 [Required]
 public DateTime CreatedAt { get; set; }
```

**[NotMapped]** will be ignored by CruDapper. This is useful as you will not need to use another intermediary object and will reduce clutter.     

```c#
[NotMapped]        
public string SomeData {
    get { return Id + " that is not mapped."; } 
}
```

# Complete method list

Check the .Test project for a more in-depth description of the usage of the provided functionality below.

```c#
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
```

# Testing setup
Change your connectionstring in CruDapper.Test App.config and alter the BaseService accordingly.

Apply the TestBaslines/TestPostgresBaseLine.sql or TestBaslines/TestSqlServerBaseLine.sql to your testing database to setup the needed tables.

# TODO

* Column name attribute
* New methods to map dynamic query result to Dictionary<string, object> and object[] 
* Hold off db execution for lazy loading if specified
* Templating
* More functionality?
* 

# Contributors

[ebergstedt](https://github.com/ebergstedt)

# Contributing

**Getting started with Git and GitHub**

* **[Setting up Git for Windows and connecting to GitHub](https://help.github.com/articles/set-up-git/)**
* **[Forking a GitHub repository](https://help.github.com/articles/fork-a-repo/)**
* **[The simple guide to GIT guide](http://rogerdudler.github.io/git-guide/)**
* **[Open an issue](https://github.com/ebergstedt/ProtoBuffer/issues)** if you encounter a bug or have a suggestion for improvements/features
Once you're familiar with Git and GitHub, clone the repository and start contributing.

# License

The MIT License (MIT)

Copyright (c) 2016 ebergstedt

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
