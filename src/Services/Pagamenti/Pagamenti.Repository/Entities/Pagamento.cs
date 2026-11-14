namespace Pagamenti.Repository.Entities;

public class Pagamento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrdineId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Importo { get; set; }
    public StatoPagamento Stato { get; set; }
    public string? Motivo { get; set; }
    public DateTime DataPagamento { get; set; } = DateTime.UtcNow;
}
