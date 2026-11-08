using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Civitas.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class DespesaUnidadeConsumidora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_despesa_fornecedor_idfornecedor",
                table: "despesa");

            migrationBuilder.DropForeignKey(
                name: "FK_despesa_instituicao_idinstituicao",
                table: "despesa");

            migrationBuilder.DropForeignKey(
                name: "FK_despesa_orcamento_idorcamento",
                table: "despesa");

            migrationBuilder.DropForeignKey(
                name: "FK_despesa_tipodespesa_idtipodespesa",
                table: "despesa");

            migrationBuilder.CreateTable(
                name: "unidadeconsumidora",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    idtipodespesa = table.Column<int>(type: "integer", nullable: false),
                    idorcamento = table.Column<int>(type: "integer", nullable: false),
                    idinstituicao = table.Column<int>(type: "integer", nullable: false),
                    idfornecedor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unidadeconsumidora", x => x.id);
                    table.ForeignKey(
                        name: "FK_unidadeconsumidora_fornecedor_idfornecedor",
                        column: x => x.idfornecedor,
                        principalTable: "fornecedor",
                        principalColumn: "idfornecedor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_unidadeconsumidora_instituicao_idinstituicao",
                        column: x => x.idinstituicao,
                        principalTable: "instituicao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_unidadeconsumidora_orcamento_idorcamento",
                        column: x => x.idorcamento,
                        principalTable: "orcamento",
                        principalColumn: "idorcamento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_unidadeconsumidora_tipodespesa_idtipodespesa",
                        column: x => x.idtipodespesa,
                        principalTable: "tipodespesa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddColumn<int>(
                name: "idunidadeconsumidora",
                table: "despesa",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                """
                INSERT INTO unidadeconsumidora (codigo, situacao, idtipodespesa, idorcamento, idinstituicao, idfornecedor)
                SELECT DISTINCT
                    COALESCE(NULLIF(uc, ''), 'UC-' || id::text),
                    1,
                    idtipodespesa,
                    idorcamento,
                    idinstituicao,
                    idfornecedor
                FROM despesa;
                """);

            migrationBuilder.Sql(
                """
                UPDATE despesa d
                SET idunidadeconsumidora = u.id
                FROM unidadeconsumidora u
                WHERE u.codigo = COALESCE(NULLIF(d.uc, ''), 'UC-' || d.id::text)
                    AND u.idtipodespesa = d.idtipodespesa
                    AND u.idorcamento = d.idorcamento
                    AND u.idinstituicao = d.idinstituicao
                    AND u.idfornecedor = d.idfornecedor;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "idunidadeconsumidora",
                table: "despesa",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.Sql(
                "ALTER TABLE despesa ALTER COLUMN dataemissao TYPE date USING dataemissao::date;");

            migrationBuilder.AlterColumn<decimal>(
                name: "consumoprevisto",
                table: "despesa",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "codigo",
                table: "despesa",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<decimal>(
                name: "consumoreal",
                table: "despesa",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "valorpago",
                table: "despesa",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "valorprevisto",
                table: "despesa",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.DropIndex(
                name: "IX_despesa_idfornecedor",
                table: "despesa");

            migrationBuilder.DropIndex(
                name: "IX_despesa_idinstituicao",
                table: "despesa");

            migrationBuilder.DropIndex(
                name: "IX_despesa_idorcamento",
                table: "despesa");

            migrationBuilder.DropIndex(
                name: "IX_despesa_idtipodespesa",
                table: "despesa");

            migrationBuilder.DropColumn(
                name: "idfornecedor",
                table: "despesa");

            migrationBuilder.DropColumn(
                name: "idinstituicao",
                table: "despesa");

            migrationBuilder.DropColumn(
                name: "idorcamento",
                table: "despesa");

            migrationBuilder.DropColumn(
                name: "idtipodespesa",
                table: "despesa");

            migrationBuilder.DropColumn(
                name: "uc",
                table: "despesa");

            migrationBuilder.CreateIndex(
                name: "IX_despesa_idunidadeconsumidora",
                table: "despesa",
                column: "idunidadeconsumidora");

            migrationBuilder.CreateIndex(
                name: "IX_unidadeconsumidora_idfornecedor",
                table: "unidadeconsumidora",
                column: "idfornecedor");

            migrationBuilder.CreateIndex(
                name: "IX_unidadeconsumidora_idinstituicao",
                table: "unidadeconsumidora",
                column: "idinstituicao");

            migrationBuilder.CreateIndex(
                name: "IX_unidadeconsumidora_idorcamento",
                table: "unidadeconsumidora",
                column: "idorcamento");

            migrationBuilder.CreateIndex(
                name: "IX_unidadeconsumidora_idtipodespesa",
                table: "unidadeconsumidora",
                column: "idtipodespesa");

            migrationBuilder.AddForeignKey(
                name: "FK_despesa_unidadeconsumidora_idunidadeconsumidora",
                table: "despesa",
                column: "idunidadeconsumidora",
                principalTable: "unidadeconsumidora",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_despesa_unidadeconsumidora_idunidadeconsumidora",
                table: "despesa");

            migrationBuilder.DropIndex(
                name: "IX_despesa_idunidadeconsumidora",
                table: "despesa");

            migrationBuilder.DropColumn(
                name: "consumoreal",
                table: "despesa");

            migrationBuilder.DropColumn(
                name: "valorpago",
                table: "despesa");

            migrationBuilder.DropColumn(
                name: "valorprevisto",
                table: "despesa");

            migrationBuilder.Sql(
                "ALTER TABLE despesa ALTER COLUMN dataemissao TYPE character varying(20) USING dataemissao::text;");

            migrationBuilder.AlterColumn<double>(
                name: "consumoprevisto",
                table: "despesa",
                type: "double precision",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "codigo",
                table: "despesa",
                type: "integer",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "idtipodespesa",
                table: "despesa",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "idorcamento",
                table: "despesa",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "idinstituicao",
                table: "despesa",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "idfornecedor",
                table: "despesa",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "uc",
                table: "despesa",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE despesa d
                SET idtipodespesa = u.idtipodespesa,
                    idorcamento = u.idorcamento,
                    idinstituicao = u.idinstituicao,
                    idfornecedor = u.idfornecedor,
                    uc = u.codigo
                FROM unidadeconsumidora u
                WHERE u.id = d.idunidadeconsumidora;
                """);

            migrationBuilder.DropColumn(
                name: "idunidadeconsumidora",
                table: "despesa");

            migrationBuilder.DropTable(
                name: "unidadeconsumidora");

            migrationBuilder.CreateIndex(
                name: "IX_despesa_idfornecedor",
                table: "despesa",
                column: "idfornecedor");

            migrationBuilder.CreateIndex(
                name: "IX_despesa_idinstituicao",
                table: "despesa",
                column: "idinstituicao");

            migrationBuilder.CreateIndex(
                name: "IX_despesa_idorcamento",
                table: "despesa",
                column: "idorcamento");

            migrationBuilder.CreateIndex(
                name: "IX_despesa_idtipodespesa",
                table: "despesa",
                column: "idtipodespesa");

            migrationBuilder.AddForeignKey(
                name: "FK_despesa_fornecedor_idfornecedor",
                table: "despesa",
                column: "idfornecedor",
                principalTable: "fornecedor",
                principalColumn: "idfornecedor",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_despesa_instituicao_idinstituicao",
                table: "despesa",
                column: "idinstituicao",
                principalTable: "instituicao",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_despesa_orcamento_idorcamento",
                table: "despesa",
                column: "idorcamento",
                principalTable: "orcamento",
                principalColumn: "idorcamento",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_despesa_tipodespesa_idtipodespesa",
                table: "despesa",
                column: "idtipodespesa",
                principalTable: "tipodespesa",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
