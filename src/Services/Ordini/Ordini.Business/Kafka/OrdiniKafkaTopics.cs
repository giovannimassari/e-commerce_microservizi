using Microsoft.Extensions.DependencyInjection;

namespace Ordini.Business.Kafka;

// Topic su cui Ordini PUBBLICA
public class OrdiniKafkaTopics : AbstractKafkaTopics
{
    public string OrdineCreato { get; set; } = "OrdineCreato";
    public string OrdineAnnullato { get; set; } = "OrdineAnnullato";

    public override IEnumerable<string> GetTopics() => new List<string> { OrdineCreato, OrdineAnnullato };
}
