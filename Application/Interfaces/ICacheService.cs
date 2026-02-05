namespace ApiClientes.Application.Interfaces;

public interface ICacheService
{
    Task<T?> ObterAsync<T>(string key) where T : class;
    Task DefinirAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    Task RemoverPorPadraoAsync(string pattern);
}