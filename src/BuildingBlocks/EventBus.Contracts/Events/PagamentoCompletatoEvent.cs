namespace EventBus.Contracts.Events;

// Pubblicato dal servizio Pagamenti quando il pagamento di un ordine va a buon fine.
public record PagamentoCompletatoEvent : IntegrationEvent
{
    public Guid PagamentoId { get; init; }
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal Importo { get; init; }
}
