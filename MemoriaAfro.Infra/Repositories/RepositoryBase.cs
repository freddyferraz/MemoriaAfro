using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace MemoriaAfro.Infra.Repositories;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;

    public RepositoryBase(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> ObterPorIdAsync(int id) => await DbSet.FindAsync(id);

    public async Task<IEnumerable<T>> ListarAsync() => await DbSet.ToListAsync();

    public async Task AdicionarAsync(T entidade) => await DbSet.AddAsync(entidade);

    public void Atualizar(T entidade) => DbSet.Update(entidade);

    public void Remover(T entidade) => DbSet.Remove(entidade);

    public async Task<int> SalvarAlteracoesAsync() => await Context.SaveChangesAsync();
}
