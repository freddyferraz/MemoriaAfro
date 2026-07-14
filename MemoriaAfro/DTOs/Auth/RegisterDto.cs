using System.ComponentModel.DataAnnotations;

namespace MemoriaAfro.DTOs.Auth;

public class RegisterDto
{
    [Required, MaxLength(150)]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Senha { get; set; } = string.Empty;
}
