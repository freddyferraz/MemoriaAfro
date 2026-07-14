using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Infra.Data;

namespace MemoriaAfro.Infra.Repositories;

public class TutorialOficinaRepository : RepositoryBase<TutorialOficina>, ITutorialOficinaRepository
{
    public TutorialOficinaRepository(ApplicationDbContext context) : base(context)
    {
    }
}
