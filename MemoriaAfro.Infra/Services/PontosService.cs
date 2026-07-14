using MemoriaAfro.Core.Enum;
using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Core.Services;

namespace MemoriaAfro.Infra.Services;

public class PontosService : IPontosService
{
    /// <summary>Pontos concedidos por contribuição aprovada. Ajuste conforme a regra de negócio real.</summary>
    private const int PontosPorUploadAprovado = 10;

    private readonly IHistoricoPontosRepository _historicoPontosRepository;

    public PontosService(IHistoricoPontosRepository historicoPontosRepository)
    {
        _historicoPontosRepository = historicoPontosRepository;
    }

    public async Task CreditarPorUploadAprovadoAsync(UploadMemoria upload)
    {
        if (string.IsNullOrEmpty(upload.UsuarioId))
        {
            // Envio anônimo (sem "Seu Nome" vinculado a uma conta): não há pontos a creditar.
            return;
        }

        var movimentacao = new HistoricoPontos
        {
            UsuarioId = upload.UsuarioId,
            TipoMovimentacao = TipoMovimentacao.Credito,
            Quantidade = PontosPorUploadAprovado,
            Descricao = $"Contribuição aprovada: {upload.Titulo}",
            UploadMemoriaId = upload.Id
        };

        await _historicoPontosRepository.AdicionarAsync(movimentacao);
        await _historicoPontosRepository.SalvarAlteracoesAsync();
    }

    public Task<int> ObterSaldoAsync(string usuarioId) =>
        _historicoPontosRepository.ObterSaldoAsync(usuarioId);

    public Task<IEnumerable<HistoricoPontos>> ObterExtratoAsync(string usuarioId) =>
        _historicoPontosRepository.ListarPorUsuarioAsync(usuarioId);
}
