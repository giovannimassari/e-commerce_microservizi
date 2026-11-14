namespace Pagamenti.Shared.DTOs;

public class PagamentoDto
{
    public Guid Id { get; set; }
    public Guid OrdineId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Importo { get; set; }
    public string Stato { get; set; } = string.Empty;
    public string? Motivo { get; set; }
    public DateTime DataPagamento { get; set; }
}
