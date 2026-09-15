using Domain.Common;

namespace Ordini.Business.Domain;

public class Ordine : Entity
{
    public Guid CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<OrdineItem> Items { get; private set; } = new();

    private Ordine() { } // per EF Core

    public Ordine(Guid customerId, List<OrdineItem> items) : base(Guid.NewGuid())
    {
        CustomerId = customerId;
        Items = items;
        TotalAmount = items.Sum(i => i.Quantity * i.UnitPrice);
        CreatedAt = DateTime.UtcNow;
    }
}

public class OrdineItem
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrdineItem() { }

    public OrdineItem(Guid productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}