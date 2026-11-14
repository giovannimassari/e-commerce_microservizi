namespace Approvvigionamento.Repository.Entities;

public class RichiestaRifornimento
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }

    // Disponibilità che il Magazzino ha segnalato al momento della richiesta
    public int QuantitaDisponibileAllaRichiesta { get; set; }

    // Quantità che il sistema chiede di rifornire
    public int QuantitaRichiesta { get; set; }

    public StatoRichiesta Stato { get; set; } = StatoRichiesta.Aperta;
    public DateTime DataRichiesta { get; set; } = DateTime.UtcNow;
    public DateTime? DataCompletamento { get; set; }
}
