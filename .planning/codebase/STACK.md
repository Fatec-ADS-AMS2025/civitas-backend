# Stack

## Runtime And Language
- The backend is an ASP.NET Core Web API targeting `net9.0` in `Civitas.WebAPI/Civitas.WebAPI.csproj`.
- The codebase is C# with nullable reference types and implicit `using`s enabled in the main and test projects.
- XML documentation is generated for the web project via `<GenerateDocumentationFile>true</GenerateDocumentationFile>`.

## Application Frameworks
- `Civitas.WebAPI/Program.cs` uses the minimal hosting model with `WebApplication.CreateBuilder`.
- Controllers are standard ASP.NET Core MVC controllers under `Civitas.WebAPI/Controllers/` plus one lowercase outlier at `Civitas.WebAPI/controllers/FornecedorController.cs`.
- Swagger/OpenAPI is wired through `Microsoft.AspNetCore.OpenApi` and `Swashbuckle.AspNetCore`.
- Authentication uses `Microsoft.AspNetCore.Authentication.JwtBearer`.
- Data access uses Entity Framework Core with PostgreSQL via `Microsoft.EntityFrameworkCore` and `Npgsql.EntityFrameworkCore.PostgreSQL`.

## Main Packages
- `AutoMapper` 15.0.1 is registered in `Civitas.WebAPI/Program.cs` with assembly scanning.
- `BCrypt.Net-Next` 4.1.0 is used for password hashing in `Civitas.WebAPI/Services/Security/BCryptPasswordHashService.cs`.
- `Microsoft.AspNetCore.Authentication.JwtBearer` 9.0.9 powers JWT validation.
- `Microsoft.EntityFrameworkCore` 9.0.9, `Microsoft.EntityFrameworkCore.Design` 9.0.9, and `Microsoft.EntityFrameworkCore.Tools` 9.0.9 support persistence and migrations.
- `Npgsql.EntityFrameworkCore.PostgreSQL` 9.0.4 is the production database provider.
- `Swashbuckle.AspNetCore` 9.0.6 provides interactive API docs.

## Project Layout
- The solution lives at `Civitas.WebAPI/Civitas.WebAPI.sln`.
- The web project contains `Controllers/`, `Data/`, `Services/`, `Objects/`, `Migrations/`, and `sql/`.
- The test project lives at `Civitas.WebAPI.Tests/Civitas.WebAPI.Tests.csproj` and references the web project directly.
- The EF model is centralized in `Civitas.WebAPI/Data/AppDbContext.cs` and configured through builder classes in `Civitas.WebAPI/Data/Builders/`.

## Configuration Files
- `Civitas.WebAPI/appsettings.json` stores the PostgreSQL connection string and JWT settings.
- `Civitas.WebAPI/Properties/launchSettings.json` defines local HTTP and HTTPS profiles on ports `5210` and `7058`.
- `Civitas.WebAPI/Civitas.WebAPI.http` is available as a manual request scratchpad.
- `README.md` describes the same stack from a human-facing perspective, but the source of truth for runtime wiring is `Program.cs` and the csproj files.

## Tooling Choices
- The repository relies on the .NET CLI ecosystem rather than a separate Node or frontend toolchain.
- The test project uses xUnit through `Microsoft.NET.Test.Sdk`, `xunit`, and `xunit.runner.visualstudio`.
- `Microsoft.AspNetCore.Mvc.Testing` is used for in-memory integration tests.
- `Microsoft.EntityFrameworkCore.Sqlite` is used only in tests to replace PostgreSQL with an in-memory SQLite database.
- The repo includes build outputs under `bin/` and `obj/`, but those are generated artifacts rather than source dependencies.
