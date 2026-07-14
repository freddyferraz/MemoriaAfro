using MemoriaAfro.Core.Models;

namespace MemoriaAfro.Core.Services;

/// <summary>
/// Regras de negócio do programa de pontos de fidelidade.
/// </summary>
public interface IPontosService
{
    /// <summary>
    /// Credita pontos ao usuário dono do upload quando ele é aprovado.
    /// Não faz nada se o upload não tiver usuário vinculado (envio anônimo).
    /// </summary>
    Task CreditarPorUploadAprovadoAsync(UploadMemoria upload);

    Task<int> ObterSaldoAsync(string usuarioId);

    Task<IEnumerable<HistoricoPontos>> ObterExtratoAsync(string usuarioId);
}
