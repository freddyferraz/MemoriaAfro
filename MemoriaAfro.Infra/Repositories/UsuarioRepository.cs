using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Infra.Data;

namespace MemoriaAfro.Infra.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorIdAsync(string id) => await _context.Usuarios.FindAsync(id);

    public async Task AdicionarAsync(Usuario usuario) => await _context.Usuarios.AddAsync(usuario);

    public async Task<int> SalvarAlteracoesAsync() => await _context.SaveChangesAsync();
}
