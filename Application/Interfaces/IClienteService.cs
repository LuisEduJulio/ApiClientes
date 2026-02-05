using ApiClientes.Application.DTOs;

namespace ApiClientes.Application.Interfaces;
public interface IClienteService
{
    Task<ClienteResponse> CriarClienteAsync(ClienteRequest request);
    Task<IEnumerable<ClienteResponse>> ListarClientesAsync();
    Task<ClienteResponse?> ObterClientePorIdAsync(int id);
}