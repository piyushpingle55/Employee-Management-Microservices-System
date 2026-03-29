# EmployeeService EF Core Database-First Steps

This is a separate document for the EF Core Database-First workflow used in the EmployeeService microservice. Do not edit the original `README.md`.

## Prerequisites
- .NET 9 SDK
- SQL Server instance
- `dotnet-ef` tool:
  - `dotnet tool install --global dotnet-ef`

## Database creation
1. Create DB and table:
   - `CREATE DATABASE EmployeeDataBase;`
   - `USE EmployeeDataBase;`
   - Create table `Employees` with columns: `Id` (uniqueidentifier PK), `Name` (nvarchar(100)), `Email` (nvarchar(100)), `Salary` (decimal(10,2)), `DepartmentId` (uniqueidentifier), `CreatedAt` (datetime2), `UpdatedAt` (datetime2).
2. Verify schema in SSMS.

## Project package setup
### In `Employee.Infrastructure`

```powershell
dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
```

### In `Employee.API`

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
```

## Scaffold from database
From `src\Services\EmployeeService\Employee.Infrastructure`:

```powershell
cd src\Services\EmployeeService\Employee.Infrastructure

dotnet ef dbcontext scaffold "Server=.;Database=EmployeeDataBase;Trusted_Connection=True;TrustServerCertificate=True;" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --project . \
  --startup-project ..\Employee.API \
  --output-dir Data \
  --context EmployeeDbContext \
  --force
```

## Fix namespace collisions
- Use alias when both `Employee.Domain.Entities.Employee` and generated `Employee` class are present.
- In `EmployeeDbContext`: `using Emp = Employee.Domain.Entities.Employee;` and `DbSet<Emp> Employees`.

## Configure DI in API
In `Employee.API\Program.cs`:

```csharp
using Employee.Infrastructure.Data;

builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDb")));

builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
```

## appsettings.json connection string

```json
"ConnectionStrings": {
  "EmployeeDb": "Server=.;Database=EmployeeDataBase;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## Run + test

```powershell
cd src\Services\EmployeeService\Employee.API
dotnet run
```

Open swagger at `https://localhost:5001/swagger`.

## Troubleshooting
- If `EmployeeDbContext` missing: add `using Employee.Infrastructure.Data`.
- If scaffold connection warning appears: move connection string out of `OnConfiguring` and use configuration.
