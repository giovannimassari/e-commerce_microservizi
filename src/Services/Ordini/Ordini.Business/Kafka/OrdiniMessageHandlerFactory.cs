using Microsoft.Extensions.DependencyInjection;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Ordini.Business.Kafka;

public class OrdiniMessageHandlerFactory : IMessageHandlerFactory<string, string>
{
    public IMessageHandler<string, string> Create(string topic, IServiceProvider serviceProvider)
    {
        return topic switch
        {
            "PagamentoCompletato" => serviceProvider.GetRequiredService<PagamentoCompletatoHandler>(),
            "PagamentoFallito" => serviceProvider.GetRequiredService<PagamentoFallitoHandler>(),
            _ => throw new NotSupportedException($"Nessun handler registrato per il topic '{topic}'")
        };
    }
}
