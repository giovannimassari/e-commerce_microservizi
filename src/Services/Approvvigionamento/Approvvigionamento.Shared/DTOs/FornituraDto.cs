namespace Approvvigionamento.Shared.DTOs;

public class FornituraDto
{
    public Guid Id { get; set; }
    public Guid RichiestaRifornimentoId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? FornitoreId { get; set; }
    public int QuantitaFornita { get; set; }
    public DateTime DataFornitura { get; set; }
}
