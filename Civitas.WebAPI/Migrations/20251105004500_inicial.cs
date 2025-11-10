using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Civitas.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fluxo",
                columns: table => new
                {
                    idfluxo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    valorpago = table.Column<float>(type: "real", maxLength: 100, nullable: false),
                    consumo = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fluxo", x => x.idfluxo);
                });

            migrationBuilder.CreateTable(
                name: "fornecedor",
                columns: table => new
                {
                    idFornecedor = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nomeFantasia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    telefone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fornecedor", x => x.idFornecedor);
                });

            migrationBuilder.CreateTable(
                name: "secretaria",
                columns: table => new
                {
                    idSecretaria = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nomeRazaoSocial = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secretaria", x => x.idSecretaria);
                });

            migrationBuilder.CreateTable(
                name: "tipoinstituicao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipoinstituicao", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rg = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    senha = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    matricula = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipousuario = table.Column<int>(type: "integer", nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "documento",
                columns: table => new
                {
                    IdDocumento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Digitalizacao = table.Column<byte[]>(type: "bytea", nullable: false),
                    NumeroDocumento = table.Column<int>(type: "integer", nullable: false),
                    IdFornecedor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documento", x => x.IdDocumento);
                    table.ForeignKey(
                        name: "FK_documento_fornecedor_IdFornecedor",
                        column: x => x.IdFornecedor,
                        principalTable: "fornecedor",
                        principalColumn: "idFornecedor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "instituicao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    nomerazaosocial = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    telefone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    idtipoinstituicao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instituicao", x => x.id);
                    table.ForeignKey(
                        name: "FK_instituicao_tipoinstituicao_idtipoinstituicao",
                        column: x => x.idtipoinstituicao,
                        principalTable: "tipoinstituicao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    usuario_id = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_documento_IdFornecedor",
                table: "documento",
                column: "IdFornecedor");

            migrationBuilder.CreateIndex(
                name: "IX_instituicao_idtipoinstituicao",
                table: "instituicao",
                column: "idtipoinstituicao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria");

            migrationBuilder.DropTable(
                name: "documento");

            migrationBuilder.DropTable(
                name: "fluxo");

            migrationBuilder.DropTable(
                name: "instituicao");

            migrationBuilder.DropTable(
                name: "secretaria");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "fornecedor");

            migrationBuilder.DropTable(
                name: "tipoinstituicao");
        }
    }
}
