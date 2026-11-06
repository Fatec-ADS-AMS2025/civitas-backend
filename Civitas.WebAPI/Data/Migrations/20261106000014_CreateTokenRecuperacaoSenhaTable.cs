using System;
using Civitas.WebAPI.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Civitas.WebAPI.Data.Migrations
{
    /// <summary>
    /// Cria a tabela <c>token_recuperacao_senha</c>.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20261106000014_CreateTokenRecuperacaoSenhaTable")]
    public partial class CreateTokenRecuperacaoSenhaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "token_recuperacao_senha",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expira_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    utilizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_token_recuperacao_senha", x => x.id);
                    table.ForeignKey(
                        name: "FK_token_recuperacao_senha_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_token_recuperacao_senha_token_hash",
                table: "token_recuperacao_senha",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_token_recuperacao_senha_usuario_id",
                table: "token_recuperacao_senha",
                column: "usuario_id",
                unique: true,
                filter: "utilizado_em IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "token_recuperacao_senha");
        }
    }
}
