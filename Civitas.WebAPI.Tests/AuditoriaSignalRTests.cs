using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Hubs;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Civitas.WebAPI.Tests;

public sealed class AuditoriaSignalRTests : IClassFixture<AuditoriaSignalRTests.SignalRFactory>
{
    private const string JwtSecret = "development-only-key-change-before-production-2026";
    private const string JwtIssuer = "Civitas.WebAPI";
    private const string JwtAudience = "Civitas.Client";
    private readonly SignalRFactory _factory;

    public AuditoriaSignalRTests(SignalRFactory factory) => _factory = factory;

    [Fact]
    public async Task HubNegotiation_RequiresAuthenticatedAdministrator_EvenWhenDevBypassIsEnabled()
    {
        using var client = _factory.CreateClient();

        var anonymous = await client.PostAsync("/hubs/auditoria/negotiate?negotiateVersion=1", null);
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);

        using var employeeRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/hubs/auditoria/negotiate?negotiateVersion=1");
        employeeRequest.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BuildToken("FUNCIONARIO"));
        var employee = await client.SendAsync(employeeRequest);
        Assert.Equal(HttpStatusCode.Forbidden, employee.StatusCode);

        var administrator = await client.PostAsync(
            $"/hubs/auditoria/negotiate?negotiateVersion=1&access_token={BuildToken("ADMINISTRADOR")}",
            null);
        Assert.Equal(HttpStatusCode.OK, administrator.StatusCode);
    }

    [Fact]
    public async Task CreatingAudit_PublishesMinimalEventAfterPersistence()
    {
        _factory.LifetimeManager.Reset();
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario());
            return Task.CompletedTask;
        });

        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auditorias", new
        {
            data = "2026-11-08",
            hora = "10:15:30",
            operacao = "UPDATE",
            nomeEntidade = "Fornecedor",
            valoresAtingidos = "snapshot antigo sensível",
            novosValores = "snapshot novo sensível",
            usuarioId = 1
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sent = Assert.Single(_factory.LifetimeManager.Messages);
        Assert.Equal(AuditoriaHub.EventoCriada, sent.Method);
        var notification = Assert.IsType<AuditoriaNotificacaoDTO>(Assert.Single(sent.Arguments));
        Assert.True(notification.Id > 0);
        Assert.Equal("UPDATE", notification.Operacao);
        Assert.Equal("Fornecedor", notification.NomeEntidade);
        Assert.Equal("2026-11-08", notification.Data);
        Assert.Equal("10:15:30", notification.Hora);
        Assert.Equal(TimeSpan.Zero, notification.NotificadoEmUtc.Offset);

        var payload = System.Text.Json.JsonSerializer.Serialize(notification,
            new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
        Assert.DoesNotContain("snapshot", payload, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("usuarioId", payload, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("valoresAtingidos", payload, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("novosValores", payload, StringComparison.OrdinalIgnoreCase);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.True(await db.Auditorias.AnyAsync(a => a.Id == notification.Id));
    }

    [Fact]
    public async Task NotificationFailure_DoesNotUndoPersistedAuditOrFailRequest()
    {
        _factory.LifetimeManager.Reset(failOnSend: true);
        await _factory.ResetDatabaseAsync(context =>
        {
            context.Usuarios.Add(CreateUsuario());
            return Task.CompletedTask;
        });

        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auditorias", new
        {
            data = "2026-11-08",
            hora = "10:15:30",
            operacao = "INSERT",
            nomeEntidade = "Fornecedor",
            valoresAtingidos = "{}",
            novosValores = "{}",
            usuarioId = 1
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, _factory.LifetimeManager.SendAttempts);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(1, await db.Auditorias.CountAsync());
    }

    [Fact]
    public async Task InvalidAudit_IsNotPublished()
    {
        _factory.LifetimeManager.Reset();
        await _factory.ResetDatabaseAsync(_ => Task.CompletedTask);

        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auditorias", new
        {
            data = "2026-11-08",
            hora = "10:15:30",
            operacao = "INSERT",
            nomeEntidade = "Fornecedor",
            valoresAtingidos = "{}",
            novosValores = "{}",
            usuarioId = 999
        });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(0, _factory.LifetimeManager.SendAttempts);
    }

    private static string BuildToken(string tipoUsuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, "1"),
                new Claim("name", "Teste"),
                new Claim("tipousuario", tipoUsuario)
            ],
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static Usuario CreateUsuario() => new(
        1, "52998224725", "Admin Teste", "12345678", "Rua das Flores", "123",
        "Maringa", "PR", "87060000", "admin@example.com", "hash", Situacao.ATIVO,
        "MAT-001", TipoUsuario.ADMINISTRADOR, "Centro");

    public sealed class SignalRFactory : WebApplicationFactory<Civitas.WebAPI.Controllers.AuditoriaController>, IAsyncLifetime
    {
        private readonly SqliteConnection _connection = new("Data Source=:memory:");

        public CapturingHubLifetimeManager LifetimeManager { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(new Dictionary<string, string?> { ["DEV"] = "true" }));

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(IDbContextOptionsConfiguration<AppDbContext>));
                services.RemoveAll<AppDbContext>();
                services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
                services.RemoveAll<HubLifetimeManager<AuditoriaHub>>();
                services.AddSingleton<HubLifetimeManager<AuditoriaHub>>(LifetimeManager);
            });
        }

        public async Task InitializeAsync() => await _connection.OpenAsync();

        public new async Task DisposeAsync()
        {
            await base.DisposeAsync();
            await _connection.DisposeAsync();
        }

        public async Task ResetDatabaseAsync(Func<AppDbContext, Task> seedAsync)
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            await seedAsync(context);
            await context.SaveChangesAsync();
        }
    }

    public sealed class CapturingHubLifetimeManager : HubLifetimeManager<AuditoriaHub>
    {
        private readonly ConcurrentQueue<(string Method, object?[] Arguments)> _messages = new();
        private int _sendAttempts;
        private volatile bool _failOnSend;

        public IReadOnlyCollection<(string Method, object?[] Arguments)> Messages => _messages.ToArray();
        public int SendAttempts => _sendAttempts;

        public void Reset(bool failOnSend = false)
        {
            while (_messages.TryDequeue(out _)) { }
            _sendAttempts = 0;
            _failOnSend = failOnSend;
        }

        public override Task OnConnectedAsync(HubConnectionContext connection) => Task.CompletedTask;
        public override Task OnDisconnectedAsync(HubConnectionContext connection) => Task.CompletedTask;

        public override Task SendAllAsync(string methodName, object?[] args, CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _sendAttempts);
            if (_failOnSend) return Task.FromException(new InvalidOperationException("simulated transport failure"));
            _messages.Enqueue((methodName, args));
            return Task.CompletedTask;
        }

        public override Task SendAllExceptAsync(string methodName, object?[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task SendConnectionAsync(string connectionId, string methodName, object?[] args, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object?[] args, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task SendGroupAsync(string groupName, string methodName, object?[] args, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object?[] args, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task SendGroupExceptAsync(string groupName, string methodName, object?[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task SendUserAsync(string userId, string methodName, object?[] args, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object?[] args, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public override Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
