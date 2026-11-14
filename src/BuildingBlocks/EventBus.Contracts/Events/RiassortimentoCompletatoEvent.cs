namespace EventBus.Contracts.Events;

// Pubblicato dall'Approvvigionamento quando una fornitura è stata consegnata:
// il Magazzino lo consuma e aumenta la disponibilità del prodotto.
public record RiassortimentoCompletatoEvent : IntegrationEvent
{
    public Guid RichiestaId { get; init; }
    public Guid ProductId { get; init; }
    public int QuantitaFornita { get; init; }
    public Guid? FornitoreId { get; init; }
}
