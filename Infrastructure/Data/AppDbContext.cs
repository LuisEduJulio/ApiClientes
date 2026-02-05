using ApiClientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiClientes.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(200)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            entity.Property(e => e.DataCadastro).IsRequired();
        });
    }
}