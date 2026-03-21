# Relatório - Sprint 13 - Validação dos Campos do Usuário

## Objetivo da task
Implementar validações completas no cadastro e atualização de usuários para garantir integridade, padronização e segurança dos dados.

## O que foi implementado
- Validação na camada `service` para campos obrigatórios do usuário.
- Normalização de `cpf`, `rg` e `cep`, removendo máscara e persistindo apenas números.
- Validação de CPF com regra de 11 dígitos, algoritmo oficial e bloqueio de duplicidade.
- Validação de email com formato válido, limite de 255 caracteres e unicidade.
- Validação de senha com mínimo de 8 caracteres, exigência de letra e número, e armazenamento com hash BCrypt.
- Tratamento de senha no `PUT`: opcional. Se não for enviada, o hash atual é preservado. Se for enviada, é re-hashada.
- Validação de nome, endereço, UF, RG, matrícula, `situacao` e `tipoUsuario`.
- Remoção da senha das respostas da API.
- Retornos HTTP adequados:
  - `400 BadRequest` para validação inválida
  - `409 Conflict` para duplicidade de `cpf`, `email` ou `matricula`

## Alterações técnicas
- `UsuarioService` passou a sobrescrever `Create` e `Update` para aplicar validação, normalização e hash.
- `UsuarioRepository` ganhou checagens de existência para `cpf`, `email` e `matricula`.
- `UsuarioBuilder` foi ajustado para novos limites e índices únicos.
- Foi adicionada migration `20260318164700_Sprint13UsuarioValidation`.
- Foi criada uma abstração de hash de senha com `BCryptPasswordHashService`.
- O mapeamento `Usuario -> UsuarioDTO` passou a ocultar a senha.

## Testes adicionados
- Cadastro válido com máscara e persistência normalizada.
- Hash seguro de senha e ausência de senha na resposta.
- CPF inválido.
- Email inválido.
- Senha fraca.
- Duplicidade de `cpf`, `email` e `matricula`.
- `estado`, `situacao` e `tipoUsuario` inválidos.
- Atualização sem senha preservando hash atual.
- Atualização com nova senha gerando novo hash.

## Verificações executadas
- `DOTNET_ROLL_FORWARD=Major dotnet test Civitas.WebAPI/Civitas.WebAPI.sln`
- `DOTNET_ROLL_FORWARD=Major dotnet build Civitas.WebAPI/Civitas.WebAPI.sln`

## Observações
- O ambiente exigiu `DOTNET_ROLL_FORWARD=Major` para executar a suíte `net9.0`.
- O projeto já possuía um aviso externo de dependência vulnerável em `AutoMapper 15.0.1` (`NU1903`). Esse ponto não fazia parte desta task, então ficou como pendência separada.
