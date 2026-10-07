namespace Magazzino.Data.Entities;

public class Stock
{
    public Guid ProductId { get; set; }

    public int QuantitaDisponibile { get; set; }

    public int QuantitaRiservata { get; set; } = 0;

    public DateTime UltimoAggiornamento { get; set; } = DateTime.UtcNow;
}