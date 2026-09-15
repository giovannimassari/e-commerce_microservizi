using Microsoft.Extensions.DependencyInjection;

namespace Ordini.Business.Kafka;

public class OrdiniKafkaTopics : AbstractKafkaTopics
{
    public string OrdineCreato { get; set; } = "OrdineCreato";

    public override IEnumerable<string> GetTopics() => new List<string> { OrdineCreato };
}