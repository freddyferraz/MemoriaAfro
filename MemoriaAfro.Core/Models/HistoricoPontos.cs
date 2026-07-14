using MemoriaAfro.Core.Enum;

namespace MemoriaAfro.Core.Models;

/// <summary>
/// Lançamento no extrato de pontos de fidelidade de um usuário.
/// Ex.: crédito automático ao ter um UploadMemoria aprovado;
/// débito futuro ao resgatar pontos no e-commerce.
/// </summary>
public class HistoricoPontos
{
    public int Id { get; set; }

    public string UsuarioId { get; set; } = string.Empty;

    public Usuario? Usuario { get; set; }

    public TipoMovimentacao TipoMovimentacao { get; set; }

    public int Quantidade { get; set; }

    public string Descricao { get; set; } = string.Empty;

    /// <summary>Upload que originou o crédito, quando aplicável.</summary>
    public int? UploadMemoriaId { get; set; }

    public UploadMemoria? UploadMemoria { get; set; }

    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;
}
