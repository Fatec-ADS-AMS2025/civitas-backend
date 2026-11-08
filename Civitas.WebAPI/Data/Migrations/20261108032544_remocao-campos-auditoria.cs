using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Civitas.WebAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class remocaocamposauditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dataexclusao",
                table: "auditoria");

            migrationBuilder.DropColumn(
                name: "excluido",
                table: "auditoria");

            migrationBuilder.DropColumn(
                name: "situacao",
                table: "auditoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "dataexclusao",
                table: "auditoria",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "excluido",
                table: "auditoria",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "situacao",
                table: "auditoria",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
