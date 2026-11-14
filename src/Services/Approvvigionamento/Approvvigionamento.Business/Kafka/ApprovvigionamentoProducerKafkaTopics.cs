using Microsoft.Extensions.DependencyInjection;

namespace Approvvigionamento.Business.Kafka;

// Topic su cui l'Approvvigionamento PUBBLICA
public class ApprovvigionamentoProducerKafkaTopics : AbstractKafkaTopics
{
    public string RiassortimentoCompletato { get; set; } = "RiassortimentoCompletato";

    public override IEnumerable<string> GetTopics() => new List<string> { RiassortimentoCompletato };
}
