using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Civitas.WebAPI.Tests;

/// <summary>
/// Sprint 30 - Task 110: validação, geração automática e imutabilidade da auditoria.
/// </summary>
public sealed class AuditoriaValidacaoTests : IClassFixture<AuditoriaValidacaoTests.AuditoriaFactory>
{
    private const string JwtSecret = "development-only-key-change-before-production-2026";
    private const string DataFixa = "2026-11-07";
    private const string HoraFixa = "14:30:59";
    private static readonly DateTimeOffset InstanteFixo = new(2026, 11, 7, 14, 30, 59, TimeSpan.Zero);
    private const int AdminId = 1;
    private const int FuncionarioId = 2;

    private readonly AuditoriaFactory _factory;

    public AuditoriaValidacaoTests(AuditoriaFactory factory) => _factory = factory;

    // ------------------------------------------------------------------
    // Validador (regras dos campos e dos snapshots)
    // ------------------------------------------------------------------

    [Fact]
    public void Validar_RegistroValido_NaoRetornaErros()
    {
        Assert.Empty(AuditoriaValidator.Validar(Registro()));
    }

    [Fact]
    public void Validar_VariosProblemas_RetornaTodosOsErrosJuntos()
    {
        var auditoria = Registro(operacao: "FOO", nomeEntidade: " ", novosValores: "{invalido");

        var errors = AuditoriaValidator.Validar(auditoria);

        Assert.Contains(errors, e => e.Contains("Operação 'FOO' inválida"));
        Assert.Contains(errors, e => e.Contains("nome da entidade é obrigatório"));
        Assert.Contains(errors, e => e.Contains("NovosValores contém JSON inválido"));
    }

    [Theory]
    [InlineData("insert")]
    [InlineData(" INSERT")]
    [InlineData("LOGOUT")]
    [InlineData("")]
    [InlineData("   ")]
    public void Validar_OperacaoForaDoPadrao_EhRejeitada(string operacao)
    {
        var errors = AuditoriaValidator.Validar(Registro(operacao: operacao));

        Assert.Contains(errors, e => e.Contains("operação", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("Pagamento")]
    [InlineData("Auditoria")]
    [InlineData("PasswordResetToken")]
    public void Validar_EntidadeDesconhecida_EhRejeitada(string nomeEntidade)
    {
        var errors = AuditoriaValidator.Validar(Registro(nomeEntidade: nomeEntidade));

        Assert.Contains(errors, e => e.Contains($"Entidade '{nomeEntidade}' inválida"));
    }

    [Fact]
    public void Validar_LoginExigeEntidadeAutenticacao()
    {
        var login = Registro(operacao: "LOGIN", nomeEntidade: "Usuario", valoresAtingidos: "", novosValores: "");
        var insertEmAutenticacao = Registro(nomeEntidade: "Autenticacao");

        Assert.Contains(AuditoriaValidator.Validar(login), e => e.Contains("LOGIN deve usar a entidade Autenticacao"));
        Assert.Contains(AuditoriaValidator.Validar(insertEmAutenticacao), e => e.Contains("só pode ser usada na operação LOGIN"));
    }

    [Theory]
    [InlineData("2026-02-30", HoraFixa)]
    [InlineData("07/11/2026", HoraFixa)]
    [InlineData("2026-11-7", HoraFixa)]
    [InlineData("", HoraFixa)]
    [InlineData(DataFixa, "25:70:90")]
    [InlineData(DataFixa, "2:30:59")]
    [InlineData(DataFixa, "14:30")]
    [InlineData(DataFixa, "")]
    public void Validar_DataOuHoraInvalida_EhRejeitada(string data, string hora)
    {
        var errors = AuditoriaValidator.Validar(Registro(data: data, hora: hora));

        Assert.Contains(errors, e => e.Contains("data", StringComparison.OrdinalIgnoreCase) || e.Contains("hora", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("INSERT", "", "", "Na operação INSERT, NovosValores é obrigatório.")]
    [InlineData("INSERT", "{\"Id\":1}", "{\"Id\":1}", "Na operação INSERT, ValoresAtingidos deve ficar vazio.")]
    [InlineData("UPDATE", "", "{\"Id\":1}", "Na operação UPDATE, ValoresAtingidos é obrigatório.")]
    [InlineData("UPDATE", "{\"Id\":1}", "", "Na operação UPDATE, NovosValores é obrigatório.")]
    [InlineData("DELETE", "", "", "Na operação DELETE, ValoresAtingidos é obrigatório.")]
    [InlineData("DELETE", "{\"Id\":1}", "{\"Id\":1}", "Na operação DELETE, NovosValores deve ficar vazio.")]
    public void Validar_SnapshotsIncompativeisComOperacao_SaoRejeitados(string operacao, string antes, string depois, string erroEsperado)
    {
        var errors = AuditoriaValidator.Validar(Registro(operacao: operacao, valoresAtingidos: antes, novosValores: depois));

        Assert.Contains(erroEsperado, errors);
    }

    [Fact]
    public void Validar_LoginComSnapshot_EhRejeitado()
    {
        var errors = AuditoriaValidator.Validar(Registro(operacao: "LOGIN", nomeEntidade: "Autenticacao",
            valoresAtingidos: "", novosValores: "{\"Id\":1}"));

        Assert.Contains("Na operação LOGIN, NovosValores deve ficar vazio.", errors);
    }

    [Theory]
    [InlineData("ValoresAtingidos", "{\"Id\":", "contém JSON inválido")]
    [InlineData("NovosValores", "{\"Id\":", "contém JSON inválido")]
    [InlineData("ValoresAtingidos", "[1,2]", "deve ser um objeto JSON")]
    [InlineData("NovosValores", "texto livre", "contém JSON inválido")]
    public void Validar_JsonMalformadoEmCadaCampo_EhRejeitado(string campo, string valor, string trecho)
    {
        var auditoria = Registro(operacao: "UPDATE", valoresAtingidos: "{\"Id\":1}", novosValores: "{\"Id\":2}");
        if (campo == "ValoresAtingidos") auditoria.ValoresAtingidos = valor;
        else auditoria.NovosValores = valor;

        Assert.Contains(AuditoriaValidator.Validar(auditoria), e => e.StartsWith(campo) && e.Contains(trecho));
    }

    [Fact]
    public void Validar_SnapshotMaiorQueOLimite_EhRejeitado()
    {
        var grande = JsonSerializer.Serialize(new { Texto = new string('a', AuditoriaValidator.TamanhoMaximoSnapshot) });

        var errors = AuditoriaValidator.Validar(Registro(novosValores: grande));

        Assert.Contains(errors, e => e.Contains("no máximo 10000 caracteres"));
    }

    [Fact]
    public void Validar_DadoSensivelSemMascara_EhRejeitado()
    {
        var exposto = Registro(nomeEntidade: "Usuario", novosValores: "{\"Id\":1,\"Senha\":\"$2a$11$hash\"}");
        var mascarado = Registro(nomeEntidade: "Usuario", novosValores: "{\"Id\":1,\"Senha\":\"***\"}");

        Assert.Contains(AuditoriaValidator.Validar(exposto), e => e.Contains("dados sensíveis sem máscara: Senha"));
        Assert.Empty(AuditoriaValidator.Validar(mascarado));
    }

    [Fact]
    public void Validar_IdInformadoOuUsuarioInvalido_SaoRejeitados()
    {
        var auditoria = Registro(usuarioId: 0);
        typeof(Auditoria).GetProperty(nameof(Auditoria.Id))!.SetValue(auditoria, 10);

        var errors = AuditoriaValidator.Validar(auditoria);

        Assert.Contains(errors, e => e.Contains("gerado pelo banco de dados"));
        Assert.Contains(errors, e => e.Contains("UsuarioId da auditoria deve ser maior que zero"));
    }

    // ------------------------------------------------------------------
    // Geração automática pelas operações reais
    // ------------------------------------------------------------------

    [Fact]
    public async Task Insert_GeraAuditoriaComAutorDataEHoraDoBackend()
    {
        await _factory.ResetAsync();
        using var client = ClienteComToken(AdminId, "ADMINISTRADOR");

        var response = await client.PostAsJsonAsync("/api/unidade-medida", new { descricao = "Litro", abreviatura = "L", situacao = 1 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var (unidade, auditoria) = await LerUnicaAsync(db => db.UnidadesMedida);
        Assert.Equal(AuditoriaOperacao.INSERT, auditoria.Operacao);
        Assert.Equal("UnidadeMedida", auditoria.NomeEntidade);
        Assert.Equal(DataFixa, auditoria.Data);
        Assert.Equal(HoraFixa, auditoria.Hora);
        Assert.Equal(AdminId, auditoria.UsuarioId);
        Assert.Equal(string.Empty, auditoria.ValoresAtingidos);

        using var novos = JsonDocument.Parse(auditoria.NovosValores);
        Assert.Equal(unidade.Id, novos.RootElement.GetProperty("Id").GetInt32());
        Assert.Equal("Litro", novos.RootElement.GetProperty("Descricao").GetString());
        Assert.Equal("ATIVO", novos.RootElement.GetProperty("Situacao").GetString());

        var notificacao = Assert.Single(_factory.Notificacoes.Itens);
        Assert.Equal(auditoria.Id, notificacao.Id);
    }

    [Fact]
    public async Task Update_RegistraSomenteCamposAlteradosComOEstadoAnteriorReal()
    {
        await _factory.ResetAsync(db => db.UnidadesMedida.Add(new UnidadeMedida(0, "Litro", "L", Situacao.ATIVO)));
        using var client = ClienteComToken(AdminId, "ADMINISTRADOR");
        var id = await IdUnicoAsync(db => db.UnidadesMedida.Select(u => u.Id));

        var response = await client.PutAsJsonAsync($"/api/unidade-medida/{id}", new { descricao = "Mililitro", abreviatura = "L", situacao = 1 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var (_, auditoria) = await LerUnicaAsync(db => db.UnidadesMedida);
        Assert.Equal(AuditoriaOperacao.UPDATE, auditoria.Operacao);
        Assert.Equal("{\"Descricao\":\"Litro\"}", auditoria.ValoresAtingidos);
        Assert.Equal("{\"Descricao\":\"Mililitro\"}", auditoria.NovosValores);
    }

    [Fact]
    public async Task ExclusaoLogica_EhRegistradaComoUpdateDoCampoExcluido()
    {
        await _factory.ResetAsync(db => db.UnidadesMedida.Add(new UnidadeMedida(0, "Litro", "L", Situacao.ATIVO)));
        using var client = ClienteComToken(AdminId, "ADMINISTRADOR");
        var id = await IdUnicoAsync(db => db.UnidadesMedida.Select(u => u.Id));

        var response = await client.PatchAsync($"/api/unidade-medida/{id}/status-exclusao", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var (_, auditoria) = await LerUnicaAsync(db => db.UnidadesMedida);
        Assert.Equal(AuditoriaOperacao.UPDATE, auditoria.Operacao);
        using var antes = JsonDocument.Parse(auditoria.ValoresAtingidos);
        using var depois = JsonDocument.Parse(auditoria.NovosValores);
        Assert.False(antes.RootElement.GetProperty("Excluido").GetBoolean());
        Assert.True(depois.RootElement.GetProperty("Excluido").GetBoolean());
    }

    [Fact]
    public async Task ExclusaoFisica_RegistraDeleteComOEstadoAnterior()
    {
        await _factory.ResetAsync(db => db.TipoCodigos.Add(new TipoCodigo { Nome = "NF", Descricao = "Nota Fiscal" }));
        using var client = ClienteComToken(AdminId, "ADMINISTRADOR");
        var id = await IdUnicoAsync(db => db.TipoCodigos.Select(t => t.Id));

        var response = await client.DeleteAsync($"/api/tipo-codigo/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(await db.TipoCodigos.ToListAsync());
        var auditoria = Assert.Single(await db.Auditorias.ToListAsync());
        Assert.Equal(AuditoriaOperacao.DELETE, auditoria.Operacao);
        Assert.Equal("TipoCodigo", auditoria.NomeEntidade);
        Assert.Equal(string.Empty, auditoria.NovosValores);
        using var antes = JsonDocument.Parse(auditoria.ValoresAtingidos);
        Assert.Equal(id, antes.RootElement.GetProperty("Id").GetInt32());
        Assert.Equal("Nota Fiscal", antes.RootElement.GetProperty("Descricao").GetString());
    }

    [Fact]
    public async Task CadastroPublico_SemUsuarioAutenticado_RegistraAutorNuloEMascaraSenha()
    {
        await _factory.ResetAsync(semUsuarios: true);
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            nome = "Ana da Silva", cpf = "52998224725", rg = "12345678", logradouro = "Rua das Flores", numero = "123",
            bairro = "Centro", cidade = "Maringá", estado = "PR", cep = "87060000", email = "ana@example.com",
            senha = "Senha123", matricula = "VIS-001"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var (usuario, auditoria) = await LerUnicaAsync(db => db.Usuarios);
        Assert.Equal(AuditoriaOperacao.INSERT, auditoria.Operacao);
        Assert.Equal("Usuario", auditoria.NomeEntidade);
        Assert.Null(auditoria.UsuarioId);
        Assert.Contains("\"Senha\":\"***\"", auditoria.NovosValores);
        Assert.DoesNotContain(usuario.Senha, auditoria.NovosValores);
        Assert.DoesNotContain("Senha123", auditoria.NovosValores);
        Assert.Contains("Maringá", auditoria.NovosValores);
    }

    [Fact]
    public async Task Login_RegistraEventoDeAutenticacaoDoProprioUsuario()
    {
        await _factory.ResetAsync(db =>
        {
            var usuario = db.Usuarios.Single(u => u.Id == FuncionarioId);
            usuario.Senha = BCrypt.Net.BCrypt.HashPassword("abc12345");
        });
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { email = "funcionario@example.com", senha = "abc12345" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await using var scope = _factory.Services.CreateAsyncScope();
        var auditoria = Assert.Single(await scope.ServiceProvider.GetRequiredService<AppDbContext>().Auditorias.ToListAsync());
        Assert.Equal(AuditoriaOperacao.LOGIN, auditoria.Operacao);
        Assert.Equal("Autenticacao", auditoria.NomeEntidade);
        Assert.Equal(FuncionarioId, auditoria.UsuarioId);
        Assert.Equal(string.Empty, auditoria.ValoresAtingidos);
        Assert.Equal(string.Empty, auditoria.NovosValores);
        Assert.Equal(DataFixa, auditoria.Data);
        Assert.Equal(HoraFixa, auditoria.Hora);
        Assert.Single(_factory.Notificacoes.Itens);
    }

    [Fact]
    public async Task OperacaoRejeitada_NaoGeraNemPublicaAuditoria()
    {
        await _factory.ResetAsync();
        using var client = ClienteComToken(AdminId, "ADMINISTRADOR");

        var response = await client.PostAsJsonAsync("/api/unidade-medida", new { descricao = " ", abreviatura = " ", situacao = 99 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, await ContarAuditoriasAsync());
        Assert.Empty(_factory.Notificacoes.Itens);
    }

    [Fact]
    public async Task FalhaAoGerarAuditoria_DesfazAOperacaoPrincipalSemExporDetalhes()
    {
        await _factory.ResetAsync();
        using var client = ClienteComToken(999, "ADMINISTRADOR");

        var response = await client.PostAsJsonAsync("/api/unidade-medida", new { descricao = "Litro", abreviatura = "L", situacao = 1 });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(await db.UnidadesMedida.ToListAsync());
        Assert.Empty(await db.Auditorias.ToListAsync());
        Assert.Empty(_factory.Notificacoes.Itens);
    }

    [Fact]
    public async Task TransacaoExterna_PublicaSomenteNoCommitEDescartaNoRollback()
    {
        await _factory.ResetAsync();
        var accessor = _factory.Services.GetRequiredService<IHttpContextAccessor>();
        accessor.HttpContext = new DefaultHttpContext { User = Principal(AdminId) };
        try
        {
            await using (var scope = _factory.Services.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await using var transacao = await db.Database.BeginTransactionAsync();
                db.UnidadesMedida.Add(new UnidadeMedida(0, "Desfeita", "D", Situacao.ATIVO));
                await db.SaveChangesAsync();

                Assert.Equal(1, await db.Auditorias.CountAsync());
                Assert.Empty(_factory.Notificacoes.Itens);
                await transacao.RollbackAsync();
            }

            Assert.Equal(0, await ContarAuditoriasAsync());
            Assert.Empty(_factory.Notificacoes.Itens);

            await using (var scope = _factory.Services.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await using var transacao = await db.Database.BeginTransactionAsync();
                db.UnidadesMedida.Add(new UnidadeMedida(0, "Confirmada", "C", Situacao.ATIVO));
                await db.SaveChangesAsync();
                Assert.Empty(_factory.Notificacoes.Itens);

                await transacao.CommitAsync();
            }

            Assert.Equal(1, await ContarAuditoriasAsync());
            Assert.Single(_factory.Notificacoes.Itens);
        }
        finally
        {
            accessor.HttpContext = null;
        }
    }

    // ------------------------------------------------------------------
    // Imutabilidade
    // ------------------------------------------------------------------

    [Fact]
    public async Task AuditoriaPersistida_NaoPodeSerAlteradaNemExcluida()
    {
        await _factory.ResetAsync(db => db.Auditorias.Add(Registro(usuarioId: AdminId)));

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var auditoria = await db.Auditorias.SingleAsync();
            auditoria.Operacao = AuditoriaOperacao.DELETE;
            var alteracao = await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
            Assert.Contains("imutáveis", alteracao.Message);

            db.ChangeTracker.Clear();
            db.Auditorias.Remove(await db.Auditorias.SingleAsync());
            var exclusao = await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
            Assert.Contains("imutáveis", exclusao.Message);
        }

        await using var verificacao = _factory.Services.CreateAsyncScope();
        var persistida = await verificacao.ServiceProvider.GetRequiredService<AppDbContext>().Auditorias.SingleAsync();
        Assert.Equal(AuditoriaOperacao.INSERT, persistida.Operacao);
    }

    [Fact]
    public async Task AuditoriaInvalida_NaoEhGravadaMesmoForaDaApi()
    {
        await _factory.ResetAsync();

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Auditorias.Add(Registro(operacao: "FOO", novosValores: "{quebrado"));

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
        var validacao = Assert.IsType<AuditoriaValidationException>(erro.InnerException);
        Assert.True(validacao.Errors.Count >= 2);
        db.ChangeTracker.Clear();
        Assert.Equal(0, await db.Auditorias.CountAsync());
    }

    // ------------------------------------------------------------------
    // Endpoints de consulta
    // ------------------------------------------------------------------

    [Fact]
    public async Task Consulta_ExigeAdministradorMesmoComDevAtivo()
    {
        await _factory.ResetAsync();

        using var anonimo = _factory.CreateClient();
        using var funcionario = ClienteComToken(FuncionarioId, "FUNCIONARIO");
        using var administrador = ClienteComToken(AdminId, "ADMINISTRADOR");

        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync("/api/auditorias")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await funcionario.GetAsync("/api/auditorias")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await administrador.GetAsync("/api/auditorias")).StatusCode);
    }

    [Fact]
    public async Task Consulta_ValidaIdsEFiltrosERetorna404ParaInexistente()
    {
        await _factory.ResetAsync(db => db.Auditorias.Add(Registro(usuarioId: AdminId)));
        using var client = ClienteComToken(AdminId, "ADMINISTRADOR");
        var id = await IdUnicoAsync(db => db.Auditorias.Select(a => a.Id));

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/auditorias/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/auditorias/0")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/auditorias/-5")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/auditorias/9999")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/auditorias/usuario/0")).StatusCode);

        var operacaoInvalida = await client.GetAsync("/api/auditorias/operacao?operacao=FOO");
        Assert.Equal(HttpStatusCode.BadRequest, operacaoInvalida.StatusCode);
        Assert.Contains("Operação 'FOO' inválida", await operacaoInvalida.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/auditorias/operacao")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/auditorias/operacao?operacao=insert")).StatusCode);

        var entidadeInvalida = await client.GetAsync("/api/auditorias/entidade?nomeEntidade=Pagamento");
        Assert.Equal(HttpStatusCode.BadRequest, entidadeInvalida.StatusCode);
        Assert.Contains("Entidade 'Pagamento' inválida", await entidadeInvalida.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/auditorias/entidade?nomeEntidade=unidademedida")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/auditorias/usuario/{AdminId}")).StatusCode);
    }

    [Fact]
    public async Task Consulta_RetornaDtoSemDadosSensiveisEPaginado()
    {
        await _factory.ResetAsync(db => db.Auditorias.Add(Registro(usuarioId: AdminId)));
        using var client = ClienteComToken(AdminId, "ADMINISTRADOR");

        var body = await client.GetStringAsync("/api/auditorias?page=1&size=10");

        Assert.Contains("\"totalRecords\":1", body);
        Assert.Contains("\"operacao\":\"INSERT\"", body);
        Assert.DoesNotContain("senha", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("stackTrace", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ClienteNaoConsegueCriarAlterarNemExcluirAuditoria()
    {
        await _factory.ResetAsync(db => db.Auditorias.Add(Registro(usuarioId: AdminId)));
        using var client = ClienteComToken(AdminId, "ADMINISTRADOR");
        var id = await IdUnicoAsync(db => db.Auditorias.Select(a => a.Id));
        var forjada = new
        {
            id = 500, data = "2020-01-01", hora = "00:00:00", operacao = "DELETE",
            nomeEntidade = "Usuario", valoresAtingidos = "{}", novosValores = "", usuarioId = FuncionarioId
        };

        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await client.PostAsJsonAsync("/api/auditorias", forjada)).StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await client.PutAsJsonAsync($"/api/auditorias/{id}", forjada)).StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await client.DeleteAsync($"/api/auditorias/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PatchAsync($"/api/auditorias/situacao/{id}", null)).StatusCode);

        await using var scope = _factory.Services.CreateAsyncScope();
        var unica = Assert.Single(await scope.ServiceProvider.GetRequiredService<AppDbContext>().Auditorias.ToListAsync());
        Assert.Equal(AuditoriaOperacao.INSERT, unica.Operacao);
        Assert.Empty(_factory.Notificacoes.Itens);
    }

    // ------------------------------------------------------------------
    // Apoio
    // ------------------------------------------------------------------

    private static Auditoria Registro(
        string data = DataFixa,
        string hora = HoraFixa,
        string operacao = "INSERT",
        string nomeEntidade = "UnidadeMedida",
        string valoresAtingidos = "",
        string novosValores = "{\"Id\":1,\"Descricao\":\"Litro\"}",
        int? usuarioId = AdminId) =>
        new(data, hora, operacao, nomeEntidade, valoresAtingidos, novosValores, usuarioId);

    private async Task<(T Entidade, Auditoria Auditoria)> LerUnicaAsync<T>(Func<AppDbContext, IQueryable<T>> consulta)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var auditoria = Assert.Single(await db.Auditorias.ToListAsync());
        var entidades = await consulta(db).ToListAsync();
        return (entidades.Last(), auditoria);
    }

    private async Task<int> IdUnicoAsync(Func<AppDbContext, IQueryable<int>> consulta)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        return await consulta(scope.ServiceProvider.GetRequiredService<AppDbContext>()).SingleAsync();
    }

    private async Task<int> ContarAuditoriasAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<AppDbContext>().Auditorias.CountAsync();
    }

    private HttpClient ClienteComToken(int usuarioId, string tipoUsuario)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token(usuarioId, tipoUsuario));
        return client;
    }

    private static string Token(int usuarioId, string tipoUsuario)
    {
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "Civitas.WebAPI",
            audience: "Civitas.Client",
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
                new Claim("name", "Teste"),
                new Claim("tipousuario", tipoUsuario)
            ],
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static ClaimsPrincipal Principal(int usuarioId) =>
        new(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())], "Teste"));

    private static Usuario NovoUsuario(int id, string cpf, string email, string matricula, TipoUsuario tipo) =>
        new(id, cpf, $"Usuario {id}", "123456789", "Rua Teste", "1", "Maringa", "PR", "87000000",
            email, "hash", Situacao.ATIVO, matricula, tipo, "Centro");

    public sealed class AuditoriaFactory : WebApplicationFactory<Civitas.WebAPI.Controllers.AuditoriaController>, IAsyncLifetime
    {
        private readonly SqliteConnection _connection = new("Data Source=:memory:");

        public NotificacoesCapturadas Notificacoes { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging => logging.ClearProviders());
            builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(new Dictionary<string, string?> { ["DEV"] = "true" }));

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(IDbContextOptionsConfiguration<AppDbContext>));
                services.RemoveAll<AppDbContext>();
                services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));

                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(new TempoFixo(InstanteFixo));

                services.RemoveAll<IAuditoriaNotificacaoService>();
                services.AddSingleton<IAuditoriaNotificacaoService>(Notificacoes);
            });
        }

        public async Task InitializeAsync() => await _connection.OpenAsync();

        public new async Task DisposeAsync()
        {
            await base.DisposeAsync();
            await _connection.DisposeAsync();
        }

        /// <summary>Recria o banco com um administrador (Id 1) e um funcionário (Id 2), sem auditorias.</summary>
        public async Task ResetAsync(Action<AppDbContext>? seed = null, bool semUsuarios = false)
        {
            await using (var scope = Services.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();

                if (!semUsuarios)
                {
                    context.Usuarios.Add(NovoUsuario(AdminId, "52998224725", "admin@example.com", "MAT-ADM", TipoUsuario.ADMINISTRADOR));
                    context.Usuarios.Add(NovoUsuario(FuncionarioId, "11144477735", "funcionario@example.com", "MAT-FUN", TipoUsuario.FUNCIONARIO));
                    await context.SaveChangesAsync();
                }

                seed?.Invoke(context);
                await context.SaveChangesAsync();
            }

            Notificacoes.Limpar();
        }
    }

    public sealed class NotificacoesCapturadas : IAuditoriaNotificacaoService
    {
        private readonly ConcurrentQueue<AuditoriaNotificacaoDTO> _itens = new();

        public IReadOnlyCollection<AuditoriaNotificacaoDTO> Itens => _itens.ToArray();

        public void Limpar()
        {
            while (_itens.TryDequeue(out _)) { }
        }

        public Task PublicarCriadaAsync(AuditoriaNotificacaoDTO notificacao)
        {
            _itens.Enqueue(notificacao);
            return Task.CompletedTask;
        }
    }

    private sealed class TempoFixo(DateTimeOffset instante) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => instante;
    }
}
