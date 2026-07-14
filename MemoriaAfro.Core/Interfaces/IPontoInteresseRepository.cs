using MemoriaAfro.Core.Models;

namespace MemoriaAfro.Core.Interfaces;

public interface IPontoInteresseRepository : IRepositoryBase<PontoInteresse>
{
    Task<IEnumerable<PontoInteresse>> BuscarPorNomeAsync(string termo);
}
