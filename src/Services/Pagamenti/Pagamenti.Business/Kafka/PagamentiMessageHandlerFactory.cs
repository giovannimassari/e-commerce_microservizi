using Microsoft.Extensions.DependencyInjection;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Pagamenti.Business.Kafka;

public class PagamentiMessageHandlerFactory : IMessageHandlerFactory<string, string>
{
    public IMessageHandler<string, string> Create(string topic, IServiceProvider serviceProvider)
    {
        return topic switch
        {
            "OrdineCreato" => serviceProvider.GetRequiredService<OrdineCreatoHandler>(),
            _ => throw new NotSupportedException($"Nessun handler registrato per il topic '{topic}'")
        };
    }
}
