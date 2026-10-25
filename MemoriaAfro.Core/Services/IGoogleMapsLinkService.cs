using MemoriaAfro.Core.Struct;

namespace MemoriaAfro.Core.Services;

public interface IGoogleMapsLinkService
{
    /// <summary>Retorna as coordenadas do link ou null se não for possível extrair.</summary>
    Task<Coordenadas?> ExtrairCoordenadasAsync(string link, CancellationToken ct = default);
}