using Microsoft.EntityFrameworkCore;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Repositories;
using Civitas.WebAPI.Repositories.Interfaces;
using Civitas.WebAPI.Services;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Entity Framework
builder.Services.AddDbContext<CivitasDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Repositories
builder.Services.AddScoped<ISecretariaRepository, SecretariaRepository>();

// Services
builder.Services.AddScoped<ISecretariaService, SecretariaService>();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

// Swagger/OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Civitas API",
        Version = "v1",
        Description = "API para gerenciamento de secretarias do sistema Civitas",
        Contact = new OpenApiContact
        {
            Name = "Equipe de Desenvolvimento",
            Email = "dev@civitas.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Inclui comentários XML para documentação mais detalhada
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurações adicionais para melhorar os exemplos
    c.EnableAnnotations();
    c.UseInlineDefinitionsForEnums();
    
    // Personalizar schema IDs para evitar conflitos
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Civitas API V1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Civitas API - Documentação";
        c.DefaultModelsExpandDepth(2); // Expande models por padrão
        c.DefaultModelExpandDepth(3); // Profundidade de expansão dos models
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // Expande lista de endpoints
        c.DisplayRequestDuration(); // Mostra tempo de resposta
        c.EnableDeepLinking(); // Permite links diretos para operações
        c.EnableFilter(); // Habilita filtro de busca
        c.ShowExtensions(); // Mostra extensões
        c.EnableValidator(); // Habilita validação de JSON
    });
}

// Usar CORS
app.UseCors();

// Comentar redirecionamento HTTPS temporariamente para desenvolvimento
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
