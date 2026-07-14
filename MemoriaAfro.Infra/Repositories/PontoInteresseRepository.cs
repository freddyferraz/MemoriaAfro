using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoriaAfro.Infra.Repositories;

public class PontoInteresseRepository : RepositoryBase<PontoInteresse>, IPontoInteresseRepository
{
    public PontoInteresseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PontoInteresse>> BuscarPorNomeAsync(string termo) =>
        await DbSet
            .Where(p => p.AtivoNoMapa && p.Nome.Contains(termo))
            .OrderBy(p => p.Nome)
            .ToListAsync();
}
