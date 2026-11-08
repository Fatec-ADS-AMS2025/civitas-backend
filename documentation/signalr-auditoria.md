# Notificações de auditoria em tempo real

## Conexão

- Hub: `/hubs/auditoria`
- Evento servidor → cliente: `AuditoriaCriada`
- Transporte: SignalR, com WebSockets quando disponíveis e fallback negociado pelo cliente.
- A conexão exige JWT válido e a claim `tipousuario=ADMINISTRADOR`. O bypass `DEV` não libera este Hub.
- Para navegador, envie o JWT no `accessTokenFactory` do cliente SignalR. O backend lê `access_token` somente em `/hubs/auditoria`; não registre a URL completa da conexão.
- Origens do Hub ficam em `SignalR:AllowedOrigins`. Configure essa lista com as origens confiáveis do frontend em cada ambiente.

## Payload

`AuditoriaCriada` recebe um objeto com os campos:

| Campo | Tipo | Significado |
| --- | --- | --- |
| `id` | number | Identificador persistido da auditoria; pode ser usado para deduplicação e ordenação. |
| `operacao` | string | Operação registrada. |
| `nomeEntidade` | string | Entidade afetada. |
| `data` | string | Valor de data armazenado no registro de auditoria. |
| `hora` | string | Valor de hora armazenado no registro de auditoria. |
| `notificadoEmUtc` | string (ISO 8601) | Instante UTC da publicação, distinto da data/hora original armazenada. |

O evento não inclui snapshots (`valoresAtingidos`, `novosValores`), usuário, credenciais ou dados pessoais. Consulte `/api/auditorias` e os demais endpoints protegidos para obter detalhes autorizados. A API de consulta permanece a fonte do histórico; SignalR não garante entrega a clientes desconectados.

## Exemplo TypeScript

```ts
import { HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";

const connection = new HubConnectionBuilder()
  .withUrl(`${apiBaseUrl}/hubs/auditoria`, {
    accessTokenFactory: () => tokenAtual
  })
  .withAutomaticReconnect()
  .build();

connection.on("AuditoriaCriada", (aviso) => {
  // O aviso é enxuto; atualize a lista consultando a API protegida.
  atualizarListaDeAuditorias(aviso.id);
});

connection.onreconnected(async () => {
  // Reconexões não recuperam mensagens perdidas: sincronize com a API.
  await recarregarAuditorias();
});

if (connection.state === HubConnectionState.Disconnected) {
  await connection.start();
}
```

O frontend deve renovar `tokenAtual` no `accessTokenFactory`. O Hub fecha a conexão quando o JWT expira. Revogações antecipadas de JWT stateless passam a valer após a expiração do token; o projeto não mantém uma lista de revogação em tempo real.

## Persistência e disponibilidade

O backend publica o aviso somente depois que o repositório confirma o `SaveChangesAsync`. Falhas no envio são registradas no log do servidor e não revertem a auditoria nem falham a operação já persistida. Para recuperar mensagens perdidas, recarregue a listagem pela API; não há garantia de entrega exatamente uma vez nem outbox nesta implementação.
