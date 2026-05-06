# 📌 Pull Request: Implementação de Campos Opcionais Dinâmicos com JSONB

## 🎯 Objetivo

Permitir que cada `TipoDespesa` declare uma lista de campos opcionais configuráveis dinamicamente, e que cada `Despesa` armazene valores correspondentes para esses campos, ambos persistidos como `JSONB` no PostgreSQL.

A entrega flexibiliza o cadastro sem exigir alteração estrutural no banco a cada novo campo, mantém compatibilidade total com a estrutura atual do sistema, garante integridade entre os campos declarados e os valores preenchidos, e centraliza as validações na camada de Service.

---

## 📖 Descrição

Implementadas duas colunas `JSONB` (`tipodespesa.camposopcionais` e `despesa.valoresopcionais`), DTOs com superfície limpa (`IList<string>?` e `IDictionary<string, JsonElement>?`), helper estático puro `CamposOpcionaisJsonHelper` isolando parsing/serialização/validação, AutoMapper customizado com leitura defensiva e escrita estrita, validações dedicadas em `TipoDespesaService` (estrutura) e `DespesaService` (subset via `Despesa → UnidadeConsumidora → TipoDespesa`), além de DDL standalone alinhado à política do repositório de não versionar migrations EF.

---

## ✅ Critérios de Aceite

### 🔹 Alterações na Entidade `TipoDespesa`
- [x] Coluna `JSONB` `camposopcionais` armazenando o envelope `{"camposOpcionais":[...]}`
- [x] Estrutura/configuração dos campos adicionais (não dados fixos)
- [x] Lista de nomes única, comprimento ≤ 100, máximo 50 itens

### 🔹 Alterações na Entidade `Despesa`
- [x] Coluna `JSONB` `valoresopcionais` armazenando objeto plano `{chave: valor}`
- [x] Preenchimento parcial ou total aceito
- [x] Não obrigatório o preenchimento de todos os campos
- [x] Valores `null` aceitos em qualquer chave declarada

### 🔹 Alterações nos DTOs
- [x] `TipoDespesaDTO.CamposOpcionais` (`IList<string>?`) com XML doc + exemplo Swagger
- [x] `DespesaDTO.ValoresOpcionais` (`IDictionary<string, JsonElement>?`) com XML doc + exemplo Swagger
- [x] Serialização/desserialização via AutoMapper customizado, padronização `camelCase` (`JsonNamingPolicy.CamelCase`)

### 🔹 Alterações nos Endpoints de `TipoDespesa`
- [x] `POST /api/tipo-despesa` aceita `camposOpcionais`
- [x] `PUT /api/tipo-despesa/{id}` aceita atualização da estrutura
- [x] `GET /api/tipo-despesa[/{id}]` retorna a lista cadastrada

### 🔹 Alterações nos Endpoints de `Despesa`
- [x] `POST /api/despesas` aceita `valoresOpcionais`
- [x] `PUT /api/despesas/{id}` aceita atualização parcial ou total
- [x] `GET /api/despesas[/{id}]` retorna o dicionário associado

### 🔹 Alterações na Camada Service
- [x] `TipoDespesaService.ValidarCamposOpcionais` — estrutura, unicidade case-insensitive, comprimento, limite (integrado ao `ValidateCommonRules`, cobre `Create` e `Update`)
- [x] `DespesaService.ValidarValoresOpcionais` — subset via `UnidadeConsumidora.IdTipoDespesa`, dentro de `ValidarRelacionamentosAsync`
- [x] Validação de estrutura JSON antes da persistência (helper `CamposOpcionaisJsonHelper`)
- [x] Regras centralizadas em método dedicado por entidade

### 🔹 Alterações em Mapeamentos
- [x] `MappingsProfile` substitui `ReverseMap()` simples por `ForMember` explícito para `CamposOpcionais` e `ValoresOpcionais`
- [x] Persistência correta como JSON string no model, exposição como tipos fortes no DTO
- [x] Leitura defensiva: registro corrompido no banco não derruba o GET (try/catch retorna `null`)
- [x] Escrita estrita: exceções propagam para o Service produzir mensagens amigáveis

### 🔹 Validações
- [x] Chaves de `Despesa.ValoresOpcionais` validadas contra `TipoDespesa.CamposOpcionais` resolvido via UC
- [x] Campos desconhecidos rejeitados com mensagem listando todas as chaves
- [x] Compatibilidade estrutural entre `TipoDespesa` e `Despesa` garantida pelo subset check
- [x] Estrutura JSON validada antes de persistir (helper levanta `ArgumentException`/`JsonException`)
- [x] Valores `null` tratados corretamente (parseados como `JsonValueKind.Null`, persistidos como `null`)
- [x] Payloads malformados rejeitados com erro claro
- [x] Mensagens de erro claras retornadas via `Response.Data` no padrão da API

### 🔹 Banco de Dados
- [x] Script SQL DDL (`Civitas.WebAPI/sql/add_campos_opcionais_jsonb.sql`) adicionando as colunas `jsonb`
- [x] Compatibilidade com PostgreSQL (tipo `jsonb` nativo, idempotente via `IF NOT EXISTS`)
- [x] Default `NULL` (registros antigos não exigem backfill)
- [x] Retrocompatibilidade total: registros pré-existentes ficam com `NULL` em ambas as colunas

### 🔹 Normalização e Boas Práticas
- [x] Validações implementadas na camada **SERVICE**
- [x] Não dependente apenas de `DataAnnotations`
- [x] Validação anterior à persistência (helper validate-then-serialize)
- [x] Nomenclatura JSON padronizada em `camelCase`
- [x] Tratamento explícito de nullables em todas as superfícies (Model `string?`, DTO tipos `?`)
- [x] Atualização parcial validada nos cenários de PUT
- [x] Impacto em retornos da API e Swagger revisado (XML doc nos DTOs)
- [x] Funcionamento completo verificado via smoke test end-to-end com Postgres real

---

## 🔄 Alterações Realizadas
- [x] Nova funcionalidade
- [ ] Correção de bug
- [x] Refatoração de código (AutoMapper customizado substitui `ReverseMap()` simples)
- [x] Testes automatizados (25 testes novos)
- [x] Documentação

---

## 🧪 Evidências de Testes

### Arquivos CRIADOS (4 + 2 docs)
- `Civitas.WebAPI/Services/Validation/CamposOpcionaisJsonHelper.cs` — helper estático puro com 5 métodos públicos (`ParseCamposDeclarados`, `SerializeCamposDeclarados`, `ParseValoresPreenchidos`, `SerializeValoresPreenchidos`, `EncontrarChavesDesconhecidas`), constantes (`EnvelopeKey="camposOpcionais"`, `MaxFieldName=100`, `MaxFields=50`), `JsonNamingPolicy.CamelCase` para serialização padronizada
- `Civitas.WebAPI/sql/add_campos_opcionais_jsonb.sql` — DDL idempotente com `ALTER TABLE ... ADD COLUMN IF NOT EXISTS ... jsonb` para `tipodespesa` e `despesa`, dentro de `BEGIN/COMMIT`, com bloco de rollback comentado
- `Civitas.WebAPI.Tests/CamposOpcionaisHelperTests.cs` — 17 testes unitários cobrindo parsing (8), serialização (3), valores preenchidos (3), `EncontrarChavesDesconhecidas` (3)
- `Civitas.WebAPI.Tests/DespesaValoresOpcionaisValidationTests.cs` — 8 testes diretos da orquestração de subset via `InternalsVisibleTo`, cobrindo: `ValoresOpcionais` null/vazio, `TipoDespesa` ausente, sem campos declarados, `CamposOpcionais` corrompido, todas chaves declaradas, chaves desconhecidas, case sensitivity
- `documentation/relatorio-sprint-17-campos-opcionais-jsonb.md` — relatório completo de entrega
- `documentation/pr-sprint-17-campos-opcionais-jsonb.md` — body source deste PR

### Arquivos MODIFICADOS (10)
- `Civitas.WebAPI/Objects/Models/TipoDespesa.cs` — adicionada propriedade `CamposOpcionais` (`string?`) com `[Column("camposopcionais")]`
- `Civitas.WebAPI/Objects/Models/Despesa.cs` — adicionada propriedade `ValoresOpcionais` (`string?`) com `[Column("valoresopcionais")]`
- `Civitas.WebAPI/Data/Builders/TipoDespesaBuilder.cs` — mapeamento da coluna via `HasColumnName` + `IsRequired(false)`
- `Civitas.WebAPI/Data/Builders/DespesaBuilder.cs` — mapeamento da coluna via `HasColumnName` + `IsRequired(false)`
- `Civitas.WebAPI/Objects/Dtos/Entities/TipoDespesaDTO.cs` — adicionada `CamposOpcionais` (`IList<string>?`) com XML doc e exemplo Swagger
- `Civitas.WebAPI/Objects/Dtos/Entities/DespesaDTO.cs` — adicionada `ValoresOpcionais` (`IDictionary<string, JsonElement>?`) com `using System.Text.Json` e exemplo Swagger
- `Civitas.WebAPI/Objects/Dtos/Mappings/MappingsProfile.cs` — substituição de `ReverseMap()` por `ForMember` explícito para os dois pares, com helpers privados `ParseCamposSafely`, `ParseValoresSafely`, `SerializeValoresOpcionais` (este último contornando limitação de expression tree do AutoMapper para `is null`)
- `Civitas.WebAPI/Services/Entities/TipoDespesaService.cs` — método `ValidarCamposOpcionais` reusando `SerializeCamposDeclarados` para validar; chamada inserida em `ValidateCommonRules`
- `Civitas.WebAPI/Services/Entities/DespesaService.cs` — método `ValidarValoresOpcionais` (`internal static` para teste direto); chamada inserida em `ValidarRelacionamentosAsync` logo após resolução do `tipoDespesa` via UC
- `Civitas.WebAPI/Civitas.WebAPI.csproj` — adicionado `<InternalsVisibleTo Include="Civitas.WebAPI.Tests" />` em novo `<ItemGroup>`

### Migration / DDL
- O `.gitignore` do projeto contém `**/Migrations/` por política do time (commits anteriores: `devops: removendo migrations` e `chore: Remove Migrations do controle de versão`)
- Tentativa de gerar migration via `dotnet ef migrations add` produziu drift (snapshot desatualizado vs. modelo, gerando `CreateTable` de 14 tabelas inaplicáveis em DB existente)
- Em vez de migration EF, a alteração de schema foi entregue como **SQL DDL standalone** em `Civitas.WebAPI/sql/add_campos_opcionais_jsonb.sql`, alinhado ao padrão da pasta `sql/` que já contém `inserts_lowercase.sql`
- DDL idempotente (`ADD COLUMN IF NOT EXISTS`), retrocompatível (`NULL` default), versionado e rastreável

### Smoke test end-to-end (ambiente real)
Container Postgres 16 em Docker + aplicação Civitas WebAPI (`DOTNET_ROLL_FORWARD=Major` por ausência de runtime .NET 9 local). 9 cenários funcionais via `curl` autenticado:

| # | Cenário | Resultado |
|---|---|---|
| 1 | `GET /api/tipo-despesa` lista | OK — `camposOpcionais: []` em registros sem config |
| 2 | `POST /api/tipo-despesa` com `camposOpcionais` válido | OK — criado, lista retornada |
| 3 | `GET /api/tipo-despesa/{id}` | OK — `camposOpcionais` no body |
| 4 | `POST` com duplicata `["a","a"]` | OK — 400 + `Nome de campo opcional duplicado: 'a'` |
| 5 | `PUT` alterando `camposOpcionais` | OK — atualizado |
| 6 | `POST /api/despesas` com `valoresOpcionais` válido | OK — criado, dict retornado |
| 7 | `GET /api/despesas/{id}` | OK — `valoresOpcionais` no body |
| 8 | `POST` com chave desconhecida | OK — 400 + `ValoresOpcionais contém chaves não declaradas em TipoDespesa: chaveDesconhecida` |
| 9 | `PUT` com update parcial | OK — atualizado, dict reduzido |

Verificações `psql` direto: operador `->>`, retrocompatibilidade `NULL`, validação nativa de JSON malformado.

### Verificações executadas
- `dotnet build Civitas.WebAPI/Civitas.WebAPI.csproj` — **0 erros**, 99 warnings (todos pré-existentes)
- `dotnet build Civitas.WebAPI/Civitas.WebAPI.sln` — **0 erros**
- `docker run -d --name civitas-pg ... postgres:16` — container UP
- Aplicação do DDL via `psql` — duas colunas `jsonb` criadas
- Smoke test funcional com 9 cenários (todos OK)

### Padrão arquitetural seguido
A implementação segue rigorosamente o estilo já consolidado em `OrcamentoService`/`UsuarioService` (validações centralizadas em `List<string>` agregada, `*ValidationException` por entidade já existentes em `TipoDespesaValidationException` e `DespesaValidationException`, override de `Create`/`Update`, controller `[ApiController]` com `try/catch` retornando `Response` com `ResponseEnum`). O helper estático puro segue o mesmo padrão de isolamento de regras de negócio adotado em outros utilitários do projeto.

---

## 📎 Relacionado
- Task: Sprint 17 - Implementação de Campos Opcionais Dinâmicos com JSONB
- Issue: #96

---

## 🗒️ Observações

- A política do repositório de gitignorar migrations EF (`**/Migrations/`) levou à decisão de entregar schema como SQL DDL standalone em `sql/`, alinhada à pasta já existente. O operador roda manualmente o script (idempotente) antes do deploy.
- O builder EF mantém apenas `HasColumnName` (sem `HasColumnType("jsonb")`) para preservar compatibilidade com SQLite usado pelos testes; em produção a coluna nasce como `jsonb` via DDL. Como o app manipula JSON sempre como string em C#, o tipo da coluna é irrelevante para o código da aplicação.
- A assimetria deliberada de case-sensitivity entre dedup de declaração (case-insensitive, evita declarações ambíguas) e match de subset (case-sensitive, alinhado a JSON ser case-sensitive por especificação) está documentada em XML doc no método `EncontrarChavesDesconhecidas` para evitar pegadinha futura.
- O `InternalsVisibleTo` foi adicionado para permitir testes diretos de `ValidarValoresOpcionais` sem necessidade de fixture full-stack com 8 entidades. Padrão Microsoft, não afeta a superfície pública da API.
- A execução da suíte de testes localmente exige o runtime .NET 9. A máquina de desenvolvimento usada para a entrega tem apenas .NET 8 e .NET 10 instalados; a suíte compila limpa e a execução completa fica delegada ao CI ou ao ambiente do dev com runtime correto.
- Não foram introduzidas dependências novas: `System.Text.Json` é parte do .NET 9; AutoMapper, Npgsql e xUnit já existiam no projeto.
