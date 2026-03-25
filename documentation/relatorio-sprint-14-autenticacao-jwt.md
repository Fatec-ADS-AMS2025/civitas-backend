# Relatório - Sprint 14 - Autenticação com JWT

## Objetivo da task
Implementar autenticação de usuários via `POST /api/auth/login`, validando credenciais e retornando um token JWT para uso nas próximas rotas protegidas.

## O que foi implementado
- Endpoint `POST /api/auth/login`.
- Validação básica de payload de login (`email` e `senha` obrigatórios).
- Busca de usuário por email no repositório.
- Verificação de senha com BCrypt.
- Bloqueio de autenticação para usuário inexistente, senha inválida ou usuário inativo.
- Geração de JWT com validade de 60 minutos.
- Inclusão das claims `sub`, `name` e `tipousuario`.
- Configuração do middleware `JwtBearer` no host.
- Documentação do esquema Bearer no Swagger.
- Atualização do `README.md` e do arquivo `Civitas.WebAPI.http`.

## Alterações técnicas
- Foi adicionado `Microsoft.AspNetCore.Authentication.JwtBearer`.
- Foram criados `AuthController`, `AuthService` e `JwtTokenService`.
- Foram criados DTOs dedicados para request/response de login.
- `IUsuarioRepository`/`UsuarioRepository` ganharam busca por email.
- Foi adicionada seção `Jwt` no `appsettings.json`.

## Resumo do git diff
### Arquivos modificados
- `Civitas.WebAPI/Civitas.WebAPI.csproj`: adição do pacote `Microsoft.AspNetCore.Authentication.JwtBearer`.
- `Civitas.WebAPI/Program.cs`: configuração de `JwtBearer`, `Authorization`, Swagger Bearer e registro dos serviços de autenticação.
- `Civitas.WebAPI/Data/Interfaces/IUsuarioRepository.cs`: inclusão do contrato de busca por email.
- `Civitas.WebAPI/Data/Repositories/UsuarioRepository.cs`: implementação de `GetByEmailAsync`.
- `Civitas.WebAPI/appsettings.json`: inclusão da seção `Jwt` com `Issuer`, `Audience`, `SecretKey` e `ExpirationMinutes`.
- `Civitas.WebAPI/Civitas.WebAPI.http`: exemplo de login e uso do header `Authorization: Bearer`.
- `README.md`: documentação do endpoint de login, configuração JWT e uso do token.

### Arquivos novos
- `Civitas.WebAPI/Controllers/AuthController.cs`
- `Civitas.WebAPI/Objects/Dtos/Auth/LoginRequestDTO.cs`
- `Civitas.WebAPI/Objects/Dtos/Auth/LoginResponseDTO.cs`
- `Civitas.WebAPI/Services/Entities/AuthService.cs`
- `Civitas.WebAPI/Services/Interfaces/IAuthService.cs`
- `Civitas.WebAPI/Services/Security/IJwtTokenService.cs`
- `Civitas.WebAPI/Services/Security/JwtOptions.cs`
- `Civitas.WebAPI/Services/Security/JwtTokenService.cs`
- `Civitas.WebAPI/Services/Validation/AuthUnauthorizedException.cs`
- `Civitas.WebAPI/Services/Validation/AuthValidationException.cs`
- `Civitas.WebAPI.Tests/AuthEndpointsTests.cs`

### Estatística do diff
- `7 files changed, 85 insertions(+), 3 deletions(-)` nos arquivos já rastreados.
- Além disso, foram adicionados `11` novos arquivos para o slice de autenticação e testes.

## Testes adicionados
- Login válido retornando token com claims esperadas.
- Login com senha inválida retornando `401 Unauthorized`.
- Login com usuário inativo retornando `401 Unauthorized`.
- Login com payload inválido retornando `400 BadRequest`.

## Verificações executadas
- `DOTNET_ROLL_FORWARD=Major dotnet test Civitas.WebAPI/Civitas.WebAPI.sln --filter AuthEndpointsTests`
