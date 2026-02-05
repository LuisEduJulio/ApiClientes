using ApiClientes.Domain.Entities;
using ApiClientes.Domain.Repositories;
using ApiClientes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiClientes.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObterPorIdAsync(int id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        return await _context.Clientes
            .AnyAsync(c => c.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<Cliente>> ListarTodosAsync()
    {
        return await _context.Clientes
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Cliente> AdicionarAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }
}