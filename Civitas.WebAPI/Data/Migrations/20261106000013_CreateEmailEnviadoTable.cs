using System;
using Civitas.WebAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Civitas.WebAPI.Data.Migrations
{
    /// <summary>
    /// Cria a tabela <c>emailenviado</c>.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20261106000013_CreateEmailEnviadoTable")]
    public partial class CreateEmailEnviadoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "emailenviado",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    emaildestinatario = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    assunto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    conteudo = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    mensagemerro = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    dataenvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    datacriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emailenviado", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_emailenviado_emaildestinatario",
                table: "emailenviado",
                column: "emaildestinatario");

            migrationBuilder.CreateIndex(
                name: "IX_emailenviado_status",
                table: "emailenviado",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emailenviado");
        }
    }
}
