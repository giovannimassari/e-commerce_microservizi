namespace Magazzino.Shared.DTOs;

public record StockDto
{
    public Guid ProductId { get; init; }
    public int QuantitaDisponibile { get; init; }
    public int QuantitaRiservata { get; init; }
    public DateTime UltimoAggiornamento { get; init; }
}