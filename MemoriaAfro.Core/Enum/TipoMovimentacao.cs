namespace MemoriaAfro.Core.Enum;

/// <summary>
/// Tipo de lançamento no histórico de pontos de fidelidade do usuário.
/// </summary>
public enum TipoMovimentacao
{
    /// <summary>Pontos ganhos (ex.: upload de memória aprovado).</summary>
    Credito = 1,

    /// <summary>Pontos utilizados (ex.: resgate futuro no e-commerce).</summary>
    Debito = 2
}
