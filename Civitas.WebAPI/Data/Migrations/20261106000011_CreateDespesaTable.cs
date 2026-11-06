using System;
using Civitas.WebAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Civitas.WebAPI.Data.Migrations
{
    /// <summary>
    /// Cria a tabela <c>despesa</c>.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20261106000011_CreateDespesaTable")]
    public partial class CreateDespesaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "despesa",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numerodocumento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nomedocumento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    hashdocumento = table.Column<string>(type: "text", nullable: true),
                    codigo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dataemissao = table.Column<DateOnly>(type: "date", nullable: false),
                    valorprevisto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    valorpago = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    juros = table.Column<decimal>(type: "numeric", nullable: false),
                    multa = table.Column<decimal>(type: "numeric", nullable: false),
                    desconto = table.Column<decimal>(type: "numeric", nullable: false),
                    consumoprevisto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    consumoreal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    datavencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    datapagamento = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    idusuario = table.Column<int>(type: "integer", nullable: false),
                    idunidadeconsumidora = table.Column<int>(type: "integer", nullable: false),
                    valoresopcionais = table.Column<string>(type: "jsonb", nullable: true),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    dataexclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_despesa", x => x.id);
                    table.ForeignKey(
                        name: "FK_despesa_unidadeconsumidora_idunidadeconsumidora",
                        column: x => x.idunidadeconsumidora,
                        principalTable: "unidadeconsumidora",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_despesa_usuario_idusuario",
                        column: x => x.idusuario,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_despesa_idunidadeconsumidora",
                table: "despesa",
                column: "idunidadeconsumidora");

            migrationBuilder.CreateIndex(
                name: "IX_despesa_idusuario",
                table: "despesa",
                column: "idusuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "despesa");
        }
    }
}
