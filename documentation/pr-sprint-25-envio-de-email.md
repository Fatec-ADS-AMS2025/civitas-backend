# Pull Request: Sistema de Envio de E-mail

## Objetivo

Implementar o módulo de envio de e-mails no backend, com validação na Service, envio SMTP, persistência de sucesso e falha, e consulta do histórico pelos endpoints da issue #77.

## Criterios de aceite

### Campos obrigatorios
- [x] Validar que `emailDestinatario`, `assunto` e `conteudo` nao estao vazios

### Validacao do e-mail do destinatario
- [x] Campo obrigatorio
- [x] Formato de e-mail valido
- [x] Recusa e-mails com espacos internos
- [x] Normaliza trim + minusculas antes do envio e da persistencia
- [x] Mensagem clara quando o e-mail e invalido

### Validacao do assunto
- [x] Campo obrigatorio
- [x] Recusa vazio ou so espacos
- [x] Tamanho maximo de 200 caracteres
- [x] Mensagem clara ao ultrapassar o limite

### Validacao do conteudo
- [x] Campo obrigatorio
- [x] Recusa vazio ou so espacos
- [x] Tamanho maximo de 10000 caracteres
- [x] Conteudo persistido no banco
- [x] Mensagem clara quando invalido

### Envio de e-mail
- [x] Servico de envio (`EmailService` + `IEmailSender`/`SmtpEmailSender`)
- [x] Remetente padrao via `Email:FromAddress` / `Email:FromName`
- [x] Envio para o destinatario do request
- [x] Registro de data/hora de envio
- [x] Status `PENDENTE`, `ENVIADO` e `FALHA`
- [x] Falha de comunicacao com o provedor tratada
- [x] Mensagem clara quando o envio falha (502, sem stack trace)

### Persistencia
- [x] Entidade `EmailEnviado`
- [x] Campos: id, emailDestinatario, assunto, conteudo, status, mensagemErro, dataEnvio, dataCriacao
- [x] Toda tentativa e registrada
- [x] DDL em `Civitas.WebAPI/sql/create_emailenviado.sql` (migrations EF continuam gitignoradas)
- [x] Integridade via builder (tamanhos, obrigatoriedade e indices)

### Endpoints
- [x] `POST /api/email`
- [x] `GET /api/email`
- [x] `GET /api/email/{id}`
- [x] `GET /api/email/destinatario/{email}`
- [x] `GET /api/email/status/{status}`

### Validacao em consulta
- [x] ID inexistente retorna 404 com mensagem clara
- [x] Destinatario com formato invalido retorna 400
- [x] Status invalido retorna 400
- [x] Respostas usam DTO, sem nulos indevidos

### DTOs
- [x] `EmailEnvioDto`
- [x] `EmailResponseDto`
- [x] `EmailFiltroDto`
- [x] Controller nao expoe a entidade
- [x] Mapeamento AutoMapper `EmailEnviado` -> `EmailResponseDto`

### Normalizacao e boas praticas
- [x] Validacoes na `EmailService`
- [x] Nao depende so de DataAnnotations
- [x] `EmailValidationException`
- [x] `EmailSendException`
- [x] Erros de validacao em lista unica
- [x] Validacao retorna 400
- [x] Falhas inesperadas retornam 500 padronizado
- [x] Sem stack trace para o usuario

## Alteracoes realizadas
- [x] Nova funcionalidade
- [ ] Correcao de bug
- [ ] Refatoracao de codigo
- [x] Documentacao
- [x] Testes automatizados

Envio síncrono no request. O registro nasce `PENDENTE`, passa a `ENVIADO` com `dataEnvio` ou `FALHA` com `mensagemErro`. Senha SMTP nao vai no codigo: `Email__Password` / `EMAIL_PASSWORD`.

## Evidencias de testes

`dotnet test --filter FullyQualifiedName~EmailEndpointsTests` — 17/17 aprovados.

Cobre campos obrigatorios, formato de e-mail, assunto e conteudo invalidos, falha do provedor com persistencia `FALHA`, persistencia `ENVIADO` com normalizacao, e consultas por ID, destinatario e status.

PDF da task: `documentation/sprint25_Task77_sistema-de-envio-de-email.pdf` (Felipe Pacheco Bianchini e Giovanni Nascimento).

Schema: aplicar `Civitas.WebAPI/sql/create_emailenviado.sql` no Postgres. O `.gitignore` do repo impede versionar `**/Migrations/`.

## Relacionado
- Task: Sprint 25 - Adicionar envio de emails
- Issue: #77

## Observacoes
- Provedor: MailKit 4.16.0 (correção GHSA-9j88-vvj5-vhgr).
- A suíte completa ainda tem 6 falhas pré-existentes (`DEV=true` nos testes de autorização/CEP e testes de paginação), fora deste módulo.
