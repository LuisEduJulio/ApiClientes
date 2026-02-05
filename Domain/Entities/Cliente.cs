namespace ApiClientes.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime DataCadastro { get; set; }

    private Cliente()
    {
        Nome = string.Empty;
        Email = string.Empty;
    }

    public Cliente(string nome, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome é obrigatório", nameof(nome));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O email é obrigatório", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("O email deve ter um formato válido", nameof(email));

        Nome = nome.Trim();
        Email = email.Trim().ToLower();
        DataCadastro = DateTime.UtcNow;
    }

    private static bool IsValidEmail(string email)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(
            email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}