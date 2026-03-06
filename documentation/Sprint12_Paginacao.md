# Sprint 12 - Paginacao no Backend

## Resumo

Os endpoints de listagem passaram a usar paginação com `LIMIT/OFFSET` no repositório genérico. A mudança foi aplicada seguindo o fluxo já existente de `Controller -> Service -> Repository`, sem criar uma camada paralela.

## Endpoints afetados

Todos os `GET` de listagem que usam o serviço genérico agora aceitam paginação, incluindo:

- `despesas`
- `fornecedores`
- `secretarias`
- `usuarios`
- `orcamentos`
- `instituicoes`
- `documentos`
- `fluxos`
- `auditorias`
- `tipos de despesa`
- `tipos de instituicao`
- `unidades de medida`

## Query params

- `page`: página atual. Padrão `1`
- `size`: quantidade por página. Padrão `20`
- `sortBy`: campo escalar para ordenação
- `sortDirection`: `asc` ou `desc`

## Regras

- `size` máximo: `100`
- `page <= 0`: volta para `1`
- `size <= 0`: volta para `20`
- `sortBy` inválido: fallback automático para a chave primária da entidade

## Estrutura de retorno

O campo `data` da resposta padrão agora retorna:

```json
{
  "items": [],
  "totalRecords": 0,
  "totalPages": 0,
  "currentPage": 1,
  "pageSize": 20
}
```

## Exemplo

```http
GET /api/fornecedores?page=2&size=20&sortBy=NomeFantasia&sortDirection=desc
```

## Testes

Projeto de testes adicionado: `Civitas.WebAPI.Tests`

Cenários automatizados:

- payload paginado com metadados
- limite máximo por página
- ordenação segura com fallback para chave primária
- comparação de leitura completa vs leitura paginada no repositório

Execução:

```bash
dotnet test Civitas.WebAPI.Tests/Civitas.WebAPI.Tests.csproj
```
