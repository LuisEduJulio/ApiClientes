using ApiClientes.Domain.Entities;

namespace ApiClientes.Domain.Repositories;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(int id);
    Task<bool> ExisteEmailAsync(string email);
    Task<IEnumerable<Cliente>> ListarTodosAsync();
    Task<Cliente> AdicionarAsync(Cliente cliente);
}