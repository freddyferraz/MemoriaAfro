using MemoriaAfro.Core.Enum;
using MemoriaAfro.Core.Models;

namespace MemoriaAfro.Core.Interfaces;

public interface IUploadMemoriaRepository : IRepositoryBase<UploadMemoria>
{
    Task<IEnumerable<UploadMemoria>> ListarPorStatusAsync(StatusUpload status);
}
