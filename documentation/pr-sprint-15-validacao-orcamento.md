# Pull Request: Sprint 15 - Validação de Orçamento

## Objetivo

Garantir que o cadastro e a manutenção de orçamentos possuam validações completas de integridade, consistência e regras de negócio, evitando dados inválidos e estouro de limite financeiro no sistema. A entrega move as validações para a camada de Service, padroniza o tipo monetário, retorna todos os erros em uma única lista e torna atômicas as operações que dependem do saldo orçamentário.

## Critérios de Aceite

- Remoção do campo `IdTipoDespesa` da entidade Orçamento.
- Validação de campos obrigatórios (`anoOrcamento`, `valorOrcamento`, `idInstituicao`).
- Validação do ano do orçamento (4 dígitos, maior ou igual a 2000, até no máximo 10 anos à frente, sem negativos ou zero).
- Validação do valor do orçamento (maior que zero, precisão monetária com `decimal`).
- Validação de integridade da instituição (existência real da `idInstituicao` no banco).
- Regra de negócio de controle de gastos (soma das despesas vinculadas não pode ultrapassar `valorOrcamento` em cadastro e atualização de despesas, com mensagem clara de estouro).
- Validação em atualização de orçamento (novo `valorOrcamento` não pode ser inferior ao total já comprometido em despesas).
- Validação de exclusão (orçamento com despesas vinculadas não pode ser removido).
- Normalização e boas práticas (`decimal` com arredondamento de 2 casas, validações na camada Service, não depender exclusivamente de `DataAnnotations`).
- Retorno dos erros agregados em uma única lista, ao invés de uma mensagem por vez.
- Uso de transações ao validar despesas e orçamento para evitar inconsistências concorrentes.

## Alterações Realizadas

- Nova funcionalidade
- Correção de bug
- Refatoração de código

Foram movidas as validações de orçamento do Controller para o Service, introduzida uma exceção de validação com lista de erros, o tipo monetário foi alterado para `decimal`, a relação com `TipoDespesa` foi removida do orçamento e todas as operações de escrita que dependem do saldo orçamentário passaram a ser executadas dentro de uma transação com isolamento `Serializable`.

## Evidências de Testes

A solução foi validada por meio de compilação limpa do projeto e execução da suíte de testes automatizados existente. Nenhum teste novo quebrou após as mudanças, e os testes que já falhavam anteriormente foram verificados como pré-existentes ao PR.

### O que foi implementado

- Exclusão do campo `IdTipoDespesa` da entidade `Orcamento`, do `OrcamentoDTO`, do `ModelSnapshot` e criação de migration que remove a coluna `idtipodespesa` e o shadow FK `TipoDespesaId` da tabela `orcamento`.
- Alteração do tipo de `ValorOrcamento` de `double` para `decimal` com `HasPrecision(18,2)`, além do mesmo ajuste em `InstituicaoOrcamentoDisponivelDTO.TotalOrcamentoDisponivel` para manter consistência com o padrão já adotado em `SecretariaOrcamentoDisponivelDTO`.
- Criação da classe `OrcamentoValidationException` em `Civitas.WebAPI/Services/Validation`, seguindo o mesmo padrão de `UsuarioValidationException`, permitindo o retorno de todos os erros de validação em uma única lista.
- Reescrita do `OrcamentoService` com sobrescrita de `Create`, `Update` e a adição de `RemoverAsync`, centralizando as validações em `ValidarOrcamentoAsync` e garantindo:
  - Validação agregada de `AnoOrcamento` (obrigatório, 4 dígitos, maior ou igual a 2000, no máximo 10 anos à frente).
  - Validação agregada de `ValorOrcamento` (maior que zero, arredondado para 2 casas decimais).
  - Validação agregada de `IdInstituicao` (obrigatório, positivo, existência validada via `IInstituicaoRepository`).
  - Validação em atualização que impede definir um `ValorOrcamento` menor que o total já comprometido em despesas.
  - Bloqueio de exclusão de orçamento com despesas vinculadas.
- Remoção das validações inline do `OrcamentoController`, que passou a delegar toda a lógica ao Service e apenas tratar `OrcamentoValidationException` e `KeyNotFoundException`, retornando a lista de erros no campo `Data` do `Response`.
- Inclusão do endpoint `DELETE /api/orcamentos/{id}` no `OrcamentoController`, que utiliza `RemoverAsync` e bloqueia a operação caso existam despesas vinculadas.
- Implementação do controle de estouro de orçamento no `DespesaService.ValidarLimiteOrcamentarioAsync`, que soma `ConsumoPrevisto` das despesas existentes (excluindo a despesa em edição, se aplicável) e compara com `ValorOrcamento`, retornando mensagem clara com o valor excedente quando o limite é ultrapassado.
- Remoção da checagem obsoleta `orcamento.IdTipoDespesa > 0 && orcamento.IdTipoDespesa != despesaDTO.IdTipoDespesa` no `DespesaService`, que dependia do campo agora removido.
- Inclusão dos métodos `SumConsumoByOrcamentoAsync` em `IDespesaRepository`/`DespesaRepository` e `IOrcamentoRepository`/`OrcamentoRepository` para calcular o total comprometido de um orçamento de forma performática e confiável, com arredondamento consistente.
- Envolvimento das operações de escrita em transações com isolamento `IsolationLevel.Serializable`:
  - `DespesaService.Create` e `DespesaService.Update` passaram a executar validação e persistência dentro de uma mesma transação, evitando race condition entre a leitura da soma de despesas e a inserção da nova despesa.
  - `OrcamentoService.Create`, `OrcamentoService.Update` e `OrcamentoService.RemoverAsync` passaram a executar a validação e a persistência dentro de uma mesma transação.
- Ajuste do teste `FornecedorValidationEndpointsTests.PostDespesa_WithInactiveFornecedor_ReturnsBadRequest` para remover a inicialização de `IdTipoDespesa` no seed do orçamento, adequando-o ao novo modelo.

### Alterações técnicas

- Todas as validações passaram a residir na camada de Service, sem depender exclusivamente de `DataAnnotations`, respeitando a arquitetura já documentada em `.planning/codebase/ARCHITECTURE.md`.
- O retorno dos erros de validação foi padronizado com o mesmo formato utilizado em Usuário, expondo os itens no campo `Data` do `Response` padrão da API.
- O tipo monetário foi uniformizado em `decimal` com `HasPrecision(18,2)` e arredondamento `MidpointRounding.AwayFromZero`, prevenindo problemas de precisão já conhecidos em operações financeiras.
- A migration `OrcamentoValidacoesRefactor` foi gerada pelo EF Core 9 e é ignorada pelo Git conforme política do repositório (`.gitignore` contém `**/Migrations/`), sendo mantido no versionamento apenas o `AppDbContextModelSnapshot.cs` atualizado.
- As transações utilizam `IsolationLevel.Serializable` via `Database.BeginTransactionAsync`, compatível tanto com o PostgreSQL (produção) quanto com o SQLite em memória (testes).

### Testes adicionados

- Não foram adicionados testes automatizados nesta entrega, pois o escopo do card prioriza as regras de validação e o ajuste do domínio.
- Foi realizado ajuste no seed de um teste existente (`PostDespesa_WithInactiveFornecedor_ReturnsBadRequest`) para compatibilizá-lo com a remoção do campo `IdTipoDespesa`.

### Verificações executadas

- `dotnet build Civitas.WebAPI/Civitas.WebAPI.sln` — 0 erros, warnings iguais à baseline anterior ao PR.
- `dotnet ef migrations add OrcamentoValidacoesRefactor` — migration gerada com sucesso, revisada e confirmada como alinhada ao modelo.
- `dotnet test Civitas.WebAPI.Tests/Civitas.WebAPI.Tests.csproj` — 33 testes passando, 5 falhas verificadas como pré-existentes ao PR (auth, CEP e `PostDespesa_WithInactiveFornecedor_ReturnsBadRequest`, confirmadas reproduzindo antes das mudanças).

## Relacionado

- Task: Sprint 15 - Validação de Orçamento (Task 79)

## Observações

- O domínio atual não possui um campo `ValorDespesa` dedicado na entidade `Despesa`. Seguindo o padrão já adotado em `InstituicaoRepository` e `SecretariaRepository`, o controle de estouro utiliza `ConsumoPrevisto` como proxy do valor comprometido. Caso o time opte por criar um campo monetário próprio para a despesa no futuro, a troca do somatório ficará restrita a `SumConsumoByOrcamentoAsync` e à regra de `ValidarLimiteOrcamentarioAsync`.
- A migration gerada também sincroniza a renomeação de `situacao` para `status` na tabela `despesa`, que estava divergente no `ModelSnapshot` desde o PR anterior (Sprint 15, Task 074). Essa mudança não faz parte do escopo desta task, mas é necessária para manter o snapshot íntegro.