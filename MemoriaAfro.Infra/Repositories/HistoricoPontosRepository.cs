using MemoriaAfro.Core.Enum;
using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoriaAfro.Infra.Repositories;

public class HistoricoPontosRepository : RepositoryBase<HistoricoPontos>, IHistoricoPontosRepository
{
    public HistoricoPontosRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<HistoricoPontos>> ListarPorUsuarioAsync(string usuarioId) =>
        await DbSet
            .Where(h => h.UsuarioId == usuarioId)
            .OrderByDescending(h => h.DataMovimentacao)
            .ToListAsync();

    public async Task<int> ObterSaldoAsync(string usuarioId)
    {
        var movimentacoes = await DbSet
            .Where(h => h.UsuarioId == usuarioId)
            .ToListAsync();

        return movimentacoes.Sum(h =>
            h.TipoMovimentacao == TipoMovimentacao.Credito ? h.Quantidade : -h.Quantidade);
    }
}
