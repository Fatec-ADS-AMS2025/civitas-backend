# Testing

## Test Stack

- Tests live in `Civitas.WebAPI.Tests`.
- The project targets `net9.0` and uses `xunit`, `Microsoft.NET.Test.Sdk`, `Microsoft.AspNetCore.Mvc.Testing`, and `Microsoft.EntityFrameworkCore.Sqlite` as defined in `Civitas.WebAPI.Tests/Civitas.WebAPI.Tests.csproj`.
- There is no Moq/FluentAssertions layer in the current test project; assertions are plain xUnit assertions.
- Tests run against the real app host through `WebApplicationFactory`, not against isolated controller mocks.

## Layout

- The main fixture is `Civitas.WebAPI.Tests/Infrastructure/TestWebApplicationFactory.cs`.
- Test classes are grouped by behavior instead of by layer:
  - `AuthEndpointsTests.cs`
  - `UsuarioValidationEndpointsTests.cs`
  - `PaginationEndpointsTests.cs`
  - `RepositoryPaginationPerformanceTests.cs`
- Most tests are integration-style and exercise HTTP endpoints end to end.
- The factory class swaps the app database to SQLite in-memory, removes the production `AppDbContext`, and recreates the schema for each seed/reset cycle.

## Patterns

- Seed data is usually prepared through `_factory.ResetDatabaseAsync(...)` and then exercised with a real `HttpClient`.
- Tests assert both the HTTP response and the persisted database state when validation or mutation behavior matters.
- Response payloads are read with `System.Net.Http.Json` or `JsonDocument`, depending on whether the test needs typed access or a quick shape check.
- `AuthEndpointsTests.cs` verifies JWT claims, expiry range, and authentication failure modes.
- `UsuarioValidationEndpointsTests.cs` checks normalization, password hashing, duplicate detection, invalid input handling, and password preservation/update behavior.
- `PaginationEndpointsTests.cs` verifies paging metadata, size clamping, and sort fallback behavior.
- `RepositoryPaginationPerformanceTests.cs` compares a paginated repository query to a full read using `Stopwatch` and reflection to keep the test generic.

## Helpers

- `Civitas.WebAPI.Tests/Infrastructure/TestWebApplicationFactory.cs` is the main shared helper and should be reused for new endpoint tests.
- `AuthEndpointsTests.cs` contains local helpers for decoding JWT payloads and building seed users.
- `UsuarioValidationEndpointsTests.cs` contains a payload builder helper and a seeded user factory to keep the test body focused.
- Existing helpers intentionally keep the tests close to production behavior instead of hiding everything behind custom test DSLs.

## Performance Testing

- The only explicit performance-oriented test is `RepositoryPaginationPerformanceTests.cs`.
- It uses an in-memory SQLite database, warms up the code path, runs each sample three times, and compares average elapsed time.
- This is useful as a smoke check, but it is not a stable benchmark harness.
- The current setup does not persist historical baselines or enforce a numeric performance budget.

## Current Gaps

- Coverage is concentrated on auth, user validation, and pagination.
- There are no dedicated tests for most CRUD controllers such as `SecretariaController`, `FornecedorController`, `InstituicaoController`, `OrcamentoController`, `DocumentoController`, `AuditoriaController`, `FluxoController`, `UnidadeMedidaController`, or `DespesaController`.
- There are no focused unit tests for service classes like `Civitas.WebAPI/Services/Entities/UsuarioService.cs` or `Civitas.WebAPI/Services/Entities/AuthService.cs`.
- The repository has no visible test coverage for authorization rules beyond the anonymous login path.
- Negative tests for invalid sort fields, malformed pagination values beyond the existing clamp checks, and repository edge cases are limited.
- The performance test is the only place that exercises `GenericRepository<T>` ordering and paging at scale.

