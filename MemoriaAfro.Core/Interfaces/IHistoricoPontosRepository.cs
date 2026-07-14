using MemoriaAfro.Core.Models;

namespace MemoriaAfro.Core.Interfaces;

public interface IHistoricoPontosRepository : IRepositoryBase<HistoricoPontos>
{
    Task<IEnumerable<HistoricoPontos>> ListarPorUsuarioAsync(string usuarioId);

    /// <summary>Soma créditos menos débitos do usuário (saldo atual de pontos).</summary>
    Task<int> ObterSaldoAsync(string usuarioId);
}
