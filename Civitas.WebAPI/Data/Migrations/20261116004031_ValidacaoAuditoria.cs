using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Civitas.WebAPI.Data.Migrations
{
    /// <summary>
    /// Ajusta a tabela <c>auditoria</c> para a validação da Sprint 30 (Task 110).
    /// </summary>
    /// <remarks>
    /// - <c>valores_atingidos</c> e <c>novos_valores</c>: de 500 para 10000 caracteres (snapshots completos em JSON).
    /// - <c>usuario_id</c>: passa a aceitar nulo (operação sem usuário autenticado).
    /// - Trigger que bloqueia UPDATE, DELETE e TRUNCATE na tabela, tornando o histórico imutável também no banco.
    /// Nenhum registro existente é alterado.
    /// </remarks>
    public partial class ValidacaoAuditoria : Migration
    {
        private const string FuncaoImutavel = "auditoria_impedir_alteracao";
        private const string TriggerLinhas = "trg_auditoria_imutavel";
        private const string TriggerTruncate = "trg_auditoria_sem_truncate";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "valores_atingidos",
                table: "auditoria",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "usuario_id",
                table: "auditoria",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "novos_valores",
                table: "auditoria",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            if (migrationBuilder.IsNpgsql())
            {
                migrationBuilder.Sql($"""
                    CREATE OR REPLACE FUNCTION {FuncaoImutavel}() RETURNS trigger AS $$
                    BEGIN
                        RAISE EXCEPTION 'Registros de auditoria são imutáveis: % não é permitido.', TG_OP
                            USING ERRCODE = 'restrict_violation';
                    END;
                    $$ LANGUAGE plpgsql;

                    CREATE TRIGGER {TriggerLinhas}
                        BEFORE UPDATE OR DELETE ON auditoria
                        FOR EACH ROW EXECUTE FUNCTION {FuncaoImutavel}();

                    CREATE TRIGGER {TriggerTruncate}
                        BEFORE TRUNCATE ON auditoria
                        FOR EACH STATEMENT EXECUTE FUNCTION {FuncaoImutavel}();
                    """);
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// O rollback não altera nem apaga histórico: falha se houver auditorias sem usuário
        /// ou snapshots maiores que 500 caracteres.
        /// </remarks>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsNpgsql())
            {
                migrationBuilder.Sql($"""
                    DROP TRIGGER IF EXISTS {TriggerTruncate} ON auditoria;
                    DROP TRIGGER IF EXISTS {TriggerLinhas} ON auditoria;
                    DROP FUNCTION IF EXISTS {FuncaoImutavel}();

                    DO $$
                    BEGIN
                        IF EXISTS (SELECT 1 FROM auditoria WHERE usuario_id IS NULL) THEN
                            RAISE EXCEPTION 'Rollback cancelado: existem auditorias sem usuário e o histórico não pode ser alterado.';
                        END IF;
                    END;
                    $$;

                    ALTER TABLE auditoria ALTER COLUMN usuario_id SET NOT NULL;
                    """);
            }
            else
            {
                migrationBuilder.AlterColumn<int>(
                    name: "usuario_id",
                    table: "auditoria",
                    type: "integer",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "integer",
                    oldNullable: true);
            }

            migrationBuilder.AlterColumn<string>(
                name: "valores_atingidos",
                table: "auditoria",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000);

            migrationBuilder.AlterColumn<string>(
                name: "novos_valores",
                table: "auditoria",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000);
        }
    }
}
