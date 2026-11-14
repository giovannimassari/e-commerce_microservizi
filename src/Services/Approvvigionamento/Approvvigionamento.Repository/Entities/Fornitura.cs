namespace Approvvigionamento.Repository.Entities;

public class Fornitura
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RichiestaRifornimentoId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? FornitoreId { get; set; }
    public int QuantitaFornita { get; set; }
    public DateTime DataFornitura { get; set; } = DateTime.UtcNow;
}
