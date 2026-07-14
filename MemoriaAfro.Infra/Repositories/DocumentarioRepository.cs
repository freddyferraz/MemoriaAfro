using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Infra.Data;

namespace MemoriaAfro.Infra.Repositories;

public class DocumentarioRepository : RepositoryBase<Documentario>, IDocumentarioRepository
{
    public DocumentarioRepository(ApplicationDbContext context) : base(context)
    {
    }
}
