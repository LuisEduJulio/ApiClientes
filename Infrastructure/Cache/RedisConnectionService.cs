using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ApiClientes.Infrastructure.Cache;

public static class RedisConnectionService
{
    public static IConnectionMultiplexer ConnectWithRetry(string connectionString, ILogger logger, int maxRetries = 10, int delaySeconds = 5)
    {
        var retryCount = 0;
        
        while (retryCount < maxRetries)
        {
            try
            {
                logger.LogInformation($"Tentando conectar ao Redis (tentativa {retryCount + 1}/{maxRetries})...");
                
                var configurationOptions = ConfigurationOptions.Parse(connectionString);
                configurationOptions.AbortOnConnectFail = false;
                configurationOptions.ConnectRetry = 5;
                configurationOptions.ConnectTimeout = 5000;
                configurationOptions.SyncTimeout = 5000;
                configurationOptions.AsyncTimeout = 5000;
                configurationOptions.ReconnectRetryPolicy = new ExponentialRetry(1000, 10000);
                
                var multiplexer = ConnectionMultiplexer.Connect(configurationOptions);
                
                if (multiplexer.IsConnected)
                {
                    logger.LogInformation("Conexão com Redis estabelecida com sucesso!");
                    
                    multiplexer.ConnectionFailed += (sender, e) =>
                    {
                        logger.LogWarning($"Conexão Redis falhou: {e.FailureType}. Reconectando...");
                    };
                    
                    multiplexer.ConnectionRestored += (sender, e) =>
                    {
                        logger.LogInformation("Conexão Redis restaurada com sucesso!");
                    };
                    
                    return multiplexer;
                }
            }
            catch (Exception ex)
            {
                retryCount++;
                logger.LogWarning(ex, $"Falha ao conectar ao Redis (tentativa {retryCount}/{maxRetries})");
                
                if (retryCount < maxRetries)
                {
                    logger.LogInformation($"Aguardando {delaySeconds} segundos antes da próxima tentativa...");
                    Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
                }
                else
                {
                    logger.LogError(ex, "Não foi possível conectar ao Redis após todas as tentativas");
                    throw new InvalidOperationException(
                        $"Não foi possível conectar ao Redis após {maxRetries} tentativas. " +
                        $"Verifique se o Redis está rodando em: {connectionString}", ex);
                }
            }
        }
        
        throw new InvalidOperationException($"Não foi possível conectar ao Redis após {maxRetries} tentativas");
    }
}
