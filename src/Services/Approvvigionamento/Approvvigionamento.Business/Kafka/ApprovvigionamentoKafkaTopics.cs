using Microsoft.Extensions.DependencyInjection;

namespace Approvvigionamento.Business.Kafka;

// Topic che l'Approvvigionamento ASCOLTA
public class ApprovvigionamentoKafkaTopics : AbstractKafkaTopics
{
    public string ScortaBassa { get; set; } = "ScortaBassa";

    public override IEnumerable<string> GetTopics() => new List<string> { ScortaBassa };
}
