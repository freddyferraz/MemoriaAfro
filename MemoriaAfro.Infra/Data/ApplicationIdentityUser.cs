using Microsoft.AspNetCore.Identity;

namespace MemoriaAfro.Infra.Data;

/// <summary>
/// Usuário do ASP.NET Core Identity (autenticação: hash de senha, e-mail, etc.).
/// Compartilha o mesmo Id do MemoriaAfro.Core.Models.Usuario (dados de domínio/perfil),
/// mantendo o Core livre de dependência do Identity.
/// </summary>
public class ApplicationIdentityUser : IdentityUser
{
}
