using System;
using Civitas.WebAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Civitas.WebAPI.Data.Migrations
{
    /// <summary>
    /// Cria a tabela <c>tipoinstituicao</c>.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20261106000005_CreateTipoInstituicaoTable")]
    public partial class CreateTipoInstituicaoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tipoinstituicao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    excluido = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    dataexclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipoinstituicao", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tipoinstituicao");
        }
    }
}
