# Strategic Planning Tool for Political Parties
The tool provides step-by-step guidance to the political parties in the improvement of key organizational capacities and leads them towards becoming more effective and sustainable institutions. 

<!-- TABLE OF CONTENTS -->
## Table of Contents

- [Strategic Planning Tool for Political Parties](#strategic-planning-tool-for-political-parties)
  - [Table of Contents](#table-of-contents)
  - [About The Project](#about-the-project)
    - [Built With](#built-with)
  - [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
      - [IDE](#ide)
      - [Database](#database)
  - [Usage](#usage)
    - [Structure](#structure)
    - [Connection String](#connection-string)
    - [Database](#database-1)
    - [Run tool](#run-tool)



<!-- ABOUT THE PROJECT -->
## About The Project


### Built With

* `ASP.NET` Core 2.2
* Microsoft SQL Server 2014
* Bootstrap 4.3.1
* JQuery 3.3.
* JQuery UI 1.2.3
* [Metronic](https://keenthemes.com/metronic/)


<!-- GETTING STARTED -->
## Getting Started

In order to run the project you need to follow the given instructions:

### Prerequisites

You can use Visual Studio (recommended) or Visual Studio Code for IDE,

#### IDE

Visual Studio:
* Download [Visual Studio 2019 Community Edition](https://visualstudio.microsoft.com/downloads/?utm_medium=microsoft&utm_source=docs.microsoft.com&utm_campaign=inline+link&utm_content=download+vs2019) with **`ASP.NET` and web developement** workload
* [.NET Core SDK 2.2 or later](https://dotnet.microsoft.com/download/dotnet-core)


Visual Studio Code:
* [Download](https://code.visualstudio.com/download) and install
* [Install Extension for C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
* [.NET Core SDK 2.2 or later](https://dotnet.microsoft.com/download/dotnet-core)

#### Database

The project uses Microsoft SQL Server with Entity Framework Core library.
You can download Develoepr edition from this [link](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).

Don't forget to downlaod [SQL Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)


<!-- USAGE EXAMPLES -->
## Usage

### Structure
The tool is divided into four parts:
* `Core` - Database context with entities and entity configurations, Migration folder
* `Application` - business logic and functionality
* `Resources` - Resource files (with .resx extension) that contains all the texts. `sharedResource.resx` is a primary resource file.
* `Web` - ASP.NET Core project

### Connection String

You can find `appsettings.json` in  `Web`, where it is possible to set a connection string by modifying a value for `PlanningDatabase`

```
"ConnectionStrings": {
    "PlanningDatabase": "Server=YOUR_SERVER;Database=strat_planning_db;Trusted_Connection=True;"
  }
```

### Database

The tool automatically creates database and applies migrations at startup. If this does not cause any exception, database is seeded with default data.

As mentioned, migrations and database seeder are located inside `Core`. 

You can apply migrations from Visual Studio using Package Manager Console that can be found by `Tools -> Nuget Package Manager -> Nuget Package Console`.

Set Default Project to Core and run a command ```Update-Database```.

In a case of using Visual Studio Code, use can perform the same operation using [.Net Core CLI](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet).

You can find a seeder inside `Context/DatabaseInitializer.cs` where seeders are put inside different methods according to data type.

In order to modify questions for a step, you need to look up for certain methods inside the `DatabaseInitializer` class and modify an array of block in accordance to your wishes.

### Run tool

Default use credentials:

Email: `admin@sp.com`
Password: `admin`
