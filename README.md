# RESTier-CLI
*The command line tool for [RESTier](https://github.com/OData/RESTier)*

## Requirements
- [MSBuild](https://msdn.microsoft.com/en-us/library/dd393573.aspx) that supports C# 6.0, e.g., the one that comes with [Visual Studio 2015](https://www.visualstudio.com/en-us/products/vs-2015-product-editions.aspx).
- An [Entity Framework](http://www.asp.net/entity-framework) supported database.

## How to Build
Open the RESTier.CLI.sln and build the project.

## Get Started with Sample Database
There is a sample database `AdventureWorksLT2012_Data.mdf` located at `/samples`. Now, we build a OData service for it using RESTier-CLI.

1. Execute
~~~
RESTier -c "Server=(LocalDB)\MSSQLLocalDB;AttachDbFilename=AdventureWorksLT2012_Data.mdf;Integrated Security=True;Trusted_Connection=True;" -db sqlserver new
~~~
This will reverse-engineer the database and create a default RESTier Visual Studio project named `AdventureWorksLT2012_Data` for it in the current directory.  
2. Execute
~~~
RESTier.exe build -p AdventureWorksLT2012_Data/AdventureWorksLT2012_Data.sln
~~~
This builds the created RESTier project and produces the OData service.  
3. Execute
~~~
RESTier.exe run -p AdventureWorksLT2012_Data/AdventureWorksLT2012_Data.sln
~~~
This puts the created OData service into action at the default port which is 3447. Now, you can access the service from a browser with the service root URL `http://localhost:3447`. What you will get is a service document for the database :smile:
