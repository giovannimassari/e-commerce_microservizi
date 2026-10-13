using System.Text.Json;
using EventBus.Contracts.Events;
using Microsoft.Extensions.Logging;
using Utility.Kafka.Abstractions.MessageHandlers;

using Magazzino.Business.Repositories;
using Microsoft.Extensions.DependencyInjection;
namespace Magazzino.Business.Kafka;

public class OrdineCreatoHandler(ILogger<OrdineCreatoHandler> logger, 
                                 IStockRepository stockRepository) : IMessageHandler<string, string>
{
    private readonly IServiceScopeFactory _scopeFactory;
    public async Task OnMessageReceivedAsync(string key, string message, CancellationToken cancellationToken = default)
    {
        var evento = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

        if (evento is null)
        {
            logger.LogWarning("Impossibile deserializzare OrderCreatedEvent. Payload: {payload}", message);
            return;
        }

        logger.LogInformation("Ricevuto OrderCreatedEvent per l'ordine {orderId}, riservo lo stock...", evento.OrderId);

        using var scope = _scopeFactory.CreateScope();
        var stockRepository = scope.ServiceProvider.GetRequiredService<IStockRepository>();

        foreach (var item in evento.Items)
            await stockRepository.ReserveAsync(item.ProductId, item.Quantity, cancellationToken);
        
        foreach (var item in evento.Items)
            await stockRepository.ReserveAsync(item.ProductId, item.Quantity, cancellationToken);

        await Task.CompletedTask;
    }
}