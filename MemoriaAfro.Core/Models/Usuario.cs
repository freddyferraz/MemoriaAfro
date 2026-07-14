namespace MemoriaAfro.Core.Models;

/// <summary>
/// Representa o usuário no domínio. Os dados de autenticação em si
/// (senha, hash, etc.) ficam no Identity, na camada Infra
/// (classe ApplicationIdentityUser), vinculados por Id.
/// </summary>
public class Usuario
{
    /// <summary>Mesmo Id da ApplicationIdentityUser (Identity) — chave compartilhada.</summary>
    public string Id { get; set; } = string.Empty;

    public string NomeCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<UploadMemoria> Uploads { get; set; } = new List<UploadMemoria>();

    public ICollection<HistoricoPontos> MovimentacoesPontos { get; set; } = new List<HistoricoPontos>();
}
