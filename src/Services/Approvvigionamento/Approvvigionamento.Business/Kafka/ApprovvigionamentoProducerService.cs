using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Services;

namespace Approvvigionamento.Business.Kafka;

public class ApprovvigionamentoProducerService(
    ILogger<ApprovvigionamentoProducerService> logger,
    IAdministatorClient adminClient,
    IOptions<ApprovvigionamentoProducerKafkaTopics> optionsTopics,
    IOptions<KafkaProducerServiceOptions> optionsProducerService)
    : AbstractProducerService<ApprovvigionamentoProducerKafkaTopics>(logger, adminClient, optionsTopics, optionsProducerService)
{
    protected override Task OperationsAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
