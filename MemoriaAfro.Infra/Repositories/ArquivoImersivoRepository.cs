using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoriaAfro.Infra.Repositories;

public class ArquivoImersivoRepository : RepositoryBase<ArquivoImersivo>, IArquivoImersivoRepository
{
    public ArquivoImersivoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ArquivoImersivo>> ListarPorPontoInteresseAsync(int pontoInteresseId) =>
        await DbSet
            .Where(a => a.PontoInteresseId == pontoInteresseId)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();
}
