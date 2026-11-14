namespace EventBus.Contracts.Events;

// Pubblicato da Ordini quando un ordine viene annullato (es. pagamento rifiutato):
// il Magazzino lo consuma per rimettere a disposizione i pezzi prenotati.
public record OrdineAnnullatoEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public List<OrderItemDto> Items { get; init; } = new();
}
