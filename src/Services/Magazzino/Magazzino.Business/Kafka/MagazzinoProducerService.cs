using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Services;

namespace Magazzino.Business.Kafka;

public class MagazzinoProducerService(
    ILogger<MagazzinoProducerService> logger,
    IAdministatorClient adminClient,
    IOptions<MagazzinoProducerKafkaTopics> optionsTopics,
    IOptions<KafkaProducerServiceOptions> optionsProducerService)
    : AbstractProducerService<MagazzinoProducerKafkaTopics>(logger, adminClient, optionsTopics, optionsProducerService)
{
    protected override Task OperationsAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
