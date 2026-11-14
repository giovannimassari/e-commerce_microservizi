namespace Approvvigionamento.Shared.DTOs;

public class CompletaRichiestaRequest
{
    public int QuantitaFornita { get; set; }
    public Guid? FornitoreId { get; set; }
}
