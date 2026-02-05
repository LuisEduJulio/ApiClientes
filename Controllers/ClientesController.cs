using ApiClientes.Application.DTOs;
using ApiClientes.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiClientes.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientesController(IClienteService clienteService,
    ILogger<ClientesController> logger) : ControllerBase
{
    private readonly IClienteService _clienteService = clienteService;
    private readonly ILogger<ClientesController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteResponse>>> Listar()
    {
        _logger.LogInformation("Iniciando listagem de clientes");

        try
        {
            var clientes = await _clienteService.ListarClientesAsync();
            _logger.LogInformation("Listagem de clientes concluída. Total: {Count}", clientes.Count());
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar clientes");
            throw;
        }
    }

    [HttpPost]
    public async Task<ActionResult<ClienteResponse>> Criar([FromBody] ClienteRequest request)
    {
        _logger.LogInformation("Iniciando criação de cliente. Nome: {Nome}, Email: {Email}", request.Nome, request.Email);

        try
        {
            var cliente = await _clienteService.CriarClienteAsync(request);
            _logger.LogInformation("Cliente criado com sucesso. ID: {Id}, Nome: {Nome}, Email: {Email}",
                cliente.Id, cliente.Nome, cliente.Email);
            return CreatedAtAction(nameof(Listar), new { id = cliente.Id }, cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente. Nome: {Nome}, Email: {Email}", request.Nome, request.Email);
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteResponse>> ObterPorId(int id)
    {
        _logger.LogInformation("Buscando cliente por ID: {Id}", id);

        try
        {
            var cliente = await _clienteService.ObterClientePorIdAsync(id);

            if (cliente == null)
            {
                _logger.LogWarning("Cliente não encontrado. ID: {Id}", id);
                return NotFound(new { error = "Cliente não encontrado" });
            }

            _logger.LogInformation("Cliente encontrado. ID: {Id}, Nome: {Nome}, Email: {Email}",
                cliente.Id, cliente.Nome, cliente.Email);
            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter cliente por ID: {Id}", id);
            throw;
        }
    }
}