-- Sprint 17 — Campos Opcionais Dinamicos com JSONB
--
-- Adiciona suporte a campos opcionais configuraveis por TipoDespesa e
-- valores correspondentes por Despesa, ambos armazenados como JSONB no
-- PostgreSQL.
--
-- Esquema esperado:
--   tipodespesa.camposopcionais  -> {"camposOpcionais": ["nome1", "nome2"]}
--   despesa.valoresopcionais     -> {"nome1": "valor", "nome2": null}
--
-- Ambas as colunas sao NULL-aveis (retrocompativel com registros existentes).
-- O default e NULL — registros antigos nao precisam de backfill.
--
-- Aplicar com: psql -f add_campos_opcionais_jsonb.sql ou via tool de migracao.

BEGIN;

ALTER TABLE tipodespesa
    ADD COLUMN IF NOT EXISTS camposopcionais jsonb;

ALTER TABLE despesa
    ADD COLUMN IF NOT EXISTS valoresopcionais jsonb;

COMMIT;

-- Rollback (executar manualmente se necessario):
--   ALTER TABLE despesa     DROP COLUMN IF EXISTS valoresopcionais;
--   ALTER TABLE tipodespesa DROP COLUMN IF EXISTS camposopcionais;
