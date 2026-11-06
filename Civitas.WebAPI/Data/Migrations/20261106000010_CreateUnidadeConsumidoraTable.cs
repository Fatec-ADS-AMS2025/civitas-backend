using System;
using Civitas.WebAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Civitas.WebAPI.Data.Migrations
{
    /// <summary>
    /// Cria a tabela <c>unidadeconsumidora</c>.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20261106000010_CreateUnidadeConsumidoraTable")]
    public partial class CreateUnidadeConsumidoraTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "unidadeconsumidora",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identificador = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    idinstituicao = table.Column<int>(type: "integer", nullable: false),
                    idtipodespesa = table.Column<int>(type: "integer", nullable: false),
                    idsecretaria = table.Column<int>(type: "integer", nullable: false),
                    idorcamento = table.Column<int>(type: "integer", nullable: false),
                    idfornecedor = table.Column<int>(type: "integer", nullable: false),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    dataexclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                        name: "FK_unidadeconsumidora_secretaria_idsecretaria",
                        column: x => x.idsecretaria,
                        principalTable: "secretaria",
                        principalColumn: "idsecretaria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_unidadeconsumidora_tipodespesa_idtipodespesa",
                        column: x => x.idtipodespesa,
                        principalTable: "tipodespesa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_unidadeconsumidora_identificador",
                table: "unidadeconsumidora",
                column: "identificador",
                unique: true);

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
                name: "IX_unidadeconsumidora_idsecretaria",
                table: "unidadeconsumidora",
                column: "idsecretaria");

            migrationBuilder.CreateIndex(
                name: "IX_unidadeconsumidora_idtipodespesa",
                table: "unidadeconsumidora",
                column: "idtipodespesa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "unidadeconsumidora");
        }
    }
}
