using Microsoft.Extensions.DependencyInjection;

namespace Pagamenti.Business.Kafka;

// Topic su cui i Pagamenti PUBBLICANO
public class PagamentiProducerKafkaTopics : AbstractKafkaTopics
{
    public string PagamentoCompletato { get; set; } = "PagamentoCompletato";
    public string PagamentoFallito { get; set; } = "PagamentoFallito";

    public override IEnumerable<string> GetTopics() => new List<string> { PagamentoCompletato, PagamentoFallito };
}
