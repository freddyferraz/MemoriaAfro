using MemoriaAfro.Core.Models;

namespace MemoriaAfro.Core.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(string id);
    Task AdicionarAsync(Usuario usuario);
    Task<int> SalvarAlteracoesAsync();
}
