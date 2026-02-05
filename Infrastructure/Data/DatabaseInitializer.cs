using Microsoft.EntityFrameworkCore;

namespace ApiClientes.Infrastructure.Data;
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            logger.LogInformation("Iniciando inicialização do banco de dados...");

            if (!await context.Database.CanConnectAsync())
                logger.LogInformation("Banco de dados não encontrado. Criando...");

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Aplicando {pendingMigrations.Count()} migration(s) pendente(s)...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations aplicadas com sucesso.");
            }
            else
                logger.LogInformation("Nenhuma migration pendente.");

            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            logger.LogInformation($"Banco de dados inicializado. Migrations aplicadas: {appliedMigrations.Count()}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao inicializar banco de dados");
            throw;
        }
    }
}