namespace EventBus.Contracts.Events;

// Pubblicato dal servizio Pagamenti quando il pagamento di un ordine viene rifiutato.
public record PagamentoFallitoEvent : IntegrationEvent
{
    public Guid PagamentoId { get; init; }
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal Importo { get; init; }
    public string Motivo { get; init; } = string.Empty;
}
