using System.ComponentModel.DataAnnotations;

namespace ApiClientes.Application.DTOs;
public class ClienteRequest
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "O email deve ter um formato válido")]
    public string Email { get; set; } = string.Empty;
}