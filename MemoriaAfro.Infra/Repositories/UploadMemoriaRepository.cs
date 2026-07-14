using MemoriaAfro.Core.Enum;
using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoriaAfro.Infra.Repositories;

public class UploadMemoriaRepository : RepositoryBase<UploadMemoria>, IUploadMemoriaRepository
{
    public UploadMemoriaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UploadMemoria>> ListarPorStatusAsync(StatusUpload status) =>
        await DbSet
            .Where(u => u.Status == status)
            .OrderByDescending(u => u.DataEnvio)
            .ToListAsync();
}
