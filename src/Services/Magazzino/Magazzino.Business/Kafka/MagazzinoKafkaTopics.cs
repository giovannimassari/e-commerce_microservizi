using Microsoft.Extensions.DependencyInjection;

namespace Magazzino.Business.Kafka;

public class MagazzinoKafkaTopics : AbstractKafkaTopics
{
    public string OrdineCreato { get; set; } = "OrdineCreato";

    public override IEnumerable<string> GetTopics() => new List<string> { OrdineCreato };
}