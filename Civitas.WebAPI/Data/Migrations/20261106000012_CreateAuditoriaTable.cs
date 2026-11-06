using System;
using Civitas.WebAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Civitas.WebAPI.Data.Migrations
{
    /// <summary>
    /// Cria a tabela <c>auditoria</c>.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20261106000012_CreateAuditoriaTable")]
    public partial class CreateAuditoriaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auditoria",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    hora = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    operacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nome_entidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    valores_atingidos = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    novos_valores = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    dataexclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria", x => x.id);
                    table.ForeignKey(
                        name: "FK_auditoria_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_usuario_id",
                table: "auditoria",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria");
        }
    }
}
