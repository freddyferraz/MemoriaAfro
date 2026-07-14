namespace MemoriaAfro.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEm { get; set; }

    public string Email { get; set; } = string.Empty;

    public string NomeCompleto { get; set; } = string.Empty;

    public IList<string> Roles { get; set; } = new List<string>();
}
