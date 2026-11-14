using Microsoft.Extensions.DependencyInjection;

namespace Ordini.Business.Kafka;

// Topic che Ordini ASCOLTA
public class OrdiniConsumerKafkaTopics : AbstractKafkaTopics
{
    public string PagamentoCompletato { get; set; } = "PagamentoCompletato";
    public string PagamentoFallito { get; set; } = "PagamentoFallito";

    public override IEnumerable<string> GetTopics() => new List<string> { PagamentoCompletato, PagamentoFallito };
}
