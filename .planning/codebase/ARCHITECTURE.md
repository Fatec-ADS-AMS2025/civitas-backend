# Architecture

## Application Pattern
`Civitas.WebAPI` is an ASP.NET Core Web API built on `net9.0` with a conventional layered structure:

- `Civitas.WebAPI/Program.cs` is the composition root.
- `Civitas.WebAPI/Controllers/*Controller.cs` exposes HTTP endpoints.
- `Civitas.WebAPI/Services/Entities/*Service.cs` holds business rules and orchestration.
- `Civitas.WebAPI/Data/Repositories/*Repository.cs` performs data access through Entity Framework Core.
- `Civitas.WebAPI/Data/AppDbContext.cs` is the persistence boundary to PostgreSQL.

The solution uses DTOs for API payloads, entity models for persistence, and AutoMapper to convert between them. Shared response and pagination contracts live under `Civitas.WebAPI/Objects/Contracts/`.

## Startup And Runtime Flow
`Program.cs` wires the runtime in this order:

1. Registers controllers and API explorer.
2. Configures `AppDbContext` with `UseNpgsql(...)` and the `DefaultConnection` from `Civitas.WebAPI/appsettings.json`.
3. Registers AutoMapper across loaded assemblies.
4. Configures JWT bearer authentication from the `Jwt` section in `Civitas.WebAPI/appsettings.json`.
5. Registers Swagger, including bearer auth metadata.
6. Registers dependency injection for repositories, services, password hashing, and token generation.
7. Enables CORS, HTTPS redirection, authentication, authorization, and controller routing.

Request flow is generally:

`Controller` -> `Service` -> `Repository` -> `AppDbContext` -> database

## Request And Response Flow
Most controllers use a shared `Response` envelope from `Civitas.WebAPI/Objects/Contracts/Response.cs`, with a `ResponseEnum` code, message, and arbitrary data payload. Pagination endpoints return `PaginatedResult<T>` from `Civitas.WebAPI/Objects/Contracts/PaginatedResult.cs` and accept `PaginationQuery` from `Civitas.WebAPI/Objects/Contracts/PaginationQuery.cs`.

`PaginationQuery` normalizes page and size, caps page size at 100, and translates `sortDirection=desc` into descending ordering. The generic repository applies ordering and pagination in `Civitas.WebAPI/Data/Repositories/GenericRepository.cs`.

## Core Abstractions
### Persistence
`Civitas.WebAPI/Data/AppDbContext.cs` declares the entity sets for the domain:

- `Usuarios`
- `Fornecedores`
- `Secretarias`
- `Documento`
- `Auditorias`
- `TipoInstituicoes`
- `Instituicoes`
- `Fluxos`
- `UnidadesMedida`
- `TiposDespesa`
- `Orcamentos`
- `Despesas`

Entity configuration is split into fluent builder classes under `Civitas.WebAPI/Data/Builders/`, such as `UsuarioBuilder.cs` and `FornecedorBuilder.cs`. These builders define keys, column lengths, uniqueness, and relationships, keeping `AppDbContext` focused on orchestration.

### Repositories
`Civitas.WebAPI/Data/Interfaces/IGenericRepository.cs` and `Civitas.WebAPI/Data/Repositories/GenericRepository.cs` define the common persistence contract and implementation. The generic repository supports:

- full listing
- paged listing
- enum-based filtering
- lookup by ID
- add, update, remove, save changes

Specialized repositories extend the generic one when an entity needs extra queries. `UsuarioRepository` adds lookups like `GetByEmailAsync`, duplicate checks, and CPF search.

### Services
`Civitas.WebAPI/Services/Interfaces/IGenericService.cs` and `Civitas.WebAPI/Services/Entities/GenericService.cs` define the standard CRUD service layer. Services map entities to DTOs and back through AutoMapper.

Specialized services add rules:

- `Civitas.WebAPI/Services/Entities/UsuarioService.cs` normalizes CPF, email, UF, and password, validates uniqueness, and hashes passwords.
- `Civitas.WebAPI/Services/Entities/AuthService.cs` validates login input, checks user status, verifies hashes, and delegates JWT creation.
- Entity services such as `DespesaService`, `InstituicaoService`, and `SecretariaService` follow the same pattern of domain-specific validation plus generic persistence.

### Security
Authentication is JWT-based:

- `Civitas.WebAPI/Services/Security/JwtOptions.cs` binds issuer, audience, secret, and expiration.
- `Civitas.WebAPI/Services/Security/JwtTokenService.cs` creates tokens and claims.
- `Civitas.WebAPI/Services/Security/BCryptPasswordHashService.cs` hashes and verifies passwords.
- `Civitas.WebAPI/Controllers/AuthController.cs` exposes `POST /api/auth/login`.

JWT bearer validation is configured in `Program.cs` with issuer, audience, lifetime, and signing key checks.

## API Organization
The API is organized by domain resource, with one controller per major aggregate or lookup table:

- `Civitas.WebAPI/Controllers/UsuarioController.cs`
- `Civitas.WebAPI/controllers/FornecedorController.cs`
- `Civitas.WebAPI/Controllers/SecretariaController.cs`
- `Civitas.WebAPI/Controllers/InstituicaoController.cs`
- `Civitas.WebAPI/Controllers/DocumentoController.cs`
- `Civitas.WebAPI/Controllers/AuditoriaController.cs`
- `Civitas.WebAPI/Controllers/FluxoController.cs`
- `Civitas.WebAPI/Controllers/OrcamentoController.cs`
- `Civitas.WebAPI/Controllers/DespesaController.cs`
- `Civitas.WebAPI/Controllers/TipoDespesaController.cs`
- `Civitas.WebAPI/Controllers/TipoInstituicaoController.cs`
- `Civitas.WebAPI/Controllers/UnidadeMedidaController.cs`

Most resource controllers expose a consistent set of verbs: `GET` list, `GET {id}`, `POST`, `PUT {id}`, and when needed `PATCH` for status transitions or specialized filters such as `cpf`, `nome`, `usuario/{usuarioId}`, `entidade`, or `operacao`.

## Dependency Boundaries
The intended dependency direction is:

`Controllers` -> `Services` -> `Repositories` -> `DbContext` -> `Models`

DTOs and response contracts stay at the boundary and are not used as persistence types. Validation exceptions live in `Civitas.WebAPI/Services/Validation/` and are translated to HTTP responses in controllers.

The tests project, `Civitas.WebAPI.Tests/`, exercises the API through `WebApplicationFactory` and swaps the PostgreSQL context for an in-memory SQLite database in `Civitas.WebAPI.Tests/Infrastructure/TestWebApplicationFactory.cs`.

