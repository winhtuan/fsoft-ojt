Install dependencies
```bash
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    dotnet tool install --global dotnet-ef
```
Run Code First
```bash
    dotnet ef migrations add InitialCreate
    dotnet ef database update

    dotnet ef migrations remove
```
Run when models change
```bash
    dotnet ef migrations add Plantpedia_v1.4.1
    dotnet ef database update
```
Create SQL file
```bash
    dotnet ef migrations script -o migration.sql
```
