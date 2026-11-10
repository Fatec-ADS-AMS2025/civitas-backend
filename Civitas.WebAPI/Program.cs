using Civitas.WebAPI.Data;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Data.Repositories;
using Civitas.WebAPI.Data.Seeders;
using Civitas.WebAPI.Hubs;
using Civitas.WebAPI.Services.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));
builder.Services.Configure<PasswordRecoveryOptions>(builder.Configuration.GetSection(PasswordRecoveryOptions.SectionName));
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(opt => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
var jwtKey = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) &&
                    context.HttpContext.Request.Path.StartsWithSegments("/hubs/auditoria"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuditoriaAuthorization.PolicyName, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new AuditoriaReadRequirement());
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, DevBypassAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AuditoriaReadHandler>();
builder.Services.AddSignalR();

var auditoriaAllowedOrigins = builder.Configuration
    .GetSection("SignalR:AllowedOrigins")
    .Get<string[]>()
    ?.Where(origin => !string.IsNullOrWhiteSpace(origin))
    .ToArray();
if (auditoriaAllowedOrigins is not { Length: > 0 })
{
    auditoriaAllowedOrigins =
    [
        builder.Configuration["PasswordRecovery:FrontendBaseUrl"] ?? "http://localhost:3000"
    ];
}

builder.Services.AddCors(options => options.AddPolicy("AuditoriaSignalR", policy =>
    policy
        .WithOrigins(auditoriaAllowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Informe o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPasswordHashService, BCryptPasswordHashService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IFornecedorRepository, FornecedorRepository>();
builder.Services.AddScoped<IFornecedorService, FornecedorService>();
builder.Services.AddScoped<ISecretariaRepository, SecretariaRepository>();
builder.Services.AddScoped<ISecretariaService, SecretariaService>();
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IAuditoriaNotificacaoService, AuditoriaNotificacaoService>();
builder.Services.AddScoped<IInstituicaoRepository, InstituicaoRepository>();
builder.Services.AddScoped<IInstituicaoService, InstituicaoService>();
builder.Services.AddScoped<ITipoInstituicaoRepository, TipoInstituicaoRepository>();
builder.Services.AddScoped<ITipoInstituicaoService, TipoInstituicaoService>();
builder.Services.AddScoped<IUnidadeMedidaRepository, UnidadeMedidaRepository>();
builder.Services.AddScoped<IUnidadeMedidaService, UnidadeMedidaService>();
builder.Services.AddScoped<ITipoDespesaRepository, TipoDespesaRepository>();
builder.Services.AddScoped<ITipoDespesaService, TipoDespesaService>();
builder.Services.AddScoped<IOrcamentoRepository, OrcamentoRepository>();
builder.Services.AddScoped<IOrcamentoService, OrcamentoService>();
builder.Services.AddScoped<IUnidadeConsumidoraRepository, UnidadeConsumidoraRepository>();
builder.Services.AddScoped<IDespesaRepository, DespesaRepository>();
builder.Services.AddScoped<IDespesaService, DespesaService>();
builder.Services.AddHttpClient<ICepService, CepService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ViaCep:BaseUrl"] ?? "https://viacep.com.br/";
    var timeoutSeconds = configuration.GetValue<int?>("ViaCep:TimeoutSeconds") ?? 10;

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});
builder.Services.AddScoped<ITipoCodigoRepository, TipoCodigoRepository>();
builder.Services.AddScoped<ITipoCodigoService, TipoCodigoService>();
builder.Services.AddScoped<IUnidadeConsumidoraRepository, UnidadeConsumidoraRepository>();
builder.Services.AddScoped<IUnidadeConsumidoraService, UnidadeConsumidoraService>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IEmailService, EmailService>();

if (DatabaseCommands.IsCommand(args))
{
    // No modo comando o EF loga como "fail" a consulta ao histórico de migrations antes dele existir;
    // os erros reais são reportados pelo próprio comando.
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
    builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
}

var app = builder.Build();

// Comandos de banco estilo Artisan: dotnet run -- migrate | migrate:fresh --seed | db:seed ...
if (DatabaseCommands.IsCommand(args))
{
    Environment.ExitCode = await DatabaseCommands.RunAsync(app.Services, args);
    return;
}

// Todo boot: aplica migrations pendentes e, se o banco estiver vazio (nenhuma tabela com dados), roda o seed.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Testes usam SQLite em memória com EnsureCreated; migrations são exclusivas do PostgreSQL.
    if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
    {
        await DatabaseCommands.MigrateAsync(dbContext);
        await Seeder.RunSeederAsync(scope.ServiceProvider, typeof(DatabaseSeeder));
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Civitas API v1");
        opt.RoutePrefix = string.Empty;
    });
}

app.UseCors(policy =>
    policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
);

if (!builder.Configuration.GetValue<bool>("DisableHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health").AllowAnonymous();
app.MapControllers();
app.MapHub<AuditoriaHub>("/hubs/auditoria", options =>
    options.CloseOnAuthenticationExpiration = true)
    .RequireCors("AuditoriaSignalR");

app.Run();
