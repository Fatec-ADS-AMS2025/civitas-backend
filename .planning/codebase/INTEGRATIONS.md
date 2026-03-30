# Integrations

## Database
- Production persistence is PostgreSQL, configured in `Civitas.WebAPI/appsettings.json` and consumed in `Civitas.WebAPI/Program.cs` via `UseNpgsql(...)`.
- The EF Core model is defined in `Civitas.WebAPI/Data/AppDbContext.cs` and mapped with builder classes under `Civitas.WebAPI/Data/Builders/`.
- The checked-in EF artifact is `Civitas.WebAPI/Migrations/AppDbContextModelSnapshot.cs`; no standalone `*Migration.cs` files were present in the repo snapshot I inspected.
- Entities cover users, fornecedores, secretarias, documentos, auditorias, tipos, orçamento, fluxo, and despesa.

## Authentication And Authorization
- Authentication is JWT-based and exposed through `Civitas.WebAPI/Controllers/AuthController.cs` and `Civitas.WebAPI/Services/Entities/AuthService.cs`.
- JWT settings come from the `Jwt` section in `Civitas.WebAPI/appsettings.json` and are bound to `Civitas.WebAPI/Services/Security/JwtOptions.cs`.
- Token validation is configured in `Civitas.WebAPI/Program.cs` with issuer, audience, lifetime, and signing-key checks.
- Passwords are hashed and verified with BCrypt in `Civitas.WebAPI/Services/Security/BCryptPasswordHashService.cs`.
- `AuthController` marks login as `[AllowAnonymous]`; the rest of the API is wired behind `UseAuthentication()` and `UseAuthorization()`.

## HTTP And API Surface
- The API is REST-style and controller-based, with routes under `Civitas.WebAPI/Controllers/`.
- Swagger/OpenAPI is enabled in development in `Civitas.WebAPI/Program.cs`, with the bearer token security definition added to Swagger.
- CORS is permissive in `Program.cs` (`AllowAnyOrigin`, `AllowAnyMethod`, `AllowAnyHeader`), which means the API is intended to be consumed by separate clients without additional origin restrictions.
- `Civitas.WebAPI/Civitas.WebAPI.http` is a local scratch file for exercising endpoints manually.

## External Files And Local Data
- `Civitas.WebAPI/sql/inserts_lowercase.sql` exists as a data script, but it is not referenced by startup wiring or tests.
- `Civitas.WebAPI/Properties/launchSettings.json` is the only environment-profile file I found for local execution.
- `README.md` documents expected connection strings and JWT values, but the runtime reads `appsettings.json` and environment overrides.
- The project does not show integration with cloud storage, email, queues, or third-party HTTP APIs in the inspected source.

## Test Infrastructure
- `Civitas.WebAPI.Tests/Civitas.WebAPI.Tests.csproj` uses `Microsoft.AspNetCore.Mvc.Testing` for end-to-end HTTP tests.
- `Civitas.WebAPI.Tests/Infrastructure/TestWebApplicationFactory.cs` swaps PostgreSQL for an in-memory SQLite connection during tests.
- The tests seed and reset state through EF Core `EnsureDeletedAsync()` and `EnsureCreatedAsync()`.
- `Civitas.WebAPI.Tests/AuthEndpointsTests.cs` validates JWT issuance and claims.
- `Civitas.WebAPI.Tests/UsuarioValidationEndpointsTests.cs` checks normalization, hashing, validation, and conflicts.
- `Civitas.WebAPI.Tests/PaginationEndpointsTests.cs` and `Civitas.WebAPI.Tests/RepositoryPaginationPerformanceTests.cs` cover paginated access patterns and repository behavior.

## Missing Or Unused Integrations
- I did not find outbound HTTP clients, message brokers, background-job runners, cache providers, object storage, or email providers in the inspected code.
- I did not find a checked-in `.env.example`, user-secrets file, or other explicit secret-management integration.
- Document storage is modeled as a byte array on `Civitas.WebAPI/Objects/Models/Documento.cs`, so there is no separate file-storage service integration in the current implementation.
