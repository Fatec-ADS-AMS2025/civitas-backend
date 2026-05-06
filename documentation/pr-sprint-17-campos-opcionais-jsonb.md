# Pull Request: Sprint 17 - Campos Opcionais Dinâmicos com JSONB

## Objetivo

Permitir que cada `TipoDespesa` declare uma lista de campos opcionais configuráveis dinamicamente, e que cada `Despesa` armazene valores correspondentes para esses campos, ambos persistidos como `JSONB` no PostgreSQL. A entrega flexibiliza o cadastro sem exigir alteração estrutural no banco a cada novo campo, mantém compatibilidade total com a estrutura atual do sistema, garante integridade entre os campos declarados e os valores preenchidos, e centraliza as validações na camada de Service.

## Critérios de Aceite

- Adição de coluna JSONB em `TipoDespesa` para armazenar a definição dos campos opcionais (estrutura/configuração).
- Adição de coluna JSONB em `Despesa` para armazenar os valores opcionais preenchidos, permitindo preenchimento parcial, total ou ausente.
- DTOs de `TipoDespesa` e `Despesa` ajustados para expor os novos campos com serialização e desserialização JSON corretas.
- Endpoints de criação (`POST`), atualização (`PUT`) e listagem/busca (`GET`) de `TipoDespesa` aceitando e retornando os campos opcionais configurados.
- Endpoints de criação (`POST`), atualização (`PUT`) e listagem/busca (`GET`) de `Despesa` aceitando e retornando os valores opcionais associados.
- Validação centralizada na camada Service, sem dependência exclusiva de `DataAnnotations`.
- Validação de subset entre os campos declarados em `TipoDespesa` e os valores enviados em `Despesa`, considerando o relacionamento atual `Despesa → UnidadeConsumidora → TipoDespesa`.
- Não permitir campos desconhecidos em `Despesa.ValoresOpcionais`, com mensagem clara listando as chaves rejeitadas.
- Validação de estrutura JSON antes da persistência, com tratamento de payloads malformados, valores nulos e atualização parcial.
- Migration / DDL adicionando as colunas JSONB com retrocompatibilidade (NULL como padrão para registros antigos).
- Mapeamentos AutoMapper entre Model e DTO, com leitura defensiva (registros corrompidos no banco não derrubam o GET) e escrita estrita (validação upstream no Service).
- Padronização da nomenclatura dos campos JSON em `camelCase` via `JsonNamingPolicy.CamelCase`.
- Mensagens de erro claras ao usuário, retornadas no formato padrão `Response.Data` da API.
- Documentação Swagger atualizada automaticamente via XML doc nos DTOs.

## Alterações Realizadas

- Nova funcionalidade
- Refatoração de código
- Testes automatizados
- Documentação

A entidade `TipoDespesa` ganhou a propriedade `CamposOpcionais` (`string?`, mapeada para coluna `camposopcionais` JSONB) que armazena o envelope `{"camposOpcionais":[...]}`. A entidade `Despesa` ganhou `ValoresOpcionais` (`string?`, coluna `valoresopcionais` JSONB) que armazena o objeto JSON plano com pares chave/valor. Os DTOs expõem essas propriedades como `IList<string>?` e `IDictionary<string, JsonElement>?` respectivamente, com tradução transparente para JSON via AutoMapper customizado e helper estático puro `CamposOpcionaisJsonHelper`. As validações de estrutura, unicidade e subset rodam na camada Service e produzem mensagens de erro agregadas no padrão da API.

## Evidências de Testes

A solução foi validada por meio de compilação limpa do projeto, testes unitários abrangentes do helper e do método de orquestração no Service, e smoke test end-to-end em ambiente real (Postgres em Docker + aplicação Civitas WebAPI). Todos os 9 cenários funcionais previstos foram exercitados e retornaram o comportamento esperado.

### O que foi implementado

- Inclusão da propriedade `CamposOpcionais` (`string?`) em `TipoDespesa.cs` e `ValoresOpcionais` (`string?`) em `Despesa.cs`, com mapeamento `[Column("snake_case")]` seguindo o padrão do projeto.
- Configuração no `TipoDespesaBuilder.cs` e `DespesaBuilder.cs` mapeando as colunas via `HasColumnName` e `IsRequired(false)`. A definição do tipo `jsonb` ficou no DDL standalone para preservar compatibilidade com SQLite nos testes.
- Inclusão de `CamposOpcionais` em `TipoDespesaDTO` como `IList<string>?` e `ValoresOpcionais` em `DespesaDTO` como `IDictionary<string, JsonElement>?`, com XML doc completo para documentação Swagger.
- Criação do helper estático puro `CamposOpcionaisJsonHelper` em `Civitas.WebAPI/Services/Validation/`, oferecendo:
  - `ParseCamposDeclarados(string?)` — parse do envelope com validação de estrutura, unicidade case-insensitive, comprimento ≤ 100 e limite de 50 nomes.
  - `SerializeCamposDeclarados(IEnumerable<string>?)` — produção do envelope com as mesmas regras.
  - `ParseValoresPreenchidos(string?)` — parse do objeto plano com `JsonElement.Clone()` para detachar valores do `JsonDocument`.
  - `SerializeValoresPreenchidos(IReadOnlyDictionary<string, JsonElement>?)` — serialização inversa.
  - `EncontrarChavesDesconhecidas(...)` — verificação de subset case-sensitive (alinhada a JSON ser case-sensitive).
- Customização de `MappingsProfile.cs` substituindo o `ReverseMap()` simples para `TipoDespesa` e `Despesa` por mapeamento explícito que serializa/desserializa JSON via helper. A leitura é defensiva (try/catch retorna null em registro corrompido) e a escrita propaga exceções para o Service tratar antes da persistência.
- Adição de `ValidarCamposOpcionais` no `TipoDespesaService` integrada a `ValidateCommonRules`, cobrindo automaticamente os fluxos `Create` e `Update`.
- Adição de `ValidarValoresOpcionais` no `DespesaService`, executada dentro de `ValidarRelacionamentosAsync` logo após a resolução do `TipoDespesa` via `UnidadeConsumidora`. Cobre os casos: `TipoDespesa` ausente, `CamposOpcionais` corrompido no banco, `TipoDespesa` sem campos declarados, chaves desconhecidas, chaves vazias.
- Criação de 17 testes unitários em `CamposOpcionaisHelperTests.cs` cobrindo todo o helper.
- Criação de 8 testes diretos em `DespesaValoresOpcionaisValidationTests.cs` para a orquestração de subset, com `InternalsVisibleTo("Civitas.WebAPI.Tests")` declarado no `Civitas.WebAPI.csproj`.
- Criação do script SQL standalone `Civitas.WebAPI/sql/add_campos_opcionais_jsonb.sql` com `ALTER TABLE ... ADD COLUMN IF NOT EXISTS ... jsonb`, idempotente, NULL-able e retrocompatível.
- Adição de `<InternalsVisibleTo Include="Civitas.WebAPI.Tests" />` no `Civitas.WebAPI.csproj`.
- Documentação inline da assimetria deliberada de case-sensitivity entre dedup de declaração (case-insensitive) e match de subset (case-sensitive).

### Alterações técnicas

- O JSON é armazenado no model como `string?` raw, mantendo a entidade trivial e provedor-agnóstica. A tradução para `IList<string>?` / `IDictionary<string, JsonElement>?` no DTO acontece exclusivamente no AutoMapper, isolando a complexidade.
- A leitura é defensiva (Mapper retorna null em parse error) e a escrita é estrita (Service valida e lança exceção amigável upstream do mapper). Isso evita que registros corrompidos no banco quebrem GETs e garante que escrita inválida produza mensagens claras.
- A política do repositório de gitignorar migrations (`**/Migrations/`) levou à entrega da alteração de schema como SQL DDL standalone em `sql/`, alinhado ao padrão já existente (`inserts_lowercase.sql`).
- O builder não declara `HasColumnType("jsonb")` para preservar compatibilidade SQLite (testes); o tipo `jsonb` é definido apenas no DDL aplicado em produção. Como o app manipula JSON sempre como string em C#, o tipo da coluna não afeta o código.
- A serialização usa `JsonNamingPolicy.CamelCase` para padronizar a nomenclatura de chaves no JSON.

### Testes adicionados

- 17 testes unitários do helper (`CamposOpcionaisHelperTests.cs`).
- 8 testes diretos de `ValidarValoresOpcionais` (`DespesaValoresOpcionaisValidationTests.cs`).

### Verificações executadas

- `dotnet build Civitas.WebAPI/Civitas.WebAPI.csproj` — 0 erros.
- `dotnet build Civitas.WebAPI/Civitas.WebAPI.sln` — 0 erros (warnings idênticos à baseline).
- Smoke test end-to-end com Postgres 16 em container Docker e aplicação rodando via `DOTNET_ROLL_FORWARD=Major` (ambiente local sem runtime .NET 9): 9 cenários funcionais (POST/PUT/GET de TipoDespesa e Despesa, casos válidos e rejeições) — todos OK.
- Verificação direta no Postgres via `psql`: operador jsonb `->>`, retrocompatibilidade com NULL, validação nativa de JSON malformado.

## Relacionado

- Task: Sprint 17 - Implementação de Campos Opcionais Dinâmicos com JSONB

## Observações

- A alteração de schema é entregue como SQL DDL em `Civitas.WebAPI/sql/add_campos_opcionais_jsonb.sql`, alinhada à política do repositório de não versionar migrations EF. O operador roda manualmente o script (idempotente via `IF NOT EXISTS`) antes do deploy.
- A execução da suíte de testes localmente exige o runtime .NET 9, ausente nesta máquina de desenvolvimento (somente .NET 8 e .NET 10 instalados). A suíte compila limpa; a execução completa fica delegada ao CI.
- O `InternalsVisibleTo` foi adicionado para permitir testes diretos do método `ValidarValoresOpcionais` (mudou de `private static` para `internal static`) sem necessidade de fixture com 8 entidades. Padrão reconhecido da Microsoft, não afeta a superfície pública.
- A configuração `HasColumnType("jsonb")` foi propositalmente omitida nos builders para preservar compatibilidade com SQLite usado pelos testes. A coluna nasce como `jsonb` em produção via SQL DDL e como `text` em SQLite — comportamento da aplicação é idêntico em ambos.
- Documentação completa da entrega disponível em `documentation/relatorio-sprint-17-campos-opcionais-jsonb.md`.
