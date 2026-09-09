-- Sprint 25 — Sistema de Envio de E-mail
--
-- Cria a tabela de histórico de e-mails enviados (e tentativas com falha).
-- Idempotente. Aplicar com: psql -f create_emailenviado.sql
--
-- Status:
--   1 = PENDENTE
--   2 = ENVIADO
--   3 = FALHA

BEGIN;

CREATE TABLE IF NOT EXISTS emailenviado (
    id SERIAL PRIMARY KEY,
    emaildestinatario VARCHAR(255) NOT NULL,
    assunto VARCHAR(200) NOT NULL,
    conteudo VARCHAR(10000) NOT NULL,
    status INTEGER NOT NULL,
    mensagemerro VARCHAR(1000) NULL,
    dataenvio TIMESTAMP NULL,
    datacriacao TIMESTAMP NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_emailenviado_destinatario
    ON emailenviado (emaildestinatario);

CREATE INDEX IF NOT EXISTS ix_emailenviado_status
    ON emailenviado (status);

COMMIT;

-- Rollback (executar manualmente se necessario):
--   DROP INDEX IF EXISTS ix_emailenviado_status;
--   DROP INDEX IF EXISTS ix_emailenviado_destinatario;
--   DROP TABLE IF EXISTS emailenviado;
