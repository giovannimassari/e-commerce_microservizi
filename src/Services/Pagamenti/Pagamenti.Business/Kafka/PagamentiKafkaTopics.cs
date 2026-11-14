using Microsoft.Extensions.DependencyInjection;

namespace Pagamenti.Business.Kafka;

// Topic che i Pagamenti ASCOLTANO
public class PagamentiKafkaTopics : AbstractKafkaTopics
{
    public string OrdineCreato { get; set; } = "OrdineCreato";

    public override IEnumerable<string> GetTopics() => new List<string> { OrdineCreato };
}
