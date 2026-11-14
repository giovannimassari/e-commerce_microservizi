using Microsoft.Extensions.DependencyInjection;
using Utility.Kafka.Abstractions.MessageHandlers;

namespace Approvvigionamento.Business.Kafka;

public class ApprovvigionamentoMessageHandlerFactory : IMessageHandlerFactory<string, string>
{
    public IMessageHandler<string, string> Create(string topic, IServiceProvider serviceProvider)
    {
        return topic switch
        {
            "ScortaBassa" => serviceProvider.GetRequiredService<ScortaBassaHandler>(),
            _ => throw new NotSupportedException($"Nessun handler registrato per il topic '{topic}'")
        };
    }
}
