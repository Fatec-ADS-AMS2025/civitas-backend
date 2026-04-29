# Documentacao de Rotas: Instituicao, Despesas e Secretaria

Todas as rotas abaixo exigem autenticacao via `[Authorize]`.

## Padrao de resposta da API

Todas as respostas seguem este envelope:

```json
{
  "code": "SUCCESS | INVALID | NOT_FOUND | CONFLICT | UNAUTHORIZED | ERROR",
  "message": "Mensagem descritiva",
  "data": {}
}
```

Valores conhecidos de `code`:

- `SUCCESS`
- `INVALID`
- `NOT_FOUND`
- `CONFLICT`
- `UNAUTHORIZED`
- `ERROR`

## Paginacao usada nas listagens

As rotas paginadas aceitam query string com:

- `page` (padrao `1`)
- `size` (padrao `20`, maximo `100`)
- `sortBy` (opcional)
- `sortDirection` (opcional, ex: `asc` ou `desc`)

Formato do `data` nas rotas paginadas:

```json
{
  "items": [],
  "totalRecords": 0,
  "totalPages": 0,
  "currentPage": 1,
  "pageSize": 20
}
```

## Enums expostos

### `Situacao`

- `ATIVO`
- `INATIVO`

### `Status`

- `A_PAGAR`
- `PAGA`
- `ATRASADO`

---

## Instituicao

Base route: `api/instituicoes`

### DTO de entrada/saida: `InstituicaoDTO`

```json
{
  "id": 0,
  "cnpj": "string",
  "nome": "string",
  "logradouro": "string",
  "numero": "string",
  "bairro": "string",
  "cep": "string",
  "nomeRazaoSocial": "string",
  "telefone": "string",
  "email": "string",
  "cidade": "string",
  "estado": "string",
  "situacao": "ATIVO | INATIVO",
  "idTipoInstituicao": 0,
  "idSecretaria": 0
}
```

Observacoes do DTO:

- `id`: no `POST` o backend zera para `0`.
- `cnpj`: obrigatorio e validado.
- `email`: obrigatorio e validado.
- `idTipoInstituicao`: obrigatorio.
- `idSecretaria`: obrigatorio.

### `GET /api/instituicoes`

O que pede:

- Query params de paginacao.

O que envia:

- `200 OK`
- `data`: `PaginatedResult<InstituicaoDTO>` com instituicoes ativas.

### `GET /api/instituicoes/inativos`

O que pede:

- Query params de paginacao.

O que envia:

- `200 OK`
- `data`: `PaginatedResult<InstituicaoDTO>` com instituicoes inativas.

### `GET /api/instituicoes/{id}`

O que pede:

- Path param `id`.

O que envia:

- `200 OK`
- `data`: `InstituicaoDTO`

Erros:

- `404 NotFound`: quando nao encontra a instituicao.

### `GET /api/instituicoes/nome?name={name}`

O que pede:

- Query param `name`.

O que envia:

- `200 OK`
- `data`: array de `InstituicaoDTO`

Erros:

- `404 NotFound`: quando nenhuma instituicao bate com o nome.

### `GET /api/instituicoes/{id}/gastos`

O que pede:

- Path param `id`.

O que envia:

- `200 OK`
- `data`:

```json
{
  "idInstituicao": 0,
  "nomeInstituicao": "string",
  "quantidadeDespesas": 0,
  "totalGastos": 0.0
}
```

Erros:

- `404 NotFound`: quando a instituicao nao existe.

### `GET /api/instituicoes/{id}/orcamento-disponivel`

O que pede:

- Path param `id`.

O que envia:

- `200 OK`
- `data`:

```json
{
  "idInstituicao": 0,
  "nomeInstituicao": "string",
  "totalOrcamentoDisponivel": 0.0
}
```

Erros:

- `404 NotFound`: quando a instituicao nao existe.

### `POST /api/instituicoes`

O que pede:

- Body JSON no formato de `InstituicaoDTO`.

Exemplo:

```json
{
  "id": 0,
  "cnpj": "12345678000199",
  "nome": "Escola Municipal Centro",
  "logradouro": "Rua A",
  "numero": "100",
  "bairro": "Centro",
  "cep": "12345678",
  "nomeRazaoSocial": "Escola Municipal Centro LTDA",
  "telefone": "11999999999",
  "email": "contato@escola.gov.br",
  "cidade": "Cidade Exemplo",
  "estado": "SP",
  "situacao": "ATIVO",
  "idTipoInstituicao": 1,
  "idSecretaria": 2
}
```

O que envia:

- `200 OK`
- `data`: `InstituicaoDTO` salvo

Erros:

- `400 BadRequest`: body nulo ou validacao invalida.
- `409 Conflict`: conflito de campos, como CNPJ/E-mail duplicado.
- `500 InternalServerError`: erro interno.

### `PUT /api/instituicoes/{id}`

O que pede:

- Path param `id`.
- Body JSON no formato de `InstituicaoDTO`.

O que envia:

- `200 OK`
- `data`: `InstituicaoDTO` atualizado

Erros:

- `400 BadRequest`: body nulo ou validacao invalida.
- `404 NotFound`: instituicao nao encontrada.
- `409 Conflict`: conflito de campos.
- `500 InternalServerError`: erro interno.

### `PATCH /api/instituicoes/situacao/{id}`

O que pede:

- Path param `id`.
- Nao recebe body.

O que envia:

- `200 OK`
- `data`:

```json
{
  "id": 0,
  "situacao": "ATIVO | INATIVO"
}
```

Comportamento:

- Se estiver `ATIVO`, vira `INATIVO`.
- Se estiver `INATIVO`, vira `ATIVO`.

Erros:

- `404 NotFound`: instituicao nao encontrada.
- `400 BadRequest`: validacao invalida.
- `409 Conflict`: conflito de dados.
- `500 InternalServerError`: erro interno.

---

## Despesas

Base route: `api/despesas`

### DTO de entrada/saida: `DespesaDTO`

```json
{
  "id": 0,
  "numeroDocumento": "string",
  "codigo": 0,
  "uc": "string",
  "dataEmissao": "YYYY-MM-DD",
  "consumoPrevisto": 0.0,
  "dataVencimento": "YYYY-MM-DD",
  "status": "A_PAGAR | PAGA | ATRASADO",
  "idTipoDespesa": 0,
  "idOrcamento": 0,
  "idInstituicao": 0,
  "idFornecedor": 0,
  "idUsuario": 0
}
```

Observacoes do DTO:

- `id`: no `POST` o backend zera para `0`.
- `numeroDocumento`: obrigatorio.
- `uc`: pode ser obrigatorio dependendo do tipo de despesa.
- `dataEmissao`: string; recomendacao do projeto e usar formato ISO `YYYY-MM-DD`.
- `dataVencimento`: obrigatorio.
- `status`: no `POST`, o backend sempre seta `A_PAGAR`.
- `idTipoDespesa`: obrigatorio.
- `idOrcamento`: obrigatorio.
- `idUsuario`: pode ser inferido do usuario autenticado, mas o DTO aceita envio.

### `GET /api/despesas`

O que pede:

- Query params de paginacao.

O que envia:

- `200 OK`
- `data`: `PaginatedResult<DespesaDTO>` com despesas `A_PAGAR`.

### `GET /api/despesas/pagas`

O que pede:

- Query params de paginacao.

O que envia:

- `200 OK`
- `data`: `PaginatedResult<DespesaDTO>` com despesas `PAGA`.

### `GET /api/despesas/atrasadas`

O que pede:

- Query params de paginacao.

O que envia:

- `200 OK`
- `data`: `PaginatedResult<DespesaDTO>` com despesas `ATRASADO`.

### `GET /api/despesas/{id}`

O que pede:

- Path param `id`.

O que envia:

- `200 OK`
- `data`: `DespesaDTO`

Erros:

- `404 NotFound`: quando nao encontra a despesa.

### `POST /api/despesas`

O que pede:

- Body JSON no formato de `DespesaDTO`.

Exemplo:

```json
{
  "id": 0,
  "numeroDocumento": "NF-2026-0001",
  "codigo": 123,
  "uc": "UC123456",
  "dataEmissao": "2026-04-25",
  "consumoPrevisto": 150.75,
  "dataVencimento": "2026-05-10",
  "status": "A_PAGAR",
  "idTipoDespesa": 1,
  "idOrcamento": 10,
  "idInstituicao": 5,
  "idFornecedor": 7,
  "idUsuario": 3
}
```

O que envia:

- `200 OK`
- `data`: `DespesaDTO` salvo

Comportamento:

- O backend sobrescreve `status` para `A_PAGAR`.

Erros:

- `400 BadRequest`: body nulo ou dados invalidos.
- `500 InternalServerError`: erro interno.

### `PUT /api/despesas/{id}`

O que pede:

- Path param `id`.
- Body JSON no formato de `DespesaDTO`.

O que envia:

- `200 OK`
- `data`: `DespesaDTO` atualizado

Erros:

- `400 BadRequest`: body nulo ou dados invalidos.
- `404 NotFound`: despesa nao encontrada.
- `500 InternalServerError`: erro interno.

### `PATCH /api/despesas/status/{id}`

O que pede:

- Path param `id`.
- Body contendo apenas o enum `Status`.

Exemplo de body:

```json
"PAGA"
```

O que envia:

- `200 OK`
- `data`:

```json
{
  "id": 0,
  "statusAtual": "A_PAGAR | PAGA | ATRASADO"
}
```

Erros:

- `404 NotFound`: despesa nao encontrada.
- `500 InternalServerError`: erro interno.

---

## Secretaria

Base route: `api/secretarias`

### DTO de entrada/saida: `SecretariaDTO`

```json
{
  "idSecretaria": 0,
  "situacao": "ATIVO | INATIVO",
  "descricao": "string",
  "cnpj": "string",
  "nome": "string",
  "logradouro": "string",
  "numero": "string",
  "bairro": "string",
  "cep": "string",
  "nomeRazaoSocial": "string",
  "email": "string",
  "telefone": "string",
  "cidade": "string",
  "estado": "string"
}
```

Observacoes do DTO:

- `idSecretaria`: no `POST` o backend zera para `0`.
- `situacao`: usa enum `ATIVO` ou `INATIVO`.
- `cnpj`: obrigatorio.
- `email`: validado.

### `GET /api/secretarias`

O que pede:

- Query params de paginacao.

O que envia:

- `200 OK`
- `data`: `PaginatedResult<SecretariaDTO>` com secretarias ativas.

### `GET /api/secretarias/inativos`

O que pede:

- Query params de paginacao.

O que envia:

- `200 OK`
- `data`: `PaginatedResult<SecretariaDTO>` com secretarias inativas.

### `GET /api/secretarias/{id}`

O que pede:

- Path param `id`.

O que envia:

- `200 OK`
- `data`: `SecretariaDTO`

Erros:

- `404 NotFound`: secretaria nao encontrada.

### `GET /api/secretarias/{id}/gastos`

O que pede:

- Path param `id`.

O que envia:

- `200 OK`
- `data`:

```json
{
  "idSecretaria": 0,
  "nomeSecretaria": "string",
  "quantidadeInstituicoes": 0,
  "quantidadeDespesas": 0,
  "totalGastos": 0.0
}
```

Erros:

- `404 NotFound`: secretaria nao encontrada.

### `GET /api/secretarias/{id}/orcamento-disponivel`

O que pede:

- Path param `id`.

O que envia:

- `200 OK`
- `data`:

```json
{
  "idSecretaria": 0,
  "nomeSecretaria": "string",
  "quantidadeInstituicoes": 0,
  "totalOrcamentoDisponivel": 0.0
}
```

Erros:

- `404 NotFound`: secretaria nao encontrada.

### `POST /api/secretarias`

O que pede:

- Body JSON no formato de `SecretariaDTO`.

Exemplo:

```json
{
  "idSecretaria": 0,
  "situacao": "ATIVO",
  "descricao": "Gestao da rede municipal de ensino",
  "cnpj": "12345678000199",
  "nome": "Secretaria de Educacao",
  "logradouro": "Avenida Central",
  "numero": "500",
  "bairro": "Centro",
  "cep": "12345678",
  "nomeRazaoSocial": "Secretaria Municipal de Educacao",
  "email": "educacao@prefeitura.gov.br",
  "telefone": "1133334444",
  "cidade": "Cidade Exemplo",
  "estado": "SP"
}
```

O que envia:

- `200 OK`
- `data`: `SecretariaDTO` salvo

Erros:

- `400 BadRequest`: body nulo ou validacao invalida.
- `500 InternalServerError`: erro interno.

### `PUT /api/secretarias/{id}`

O que pede:

- Path param `id`.
- Body JSON no formato de `SecretariaDTO`.

O que envia:

- `200 OK`
- `data`: `SecretariaDTO` atualizado

Erros:

- `400 BadRequest`: body nulo ou validacao invalida.
- `404 NotFound`: secretaria nao encontrada.
- `500 InternalServerError`: erro interno.

### `PATCH /api/secretarias/situacao/{id}`

O que pede:

- Path param `id`.
- Nao recebe body.

O que envia:

- `200 OK`
- `data`:

```json
{
  "idSecretaria": 0,
  "situacao": "ATIVO | INATIVO"
}
```

Comportamento:

- Se estiver `ATIVO`, vira `INATIVO`.
- Se estiver `INATIVO`, vira `ATIVO`.

Erros:

- `404 NotFound`: secretaria nao encontrada.
- `500 InternalServerError`: erro interno.

---

## Observacoes finais

- Os controllers retornam `500` com `ErrorMessage` e `StackTrace` no `data` em caso de erro interno.
- Em `Instituicao` existem tratamentos explicitos para `409 Conflict` e validacoes detalhadas.
- Em `Despesa`, a mudanca de status e feita pela rota `PATCH /api/despesas/status/{id}` com body simples contendo o enum.
- Em `Secretaria` e `Instituicao`, a alteracao de situacao nao recebe body; o backend apenas alterna entre `ATIVO` e `INATIVO`.
