# Conventions

## Repository Shape

- The codebase is split into `Civitas.WebAPI` and `Civitas.WebAPI.Tests`.
- Production code follows a layered structure: `Controllers`, `Services`, `Data`, and `Objects`.
- Domain objects live under `Civitas.WebAPI/Objects/Models`, DTOs under `Civitas.WebAPI/Objects/Dtos`, and shared transport/contracts under `Civitas.WebAPI/Objects/Contracts`.
- Route handlers are grouped by resource in files such as `Civitas.WebAPI/Controllers/UsuarioController.cs`, `Civitas.WebAPI/Controllers/AuthController.cs`, and `Civitas.WebAPI/Controllers/SecretariaController.cs`.
- Dependency wiring is explicit in `Civitas.WebAPI/Program.cs`; services and repositories are registered one by one with `AddScoped`.

## Naming

- Controllers use resource-based names like `UsuarioController`, `AuthController`, `FornecedorController`, and `OrcamentoController`.
- Services and repositories follow the same entity name plus suffix pattern, for example `UsuarioService` and `UsuarioRepository`.
- Interface names use the `I` prefix, such as `IUsuarioService`, `IUsuarioRepository`, and `IGenericRepository<T>`.
- DTOs are named per entity and purpose, for example `UsuarioDTO`, `LoginRequestDTO`, and `LoginResponseDTO`.
- Entity models use singular names and are mapped to tables through attributes in files like `Civitas.WebAPI/Objects/Models/Usuario.cs`.
- There is one casing outlier in the tree: `Civitas.WebAPI/controllers/FornecedorController.cs` uses a lowercase folder name while the rest of the repo uses `Controllers`.

## API Patterns

- Controllers expose REST-style routes under `api/*`, for example `/api/usuarios`, `/api/auth/login`, and `/api/instituicoes`.
- Responses are wrapped in the custom `Response` envelope from `Civitas.WebAPI/Objects/Contracts/Response.cs`, with `Code`, `Message`, and `Data`.
- Status mapping is centralized by `ResponseEnum` in `Civitas.WebAPI/Objects/Contracts/ResponseEnum.cs`; the main codes are `SUCCESS`, `INVALID`, `NOT_FOUND`, `CONFLICT`, `UNAUTHORIZED`, and `ERROR`.
- Pagination uses `PaginationQuery` and `PaginatedResult<T>` from `Civitas.WebAPI/Objects/Contracts/PaginationQuery.cs` and `Civitas.WebAPI/Objects/Contracts/PaginatedResult.cs`.
- The pagination contract normalizes page and size values, and repository ordering falls back to the primary key when `SortBy` is missing or invalid in `Civitas.WebAPI/Data/Repositories/GenericRepository.cs`.
- Authentication login is anonymous via `[AllowAnonymous]` in `Civitas.WebAPI/Controllers/AuthController.cs`; the rest of the API is wired for JWT bearer auth in `Civitas.WebAPI/Program.cs`.
- Swagger is only enabled in development, and the bearer scheme is documented in `Civitas.WebAPI/Program.cs`.

## Validation Conventions

- Validation is mostly service-driven, not annotation-driven.
- Controllers typically check for `null` payloads, then defer business validation to services.
- `Civitas.WebAPI/Services/Entities/UsuarioService.cs` is the clearest validation model:
  - it normalizes input before checks,
  - strips punctuation from CPF, RG, and CEP,
  - trims text fields,
  - lowercases email,
  - uppercases state codes,
  - hashes passwords before persistence.
- `UsuarioService` raises `UsuarioValidationException` for rule failures and `UsuarioConflictException` for duplicate CPF, email, or matrícula.
- `AuthService` follows the same pattern with `AuthValidationException` for missing login fields and `AuthUnauthorizedException` for invalid credentials.
- Database-level constraints are reinforced in the builder classes, for example `Civitas.WebAPI/Data/Builders/UsuarioBuilder.cs` and `Civitas.WebAPI/Data/Builders/FornecedorBuilder.cs`, where required fields, length limits, and unique indexes are declared.
- DTO annotations exist, but they are not the primary enforcement mechanism; service code and EF configuration carry most of the rules.

## Error Handling

- Controllers map known service exceptions to HTTP codes explicitly and keep the payload inside the standard `Response` envelope.
- `UsuarioController` returns `400` for `UsuarioValidationException`, `409` for `UsuarioConflictException`, and `500` for unexpected failures.
- `AuthController` returns `400` for validation errors and `401` for bad credentials.
- Several other controllers still use broad `catch (Exception)` blocks, so the current style favors defensive error wrapping over fine-grained exception hierarchies.
- `GenericService<T, TDto>` and `GenericRepository<T>` throw `KeyNotFoundException` and `InvalidOperationException` for missing entities, mapping problems, or invalid metadata.
- Keep error messages short and operational, but preserve the field name when the failure is about uniqueness or invalid input.

## Organization And Maintenance

- Keep layer boundaries intact: controllers orchestrate, services validate and apply rules, repositories query and persist.
- Prefer adding new entity behavior in a dedicated service and repository pair rather than embedding logic in controllers.
- When adding a new resource, mirror the existing stack: model, DTO, builder, repository, service, controller, and tests.
- Keep routes and file names aligned with the entity name and the `Controllers` folder casing.
- Preserve the custom response envelope unless there is a deliberate project-wide move to a different API contract.
- Update builder classes whenever a new field or constraint is added so the EF model and runtime validation stay aligned.
- Maintain the existing XML-doc-heavy style on public controllers, DTOs, and services; it is used as the main in-repo guidance.

