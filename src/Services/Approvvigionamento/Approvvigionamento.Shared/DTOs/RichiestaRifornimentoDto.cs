namespace Approvvigionamento.Shared.DTOs;

public class RichiestaRifornimentoDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int QuantitaDisponibileAllaRichiesta { get; set; }
    public int QuantitaRichiesta { get; set; }
    public string Stato { get; set; } = string.Empty;
    public DateTime DataRichiesta { get; set; }
    public DateTime? DataCompletamento { get; set; }
}
