using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Services;

namespace Ordini.Business.Kafka;

public class OrdiniProducerService(
    ILogger<OrdiniProducerService> logger,
    IAdministatorClient adminClient,
    IOptions<OrdiniKafkaTopics> optionsTopics,
    IOptions<KafkaProducerServiceOptions> optionsProducerService)
    : AbstractProducerService<OrdiniKafkaTopics>(logger, adminClient, optionsTopics, optionsProducerService)
{
    protected override Task OperationsAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}