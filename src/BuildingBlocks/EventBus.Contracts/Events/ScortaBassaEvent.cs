namespace EventBus.Contracts.Events;

// Pubblicato dal Magazzino quando la disponibilità di un prodotto scende sotto la soglia di sicurezza.
public record ScortaBassaEvent : IntegrationEvent
{
    public Guid ProductId { get; init; }
    public int QuantitaDisponibile { get; init; }
    public int Soglia { get; init; }
}
