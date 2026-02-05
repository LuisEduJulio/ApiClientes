using ApiClientes.Application.Interfaces;
using ApiClientes.Application.Services;
using ApiClientes.Domain.Repositories;
using ApiClientes.Infrastructure.Cache;
using ApiClientes.Infrastructure.Data;
using ApiClientes.Infrastructure.Middleware;
using ApiClientes.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=clientes.db"));

var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? string.Empty;

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    return RedisConnectionService.ConnectWithRetry(redisConnectionString, logger);
});

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IClienteService, ClienteService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Clientes",
        Version = "v1",
        Description = "API para cadastro e gerenciamento de clientes",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Clientes",
            Email = "contato@example.com"
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await DatabaseInitializer.InitializeAsync(db, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro crítico ao inicializar banco de dados");
        if (app.Environment.IsDevelopment())
        {
            try
            {
                logger.LogWarning("Tentando criar banco de dados sem migrations (modo desenvolvimento)...");
                await db.Database.EnsureCreatedAsync();
            }
            catch (Exception devEx)
            {
                logger.LogError(devEx, "Falha ao criar banco de dados em modo desenvolvimento");
            }
        }
    }
}

app.UseCors();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Clientes v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "API de Clientes - Documentação";
    c.DefaultModelsExpandDepth(-1);
});

app.MapControllers();

app.Run();
