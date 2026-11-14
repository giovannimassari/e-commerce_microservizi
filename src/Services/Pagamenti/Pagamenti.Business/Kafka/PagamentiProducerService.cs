using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Services;

namespace Pagamenti.Business.Kafka;

public class PagamentiProducerService(
    ILogger<PagamentiProducerService> logger,
    IAdministatorClient adminClient,
    IOptions<PagamentiProducerKafkaTopics> optionsTopics,
    IOptions<KafkaProducerServiceOptions> optionsProducerService)
    : AbstractProducerService<PagamentiProducerKafkaTopics>(logger, adminClient, optionsTopics, optionsProducerService)
{
    protected override Task OperationsAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
