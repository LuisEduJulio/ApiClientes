using ApiClientes.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace ApiClientes.Infrastructure.Cache;
public class RedisCacheService(IConnectionMultiplexer redis,
    ILogger<RedisCacheService>? logger = null) : ICacheService
{
    private readonly IConnectionMultiplexer _redis = redis;
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly ILogger<RedisCacheService>? _logger = logger;
    private bool IsConnected()
    {
        return _redis.IsConnected;
    }
    public async Task<T?> ObterAsync<T>(string key) where T : class
    {
        try
        {
            if (!IsConnected())
            {
                _logger?.LogWarning("Redis não está conectado. Retornando null para chave: {Key}", key);
                return null;
            }

            var value = await _database.StringGetAsync(key);
            if (!value.HasValue)
                return null;

            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao obter valor do Redis para chave: {Key}", key);
            return null;
        }
    }
    public async Task DefinirAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        try
        {
            if (!IsConnected())
            {
                _logger?.LogWarning("Redis não está conectado. Não é possível definir chave: {Key}", key);
                return;
            }

            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiry);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao definir valor no Redis para chave: {Key}", key);
        }
    }

    public async Task RemoverPorPadraoAsync(string pattern)
    {
        try
        {
            if (!IsConnected())
            {
                _logger?.LogWarning("Redis não está conectado. Não é possível remover padrão: {Pattern}", pattern);
                return;
            }

            var endpoints = _redis.GetEndPoints();
            if (endpoints == null || !endpoints.Any())
            {
                _logger?.LogWarning("Nenhum endpoint Redis disponível");
                return;
            }

            var server = _redis.GetServer(endpoints.First());
            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                await _database.KeyDeleteAsync(key);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao remover padrão do Redis: {Pattern}", pattern);
        }
    }
}