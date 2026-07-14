using MemoriaAfro.Core.Enum;

namespace MemoriaAfro.DTOs.HistoricoPontos;

public class HistoricoPontosDto
{
    public int Id { get; set; }
    public TipoMovimentacao TipoMovimentacao { get; set; }
    public int Quantidade { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataMovimentacao { get; set; }
}

public class SaldoPontosDto
{
    public int Saldo { get; set; }
}
