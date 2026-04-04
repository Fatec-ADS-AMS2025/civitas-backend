# Structure

## Top-Level Layout
- `Civitas.WebAPI/` contains the API application, startup, controllers, data access, services, DTOs, models, and configuration.
- `Civitas.WebAPI.Tests/` contains integration-style endpoint tests and repository performance checks.
- `documentation/` contains legacy project documentation and sprint artifacts.
- `.planning/codebase/` is reserved for repository mapping notes.

## Solution And Project Files
- `Civitas.WebAPI/Civitas.WebAPI.sln` is the solution file.
- `Civitas.WebAPI/Civitas.WebAPI.csproj` is the main web project.
- `Civitas.WebAPI.Tests/Civitas.WebAPI.Tests.csproj` is the test project referencing the web project.
- `Civitas.WebAPI/Civitas.WebAPI.http` is the local HTTP scratch file for endpoint calls.

## Directory Map
### `Civitas.WebAPI/Controllers/`
HTTP entry points for the API. Each controller is named after the resource it exposes, for example:

- `AuthController.cs`
- `UsuarioController.cs`
- `DespesaController.cs`
- `DocumentoController.cs`
- `FluxoController.cs`
- `InstituicaoController.cs`
- `OrcamentoController.cs`
- `SecretariaController.cs`
- `TipoDespesaController.cs`
- `TipoInstituicaoController.cs`
- `UnidadeMedidaController.cs`
- `AuditoriaController.cs`

There is also a lowercase sibling directory, `Civitas.WebAPI/controllers/`, which currently contains `FornecedorController.cs`. The project therefore has mixed-case controller paths, even though the namespace and route conventions remain the same.

### `Civitas.WebAPI/Data/`
Persistence infrastructure lives here.

- `AppDbContext.cs` defines the EF Core `DbContext` and `DbSet` properties.
- `Builders/` contains fluent entity configuration classes such as `UsuarioBuilder.cs` and `FornecedorBuilder.cs`.
- `Interfaces/` contains repository contracts like `IGenericRepository.cs` and `IUsuarioRepository.cs`.
- `Repositories/` contains EF Core repository implementations.

### `Civitas.WebAPI/Services/`
Business logic and security concerns.

- `Entities/` contains service implementations for each domain area.
- `Interfaces/` contains service contracts.
- `Security/` contains JWT and password hashing helpers.
- `Validation/` contains domain exceptions translated into API errors.

### `Civitas.WebAPI/Objects/`
Shared domain-shaped objects.

- `Models/` contains EF entities such as `Usuario.cs`, `Fornecedor.cs`, `Despesa.cs`, and `Documento.cs`.
- `Dtos/Entities/` contains API DTOs for each resource.
- `Dtos/Auth/` contains login request and response DTOs.
- `Dtos/Mappings/` contains `MappingsProfile.cs` for AutoMapper mappings.
- `Contracts/` contains `Response.cs`, `ResponseEnum.cs`, `PaginationQuery.cs`, and `PaginatedResult.cs`.
- `Enums/` contains state and type enums such as `Situacao.cs`, `TipoUsuario.cs`, and `Status.cs`.

## Naming Conventions
- Controllers are suffixed with `Controller`.
- Service interfaces start with `I` and are suffixed with `Service`, such as `IUsuarioService.cs`.
- Repository interfaces start with `I` and are suffixed with `Repository`, such as `IUsuarioRepository.cs`.
- Entity models are singular and match the domain concept, such as `Usuario.cs` and `Fornecedor.cs`.
- DTOs are suffixed with `DTO`.
- Fluent mapping helpers are suffixed with `Builder`.

## Major Concern Locations
- Authentication and login flow: `Civitas.WebAPI/Controllers/AuthController.cs`, `Civitas.WebAPI/Services/Entities/AuthService.cs`, `Civitas.WebAPI/Services/Security/JwtTokenService.cs`.
- User validation, uniqueness, and password hashing: `Civitas.WebAPI/Services/Entities/UsuarioService.cs`, `Civitas.WebAPI/Data/Repositories/UsuarioRepository.cs`.
- Shared CRUD + paging behavior: `Civitas.WebAPI/Services/Entities/GenericService.cs`, `Civitas.WebAPI/Data/Repositories/GenericRepository.cs`.
- EF mapping and relationships: `Civitas.WebAPI/Data/Builders/`.
- API payload contracts: `Civitas.WebAPI/Objects/Contracts/` and `Civitas.WebAPI/Objects/Dtos/`.
- Environment and runtime configuration: `Civitas.WebAPI/appsettings.json` and `Civitas.WebAPI/Properties/launchSettings.json`.

## Test Structure
`Civitas.WebAPI.Tests/` is organized around API behavior:

- `Infrastructure/TestWebApplicationFactory.cs` swaps the database to SQLite in-memory for tests.
- `AuthEndpointsTests.cs` validates JWT login behavior.
- `PaginationEndpointsTests.cs` checks paging and ordering metadata.
- `UsuarioValidationEndpointsTests.cs` checks normalization, uniqueness, and password handling.
- `RepositoryPaginationPerformanceTests.cs` checks that paged queries outperform full scans.

