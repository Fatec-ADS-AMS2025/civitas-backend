using System;
using Civitas.WebAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Civitas.WebAPI.Data.Migrations
{
    /// <summary>
    /// Cria a tabela <c>tipodespesa</c>.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20261106000006_CreateTipoDespesaTable")]
    public partial class CreateTipoDespesaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tipodespesa",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    idunidademedida = table.Column<int>(type: "integer", nullable: false),
                    solicitauc = table.Column<int>(type: "integer", nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    idtipocodigo = table.Column<int>(type: "integer", nullable: false),
                    camposopcionais = table.Column<string>(type: "jsonb", nullable: true),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    dataexclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipodespesa", x => x.id);
                    table.ForeignKey(
                        name: "FK_tipodespesa_tipocodigo_idtipocodigo",
                        column: x => x.idtipocodigo,
                        principalTable: "tipocodigo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tipodespesa_unidademedida_idunidademedida",
                        column: x => x.idunidademedida,
                        principalTable: "unidademedida",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tipodespesa_idtipocodigo",
                table: "tipodespesa",
                column: "idtipocodigo");

            migrationBuilder.CreateIndex(
                name: "IX_tipodespesa_idunidademedida",
                table: "tipodespesa",
                column: "idunidademedida");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tipodespesa");
        }
    }
}
