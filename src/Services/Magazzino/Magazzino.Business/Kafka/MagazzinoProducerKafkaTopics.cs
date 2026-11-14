using Microsoft.Extensions.DependencyInjection;

namespace Magazzino.Business.Kafka;

// Topic su cui il Magazzino PUBBLICA
public class MagazzinoProducerKafkaTopics : AbstractKafkaTopics
{
    public string ScortaBassa { get; set; } = "ScortaBassa";

    public override IEnumerable<string> GetTopics() => new List<string> { ScortaBassa };
}
