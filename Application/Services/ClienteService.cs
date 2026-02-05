using ApiClientes.Application.DTOs;
using ApiClientes.Application.Interfaces;
using ApiClientes.Domain.Entities;
using ApiClientes.Domain.Repositories;

namespace ApiClientes.Application.Services;
public class ClienteService(IClienteRepository clienteRepository,
    ICacheService cacheService) : IClienteService
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly ICacheService _cacheService = cacheService;
    public async Task<ClienteResponse> CriarClienteAsync(ClienteRequest request)
    {
        var emailExiste = await _clienteRepository.ExisteEmailAsync(request.Email);

        if (emailExiste)
            throw new InvalidOperationException("Este email já está cadastrado");

        var clienteSalvo = await _clienteRepository.AdicionarAsync(new Cliente(request.Nome, request.Email));

        await _cacheService.RemoverPorPadraoAsync("clientes:*");

        return MapToResponse(clienteSalvo);
    }
    public async Task<IEnumerable<ClienteResponse>> ListarClientesAsync()
    {
        const string cacheKey = "clientes:all";

        var cached = await _cacheService.ObterAsync<List<ClienteResponse>>(cacheKey);
       
        if (cached != null)
            return cached;
        
        var clientes = await _clienteRepository.ListarTodosAsync();
        
        var response = clientes.Select(MapToResponse).ToList();

        await _cacheService.DefinirAsync(cacheKey, response, TimeSpan.FromMinutes(5));

        return response;
    }
    public async Task<ClienteResponse?> ObterClientePorIdAsync(int id)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id);
        return cliente != null ? MapToResponse(cliente) : null;
    }
    private static ClienteResponse MapToResponse(Cliente cliente)
    {
        return new ClienteResponse
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            DataCadastro = cliente.DataCadastro
        };
    }
}