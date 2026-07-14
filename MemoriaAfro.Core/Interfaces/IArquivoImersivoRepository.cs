using MemoriaAfro.Core.Models;

namespace MemoriaAfro.Core.Interfaces;

public interface IArquivoImersivoRepository : IRepositoryBase<ArquivoImersivo>
{
    Task<IEnumerable<ArquivoImersivo>> ListarPorPontoInteresseAsync(int pontoInteresseId);
}
