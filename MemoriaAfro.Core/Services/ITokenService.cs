using MemoriaAfro.Core.Models;

namespace MemoriaAfro.Core.Services;

public interface ITokenService
{
    /// <summary>Gera um JWT assinado contendo os claims do usuário e suas roles.</summary>
    (string Token, DateTime ExpiraEm) GerarToken(Usuario usuario, IList<string> roles);
}
