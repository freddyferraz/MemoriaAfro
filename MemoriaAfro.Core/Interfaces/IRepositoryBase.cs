namespace MemoriaAfro.Core.Interfaces;

/// <summary>
/// Operações básicas compartilhadas por todos os repositórios.
/// Cada repositório específico adiciona métodos de consulta próprios do seu domínio.
/// </summary>
public interface IRepositoryBase<T> where T : class
{
    Task<T?> ObterPorIdAsync(int id);
    Task<IEnumerable<T>> ListarAsync();
    Task AdicionarAsync(T entidade);
    void Atualizar(T entidade);
    void Remover(T entidade);
    Task<int> SalvarAlteracoesAsync();
}
